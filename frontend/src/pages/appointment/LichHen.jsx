import React, {
  useEffect,
  useState,
} from 'react';

import {
  Link,
} from 'react-router-dom';

import {
  getApiErrorMessage,
  getMyAppointments,
} from '../../services/appointmentService';

// =====================================================
// DATE
// =====================================================

const normalizeDate = (value) => {
  if (!value) {
    return '';
  }

  return String(value)
    .substring(0, 10);
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

const formatDate = (dateStr) => {
  const cleanDate =
    normalizeDate(
      dateStr
    );

  if (!cleanDate) {
    return '—';
  }

  const parts =
    cleanDate.split('-');

  if (
    parts.length !== 3
  ) {
    return cleanDate;
  }

  return (
    `${parts[2]}/${parts[1]}/${parts[0]}`
  );
};

// =====================================================
// STATUS
// =====================================================

const renderStatusBadge = (
  status,
  type
) => {
  const value =
    String(
      status || ''
    ).toLowerCase();

  const isExam =
    type === 'EXAMINATION';

  switch (value) {
    case 'cancelled':
    case 'huy':
      return (
        <span className="badge bg-danger px-3 py-2 rounded-pill">
          Đã hủy
        </span>
      );

    case 'no_show':
    case 'bo_luot':
      return (
        <span className="badge bg-dark px-3 py-2 rounded-pill">
          Không đến / Bỏ lượt
        </span>
      );

    case 'pending':
      return (
        <span className="badge bg-info text-dark px-3 py-2 rounded-pill">
          Đã đặt lịch
        </span>
      );

    case 'confirmed':
      return (
        <span className="badge bg-primary px-3 py-2 rounded-pill">
          Đã xác nhận
        </span>
      );

    case 'checked_in':
    case 'da_tiep_nhan':
      return (
        <span className="badge bg-primary px-3 py-2 rounded-pill">
          Đã tiếp nhận
        </span>
      );

    case 'cho_kham':
    case 'cho_xet_nghiem':
    case 'da_den_luot':
    case 'cho_goi_lai':
      return (
        <span className="badge bg-warning text-dark px-3 py-2 rounded-pill">
          {isExam
            ? 'Chờ khám'
            : 'Chờ xét nghiệm'}
        </span>
      );

    case 'dang_kham':
      return (
        <span className="badge bg-warning text-dark px-3 py-2 rounded-pill">
          Đang khám
        </span>
      );

    case 'dang_lay_mau':
      return (
        <span className="badge bg-warning text-dark px-3 py-2 rounded-pill">
          Đang lấy mẫu
        </span>
      );

    case 'da_chi_dinh_xn':
      return (
        <span className="badge bg-secondary px-3 py-2 rounded-pill">
          Đã chỉ định xét nghiệm
        </span>
      );

    case 'da_lay_mau':
      return (
        <span className="badge bg-secondary px-3 py-2 rounded-pill">
          Đã lấy mẫu
        </span>
      );

    case 'ktv_tiep_nhan':
      return (
        <span className="badge bg-secondary px-3 py-2 rounded-pill">
          KTV đã tiếp nhận
        </span>
      );

    case 'dang_xet_nghiem':
      return (
        <span className="badge bg-secondary px-3 py-2 rounded-pill">
          Đang xét nghiệm
        </span>
      );

    case 'cho_duyet':
    case 'cho_duyet_kq':
      return (
        <span className="badge bg-secondary px-3 py-2 rounded-pill">
          Chờ duyệt kết quả
        </span>
      );

    case 'da_duyet':
    case 'da_co_kq':
    case 'moi_doc_kq':
    case 'dang_tu_van_kq':
      return (
        <span className="badge bg-success px-3 py-2 rounded-pill">
          Đã có kết quả
        </span>
      );

    case 'completed':
    case 'hoan_tat':
      return (
        <span className="badge bg-success px-3 py-2 rounded-pill">
          Hoàn thành
        </span>
      );

    default:
      return (
        <span className="badge bg-secondary px-3 py-2 rounded-pill">
          {status || 'Chưa xác định'}
        </span>
      );
  }
};

// =====================================================
// COMPONENT
// =====================================================

export default function MyAppointments({
  initialAppointments = null,
}) {
  const [
    appointments,
    setAppointments,
  ] = useState(
    initialAppointments ||
    []
  );

  const [
    loading,
    setLoading,
  ] = useState(
    !initialAppointments
  );

  const [
    error,
    setError,
  ] = useState('');

  const [
    searchTerm,
    setSearchTerm,
  ] = useState('');

  const [
    statusFilter,
    setStatusFilter,
  ] = useState('ALL');

  const [
    startDate,
    setStartDate,
  ] = useState('');

  const [
    endDate,
    setEndDate,
  ] = useState('');

  const [
    currentPage,
    setCurrentPage,
  ] = useState(1);

  const itemsPerPage = 10;

  // =====================================================
  // LOAD
  // =====================================================

  useEffect(() => {
    if (initialAppointments) {
      return undefined;
    }

    let active = true;

    const loadAppointments =
      async () => {
        try {
          setLoading(true);
          setError('');

          const data =
            await getMyAppointments();

          if (!active) {
            return;
          }

          const items =
            Array.isArray(data)
              ? data
              : data?.appointments ||
                data?.items ||
                [];

          setAppointments(
            items
          );
        } catch (err) {
          if (!active) {
            return;
          }

          setError(
            getApiErrorMessage(
              err,
              'Không thể tải danh sách lịch hẹn.'
            )
          );
        } finally {
          if (active) {
            setLoading(false);
          }
        }
      };

    loadAppointments();

    return () => {
      active = false;
    };
  }, [initialAppointments]);

  // =====================================================
  // RESET PAGE
  // =====================================================

  useEffect(() => {
    setCurrentPage(1);
  }, [
    searchTerm,
    statusFilter,
    startDate,
    endDate,
  ]);

  // =====================================================
  // FILTER
  // =====================================================

  const filteredAppointments =
    appointments.filter(
      (item) => {
        const itemStatus =
          String(
            item?.status ||
            ''
          ).toLowerCase();

        if (
          statusFilter !==
          'ALL'
        ) {
          if (
            statusFilter ===
              'pending' &&
            itemStatus !==
              'pending'
          ) {
            return false;
          }

          if (
            statusFilter ===
              'confirmed' &&
            ![
              'confirmed',
              'checked_in',
              'da_tiep_nhan',
            ].includes(
              itemStatus
            )
          ) {
            return false;
          }

          if (
            statusFilter ===
              'waiting' &&
            ![
              'cho_kham',
              'cho_xet_nghiem',
              'da_den_luot',
              'cho_goi_lai',
            ].includes(
              itemStatus
            )
          ) {
            return false;
          }

          if (
            statusFilter ===
              'in_progress' &&
            ![
              'dang_kham',
              'dang_lay_mau',
              'da_chi_dinh_xn',
            ].includes(
              itemStatus
            )
          ) {
            return false;
          }

          if (
            statusFilter ===
              'testing' &&
            ![
              'da_lay_mau',
              'ktv_tiep_nhan',
              'dang_xet_nghiem',
              'cho_duyet',
              'cho_duyet_kq',
            ].includes(
              itemStatus
            )
          ) {
            return false;
          }

          if (
            statusFilter ===
              'has_result' &&
            ![
              'da_duyet',
              'da_co_kq',
              'moi_doc_kq',
              'dang_tu_van_kq',
            ].includes(
              itemStatus
            )
          ) {
            return false;
          }

          if (
            statusFilter ===
              'completed' &&
            ![
              'completed',
              'hoan_tat',
            ].includes(
              itemStatus
            )
          ) {
            return false;
          }

          if (
            statusFilter ===
              'cancelled' &&
            ![
              'cancelled',
              'huy',
              'no_show',
              'bo_luot',
            ].includes(
              itemStatus
            )
          ) {
            return false;
          }
        }

        const appointmentDate =
          normalizeDate(
            item?.appointmentDate
          );

        if (
          startDate &&
          (
            !appointmentDate ||
            appointmentDate <
              startDate
          )
        ) {
          return false;
        }

        if (
          endDate &&
          (
            !appointmentDate ||
            appointmentDate >
              endDate
          )
        ) {
          return false;
        }

        const term =
          searchTerm
            .trim()
            .toLowerCase();

        if (term) {
          const id =
            String(
              item?.id ||
              ''
            ).toLowerCase();

          const customer =
            String(
              item?.customerName ||
              ''
            ).toLowerCase();

          const doctor =
            String(
              item?.doctorName ||
              ''
            ).toLowerCase();

          const type =
            item?.type ===
            'EXAMINATION'
              ? 'khám bệnh'
              : 'xét nghiệm';

          if (
            !id.includes(term) &&
            !customer.includes(
              term
            ) &&
            !doctor.includes(
              term
            ) &&
            !type.includes(
              term
            )
          ) {
            return false;
          }
        }

        return true;
      }
    );

  // =====================================================
  // PAGINATION
  // =====================================================

  const totalPages =
    Math.ceil(
      filteredAppointments.length /
      itemsPerPage
    );

  const safeCurrentPage =
    totalPages === 0
      ? 1
      : Math.min(
          currentPage,
          totalPages
        );

  const indexOfLastItem =
    safeCurrentPage *
    itemsPerPage;

  const indexOfFirstItem =
    indexOfLastItem -
    itemsPerPage;

  const currentItems =
    filteredAppointments.slice(
      indexOfFirstItem,
      indexOfLastItem
    );

  const handlePageChange =
    (pageNumber) => {
      const target =
        Math.max(
          1,
          Math.min(
            pageNumber,
            totalPages || 1
          )
        );

      setCurrentPage(
        target
      );
    };

  const today =
    getLocalToday();

  // =====================================================
  // RENDER
  // =====================================================

  return (
    <div className="bg-light min-vh-100 py-4">
      <div className="container-fluid px-md-5">
        <div className="d-flex justify-content-between align-items-center mb-4">
          <div>
            <h3
              className="fw-bold mb-1"
              style={{
                color:
                  '#0b63e5',
              }}
            >
              Lịch hẹn của tôi
            </h3>

            <p className="text-muted mb-0">
              Xem và quản lý các lịch hẹn đã đặt trên hệ thống.
            </p>
          </div>

          <Link
            to="/dat-lich"
            className="btn btn-primary rounded-pill px-4"
          >
            <i className="fa-solid fa-plus me-2" />
            Đặt lịch mới
          </Link>
        </div>

        {error && (
          <div className="alert alert-danger shadow-sm">
            {error}
          </div>
        )}

        <div className="card border-0 shadow-sm rounded-3">
          <div className="card-body p-4">
            {/* Filters */}

            <div className="row mb-4 g-3">
              <div className="col-lg-4 col-md-6">
                <div className="input-group">
                  <span className="input-group-text bg-white text-muted border-end-0">
                    <i className="fa-solid fa-magnifying-glass" />
                  </span>

                  <input
                    type="text"
                    className="form-control border-start-0 ps-0"
                    placeholder="Tìm mã, bác sĩ, bệnh nhân..."
                    value={
                      searchTerm
                    }
                    onChange={(e) =>
                      setSearchTerm(
                        e.target.value
                      )
                    }
                  />
                </div>
              </div>

              <div className="col-lg-3 col-md-6">
                <select
                  className="form-select text-secondary"
                  value={
                    statusFilter
                  }
                  onChange={(e) =>
                    setStatusFilter(
                      e.target.value
                    )
                  }
                >
                  <option value="ALL">
                    Tất cả trạng thái
                  </option>

                  <option value="pending">
                    Đã đặt lịch
                  </option>

                  <option value="confirmed">
                    Xác nhận / Tiếp nhận
                  </option>

                  <option value="waiting">
                    Chờ khám / Chờ XN
                  </option>

                  <option value="in_progress">
                    Đang khám / Lấy mẫu
                  </option>

                  <option value="testing">
                    Đang xét nghiệm / Chờ duyệt
                  </option>

                  <option value="has_result">
                    Đã có kết quả
                  </option>

                  <option value="completed">
                    Hoàn thành
                  </option>

                  <option value="cancelled">
                    Đã hủy / Không đến
                  </option>
                </select>
              </div>

              <div className="col-lg-5 col-md-12">
                <div className="input-group">
                  <span className="input-group-text bg-light text-muted">
                    Từ
                  </span>

                  <input
                    type="date"
                    className="form-control text-secondary"
                    value={
                      startDate
                    }
                    onChange={(e) =>
                      setStartDate(
                        e.target.value
                      )
                    }
                  />

                  <span className="input-group-text bg-light text-muted border-start-0 border-end-0">
                    đến
                  </span>

                  <input
                    type="date"
                    className="form-control text-secondary"
                    value={
                      endDate
                    }
                    onChange={(e) =>
                      setEndDate(
                        e.target.value
                      )
                    }
                  />
                </div>
              </div>
            </div>

            {/* Content */}

            {loading ? (
              <div className="text-center py-5 text-secondary">
                <div
                  className="spinner-border text-primary mb-2"
                  role="status"
                />

                <div>
                  Đang tải lịch hẹn...
                </div>
              </div>
            ) : filteredAppointments.length ===
              0 ? (
              <div className="text-center py-5 text-muted">
                <i className="fa-regular fa-calendar-xmark fs-1 mb-3 d-block" />

                Chưa có lịch hẹn phù hợp.
              </div>
            ) : (
              <>
                <div className="table-responsive">
                  <table
                    className="table table-hover align-middle mb-0"
                    style={{
                      fontSize:
                        '0.95rem',
                    }}
                  >
                    <thead className="table-light">
                      <tr>
                        <th className="text-center text-nowrap">
                          #
                        </th>

                        <th className="text-nowrap">
                          Mã đặt lịch
                        </th>

                        <th className="text-nowrap">
                          Ngày
                        </th>

                        <th className="text-nowrap">
                          Giờ
                        </th>

                        <th>
                          Phân loại
                        </th>

                        <th>
                          Bệnh nhân
                        </th>

                        <th>
                          Bác sĩ
                        </th>

                        <th className="text-center text-nowrap">
                          Trạng thái
                        </th>

                        <th className="text-center text-nowrap">
                          Thao tác
                        </th>
                      </tr>
                    </thead>

                    <tbody>
                      {currentItems.map(
                        (
                          item,
                          index
                        ) => {
                          const appointmentDate =
                            normalizeDate(
                              item?.appointmentDate
                            );

                          const isPast =
                            Boolean(
                              appointmentDate &&
                              appointmentDate <
                                today
                            );

                          const itemStatus =
                            String(
                              item?.status ||
                              ''
                            ).toLowerCase();

                          const canEdit =
                            !isPast &&
                            [
                              'pending',
                              'confirmed',
                            ].includes(
                              itemStatus
                            );

                          return (
                            <tr
                              key={
                                item?.id ||
                                `${indexOfFirstItem}-${index}`
                              }
                            >
                              <td className="text-center text-muted">
                                {indexOfFirstItem +
                                  index +
                                  1}
                              </td>

                              <td className="text-nowrap">
                                <strong className="text-dark">
                                  {item?.id ||
                                    '—'}
                                </strong>
                              </td>

                              <td className="text-nowrap">
                                {formatDate(
                                  item?.appointmentDate
                                )}
                              </td>

                              <td className="text-nowrap">
                                {item?.appointmentTime
                                  ? String(
                                      item.appointmentTime
                                    ).substring(
                                      0,
                                      5
                                    )
                                  : '—'}
                              </td>

                              <td>
                                {item?.type ===
                                'EXAMINATION' ? (
                                  <span className="text-primary fw-medium">
                                    <i className="fa-solid fa-stethoscope me-2" />
                                    Khám bệnh
                                  </span>
                                ) : (
                                  <span className="text-success fw-medium">
                                    <i className="fa-solid fa-flask me-2" />
                                    Xét nghiệm
                                  </span>
                                )}
                              </td>

                              <td>
                                {item?.customerName ||
                                  '—'}
                              </td>

                              <td>
                                {item?.doctorName ||
                                  'Được sắp xếp khi đến'}
                              </td>

                              <td className="text-center text-nowrap">
                                {renderStatusBadge(
                                  item?.status,
                                  item?.type
                                )}
                              </td>

                              <td className="text-center text-nowrap">
                                <Link
                                  className="btn btn-sm btn-outline-info rounded-pill px-3 me-2"
                                  to={`/lich-hen/${encodeURIComponent(
                                    item.id
                                  )}`}
                                >
                                  Chi tiết
                                </Link>

                                {canEdit ? (
                                  <Link
                                    className="btn btn-sm btn-outline-primary rounded-pill px-3"
                                    to={`/lich-hen/sua?id=${encodeURIComponent(
                                      item.id
                                    )}`}
                                  >
                                    Đổi lịch
                                  </Link>
                                ) : (
                                  <button
                                    type="button"
                                    className="btn btn-sm btn-outline-secondary rounded-pill px-3"
                                    title="Lịch đã qua ngày hoặc không thể thay đổi"
                                    disabled
                                  >
                                    Đổi lịch
                                  </button>
                                )}
                              </td>
                            </tr>
                          );
                        }
                      )}
                    </tbody>
                  </table>
                </div>

                {/* Pagination */}

                {totalPages >
                  1 && (
                  <div className="d-flex justify-content-between align-items-center mt-4">
                    <span className="text-muted small">
                      Hiển thị{' '}
                      {indexOfFirstItem +
                        1}
                      {' - '}
                      {Math.min(
                        indexOfLastItem,
                        filteredAppointments.length
                      )}
                      {' trong tổng số '}
                      {
                        filteredAppointments.length
                      }
                      {' lịch hẹn'}
                    </span>

                    <nav>
                      <ul className="pagination pagination-sm mb-0">
                        <li
                          className={`page-item ${
                            safeCurrentPage ===
                            1
                              ? 'disabled'
                              : ''
                          }`}
                        >
                          <button
                            type="button"
                            className="page-link"
                            onClick={() =>
                              handlePageChange(
                                1
                              )
                            }
                            disabled={
                              safeCurrentPage ===
                              1
                            }
                          >
                            &laquo;
                          </button>
                        </li>

                        <li
                          className={`page-item ${
                            safeCurrentPage ===
                            1
                              ? 'disabled'
                              : ''
                          }`}
                        >
                          <button
                            type="button"
                            className="page-link"
                            onClick={() =>
                              handlePageChange(
                                safeCurrentPage -
                                  1
                              )
                            }
                            disabled={
                              safeCurrentPage ===
                              1
                            }
                          >
                            &lsaquo;
                          </button>
                        </li>

                        {Array.from(
                          {
                            length:
                              totalPages,
                          },
                          (_, i) =>
                            i + 1
                        ).map(
                          (page) => (
                            <li
                              key={
                                page
                              }
                              className={`page-item ${
                                safeCurrentPage ===
                                page
                                  ? 'active'
                                  : ''
                              }`}
                            >
                              <button
                                type="button"
                                className="page-link"
                                onClick={() =>
                                  handlePageChange(
                                    page
                                  )
                                }
                              >
                                {page}
                              </button>
                            </li>
                          )
                        )}

                        <li
                          className={`page-item ${
                            safeCurrentPage ===
                            totalPages
                              ? 'disabled'
                              : ''
                          }`}
                        >
                          <button
                            type="button"
                            className="page-link"
                            onClick={() =>
                              handlePageChange(
                                safeCurrentPage +
                                  1
                              )
                            }
                            disabled={
                              safeCurrentPage ===
                              totalPages
                            }
                          >
                            &rsaquo;
                          </button>
                        </li>

                        <li
                          className={`page-item ${
                            safeCurrentPage ===
                            totalPages
                              ? 'disabled'
                              : ''
                          }`}
                        >
                          <button
                            type="button"
                            className="page-link"
                            onClick={() =>
                              handlePageChange(
                                totalPages
                              )
                            }
                            disabled={
                              safeCurrentPage ===
                              totalPages
                            }
                          >
                            &raquo;
                          </button>
                        </li>
                      </ul>
                    </nav>
                  </div>
                )}
              </>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}