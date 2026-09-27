import React, {
  useCallback,
  useEffect,
  useMemo,
  useState,
} from 'react';

import { useNavigate } from 'react-router-dom';

import {
  getDoctorErrorMessage,
  getDoctors,
  getDoctorSlots,
} from '../../services/doctorService';

const DEFAULT_SLOTS = [
  '07:00',
  '08:00',
  '09:00',
  '10:00',
  '11:00',
  '13:00',
  '14:00',
  '15:00',
  '16:00',
];

const getNextNDays = (n = 4) => {
  const days = [];

  const dayNames = [
    'Chủ nhật',
    'Thứ 2',
    'Thứ 3',
    'Thứ 4',
    'Thứ 5',
    'Thứ 6',
    'Thứ 7',
  ];

  for (let i = 0; i < n; i++) {
    const d = new Date();

    d.setDate(d.getDate() + i);

    const yyyy = d.getFullYear();

    const mm = String(
      d.getMonth() + 1
    ).padStart(2, '0');

    const dd = String(
      d.getDate()
    ).padStart(2, '0');

    days.push({
      key: `${yyyy}-${mm}-${dd}`,
      dmy: `${dd}/${mm}`,
      name: dayNames[d.getDay()],
    });
  }

  return days;
};

const getDoctorId = (doctor) =>
  doctor?.IDBacSi ??
  doctor?.idBacSi ??
  doctor?.id;

const getDoctorName = (doctor) =>
  doctor?.TenBacSi ??
  doctor?.tenBacSi ??
  doctor?.name ??
  '';

const getDoctorImage = (doctor) =>
  doctor?.HinhAnh ??
  doctor?.hinhAnh ??
  doctor?.image ??
  'https://placehold.co/300x300?text=Doctor';

const getDoctorTitle = (doctor) =>
  doctor?.ChucDanh ??
  doctor?.chucDanh ??
  doctor?.title ??
  '';

const getDoctorDescription = (doctor) =>
  doctor?.MoTa ??
  doctor?.moTa ??
  doctor?.description ??
  'Đang cập nhật mô tả…';

const getDoctorSpecialty = (doctor) =>
  doctor?.KhoaID ??
  doctor?.khoaId ??
  doctor?.specialtyId ??
  '';

const getDoctorStars = (doctor) => {
  const value =
    doctor?.SoSao ??
    doctor?.soSao ??
    doctor?.rating ??
    0;

  const number = Number(value);

  return Number.isFinite(number)
    ? number
    : 0;
};

const normalizeTakenMap = (raw = {}) => {
  const result = {};

  Object.entries(raw || {}).forEach(
    ([doctorId, dateMap]) => {
      result[doctorId] = {};

      Object.entries(
        dateMap || {}
      ).forEach(
        ([date, timeMap]) => {
          result[doctorId][date] = {};

          if (Array.isArray(timeMap)) {
            timeMap.forEach((time) => {
              result[doctorId][date][
                String(time).substring(0, 5)
              ] = true;
            });
          } else {
            Object.entries(
              timeMap || {}
            ).forEach(
              ([time, value]) => {
                result[doctorId][date][
                  String(time).substring(0, 5)
                ] = Boolean(value);
              }
            );
          }
        }
      );
    }
  );

  return result;
};

