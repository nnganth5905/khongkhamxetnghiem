import React, {
  useEffect,
  useMemo,
  useState,
} from 'react';

import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';

import {
  createTestAppointment,
  getAppointmentOptions,
  getApiErrorMessage,
  getAvailableTimeSlots,
} from '../../services/appointmentService';

// =====================================================
// HELPERS
// =====================================================

const sameCK = (a, b) => {
  const A =
    String(a ?? '')
      .toUpperCase();

  const B =
    String(b ?? '')
      .toUpperCase();

  if (!A || !B) {
    return false;
  }

  if (A === B) {
    return true;
  }

  const na =
    A.replace(
      /\D/g,
      ''
    );

  const nb =
    B.replace(
      /\D/g,
      ''
    );

  return Boolean(
    na &&
    nb &&
    na === nb
  );
};

const getLocalToday = () => {
  const now =
    new Date();

  const year =
    now.getFullYear();

  const month =
    String(
      now.getMonth() + 1
    ).padStart(
      2,
      '0'
    );

  const day =
    String(
      now.getDate()
    ).padStart(
      2,
      '0'
    );

  return `${year}-${month}-${day}`;
};

const getUserName = (user) =>
  user?.FullName ||
  user?.fullName ||
  user?.HoTen ||
  user?.hoTen ||
  user?.TenKhachHang ||
  user?.tenKhachHang ||
  '';

const getUserEmail = (user) =>
  user?.Email ||
  user?.email ||
  '';

const getUserPhone = (user) =>
  user?.Phone ||
  user?.phone ||
  user?.SoDienThoai ||
  user?.soDienThoai ||
  user?.sodienthoai ||
  user?.sdt ||
  user?.phoneNumber ||
  '';

const getDoctorId = (doctor) =>
  doctor?.IDBacSi ??
  doctor?.id ??
  doctor?.doctorId;

const getDoctorName = (doctor) =>
  doctor?.TenBacSi ??
  doctor?.doctorName ??
  doctor?.name ??
  'Bác sĩ';

const getDoctorSpecialtyId = (doctor) =>
  doctor?.KhoaID ??
  doctor?.khoaId ??
  doctor?.specialtyId ??
  doctor?.SpecialtyId;

const getDoctorFacilityId = (doctor) =>
  doctor?.CoSoID ??
  doctor?.coSoId ??
  doctor?.facilityId ??
  doctor?.FacilityId ??
  null;

const getTestId = (test) =>
  test?.IDXetNghiem ??
  test?.id ??
  test?.testId;

const getTestName = (test) =>
  test?.TenXetNghiem ??
  test?.name ??
  test?.testName ??
  'Xét nghiệm';

const getTestPrice = (test) =>
  test?.Gia ??
  test?.price ??
  0;

const getTestSpecialtyId = (test) =>
  test?.ChuyenKhoaID ??
  test?.chuyenKhoaId ??
  test?.specialtyId ??
  test?.categoryId;

// =====================================================
// COMPONENT
// =====================================================

