import React, {
  useEffect,
  useMemo,
  useState,
} from 'react';

import { useNavigate } from 'react-router-dom';

import {
  createExaminationAppointment,
  getAppointmentOptions,
  getApiErrorMessage,
  getTakenTimes,
} from '../../services/appointmentService';

const generateSlots = (
  start = '07:00',
  end = '16:00',
  stepMinutes = 60
) => {
  const slots = [];

  const [startHour, startMin] = start
    .split(':')
    .map(Number);

  const [endHour, endMin] = end
    .split(':')
    .map(Number);

  const current = new Date();

  current.setHours(
    startHour,
    startMin,
    0,
    0
  );

  const endTime = new Date();

  endTime.setHours(
    endHour,
    endMin,
    0,
    0
  );

  while (current <= endTime) {
    const hh = String(
      current.getHours()
    ).padStart(2, '0');

    const mm = String(
      current.getMinutes()
    ).padStart(2, '0');

    slots.push(`${hh}:${mm}`);

    current.setMinutes(
      current.getMinutes() + stepMinutes
    );
  }

  return slots;
};

const SLOTS = generateSlots(
  '07:00',
  '16:00',
  60
);

// Giữ nguyên mã trong project cũ
const CK_GENERAL = 'CK015';

const sameCK = (a, b) => {
  const A = String(
    a ?? ''
  ).toUpperCase();

  const B = String(
    b ?? ''
  ).toUpperCase();

  if (!A || !B) {
    return false;
  }

  if (A === B) {
    return true;
  }

  const na = A.replace(/\D/g, '');
  const nb = B.replace(/\D/g, '');

  return Boolean(
    na &&
    nb &&
    na === nb
  );
};

const getLocalToday = () => {
  const now = new Date();

  const year = now.getFullYear();

  const month = String(
    now.getMonth() + 1
  ).padStart(2, '0');

  const day = String(
    now.getDate()
  ).padStart(2, '0');

  return `${year}-${month}-${day}`;
};

const getUserName = (user) =>
  user?.FullName ||
  user?.fullName ||
  user?.HoTen ||
  user?.hoTen ||
  '';

const getUserEmail = (user) =>
  user?.Email ||
  user?.email ||
  '';

