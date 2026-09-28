import React, {
  useEffect,
  useMemo,
  useState,
} from 'react';

import Notification from '../../components/Notification';

import {
  getAppointments,
} from '../../services/appointmentService';

import visitService from '../../services/visitService';

import {
  getApiErrorMessage,
} from '../../services/api';

// =====================================================
// DATE
// =====================================================

const getLocalToday = () => {
  const now =
    new Date();

  return [
    now.getFullYear(),

    String(
      now.getMonth() + 1
    ).padStart(
      2,
      '0'
    ),

    String(
      now.getDate()
    ).padStart(
      2,
      '0'
    ),
  ].join('-');
};

const statusBadge = (
  status
) => {
  const value =
    String(
      status || ''
    ).toLowerCase();

  switch (value) {
    case 'pending':
      return (
        <span className="badge bg-warning text-dark px-2 py-1">
          Chờ xác nhận
        </span>
      );

    case 'confirmed':
      return (
        <span className="badge bg-primary px-2 py-1">
          Đã xác nhận
        </span>
      );

    case 'checked_in':
      return (
        <span className="badge bg-success px-2 py-1">
          Đã check-in
        </span>
      );

    case 'cancelled':
      return (
        <span className="badge bg-danger px-2 py-1">
          Đã hủy
        </span>
      );

    case 'no_show':
      return (
        <span className="badge bg-dark px-2 py-1">
          Không đến
        </span>
      );

    case 'completed':
      return (
        <span className="badge bg-success px-2 py-1">
          Hoàn tất
        </span>
      );

    default:
      return (
        <span className="badge bg-secondary px-2 py-1">
          {status ||
            'Chưa xác định'}
        </span>
      );
  }
};

// =====================================================
// COMPONENT
// =====================================================

