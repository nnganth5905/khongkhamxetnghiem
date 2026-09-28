import React, {
  useEffect,
  useState,
} from 'react';

import {
  Link,
  useNavigate,
  useParams,
} from 'react-router-dom';

import {
  getApiErrorMessage,
  getAppointmentDetail,
} from '../../services/appointmentService';

// =====================================================
// DATE HELPERS
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

/*
 * QUAN TRỌNG:
 * Không dùng new Date(value) ở đây.
 *
 * Backend đang trả thời gian nghiệp vụ MySQL
 * theo giờ Việt Nam:
 *
 * 2026-09-24 14:59:33
 * hoặc
 * 2026-09-24T14:59:33
 *
 * Ta chỉ format text, không convert timezone lần nữa.
 */
const formatDateTime = (value) => {
  if (!value) {
    return '—';
  }

  const text =
    String(value)
      .trim()
      .replace('T', ' ');

  const match =
    text.match(
      /^(\d{4})-(\d{2})-(\d{2})(?:\s+(\d{2}):(\d{2})(?::(\d{2}))?)?/
    );

  if (!match) {
    return text;
  }

  const [
    ,
    year,
    month,
    day,
    hour,
    minute,
  ] = match;

  const date =
    `${day}/${month}/${year}`;

  if (
    hour === undefined ||
    minute === undefined
  ) {
    return date;
  }

  return `${hour}:${minute} - ${date}`;
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

    case 'rescheduled':
    case 'da_doi_lich':
      return (
        <span className="badge bg-warning text-dark px-3 py-2 rounded-pill">
          Đã đổi lịch
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

export default function LichHenChiTiet() {
  const {
    id,
  } = useParams();

  const navigate =
    useNavigate();

  const [
    appointment,
    setAppointment,
  ] = useState(null);

  const [
    loading,
    setLoading,
  ] = useState(true);

  const [
    error,
    setError,
  ] = useState('');

  // =====================================================
  // LOAD DETAIL
  // =====================================================

  useEffect(() => {
    let active = true;

    const fetchDetail =
      async () => {
        try {
          setLoading(true);
          setError('');

          const data =
            await getAppointmentDetail(
              id
            );

          if (!active) {
            return;
          }

          if (
            !data ||
            data.error
          ) {
            setError(
              data?.error ||
              'Không tìm thấy lịch hẹn.'
            );

            return;
          }

          setAppointment(
            data
          );
        } catch (err) {
          if (!active) {
            return;
          }

          setError(
            getApiErrorMessage(
              err,
              'Không thể tải thông tin chi tiết lịch hẹn.'
            )
          );
        } finally {
          if (active) {
            setLoading(false);
          }
        }
      };

    if (id) {
      fetchDetail();
    } else {
      setError(
        'Mã lịch hẹn không hợp lệ.'
      );

      setLoading(false);
    }

    return () => {
      active = false;
    };
  }, [id]);

  // =====================================================
  // LOADING
  // =====================================================

  if (loading) {
    return (
      <div className="bg-light min-vh-100 py-5 d-flex justify-content-center align-items-center">
        <div className="text-center text-secondary">
          <div
            className="spinner-border text-primary mb-3"
            role="status"
          />

          <h5>
            Đang tải chi tiết lịch hẹn...
          </h5>
        </div>
      </div>
    );
  }

  // =====================================================
  // ERROR
  // =====================================================

  if (
    error ||
    !appointment
  ) {
    return (
      <div className="bg-light min-vh-100 py-5">
        <div className="container-fluid px-md-5">
          <div className="alert alert-danger shadow-sm">
            <h5 className="alert-heading">
              Lỗi!
            </h5>

            <p className="mb-0">
              {error ||
                'Không tìm thấy lịch hẹn.'}
            </p>
          </div>

          <button
            type="button"
            className="btn btn-outline-secondary mt-3"
            onClick={() =>
              navigate(-1)
            }
          >
            <i className="fa-solid fa-arrow-left me-2" />
            Quay lại
          </button>
        </div>
      </div>
    );
  }

  // =====================================================
  // DATA
  // =====================================================

  const appointmentDate =
    normalizeDate(
      appointment.date ||
      appointment.appointmentDate
    );

  const isPast =
    Boolean(
      appointmentDate &&
      appointmentDate <
        getLocalToday()
    );

  const statusLower =
    String(
      appointment.status ||
      ''
    ).toLowerCase();

  const canEdit =
    !isPast &&
    [
      'pending',
      'confirmed',
    ].includes(
      statusLower
    );

  const appointmentTime =
    appointment.time ||
    appointment.appointmentTime ||
    '';

  const testOrders =
    Array.isArray(
      appointment.testOrders
    )
      ? appointment.testOrders
      : [];

  const registeredTests =
    Array.isArray(
      appointment.registeredTests
    )
      ? appointment.registeredTests
      : [];

  const timeline =
    Array.isArray(
      appointment.timeline
    )
      ? appointment.timeline
      : [];

  // =====================================================
  // RENDER
  // =====================================================

  return (
    <div className="bg-light min-vh-100 py-4">
      <div className="container-fluid px-md-5">
        {/* Header */}

        <div className="d-flex justify-content-between align-items-center mb-4 flex-wrap gap-3">
          <div className="d-flex align-items-center">
            <button
              type="button"
              className="btn btn-white border shadow-sm rounded-circle me-3"
              onClick={() =>
                navigate(
                  '/lich-hen'
                )
              }
            >
              <i className="fa-solid fa-arrow-left" />
            </button>

            <div>
              <h3
                className="fw-bold mb-1"
                style={{
                  color:
                    '#0b63e5',
                }}
              >
                Chi tiết lịch hẹn:{' '}
                {appointment.id}
              </h3>

              <p className="text-muted mb-0">
                Theo dõi thông tin và tiến trình khám/xét nghiệm của bạn.
              </p>
            </div>
          </div>

          {canEdit && (
            <Link
              to={`/lich-hen/sua?id=${encodeURIComponent(
                id
              )}`}
              className="btn btn-outline-primary rounded-pill px-4 shadow-sm"
            >
              <i className="fa-solid fa-pen me-2" />
              Thay đổi / Hủy lịch
            </Link>
          )}
        </div>

        <div className="row g-4">
          {/* Left */}

          <div className="col-lg-7 col-md-12">
            {/* General info */}

            <div className="card border-0 shadow-sm rounded-3 mb-4">
              <div className="card-header bg-white border-bottom-0 pt-4 pb-0 px-4">
                <h5 className="fw-bold text-dark">
                  Thông tin chung
                </h5>
              </div>

              <div className="card-body p-4">
                <div className="row mb-3">
                  <div className="col-sm-4 text-muted">
                    Mã đặt lịch:
                  </div>

                  <div className="col-sm-8 fw-bold">
                    {appointment.id}
                  </div>
                </div>

                <hr className="text-muted opacity-25" />

                <div className="row mb-3">
                  <div className="col-sm-4 text-muted">
                    Trạng thái hiện tại:
                  </div>

                  <div className="col-sm-8">
                    {renderStatusBadge(
                      appointment.status,
                      appointment.type
                    )}
                  </div>
                </div>

                <hr className="text-muted opacity-25" />

                <div className="row mb-3">
                  <div className="col-sm-4 text-muted">
                    Phân loại:
                  </div>

                  <div className="col-sm-8 fw-medium">
                    {appointment.type ===
                    'EXAMINATION' ? (
                      <span className="text-primary">
                        <i className="fa-solid fa-stethoscope me-2" />
                        Khám bệnh
                      </span>
                    ) : (
                      <span className="text-success">
                        <i className="fa-solid fa-flask me-2" />
                        Xét nghiệm
                      </span>
                    )}
                  </div>
                </div>

                <div className="row mb-3">
                  <div className="col-sm-4 text-muted">
                    Ngày thực hiện:
                  </div>

                  <div className="col-sm-8 fw-medium">
                    {formatDate(
                      appointmentDate
                    )}
                  </div>
                </div>

                <div className="row mb-3">
                  <div className="col-sm-4 text-muted">
                    Giờ dự kiến:
                  </div>

                  <div className="col-sm-8 fw-medium">
                    {appointmentTime
                      ? String(
                          appointmentTime
                        ).substring(
                          0,
                          5
                        )
                      : '—'}
                  </div>
                </div>

                <div className="row mb-3">
                  <div className="col-sm-4 text-muted">
                    Bệnh nhân:
                  </div>

                  <div className="col-sm-8 fw-medium">
                    {appointment.customerName ||
                      '—'}
                  </div>
                </div>

                {appointment.customerPhone && (
                  <div className="row mb-3">
                    <div className="col-sm-4 text-muted">
                      Số điện thoại:
                    </div>

                    <div className="col-sm-8 fw-medium">
                      {
                        appointment.customerPhone
                      }
                    </div>
                  </div>
                )}

                <div className="row mb-3">
                  <div className="col-sm-4 text-muted">
                    Bác sĩ phụ trách:
                  </div>

                  <div className="col-sm-8 fw-medium">
                    {appointment.doctorName ||
                      'Được sắp xếp khi đến nơi'}
                  </div>
                </div>

                {appointment.facilityId && (
                  <div className="row mb-3">
                    <div className="col-sm-4 text-muted">
                      Cơ sở:
                    </div>

                    <div className="col-sm-8 fw-medium">
                      {
                        appointment.facilityId
                      }
                    </div>
                  </div>
                )}

                <div className="row mb-0">
                  <div className="col-sm-4 text-muted">
                    Ghi chú:
                  </div>

                  <div
                    className="col-sm-8 fw-medium"
                    style={{
                      whiteSpace:
                        'pre-line',
                    }}
                  >
                    {appointment.note ||
                      'Không có ghi chú'}
                  </div>
                </div>
              </div>
            </div>

            {/* TEST appointment registered tests */}

            {appointment.type ===
              'TEST' &&
              registeredTests.length >
                0 && (
                <div className="card border-0 shadow-sm rounded-3 mb-4">
                  <div className="card-header bg-white border-bottom-0 pt-4 pb-0 px-4">
                    <h5 className="fw-bold text-dark">
                      Xét nghiệm đã đăng ký
                    </h5>
                  </div>

                  <div className="card-body p-4">
                    <div className="d-flex flex-wrap gap-2">
                      {registeredTests.map(
                        (
                          testName,
                          index
                        ) => (
                          <span
                            key={`${testName}-${index}`}
                            className="badge bg-success-subtle text-success border border-success-subtle p-2"
                          >
                            <i className="fa-solid fa-flask me-2" />
                            {testName}
                          </span>
                        )
                      )}
                    </div>
                  </div>
                </div>
              )}

            {/* Test orders from examination */}

            {testOrders.length >
              0 && (
              <div className="card border-0 shadow-sm rounded-3 mb-4">
                <div className="card-header bg-white border-bottom-0 pt-4 pb-0 px-4">
                  <h5 className="fw-bold text-dark">
                    Chỉ định xét nghiệm đi kèm
                  </h5>
                </div>

                <div className="card-body p-4">
                  {testOrders.map(
                    (
                      order,
                      idx
                    ) => {
                      const orderCode =
                        order?.idPhieuXetNghiem ??
                        order?.IDPhieuXetNghiem ??
                        '—';

                      const orderStatus =
                        order?.trangThai ??
                        order?.TrangThai ??
                        '';

                      const createdAt =
                        order?.ngayTao ??
                        order?.NgayTao ??
                        null;

                      const tests =
                        Array.isArray(
                          order?.tests
                        )
                          ? order.tests
                          : Array.isArray(
                                order?.Tests
                              )
                            ? order.Tests
                            : [];

                      return (
                        <div
                          key={`${orderCode}-${idx}`}
                          className="p-3 mb-3 bg-light rounded border"
                        >
                          <div className="d-flex justify-content-between align-items-center gap-3 mb-2">
                            <strong className="text-primary">
                              Mã phiếu:{' '}
                              {
                                orderCode
                              }
                            </strong>

                            {renderStatusBadge(
                              orderStatus,
                              'TEST'
                            )}
                          </div>

                          <div className="text-muted small mb-2">
                            Ngày tạo:{' '}
                            {formatDateTime(
                              createdAt
                            )}
                          </div>

                          {tests.length >
                          0 ? (
                            <div className="d-flex flex-wrap gap-2 mt-2">
                              {tests.map(
                                (
                                  testName,
                                  index
                                ) => (
                                  <span
                                    key={`${testName}-${index}`}
                                    className="badge bg-white text-dark border p-2"
                                  >
                                    {
                                      testName
                                    }
                                  </span>
                                )
                              )}
                            </div>
                          ) : (
                            <em className="text-muted small">
                              Chưa có danh sách xét nghiệm
                            </em>
                          )}
                        </div>
                      );
                    }
                  )}
                </div>
              </div>
            )}
          </div>

          {/* Right timeline */}

          <div className="col-lg-5 col-md-12">
            <div className="card border-0 shadow-sm rounded-3">
              <div className="card-header bg-white border-bottom-0 pt-4 pb-0 px-4">
                <h5 className="fw-bold text-dark">
                  Lịch sử trạng thái
                </h5>
              </div>

              <div className="card-body p-4">
                {timeline.length ===
                0 ? (
                  <div className="text-center text-muted py-4">
                    <i className="fa-regular fa-clock fs-2 mb-3 d-block" />

                    Chưa có dữ liệu lịch sử.
                  </div>
                ) : (
                  <div
                    className="position-relative ms-3 mt-2"
                    style={{
                      borderLeft:
                        '2px solid #dee2e6',
                    }}
                  >
                    {timeline.map(
                      (
                        item,
                        idx
                      ) => {
                        const isLatest =
                          idx ===
                          timeline.length -
                            1;

                        return (
                          <div
                            key={`${item?.time || ''}-${item?.event || ''}-${idx}`}
                            className="position-relative mb-4"
                            style={{
                              paddingLeft:
                                '1.5rem',
                            }}
                          >
                            <div
                              className={`position-absolute rounded-circle ${
                                isLatest
                                  ? 'bg-primary'
                                  : 'bg-secondary'
                              }`}
                              style={{
                                width:
                                  '14px',

                                height:
                                  '14px',

                                left:
                                  '-8px',

                                top:
                                  '2px',

                                border:
                                  '2px solid #fff',

                                boxShadow:
                                  isLatest
                                    ? '0 0 0 3px rgba(11, 99, 229, 0.2)'
                                    : 'none',
                              }}
                            />

                            <div
                              className={`small mb-1 ${
                                isLatest
                                  ? 'text-primary fw-medium'
                                  : 'text-muted'
                              }`}
                            >
                              {formatDateTime(
                                item?.time
                              )}
                            </div>

                            <div
                              className={`fw-medium ${
                                isLatest
                                  ? 'text-dark'
                                  : 'text-secondary'
                              }`}
                            >
                              {item?.event ||
                                'Cập nhật trạng thái'}
                            </div>
                          </div>
                        );
                      }
                    )}
                  </div>
                )}
              </div>
            </div>

            {/* QR */}

            {appointment.qrCode && (
              <div className="card border-0 shadow-sm rounded-3 mt-4">
                <div className="card-body p-4">
                  <h6 className="fw-bold mb-2">
                    Mã check-in
                  </h6>

                  <div className="bg-light border rounded p-3 text-center">
                    <i className="fa-solid fa-qrcode fs-2 text-primary mb-2 d-block" />

                    <code>
                      {
                        appointment.qrCode
                      }
                    </code>
                  </div>

                  <small className="text-muted d-block mt-2">
                    Mã này sẽ được sử dụng trong quy trình tiếp nhận/check-in.
                  </small>
                </div>
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}