export default function DatLichKham({
  departments: departmentsProp = null,
  allDoctors: allDoctorsProp = null,
  currentUser: currentUserProp = null,
}) {
  const navigate = useNavigate();

  const [
    departments,
    setDepartments,
  ] = useState(
    departmentsProp || {}
  );

  const [
    allDoctors,
    setAllDoctors,
  ] = useState(
    allDoctorsProp || []
  );

  const [
    currentUser,
    setCurrentUser,
  ] = useState(
    currentUserProp || null
  );

  const [
    selectedCK,
    setSelectedCK,
  ] = useState('');

  const [
    unknownCK,
    setUnknownCK,
  ] = useState(false);

  const [
    prevCK,
    setPrevCK,
  ] = useState('');

  const [
    selectedDoctor,
    setSelectedDoctor,
  ] = useState('');

  const [
    selectedDate,
    setSelectedDate,
  ] = useState('');

  const [
    selectedTime,
    setSelectedTime,
  ] = useState('');

  const [
    note,
    setNote,
  ] = useState('');

  const [
    forOther,
    setForOther,
  ] = useState(false);

  const [
    selfInfo,
    setSelfInfo,
  ] = useState({
    hoten:
      getUserName(
        currentUserProp
      ),

    sdt: '',
    gioitinh: '',
    ngaysinh: '',

    email:
      getUserEmail(
        currentUserProp
      ),
  });

  const [
    otherInfo,
    setOtherInfo,
  ] = useState({
    p_name: '',
    p_phone: '',
    p_gender: '',
    p_dob: '',
    p_email: '',
  });

  const [
    takenTimes,
    setTakenTimes,
  ] = useState(
    new Set()
  );

  const [
    errMsg,
    setErrMsg,
  ] = useState('');

  const [
    loading,
    setLoading,
  ] = useState(false);

  useEffect(() => {
    let active = true;

    const shouldLoad =
      !departmentsProp ||
      !allDoctorsProp ||
      !currentUserProp;

    if (!shouldLoad) {
      return undefined;
    }

    const loadOptions =
      async () => {
        try {
          const data =
            await getAppointmentOptions();

          if (!active) {
            return;
          }

          if (
            !departmentsProp
          ) {
            setDepartments(
              data?.departments ||
                data?.chuyenKhoa ||
                {}
            );
          }

          if (
            !allDoctorsProp
          ) {
            setAllDoctors(
              data?.doctors ||
                data?.allDoctors ||
                []
            );
          }

          if (
            !currentUserProp
          ) {
            setCurrentUser(
              data?.currentUser ||
                data?.user ||
                null
            );
          }
        } catch (error) {
          if (active) {
            setErrMsg(
              getApiErrorMessage(
                error,
                'Không thể tải dữ liệu đặt lịch.'
              )
            );
          }
        }
      };

    loadOptions();

    return () => {
      active = false;
    };
  }, [
    departmentsProp,
    allDoctorsProp,
    currentUserProp,
  ]);

  useEffect(() => {
    if (!currentUser) {
      return;
    }

    setSelfInfo(
      (prev) => ({
        ...prev,

        hoten:
          prev.hoten ||
          getUserName(
            currentUser
          ),

        email:
          prev.email ||
          getUserEmail(
            currentUser
          ),
      })
    );
  }, [currentUser]);

  useEffect(() => {
    const params =
      new URLSearchParams(
        window.location.search
      );

    const prefillCK =
      params.get('khoa') ||
      params.get('ck') ||
      '';

    const prefillIdbs =
      params.get('idbs') ||
      '';

    const prefillNote =
      params.get('note') ||
      '';

    const prefillDate =
      params.get('date') ||
      '';

    const prefillTime =
      params.get('time') ||
      '';

    if (prefillCK) {
      setSelectedCK(
        prefillCK
      );
    } else if (
      Object.keys(
        departments
      ).length > 0
    ) {
      setSelectedCK(
        (current) =>
          current ||
          Object.keys(
            departments
          )[0]
      );
    }

    if (prefillIdbs) {
      setSelectedDoctor(
        prefillIdbs
      );
    }

    if (prefillNote) {
      setNote(
        prefillNote
      );
    }

    if (prefillDate) {
      setSelectedDate(
        prefillDate
      );
    }

    if (prefillTime) {
      setSelectedTime(
        prefillTime
      );
    }
  }, [departments]);

  const filteredDoctors =
    useMemo(() => {
      return allDoctors.filter(
        (doctor) =>
          sameCK(
            doctor.KhoaID ??
              doctor.khoaId ??
              doctor.specialtyId,

            selectedCK
          )
      );
    }, [
      allDoctors,
      selectedCK,
    ]);

  const handleUnknownCKChange =
    (e) => {
      const checked =
        e.target.checked;

      setUnknownCK(
        checked
      );

      if (checked) {
        setPrevCK(
          selectedCK
        );

        setSelectedCK(
          CK_GENERAL
        );

        const genDocs =
          allDoctors.filter(
            (doctor) =>
              sameCK(
                doctor.KhoaID ??
                  doctor.khoaId ??
                  doctor.specialtyId,

                CK_GENERAL
              )
          );

        if (
          genDocs.length > 0
        ) {
          const id =
            genDocs[0]
              .IDBacSi ??
            genDocs[0].id;

          setSelectedDoctor(
            String(id)
          );
        }
      } else {
        setSelectedCK(
          prevCK ||
            Object.keys(
              departments
            )[0] ||
            ''
        );

        setSelectedDoctor('');
      }
    };

  useEffect(() => {
    if (
      !selectedDoctor ||
      !selectedDate
    ) {
      setTakenTimes(
        new Set()
      );

      return;
    }

    let active = true;

    const loadTakenTimes =
      async () => {
        try {
          const data =
            await getTakenTimes(
              'EXAMINATION',
              selectedDoctor,
              selectedDate
            );

          if (!active) {
            return;
          }

          const times =
            Array.isArray(data)
              ? data
              : data?.times || [];

          setTakenTimes(
            new Set(
              times.map(
                (time) =>
                  String(
                    time
                  ).substring(
                    0,
                    5
                  )
              )
            )
          );
        } catch {
          if (active) {
            setTakenTimes(
              new Set()
            );
          }
        }
      };

    loadTakenTimes();

    return () => {
      active = false;
    };
  }, [
    selectedDoctor,
    selectedDate,
  ]);

  const handleSubmit =
    async (e) => {
      e.preventDefault();

      setErrMsg('');

      if (
        !selectedDoctor ||
        !selectedDate ||
        !selectedTime
      ) {
        setErrMsg(
          'Vui lòng chọn bác sĩ, ngày và giờ.'
        );

        return;
      }

      const recipientEmail =
        forOther
          ? otherInfo.p_email.trim()
          : (
              getUserEmail(
                currentUser
              ) ||
              selfInfo.email
            ).trim();

      if (
        !recipientEmail ||
        !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(
          recipientEmail
        )
      ) {
        setErrMsg(
          'Vui lòng nhập email hợp lệ.'
        );

        return;
      }

      const payload = {
        bacsi:
          selectedDoctor,

        ngaykham:
          selectedDate,

        gio:
          selectedTime,

        noidung:
          note,

        for_other:
          forOther,

        ...(forOther
          ? otherInfo
          : selfInfo),
      };

      try {
        setLoading(true);

        const data =
          await createExaminationAppointment(
            payload
          );

        const id =
          data?.id ??
          data?.IDDatLich ??
          data?.appointmentId;

        if (id) {
          navigate(
            `/lich-hen/${id}`
          );
        } else {
          navigate(
            '/lich-hen'
          );
        }
      } catch (error) {
        setErrMsg(
          getApiErrorMessage(
            error,
            'Không thể lưu lịch.'
          )
        );
      } finally {
        setLoading(false);
      }
    };

  const todayStr =
    getLocalToday();

  return (
    <div
      className="container py-4"
      style={{
        maxWidth: '780px',
      }}
    >
      <h3 className="mb-3 fw-bold">
        ĐẶT LỊCH KHÁM
      </h3>

      {errMsg && (
        <div className="alert alert-danger">
          {errMsg}
        </div>
      )}

      <form
        onSubmit={handleSubmit}
        className="row g-3"
        noValidate
      >
        {/* Chuyên khoa */}
        <div className="col-md-6">
          <label className="form-label d-flex align-items-center justify-content-between">
            <span>
              Chuyên khoa
            </span>

            <span className="form-check ms-2">
              <input
                type="checkbox"
                id="unknownChk"
                className="form-check-input"
                checked={
                  unknownCK
                }
                onChange={
                  handleUnknownCKChange
                }
              />

              <label
                className="form-check-label"
                htmlFor="unknownChk"
                style={{
                  cursor:
                    'pointer',
                }}
              >
                Tôi chưa biết cần
                khám gì
              </label>
            </span>
          </label>

          <select
            className="form-select"
            disabled={unknownCK}
            value={selectedCK}
            onChange={(e) => {
              setSelectedCK(
                e.target.value
              );

              setSelectedDoctor('');
            }}
          >
            {Object.entries(
              departments
            ).map(
              ([id, name]) => (
                <option
                  key={id}
                  value={id}
                >
                  {name}
                </option>
              )
            )}
          </select>
        </div>

        {/* Ngày */}
        <div className="col-md-6">
          <label className="form-label">
            Ngày khám *
          </label>

          <input
            type="date"
            className="form-control"
            required
            min={todayStr}
            value={selectedDate}
            onChange={(e) =>
              setSelectedDate(
                e.target.value
              )
            }
          />
        </div>

        {/* Giờ */}
        <div className="col-md-6">
          <label className="form-label">
            Giờ *
          </label>

          <select
            className="form-select"
            required
            value={selectedTime}
            onChange={(e) =>
              setSelectedTime(
                e.target.value
              )
            }
          >
            <option value="">
              -- Chọn giờ --
            </option>

            {SLOTS.map(
              (slot) => {
                const isTaken =
                  takenTimes.has(
                    slot
                  );

                return (
                  <option
                    key={slot}
                    value={slot}
                    disabled={
                      isTaken
                    }
                  >
                    {slot}{' '}
                    {isTaken
                      ? '(đã kín)'
                      : ''}
                  </option>
                );
              }
            )}
          </select>
        </div>

        {/* Bác sĩ */}
        <div className="col-md-6">
          <label className="form-label">
            Bác sĩ *
          </label>

          <select
            className="form-select"
            required
            value={
              selectedDoctor
            }
            onChange={(e) =>
              setSelectedDoctor(
                e.target.value
              )
            }
          >
            <option value="">
              -- Chọn --
            </option>

            {filteredDoctors.map(
              (doctor) => {
                const id =
                  doctor.IDBacSi ??
                  doctor.id;

                const name =
                  doctor.TenBacSi ??
                  doctor.doctorName ??
                  doctor.name;

                return (
                  <option
                    key={id}
                    value={id}
                  >
                    {name} ({id})
                  </option>
                );
              }
            )}
          </select>
        </div>

        {/* Đặt cho người khác */}
        <div className="col-12">
          <div className="form-check">
            <input
              type="checkbox"
              id="forOther"
              className="form-check-input"
              checked={
                forOther
              }
              onChange={(e) =>
                setForOther(
                  e.target.checked
                )
              }
            />

            <label
              className="form-check-label"
              htmlFor="forOther"
            >
              Đặt cho người khác
            </label>
          </div>
        </div>

        {!forOther && (
          <div className="col-12">
            <div className="row g-3 p-3 border rounded">
              <div className="col-md-6">
                <label className="form-label">
                  Họ tên
                </label>

                <input
                  type="text"
                  className="form-control"
                  value={
                    selfInfo.hoten
                  }
                  onChange={(e) =>
                    setSelfInfo(
                      {
                        ...selfInfo,
                        hoten:
                          e.target
                            .value,
                      }
                    )
                  }
                />
              </div>

              <div className="col-md-3">
                <label className="form-label">
                  SĐT
                </label>

                <input
                  type="text"
                  className="form-control"
                  value={
                    selfInfo.sdt
                  }
                  onChange={(e) =>
                    setSelfInfo(
                      {
                        ...selfInfo,
                        sdt:
                          e.target
                            .value,
                      }
                    )
                  }
                />
              </div>

              <div className="col-md-3">
                <label className="form-label">
                  Giới tính
                </label>

                <select
                  className="form-select"
                  value={
                    selfInfo.gioitinh
                  }
                  onChange={(e) =>
                    setSelfInfo(
                      {
                        ...selfInfo,
                        gioitinh:
                          e.target
                            .value,
                      }
                    )
                  }
                >
                  <option value="">
                    --
                  </option>

                  <option value="Nam">
                    Nam
                  </option>

                  <option value="Nữ">
                    Nữ
                  </option>

                  <option value="Khác">
                    Khác
                  </option>
                </select>
              </div>

              <div className="col-md-4">
                <label className="form-label">
                  Ngày sinh
                </label>

                <input
                  type="date"
                  className="form-control"
                  value={
                    selfInfo.ngaysinh
                  }
                  onChange={(e) =>
                    setSelfInfo(
                      {
                        ...selfInfo,
                        ngaysinh:
                          e.target
                            .value,
                      }
                    )
                  }
                />
              </div>

              <div className="col-md-8">
                <label className="form-label">
                  Email nhận thông
                  tin
                </label>

                <input
                  type="email"
                  className="form-control"
                  placeholder="..."
                  value={
                    selfInfo.email
                  }
                  onChange={(e) =>
                    setSelfInfo(
                      {
                        ...selfInfo,
                        email:
                          e.target
                            .value,
                      }
                    )
                  }
                />
              </div>
            </div>
          </div>
        )}

        {forOther && (
          <div className="col-12">
            <div className="row g-3 p-3 border rounded">
              <div className="col-md-6">
                <label className="form-label">
                  Họ tên người khám *
                </label>

                <input
                  type="text"
                  className="form-control"
                  required
                  value={
                    otherInfo.p_name
                  }
                  onChange={(e) =>
                    setOtherInfo(
                      {
                        ...otherInfo,
                        p_name:
                          e.target
                            .value,
                      }
                    )
                  }
                />
              </div>

              <div className="col-md-3">
                <label className="form-label">
                  SĐT *
                </label>

                <input
                  type="text"
                  className="form-control"
                  required
                  value={
                    otherInfo.p_phone
                  }
                  onChange={(e) =>
                    setOtherInfo(
                      {
                        ...otherInfo,
                        p_phone:
                          e.target
                            .value,
                      }
                    )
                  }
                />
              </div>

              <div className="col-md-3">
                <label className="form-label">
                  Giới tính *
                </label>

                <select
                  className="form-select"
                  value={
                    otherInfo.p_gender
                  }
                  onChange={(e) =>
                    setOtherInfo(
                      {
                        ...otherInfo,
                        p_gender:
                          e.target
                            .value,
                      }
                    )
                  }
                >
                  <option value="">
                    --
                  </option>

                  <option value="Nam">
                    Nam
                  </option>

                  <option value="Nữ">
                    Nữ
                  </option>

                  <option value="Khác">
                    Khác
                  </option>
                </select>
              </div>

              <div className="col-md-4">
                <label className="form-label">
                  Ngày sinh
                </label>

                <input
                  type="date"
                  className="form-control"
                  value={
                    otherInfo.p_dob
                  }
                  onChange={(e) =>
                    setOtherInfo(
                      {
                        ...otherInfo,
                        p_dob:
                          e.target
                            .value,
                      }
                    )
                  }
                />
              </div>

              <div className="col-md-8">
                <label className="form-label">
                  Email nhận thông
                  tin *
                </label>

                <input
                  type="email"
                  className="form-control"
                  required
                  value={
                    otherInfo.p_email
                  }
                  onChange={(e) =>
                    setOtherInfo(
                      {
                        ...otherInfo,
                        p_email:
                          e.target
                            .value,
                      }
                    )
                  }
                />
              </div>
            </div>
          </div>
        )}

        {/* Ghi chú */}
        <div className="col-12">
          <label className="form-label">
            Ghi chú
          </label>

          <textarea
            rows="2"
            className="form-control"
            placeholder="Yêu cầu thêm (nếu có)"
            value={note}
            onChange={(e) =>
              setNote(
                e.target.value
              )
            }
          />
        </div>

        <div className="col-12">
          <button
            type="submit"
            className="btn btn-primary"
            disabled={loading}
            style={{
              backgroundColor:
                'var(--primary, #0b63e5)',

              borderColor:
                'var(--primary, #0b63e5)',
            }}
          >
            {loading
              ? 'Đang xử lý...'
              : 'Xác nhận'}
          </button>
        </div>
      </form>
    </div>
  );
}