export default function DatLichXetNghiem({
  departments: departmentsProp = null,
  allTests: allTestsProp = null,
  allDoctors: allDoctorsProp = null,
  currentUser: currentUserProp = null,
}) {
  const navigate =
    useNavigate();

  const {
    user: authUser,
  } = useAuth();

  // =====================================================
  // OPTIONS
  // =====================================================

  const [
    departments,
    setDepartments,
  ] = useState(
    departmentsProp ||
    {}
  );

  const [
    allTests,
    setAllTests,
  ] = useState(
    allTestsProp ||
    []
  );

  const [
    allDoctors,
    setAllDoctors,
  ] = useState(
    allDoctorsProp ||
    []
  );

  const [
    currentUser,
    setCurrentUser,
  ] = useState(
    currentUserProp ||
    authUser ||
    null
  );

  // =====================================================
  // APPOINTMENT
  // =====================================================

  const [
    selectedCK,
    setSelectedCK,
  ] = useState('');

  const [
    selectedXN,
    setSelectedXN,
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
    availableSlots,
    setAvailableSlots,
  ] = useState([]);

  const [
    slotsLoading,
    setSlotsLoading,
  ] = useState(false);

  const [
    note,
    setNote,
  ] = useState('');

  // =====================================================
  // PATIENT
  // =====================================================

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
        currentUserProp ||
        authUser
      ),

    sdt:
      getUserPhone(
        currentUserProp ||
        authUser
      ),

    gioitinh: '',

    ngaysinh: '',

    email:
      getUserEmail(
        currentUserProp ||
        authUser
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
    errMsg,
    setErrMsg,
  ] = useState('');

  const [
    loading,
    setLoading,
  ] = useState(false);

  // =====================================================
  // AUTH
  // =====================================================

  useEffect(() => {
    if (!authUser) {
      return;
    }

    setCurrentUser(
      authUser
    );

    setSelfInfo(
      (prev) => ({
        ...prev,

        hoten:
          prev.hoten ||
          getUserName(
            authUser
          ),

        email:
          prev.email ||
          getUserEmail(
            authUser
          ),

        sdt:
          prev.sdt ||
          getUserPhone(
            authUser
          ),
      })
    );
  }, [authUser]);

  // =====================================================
  // LOAD OPTIONS
  // =====================================================

  useEffect(() => {
    let active = true;

    const shouldLoad =
      !departmentsProp ||
      !allTestsProp ||
      !allDoctorsProp;

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

          if (!departmentsProp) {
            setDepartments(
              data?.departments ||
              data?.chuyenKhoa ||
              {}
            );
          }

          if (!allTestsProp) {
            setAllTests(
              data?.tests ||
              data?.allTests ||
              []
            );
          }

          if (!allDoctorsProp) {
            setAllDoctors(
              data?.doctors ||
              data?.allDoctors ||
              []
            );
          }

          if (
            !currentUserProp &&
            !authUser
          ) {
            setCurrentUser(
              data?.currentUser ||
              data?.user ||
              null
            );
          }
        } catch (error) {
          if (!active) {
            return;
          }

          setErrMsg(
            getApiErrorMessage(
              error,
              'Không thể tải dữ liệu đặt lịch.'
            )
          );
        }
      };

    loadOptions();

    return () => {
      active = false;
    };
  }, [
    departmentsProp,
    allTestsProp,
    allDoctorsProp,
    currentUserProp,
    authUser,
  ]);

  // =====================================================
  // PROFILE
  // =====================================================

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

        sdt:
          prev.sdt ||
          getUserPhone(
            currentUser
          ),
      })
    );
  }, [currentUser]);

  // =====================================================
  // QUERY PARAMS
  // =====================================================

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

    const prefillIdxn =
      params.get('idxn') ||
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

    if (prefillIdxn) {
      setSelectedXN(
        prefillIdxn
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

  // =====================================================
  // FILTER TESTS
  // =====================================================

  const filteredTests =
    useMemo(
      () =>
        allTests.filter(
          (test) =>
            sameCK(
              getTestSpecialtyId(
                test
              ),
              selectedCK
            )
        ),
      [
        allTests,
        selectedCK,
      ]
    );

  // =====================================================
  // FILTER DOCTORS
  // =====================================================

  const filteredDoctors =
    useMemo(
      () =>
        allDoctors.filter(
          (doctor) =>
            sameCK(
              getDoctorSpecialtyId(
                doctor
              ),
              selectedCK
            )
        ),
      [
        allDoctors,
        selectedCK,
      ]
    );

  // =====================================================
  // LOAD REAL SLOTS
  // =====================================================

  useEffect(() => {
    if (
      !selectedDoctor ||
      !selectedDate
    ) {
      setAvailableSlots([]);
      setSelectedTime('');

      return undefined;
    }

    let active = true;

    const loadSlots =
      async () => {
        try {
          setSlotsLoading(true);

          const data =
            await getAvailableTimeSlots({
              type: 'TEST',

              doctorId:
                selectedDoctor,

              date:
                selectedDate,
            });

          if (!active) {
            return;
          }

          const slots =
            Array.isArray(data)
              ? data
              : data?.slots || [];

          const normalizedSlots =
            slots.map(
              (time) =>
                String(time)
                  .substring(0, 5)
            );

          setAvailableSlots(
            normalizedSlots
          );

          setSelectedTime(
            (current) =>
              normalizedSlots.includes(
                current
              )
                ? current
                : ''
          );
        } catch (error) {
          if (!active) {
            return;
          }

          setAvailableSlots([]);
          setSelectedTime('');

          setErrMsg(
            getApiErrorMessage(
              error,
              'Không thể tải khung giờ làm việc của bác sĩ.'
            )
          );
        } finally {
          if (active) {
            setSlotsLoading(false);
          }
        }
      };

    loadSlots();

    return () => {
      active = false;
    };
  }, [
    selectedDoctor,
    selectedDate,
  ]);

  // =====================================================
  // SUBMIT
  // =====================================================

  const handleSubmit =
    async (e) => {
      e.preventDefault();

      setErrMsg('');

      if (
        !selectedCK ||
        !selectedXN ||
        !selectedDoctor ||
        !selectedDate ||
        !selectedTime
      ) {
        setErrMsg(
          'Vui lòng chọn đầy đủ chuyên khoa, xét nghiệm, bác sĩ, ngày và giờ.'
        );

        return;
      }

      if (
        availableSlots.length > 0 &&
        !availableSlots.includes(
          selectedTime
        )
      ) {
        setErrMsg(
          'Khung giờ đã chọn không còn khả dụng. Vui lòng chọn lại.'
        );

        return;
      }

      const finalName =
        forOther
          ? otherInfo.p_name.trim()
          : selfInfo.hoten.trim();

      const finalPhone =
        forOther
          ? otherInfo.p_phone.trim()
          : selfInfo.sdt.trim();

      const finalEmail =
        forOther
          ? otherInfo.p_email.trim()
          : selfInfo.email.trim();

      const finalGender =
        forOther
          ? otherInfo.p_gender
          : selfInfo.gioitinh;

      const finalDob =
        forOther
          ? otherInfo.p_dob
          : selfInfo.ngaysinh;

      if (!finalName) {
        setErrMsg(
          'Họ tên không được để trống.'
        );

        return;
      }

      if (!finalPhone) {
        setErrMsg(
          'Số điện thoại không được để trống.'
        );

        return;
      }

      if (
        !finalEmail ||
        !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(
          finalEmail
        )
      ) {
        setErrMsg(
          'Vui lòng nhập email hợp lệ.'
        );

        return;
      }

      const selectedDoctorInfo =
        allDoctors.find(
          (doctor) =>
            String(
              getDoctorId(
                doctor
              )
            ) ===
            String(
              selectedDoctor
            )
        );

      const facilityId =
        getDoctorFacilityId(
          selectedDoctorInfo
        );

      const payload = {
        hoten:
          finalName,

        email:
          finalEmail,

        sodienthoai:
          finalPhone,

        gioitinh:
          finalGender || 'khac',

        ngaysinh:
          finalDob || null,

        ngay:
          selectedDate,

        gio:
          selectedTime,

        idbacsi:
          selectedDoctor,

        idxetnghiem:
          selectedXN,

        /*
         * Không còn "CS001".
         * Backend có thể fallback
         * theo CoSoID của bác sĩ.
         */
        idcoso:
          facilityId,

        ghichu:
          note.trim(),
      };

      try {
        setLoading(true);

        await createTestAppointment(
          payload
        );

        navigate(
          '/lich-hen'
        );
      } catch (error) {
        setErrMsg(
          getApiErrorMessage(
            error,
            'Không thể lưu lịch xét nghiệm.'
          )
        );
      } finally {
        setLoading(false);
      }
    };

  const todayStr =
    getLocalToday();

  // =====================================================
  // RENDER
  // =====================================================

  return (
    <div
      className="container py-4"
      style={{
        maxWidth: '780px',
      }}
    >
      <h3 className="mb-3 fw-bold">
        ĐẶT LỊCH XÉT NGHIỆM
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
          <label className="form-label">
            Chuyên khoa
          </label>

          <select
            className="form-select"
            value={selectedCK}
            onChange={(e) => {
              setSelectedCK(
                e.target.value
              );

              setSelectedXN('');
              setSelectedDoctor('');
              setSelectedTime('');
              setAvailableSlots([]);
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

        {/* Xét nghiệm */}

        <div className="col-md-6">
          <label className="form-label">
            Xét nghiệm *
          </label>

          <select
            className="form-select"
            required
            value={selectedXN}
            onChange={(e) =>
              setSelectedXN(
                e.target.value
              )
            }
          >
            <option value="">
              -- Chọn xét nghiệm --
            </option>

            {filteredTests.map(
              (test) => {
                const id =
                  getTestId(
                    test
                  );

                const name =
                  getTestName(
                    test
                  );

                const price =
                  Number(
                    getTestPrice(
                      test
                    )
                  ) || 0;

                return (
                  <option
                    key={id}
                    value={id}
                  >
                    {name}
                    {' — '}
                    {price.toLocaleString(
                      'vi-VN'
                    )}
                    {' đ'}
                  </option>
                );
              }
            )}
          </select>

          {selectedCK &&
            filteredTests.length === 0 && (
              <div className="form-text text-danger">
                Chưa có xét nghiệm đang hoạt động thuộc chuyên khoa này.
              </div>
            )}
        </div>

        {/* Bác sĩ */}

        <div className="col-md-6">
          <label className="form-label">
            Bác sĩ *
          </label>

          <select
            className="form-select"
            required
            value={selectedDoctor}
            onChange={(e) => {
              setSelectedDoctor(
                e.target.value
              );

              setSelectedTime('');
              setAvailableSlots([]);
            }}
          >
            <option value="">
              -- Chọn bác sĩ --
            </option>

            {filteredDoctors.map(
              (doctor) => {
                const id =
                  getDoctorId(
                    doctor
                  );

                const name =
                  getDoctorName(
                    doctor
                  );

                return (
                  <option
                    key={id}
                    value={id}
                  >
                    {name}
                  </option>
                );
              }
            )}
          </select>

          {selectedCK &&
            filteredDoctors.length === 0 && (
              <div className="form-text text-danger">
                Chưa có bác sĩ đang hoạt động thuộc chuyên khoa này.
              </div>
            )}
        </div>

        {/* Ngày */}

        <div className="col-md-6">
          <label className="form-label">
            Ngày xét nghiệm *
          </label>

          <input
            type="date"
            className="form-control"
            required
            min={todayStr}
            value={selectedDate}
            onChange={(e) => {
              setSelectedDate(
                e.target.value
              );

              setSelectedTime('');
            }}
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
            disabled={
              !selectedDoctor ||
              !selectedDate ||
              slotsLoading
            }
            onChange={(e) =>
              setSelectedTime(
                e.target.value
              )
            }
          >
            <option value="">
              {slotsLoading
                ? 'Đang tải khung giờ...'
                : !selectedDoctor
                  ? '-- Chọn bác sĩ trước --'
                  : !selectedDate
                    ? '-- Chọn ngày trước --'
                    : '-- Chọn giờ --'}
            </option>

            {availableSlots.map(
              (slot) => (
                <option
                  key={slot}
                  value={slot}
                >
                  {slot}
                </option>
              )
            )}
          </select>

          {selectedDoctor &&
            selectedDate &&
            !slotsLoading &&
            availableSlots.length === 0 && (
              <div className="form-text text-danger">
                Bác sĩ không có khung giờ trống trong ngày này.
              </div>
            )}
        </div>

        {/* Đặt cho người khác */}

        <div className="col-12 mt-4">
          <div className="form-check">
            <input
              type="checkbox"
              id="forOther"
              className="form-check-input"
              checked={forOther}
              onChange={(e) =>
                setForOther(
                  e.target.checked
                )
              }
            />

            <label
              className="form-check-label fw-bold"
              htmlFor="forOther"
            >
              Đặt cho người khác
            </label>
          </div>
        </div>

        {/* Patient */}

        {forOther && (
          <div className="col-12">
            <div className="row g-3 p-3 border rounded bg-light">
              <div className="col-md-6">
                <label className="form-label">
                  Họ tên *
                </label>

                <input
                  type="text"
                  className="form-control"
                  required
                  value={
                    otherInfo.p_name
                  }
                  onChange={(e) =>
                    setOtherInfo({
                      ...otherInfo,

                      p_name:
                        e.target.value,
                    })
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
                    setOtherInfo({
                      ...otherInfo,

                      p_phone:
                        e.target.value,
                    })
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
                    otherInfo.p_gender
                  }
                  onChange={(e) =>
                    setOtherInfo({
                      ...otherInfo,

                      p_gender:
                        e.target.value,
                    })
                  }
                >
                  <option value="">
                    --
                  </option>

                  <option value="nam">
                    Nam
                  </option>

                  <option value="nu">
                    Nữ
                  </option>

                  <option value="khac">
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
                  max={todayStr}
                  value={
                    otherInfo.p_dob
                  }
                  onChange={(e) =>
                    setOtherInfo({
                      ...otherInfo,

                      p_dob:
                        e.target.value,
                    })
                  }
                />
              </div>

              <div className="col-md-8">
                <label className="form-label">
                  Email nhận thông tin *
                </label>

                <input
                  type="email"
                  className="form-control"
                  required
                  value={
                    otherInfo.p_email
                  }
                  onChange={(e) =>
                    setOtherInfo({
                      ...otherInfo,

                      p_email:
                        e.target.value,
                    })
                  }
                />
              </div>
            </div>
          </div>
        )}

        {/* Ghi chú */}

        <div className="col-12 mt-3">
          <label className="form-label">
            Ghi chú
          </label>

          <textarea
            rows="3"
            className="form-control"
            placeholder="Yêu cầu thêm (nếu có)"
            maxLength={500}
            value={note}
            onChange={(e) =>
              setNote(
                e.target.value
              )
            }
          />
        </div>

        {/* Submit */}

        <div className="col-12 mt-4">
          <button
            type="submit"
            className="btn btn-primary px-4 py-2"
            disabled={
              loading ||
              slotsLoading ||
              !selectedTime
            }
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