export default function CheckIn() {
  const [
    appointments,
    setAppointments,
  ] = useState([]);

  const [
    loading,
    setLoading,
  ] = useState(false);

  const [
    checkInId,
    setCheckInId,
  ] = useState('');

  const [
    filterDate,
    setFilterDate,
  ] = useState(
    getLocalToday()
  );

  const [
    searchTerm,
    setSearchTerm,
  ] = useState('');

  const [
    filterStatus,
    setFilterStatus,
  ] = useState('all');

  const [
    currentPage,
    setCurrentPage,
  ] = useState(1);

  const [
    message,
    setMessage,
  ] = useState({
    type: '',
    text: '',
  });

  const recordsPerPage =
    10;

  // =====================================================
  // LOAD
  // =====================================================

  const fetchAppointments =
    async (
      showLoading = true
    ) => {
      try {
        if (showLoading) {
          setLoading(true);
        }

        const data =
          await getAppointments({
            date:
              filterDate,
          });

        const list =
          Array.isArray(data)
            ? data
            : data?.items ||
              data?.content ||
              [];

        setAppointments(
          list
        );
      } catch (error) {
        setMessage({
          type: 'danger',

          text:
            getApiErrorMessage(
              error,
              'Không thể tải lịch hẹn.'
            ),
        });
      } finally {
        if (showLoading) {
          setLoading(false);
        }
      }
    };

  useEffect(() => {
    fetchAppointments();
  }, [filterDate]);

  useEffect(() => {
    setCurrentPage(1);
  }, [
    searchTerm,
    filterStatus,
  ]);

  // =====================================================
  // CHECK-IN
  // =====================================================

  const handleCheckIn =
    async (
      appointment
    ) => {
      if (
        !window.confirm(
          'Xác nhận khách hàng đã đến và tiến hành check-in?'
        )
      ) {
        return;
      }

      try {
        setCheckInId(
          appointment.id
        );

        setMessage({
          type: '',
          text: '',
        });

        const data =
          await visitService
            .checkInAppointment(
              appointment.id,
              appointment.type
            );

        setMessage({
          type: 'success',

          text:
            data?.message
            ||
            `Check-in thành công. Số thứ tự: ${
              data?.queueNumber ??
              '—'
            }.`,
        });

        await fetchAppointments(
          false
        );
      } catch (error) {
        setMessage({
          type: 'danger',

          text:
            getApiErrorMessage(
              error,
              'Không thể check-in lịch hẹn.'
            ),
        });
      } finally {
        setCheckInId('');
      }
    };

  // =====================================================
  // FILTER
  // =====================================================

  const filteredAppointments =
    useMemo(
      () => {
        const term =
          searchTerm
            .trim()
            .toLowerCase();

        return appointments.filter(
          (appointment) => {
            const status =
              String(
                appointment?.status ||
                ''
              ).toLowerCase();

            if (
              filterStatus !==
                'all'
              &&
              status !==
                filterStatus
            ) {
              return false;
            }

            if (!term) {
              return true;
            }

            return [
              appointment?.id,
              appointment?.customerName,
              appointment?.customerPhone,
              appointment?.doctorName,
            ]
              .filter(Boolean)
              .join(' ')
              .toLowerCase()
              .includes(term);
          }
        );
      },
      [
        appointments,
        searchTerm,
        filterStatus,
      ]
    );

  // =====================================================
  // PAGINATION
  // =====================================================

  const totalPages =
    Math.ceil(
      filteredAppointments.length /
      recordsPerPage
    );

  const safePage =
    totalPages === 0
      ? 1
      : Math.min(
          currentPage,
          totalPages
        );

  const indexOfLastRecord =
    safePage *
    recordsPerPage;

  const indexOfFirstRecord =
    indexOfLastRecord -
    recordsPerPage;

  const currentRecords =
    filteredAppointments.slice(
      indexOfFirstRecord,
      indexOfLastRecord
    );

  const today =
    getLocalToday();

  return (
    <div
      className="container-fluid p-4"
      style={{
        backgroundColor:
          '#f8f9fa',

        minHeight:
          '100vh',
      }}
    >
      <div className="d-flex flex-column flex-md-row justify-content-between align-items-md-end mb-4">
        <div className="mb-3 mb-md-0">
          <h2
            className="fw-bold mb-1"
            style={{
              color:
                'var(--primary, #0d6efd)',
            }}
          >
            Tiếp nhận & Check-in
          </h2>

          <p className="text-secondary mb-0">
            Xem danh sách đặt lịch và check-in khi khách hàng tới cơ sở.
          </p>
        </div>

        <div className="d-flex gap-2">
          <input
            type="date"
            className="form-control"
            value={filterDate}
            onChange={(e) =>
              setFilterDate(
                e.target.value
              )
            }
            style={{
              width: 200,
            }}
          />

          <button
            type="button"
            className="btn btn-outline-primary"
            onClick={() =>
              fetchAppointments()
            }
            disabled={loading}
          >
            <i className="fa-solid fa-rotate-right" />
          </button>
        </div>
      </div>

      {message.text && (
        <Notification
          type={message.type}
          message={message.text}
          onClose={() =>
            setMessage({
              type: '',
              text: '',
            })
          }
        />
      )}

      {filterDate !==
        today && (
        <div className="alert alert-info">
          Bạn đang xem ngày{' '}
          <strong>
            {filterDate}
          </strong>
          . Chỉ lịch của ngày hôm nay mới được phép check-in.
        </div>
      )}

      <div className="card shadow-sm border-0 rounded-4">
        <div className="card-body p-4">
          <div className="row mb-4">
            <div className="col-md-6 col-lg-5 mb-3 mb-md-0">
              <div className="input-group">
                <span className="input-group-text bg-white border-end-0">
                  <i className="fa-solid fa-magnifying-glass text-muted" />
                </span>

                <input
                  type="search"
                  className="form-control border-start-0 ps-0"
                  placeholder="Tìm mã đặt lịch, tên khách hàng, SĐT..."
                  value={searchTerm}
                  onChange={(e) =>
                    setSearchTerm(
                      e.target.value
                    )
                  }
                />
              </div>
            </div>

            <div className="col-md-4 col-lg-3">
              <select
                className="form-select"
                value={filterStatus}
                onChange={(e) =>
                  setFilterStatus(
                    e.target.value
                  )
                }
              >
                <option value="all">
                  Tất cả trạng thái
                </option>

                <option value="pending">
                  Chờ xác nhận
                </option>

                <option value="confirmed">
                  Đã xác nhận
                </option>

                <option value="checked_in">
                  Đã check-in
                </option>

                <option value="cancelled">
                  Đã hủy
                </option>

                <option value="no_show">
                  Không đến
                </option>

                <option value="completed">
                  Hoàn tất
                </option>
              </select>
            </div>
          </div>

          <div className="table-responsive mb-3">
            <table className="table table-borderless table-hover align-middle mb-0">
              <thead className="border-bottom">
                <tr>
                  <th className="py-3">
                    Mã lịch hẹn
                  </th>

                  <th className="py-3">
                    Thời gian
                  </th>

                  <th className="py-3">
                    Họ tên
                  </th>

                  <th className="py-3">
                    Số điện thoại
                  </th>

                  <th className="py-3">
                    Dịch vụ / Bác sĩ
                  </th>

                  <th className="py-3">
                    Trạng thái
                  </th>

                  <th className="py-3 text-end">
                    Thao tác
                  </th>
                </tr>
              </thead>

              <tbody>
                {loading ? (
                  <tr>
                    <td
                      colSpan="7"
                      className="text-center py-5"
                    >
                      <div className="spinner-border text-primary" />
                    </td>
                  </tr>
                ) : currentRecords.length ===
                  0 ? (
                  <tr>
                    <td
                      colSpan="7"
                      className="text-center py-5 text-muted"
                    >
                      Chưa có lịch hẹn phù hợp.
                    </td>
                  </tr>
                ) : (
                  currentRecords.map(
                    (appointment) => {
                      const status =
                        String(
                          appointment.status ||
                          ''
                        ).toLowerCase();

                      const canCheckIn =
                        filterDate ===
                          today
                        &&
                        [
                          'pending',
                          'confirmed',
                        ].includes(
                          status
                        );

                      return (
                        <tr
                          key={
                            appointment.id
                          }
                          className="border-bottom"
                        >
                          <td className="fw-semibold text-primary">
                            {
                              appointment.id
                            }
                          </td>

                          <td>
                            <div className="fw-bold">
                              {appointment.appointmentTime
                                ? String(
                                    appointment.appointmentTime
                                  ).substring(
                                    0,
                                    5
                                  )
                                : '—'}
                            </div>
                          </td>

                          <td>
                            {appointment.customerName ||
                              '—'}
                          </td>

                          <td>
                            {appointment.customerPhone ||
                              '—'}
                          </td>

                          <td>
                            <div className="mb-1">
                              <span
                                className={`badge ${
                                  appointment.type ===
                                  'EXAMINATION'
                                    ? 'bg-info text-dark'
                                    : 'bg-secondary'
                                }`}
                              >
                                {appointment.type ===
                                'EXAMINATION'
                                  ? 'Khám bệnh'
                                  : 'Xét nghiệm'}
                              </span>
                            </div>

                            <small className="text-muted">
                              {appointment.doctorName ||
                                'Chưa phân công'}
                            </small>
                          </td>

                          <td>
                            {statusBadge(
                              appointment.status
                            )}
                          </td>

                          <td className="text-end">
                            {canCheckIn && (
                              <button
                                type="button"
                                className="btn btn-outline-success rounded-pill px-3 py-1"
                                disabled={
                                  checkInId ===
                                  appointment.id
                                }
                                onClick={() =>
                                  handleCheckIn(
                                    appointment
                                  )
                                }
                              >
                                {checkInId ===
                                appointment.id ? (
                                  <>
                                    <span className="spinner-border spinner-border-sm me-1" />
                                    Đang xử lý
                                  </>
                                ) : (
                                  <>
                                    <i className="fa-solid fa-check me-1" />
                                    Check-in
                                  </>
                                )}
                              </button>
                            )}
                          </td>
                        </tr>
                      );
                    }
                  )
                )}
              </tbody>
            </table>
          </div>

          {totalPages >
            0 && (
            <div className="d-flex justify-content-between align-items-center pt-3 border-top">
              <span className="text-muted small">
                Hiển thị{' '}
                {filteredAppointments.length ===
                0
                  ? 0
                  : indexOfFirstRecord +
                    1}
                {' đến '}
                {Math.min(
                  indexOfLastRecord,
                  filteredAppointments.length
                )}
                {' trong '}
                {
                  filteredAppointments.length
                }
                {' bản ghi'}
              </span>

              <nav>
                <ul className="pagination pagination-sm mb-0">
                  <li
                    className={`page-item ${
                      safePage ===
                      1
                        ? 'disabled'
                        : ''
                    }`}
                  >
                    <button
                      type="button"
                      className="page-link"
                      onClick={() =>
                        setCurrentPage(
                          1
                        )
                      }
                    >
                      <i className="fa-solid fa-angles-left" />
                    </button>
                  </li>

                  <li
                    className={`page-item ${
                      safePage ===
                      1
                        ? 'disabled'
                        : ''
                    }`}
                  >
                    <button
                      type="button"
                      className="page-link"
                      onClick={() =>
                        setCurrentPage(
                          Math.max(
                            safePage -
                              1,
                            1
                          )
                        )
                      }
                    >
                      <i className="fa-solid fa-angle-left" />
                    </button>
                  </li>

                  <li className="page-item disabled">
                    <span className="page-link text-dark fw-semibold">
                      Trang{' '}
                      {safePage}
                      {' / '}
                      {totalPages}
                    </span>
                  </li>

                  <li
                    className={`page-item ${
                      safePage ===
                      totalPages
                        ? 'disabled'
                        : ''
                    }`}
                  >
                    <button
                      type="button"
                      className="page-link"
                      onClick={() =>
                        setCurrentPage(
                          Math.min(
                            safePage +
                              1,
                            totalPages
                          )
                        )
                      }
                    >
                      <i className="fa-solid fa-angle-right" />
                    </button>
                  </li>

                  <li
                    className={`page-item ${
                      safePage ===
                      totalPages
                        ? 'disabled'
                        : ''
                    }`}
                  >
                    <button
                      type="button"
                      className="page-link"
                      onClick={() =>
                        setCurrentPage(
                          totalPages
                        )
                      }
                    >
                      <i className="fa-solid fa-angles-right" />
                    </button>
                  </li>
                </ul>
              </nav>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}