export default function BacSiTQ() {
  const navigate = useNavigate();

  const daysList = useMemo(
    () => getNextNDays(4),
    []
  );

  const [searchTerm, setSearchTerm] =
    useState('');

  const [starFilter, setStarFilter] =
    useState('0');

  const [page, setPage] =
    useState(1);

  const [totalPages, setTotalPages] =
    useState(1);

  const [doctors, setDoctors] =
    useState([]);

  const [loading, setLoading] =
    useState(true);

  const [error, setError] =
    useState('');

  const [
    selectedSchedule,
    setSelectedSchedule,
  ] = useState({});

  const [
    takenSlotsMap,
    setTakenSlotsMap,
  ] = useState({});

  const fetchDoctors = useCallback(
    async () => {
      try {
        setLoading(true);
        setError('');

        const data = await getDoctors({
          q: searchTerm,
          star: starFilter,
          page,
        });

        const doctorList =
          Array.isArray(data)
            ? data
            : data?.doctors ||
              data?.items ||
              data?.content ||
              [];

        const pages =
          data?.totalPages ??
          data?.pages ??
          1;

        setDoctors(doctorList);

        setTotalPages(
          Math.max(
            Number(pages) || 1,
            1
          )
        );

        setTakenSlotsMap(
          normalizeTakenMap(
            data?.takenMap || {}
          )
        );

        const initialSchedule = {};

        doctorList.forEach((doctor) => {
          const id =
            getDoctorId(doctor);

          if (!id) {
            return;
          }

          initialSchedule[id] = {
            date:
              daysList[0]?.key || '',
            time: '',
            isOtherDate: false,
          };
        });

        setSelectedSchedule(
          initialSchedule
        );
      } catch (err) {
        setDoctors([]);

        setError(
          getDoctorErrorMessage(
            err,
            'Không thể tải danh sách bác sĩ.'
          )
        );
      } finally {
        setLoading(false);
      }
    },
    [
      searchTerm,
      starFilter,
      page,
      daysList,
    ]
  );

  useEffect(() => {
    fetchDoctors();
  }, [page]);

  const handleSearchSubmit = (e) => {
    e.preventDefault();

    if (page === 1) {
      fetchDoctors();
    } else {
      setPage(1);
    }
  };

  const handleSelectDate = async (
    doctorId,
    dateKey,
    isOther = false
  ) => {
    setSelectedSchedule(
      (prev) => ({
        ...prev,

        [doctorId]: {
          ...prev[doctorId],

          date: dateKey,

          time: '',

          isOtherDate: isOther,
        },
      })
    );

    if (!dateKey) {
      return;
    }

    if (
      takenSlotsMap[doctorId]?.[
        dateKey
      ]
    ) {
      return;
    }

    try {
      const data =
        await getDoctorSlots(
          doctorId,
          dateKey
        );

      const times =
        Array.isArray(data)
          ? data
          : data?.times || [];

      const mapForDate = {};

      times.forEach((time) => {
        mapForDate[
          String(time).substring(
            0,
            5
          )
        ] = true;
      });

      setTakenSlotsMap(
        (prev) => ({
          ...prev,

          [doctorId]: {
            ...(prev[doctorId] ||
              {}),

            [dateKey]:
              mapForDate,
          },
        })
      );
    } catch (err) {
      console.error(
        'Không thể tải slot bác sĩ:',
        err
      );
    }
  };

  const handleSelectTime = (
    doctorId,
    timeStr
  ) => {
    const docDate =
      selectedSchedule[
        doctorId
      ]?.date;

    const isTaken =
      takenSlotsMap[
        doctorId
      ]?.[docDate]?.[
        timeStr
      ];

    if (isTaken) {
      return;
    }

    setSelectedSchedule(
      (prev) => ({
        ...prev,

        [doctorId]: {
          ...prev[doctorId],

          time: timeStr,
        },
      })
    );
  };

  const handleBooking = (
    doctor,
    type
  ) => {
    const doctorId =
      getDoctorId(doctor);

    const schedule =
      selectedSchedule[
        doctorId
      ] || {};

    const {
      date,
      time,
    } = schedule;

    if (!date || !time) {
      window.alert(
        'Vui lòng chọn Ngày và Giờ!'
      );

      return;
    }

    if (
      takenSlotsMap[
        doctorId
      ]?.[date]?.[time]
    ) {
      window.alert(
        'Khung giờ này đã có người đặt. Vui lòng chọn khung khác.'
      );

      return;
    }

    const query =
      new URLSearchParams({
        idbs: String(
          doctorId
        ),

        bacsi:
          getDoctorName(
            doctor
          ),

        ck: String(
          getDoctorSpecialty(
            doctor
          )
        ),

        date,

        time,
      });

    if (type === 'visit') {
      navigate(
        `/dat-lich-kham?${query.toString()}`
      );
    } else {
      navigate(
        `/dat-lich-xet-nghiem?${query.toString()}`
      );
    }
  };

  return (
    <div
      className="container my-4"
      style={{
        maxWidth: '1200px',
      }}
    >
      <h2
        className="text-center fw-bold mb-4"
        style={{
          color:
            'var(--primary, #0b63e5)',
        }}
      >
        Đội ngũ bác sĩ
      </h2>

      <form
        className="row mb-4 justify-content-center gy-2"
        onSubmit={
          handleSearchSubmit
        }
      >
        <div className="col-md-5">
          <input
            type="text"
            className="form-control"
            placeholder="Tìm tên, chức danh, mô tả..."
            value={searchTerm}
            onChange={(e) =>
              setSearchTerm(
                e.target.value
              )
            }
          />
        </div>

        <div className="col-md-3">
          <select
            className="form-select"
            value={starFilter}
            onChange={(e) =>
              setStarFilter(
                e.target.value
              )
            }
          >
            <option value="0">
              Tất cả xếp hạng
            </option>

            <option value="3">
              Từ 3★
            </option>

            <option value="4">
              Từ 4★
            </option>

            <option value="4.5">
              Từ 4.5★
            </option>

            <option value="5">
              Chỉ 5★
            </option>
          </select>
        </div>

        <div className="col-md-2">
          <button
            type="submit"
            className="btn btn-primary w-100 fw-semibold"
            style={{
              backgroundColor:
                'var(--primary, #0b63e5)',

              borderColor:
                'var(--primary, #0b63e5)',
            }}
          >
            Tìm kiếm
          </button>
        </div>
      </form>

      {error && (
        <div className="alert alert-danger">
          {error}
        </div>
      )}

      {loading ? (
        <div className="text-center py-5 text-secondary">
          Đang tải danh sách
          bác sĩ...
        </div>
      ) : doctors.length ===
        0 ? (
        <div className="alert alert-light border text-center">
          Không tìm thấy bác sĩ
          phù hợp.
        </div>
      ) : (
        doctors.map(
          (doctor, idx) => {
            const idBs =
              getDoctorId(
                doctor
              );

            const doctorName =
              getDoctorName(
                doctor
              );

            const currentSched =
              selectedSchedule[
                idBs
              ] || {
                date:
                  daysList[0]
                    ?.key || '',
                time: '',
              };

            const starVal =
              getDoctorStars(
                doctor
              );

            return (
              <div
                key={
                  idBs || idx
                }
                className="card border-0 rounded-4 p-4 mb-4 shadow-sm bg-white"
                style={{
                  boxShadow:
                    '0 6px 16px rgba(0,0,0,.06)',
                }}
              >
                <div className="row g-4">
                  <div className="col-lg-5">
                    <div className="d-flex gap-3 align-items-start">
                      <img
                        src={getDoctorImage(
                          doctor
                        )}
                        className="rounded-3 flex-shrink-0"
                        alt={
                          doctorName
                        }
                        style={{
                          width:
                            '150px',

                          height:
                            '150px',

                          objectFit:
                            'cover',
                        }}
                      />

                      <div>
                        <h5 className="fw-bold mb-1">
                          {
                            doctorName
                          }
                        </h5>

                        <div className="stars text-warning mb-2 small">
                          {Array.from(
                            {
                              length:
                                5,
                            },
                            (
                              _,
                              i
                            ) => (
                              <i
                                key={
                                  i
                                }
                                className={`fa-star me-1 ${
                                  i <
                                  Math.round(
                                    starVal
                                  )
                                    ? 'fa-solid'
                                    : 'fa-regular'
                                }`}
                              />
                            )
                          )}

                          <span className="ms-1 text-secondary">
                            (
                            {starVal.toFixed(
                              1
                            )}
                            )
                          </span>
                        </div>

                        {getDoctorTitle(
                          doctor
                        ) && (
                          <div className="text-muted small mb-1">
                            <i className="fa-solid fa-briefcase me-2" />

                            {getDoctorTitle(
                              doctor
                            )}
                          </div>
                        )}

                        <div className="text-muted small mb-2">
                          <i className="fa-solid fa-hospital me-2" />

                          Phòng khám xét
                          nghiệm Bio Medic
                          Center
                        </div>
                      </div>
                    </div>

                    <hr className="my-3 opacity-25" />

                    <p
                      className="text-secondary small mb-0"
                      style={{
                        lineHeight:
                          '1.6',
                      }}
                    >
                      {getDoctorDescription(
                        doctor
                      )}
                    </p>
                  </div>

                  <div className="col-lg-7">
                    <div className="row gy-3">
                      <div className="col-12">
                        <h6 className="fw-bold mb-2">
                          Ngày{' '}
                          <span className="text-danger">
                            *
                          </span>
                        </h6>

                        <div className="d-flex gap-2 flex-wrap align-items-center">
                          {daysList.map(
                            (
                              day
                            ) => {
                              const isActive =
                                currentSched.date ===
                                  day.key &&
                                !currentSched.isOtherDate;

                              return (
                                <div
                                  key={
                                    day.key
                                  }
                                  className={`p-2 text-center rounded-3 border ${
                                    isActive
                                      ? 'bg-success-subtle border-success'
                                      : 'bg-light'
                                  }`}
                                  style={{
                                    minWidth:
                                      '100px',

                                    cursor:
                                      'pointer',

                                    userSelect:
                                      'none',
                                  }}
                                  onClick={() =>
                                    handleSelectDate(
                                      idBs,
                                      day.key,
                                      false
                                    )
                                  }
                                >
                                  <div className="fw-bold fs-6">
                                    {
                                      day.dmy
                                    }
                                  </div>

                                  <div className="small text-muted">
                                    {
                                      day.name
                                    }
                                  </div>
                                </div>
                              );
                            }
                          )}

                          <div
                            className={`p-2 text-center rounded-3 border ${
                              currentSched.isOtherDate
                                ? 'bg-success-subtle border-success'
                                : 'bg-light'
                            }`}
                            style={{
                              minWidth:
                                '100px',

                              cursor:
                                'pointer',

                              userSelect:
                                'none',
                            }}
                            onClick={() =>
                              handleSelectDate(
                                idBs,
                                '',
                                true
                              )
                            }
                          >
                            <div className="fw-bold fs-6">
                              Ngày khác
                            </div>

                            <div className="small text-muted">
                              <i className="fa-regular fa-calendar" />
                            </div>
                          </div>

                          {currentSched.isOtherDate && (
                            <input
                              type="date"
                              className="form-control mt-2"
                              style={{
                                maxWidth:
                                  '200px',
                              }}
                              value={
                                currentSched.date
                              }
                              min={
                                new Date()
                                  .toISOString()
                                  .split(
                                    'T'
                                  )[0]
                              }
                              onChange={(
                                e
                              ) =>
                                handleSelectDate(
                                  idBs,
                                  e
                                    .target
                                    .value,
                                  true
                                )
                              }
                            />
                          )}
                        </div>
                      </div>

                      <div className="col-12">
                        <h6 className="fw-bold mb-2">
                          Giờ{' '}
                          <span className="text-danger">
                            *
                          </span>
                        </h6>

                        <div className="row g-2">
                          {DEFAULT_SLOTS.map(
                            (
                              slot
                            ) => {
                              const isTaken =
                                Boolean(
                                  takenSlotsMap[
                                    idBs
                                  ]?.[
                                    currentSched
                                      .date
                                  ]?.[
                                    slot
                                  ]
                                );

                              const isSelected =
                                currentSched.time ===
                                slot;

                              return (
                                <div
                                  key={
                                    slot
                                  }
                                  className="col-4 col-md-3"
                                >
                                  <div
                                    className={`text-center py-2 px-1 rounded-3 border small ${
                                      isTaken
                                        ? 'bg-secondary-subtle text-muted text-decoration-line-through'
                                        : isSelected
                                        ? 'bg-success text-white border-success fw-bold'
                                        : 'bg-light'
                                    }`}
                                    style={{
                                      cursor:
                                        isTaken
                                          ? 'not-allowed'
                                          : 'pointer',

                                      userSelect:
                                        'none',
                                    }}
                                    onClick={() =>
                                      handleSelectTime(
                                        idBs,
                                        slot
                                      )
                                    }
                                  >
                                    {
                                      slot
                                    }
                                  </div>
                                </div>
                              );
                            }
                          )}
                        </div>
                      </div>

                      <div className="col-12 pt-2">
                        <button
                          type="button"
                          className="btn btn-primary me-2 mb-2 px-3 fw-semibold"
                          style={{
                            backgroundColor:
                              'var(--primary, #0b63e5)',

                            borderColor:
                              'var(--primary, #0b63e5)',
                          }}
                          onClick={() =>
                            handleBooking(
                              doctor,
                              'exam'
                            )
                          }
                        >
                          Đặt lịch xét
                          nghiệm
                        </button>

                        <button
                          type="button"
                          className="btn btn-outline-primary mb-2 px-3 fw-semibold"
                          onClick={() =>
                            handleBooking(
                              doctor,
                              'visit'
                            )
                          }
                        >
                          Đặt lịch khám
                        </button>

                        {currentSched.date &&
                          currentSched.time && (
                            <div className="small text-success mt-1 fw-semibold">
                              Đã chọn:{' '}
                              {
                                currentSched.date
                              }{' '}
                              •{' '}
                              {
                                currentSched.time
                              }
                            </div>
                          )}
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            );
          }
        )
      )}

      {totalPages > 1 && (
        <nav className="d-flex justify-content-center mt-4">
          <ul className="pagination">
            <li
              className={`page-item ${
                page === 1
                  ? 'disabled'
                  : ''
              }`}
            >
              <button
                type="button"
                className="page-link"
                onClick={() =>
                  setPage(
                    (p) =>
                      Math.max(
                        1,
                        p - 1
                      )
                  )
                }
              >
                &larr;
              </button>
            </li>

            {Array.from(
              {
                length:
                  totalPages,
              },
              (_, i) => i + 1
            ).map(
              (pageNumber) => (
                <li
                  key={
                    pageNumber
                  }
                  className={`page-item ${
                    pageNumber ===
                    page
                      ? 'active'
                      : ''
                  }`}
                >
                  <button
                    type="button"
                    className="page-link"
                    style={
                      pageNumber ===
                      page
                        ? {
                            backgroundColor:
                              'var(--primary, #0b63e5)',

                            borderColor:
                              'var(--primary, #0b63e5)',
                          }
                        : {}
                    }
                    onClick={() =>
                      setPage(
                        pageNumber
                      )
                    }
                  >
                    {
                      pageNumber
                    }
                  </button>
                </li>
              )
            )}

            <li
              className={`page-item ${
                page ===
                totalPages
                  ? 'disabled'
                  : ''
              }`}
            >
              <button
                type="button"
                className="page-link"
                onClick={() =>
                  setPage(
                    (p) =>
                      Math.min(
                        totalPages,
                        p + 1
                      )
                  )
                }
              >
                &rarr;
              </button>
            </li>
          </ul>
        </nav>
      )}
    </div>
  );
}