// src/pages/tracking/TheoDoiLuotKham.jsx

import React, {
  useEffect,
  useMemo,
  useState,
} from 'react';

import {
  useSearchParams,
} from 'react-router-dom';

import Loading from '../../components/Loading';
import Notification from '../../components/Notification';

import {
  getMyVisits,
  getVisitTracking,
  getTrackingStatusLabel,
  getTrackingEventLabel,
  normalizeTimeline,
} from '../../services/trackingService';

import {
  getApiErrorMessage,
} from '../../services/api';

// ============================================================
// FORMAT
// ============================================================

const formatDate = (value) => {
  if (!value) {
    return '—';
  }

  const text =
    String(value).substring(0, 10);

  const parts =
    text.split('-');

  if (parts.length !== 3) {
    return text;
  }

  return `${parts[2]}/${parts[1]}/${parts[0]}`;
};

const formatTime = (value) => {
  if (!value) {
    return '—';
  }

  const text =
    String(value);

  if (
    text.includes('T') ||
    text.includes(' ')
  ) {
    const date =
      new Date(text);

    if (!Number.isNaN(date.getTime())) {
      return date.toLocaleTimeString(
        'vi-VN',
        {
          hour: '2-digit',
          minute: '2-digit',
        }
      );
    }
  }

  return text.substring(0, 5);
};

const formatDateTime = (value) => {
  if (!value) {
    return '—';
  }

  const date =
    new Date(value);

  if (!Number.isNaN(date.getTime())) {
    return date.toLocaleString(
      'vi-VN',
      {
        hour: '2-digit',
        minute: '2-digit',
        day: '2-digit',
        month: '2-digit',
        year: 'numeric',
      }
    );
  }

  return String(value);
};

// ============================================================
// STATUS
// ============================================================

const statusClass = (status) => {
  const value =
    String(status || '')
      .toLowerCase();

  switch (value) {
    case 'da_tiep_nhan':
      return 'bg-primary-subtle text-primary';

    case 'cho_kham':
      return 'bg-warning-subtle text-warning-emphasis';

    case 'da_den_luot':
      return 'bg-info-subtle text-info-emphasis';

    case 'cho_goi_lai':
      return 'bg-secondary-subtle text-secondary';

    case 'dang_kham':
      return 'bg-primary text-white';

    case 'da_chi_dinh_xn':
      return 'bg-warning text-dark';

    case 'moi_doc_kq':
      return 'bg-info text-dark';

    case 'dang_tu_van_kq':
      return 'bg-info-subtle text-info-emphasis';

    case 'hoan_tat':
      return 'bg-success text-white';

    case 'bo_luot':
      return 'bg-danger-subtle text-danger';

    default:
      return 'bg-secondary-subtle text-secondary';
  }
};

// ============================================================
// TIMELINE ICON
// ============================================================

const getEventIcon = (code) => {
  switch (
    String(code || '').toUpperCase()
  ) {
    case 'BOOKED':
      return 'fa-calendar-check';

    case 'APPOINTMENT_RESCHEDULED':
      return 'fa-calendar-days';

    case 'APPOINTMENT_CANCELLED':
      return 'fa-calendar-xmark';

    case 'CHECKED_IN':
      return 'fa-clipboard-check';

    case 'CALLED':
      return 'fa-bell';

    case 'SKIPPED':
      return 'fa-clock-rotate-left';

    case 'EXAM_STARTED':
      return 'fa-user-doctor';

    case 'EXAM_COMPLETED':
      return 'fa-circle-check';

    case 'SPECIMEN_COLLECTED':
      return 'fa-vial';

    case 'RESULT_AVAILABLE':
      return 'fa-file-medical';

    default:
      return 'fa-circle';
  }
};

// ============================================================
// COMPONENT
// ============================================================

export default function TheoDoiLuotKham() {
  const [
    searchParams,
    setSearchParams,
  ] = useSearchParams();

  const queryId =
    searchParams.get('id') || '';

  const [visits, setVisits] =
    useState([]);

  const [selectedId, setSelectedId] =
    useState(queryId);

  const [tracking, setTracking] =
    useState(null);

  const [keyword, setKeyword] =
    useState('');

  const [loadingList, setLoadingList] =
    useState(true);

  const [loadingDetail, setLoadingDetail] =
    useState(false);

  const [message, setMessage] =
    useState({
      type: '',
      text: '',
    });

  // ==========================================================
  // LOAD MY VISITS
  // ==========================================================

  const loadVisits = async () => {
    try {
      setLoadingList(true);

      const data =
        await getMyVisits();

      setVisits(
        Array.isArray(data)
          ? data
          : []
      );
    } catch (err) {
      /*
       * Không coi đây là lỗi fatal.
       * Nhân viên nội bộ vẫn có thể nhập mã lượt trực tiếp.
       */
      setVisits([]);

      const status =
        err?.response?.status;

      if (
        status !== 401 &&
        status !== 403
      ) {
        setMessage({
          type: 'danger',
          text: getApiErrorMessage(
            err,
            'Không thể tải danh sách lượt khám.'
          ),
        });
      }
    } finally {
      setLoadingList(false);
    }
  };

  // ==========================================================
  // LOAD DETAIL
  // ==========================================================

  const loadTracking = async (id) => {
    const normalizedId =
      String(id || '').trim();

    if (!normalizedId) {
      setTracking(null);
      return;
    }

    try {
      setLoadingDetail(true);

      setMessage({
        type: '',
        text: '',
      });

      const data =
        await getVisitTracking(
          normalizedId
        );

      setTracking({
        ...data,

        timeline:
          normalizeTimeline(
            data?.timeline || []
          ),
      });

      setSelectedId(
        normalizedId
      );

      setSearchParams({
        id: normalizedId,
      });
    } catch (err) {
      setTracking(null);

      setMessage({
        type: 'danger',
        text: getApiErrorMessage(
          err,
          'Không thể tải thông tin lượt khám.'
        ),
      });
    } finally {
      setLoadingDetail(false);
    }
  };

  // ==========================================================
  // EFFECT
  // ==========================================================

  useEffect(() => {
    loadVisits();
  }, []);

  useEffect(() => {
    if (queryId) {
      loadTracking(
        queryId
      );
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [queryId]);

  // ==========================================================
  // FILTER
  // ==========================================================

  const filteredVisits =
    useMemo(() => {
      const q =
        keyword
          .trim()
          .toLowerCase();

      if (!q) {
        return visits;
      }

      return visits.filter(
        (item) =>
          [
            item.id,
            item.appointmentId,
            item.doctorName,
            item.roomName,
            item.status,
          ]
            .filter(Boolean)
            .join(' ')
            .toLowerCase()
            .includes(q)
      );
    }, [
      visits,
      keyword,
    ]);

  // ==========================================================
  // SEARCH
  // ==========================================================

  const handleSearch = (e) => {
    e.preventDefault();

    if (!selectedId.trim()) {
      setMessage({
        type: 'warning',
        text:
          'Vui lòng nhập mã lượt khám hoặc mã đặt lịch.',
      });

      return;
    }

    loadTracking(
      selectedId
    );
  };

  // ==========================================================
  // RENDER
  // ==========================================================

  return (
    <div className="container-fluid py-4">
      <div className="mb-4">
        <h1 className="dashboard-page-title mb-1">
          Theo dõi lượt khám
        </h1>

        <p className="text-secondary mb-0">
          Theo dõi toàn bộ tiến trình khám bệnh từ lúc đặt lịch,
          check-in đến khi hoàn tất.
        </p>
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

      {/* =====================================================
          SEARCH
      ====================================================== */}

      <div className="card border-0 shadow-sm rounded-4 mb-4">
        <div className="card-body p-4">
          <form
            className="row g-3 align-items-end"
            onSubmit={handleSearch}
          >
            <div className="col-lg-9">
              <label className="form-label fw-semibold">
                Mã lượt khám / mã đặt lịch
              </label>

              <input
                type="text"
                className="form-control"
                placeholder="Ví dụ: 12 hoặc DLK..."
                value={selectedId}
                onChange={(e) =>
                  setSelectedId(
                    e.target.value
                  )
                }
              />
            </div>

            <div className="col-lg-3">
              <button
                type="submit"
                className="btn btn-primary w-100"
                disabled={loadingDetail}
              >
                <i className="fa-solid fa-magnifying-glass me-2" />

                {loadingDetail
                  ? 'Đang tìm...'
                  : 'Theo dõi'}
              </button>
            </div>
          </form>
        </div>
      </div>

      {/* =====================================================
          MY VISITS
      ====================================================== */}

      {!loadingList &&
        visits.length > 0 && (
          <div className="card border-0 shadow-sm rounded-4 mb-4">
            <div className="card-body p-4">
              <div className="d-flex flex-wrap justify-content-between align-items-center gap-3 mb-3">
                <div>
                  <h5 className="fw-bold mb-1">
                    Lượt khám của bạn
                  </h5>

                  <div className="small text-secondary">
                    Chọn một lượt để xem tiến trình.
                  </div>
                </div>

                <input
                  type="search"
                  className="form-control"
                  style={{
                    maxWidth: 320,
                  }}
                  placeholder="Tìm mã lịch, bác sĩ..."
                  value={keyword}
                  onChange={(e) =>
                    setKeyword(
                      e.target.value
                    )
                  }
                />
              </div>

              <div className="table-responsive">
                <table className="table table-hover align-middle mb-0">
                  <thead className="table-light">
                    <tr>
                      <th>Mã lịch</th>
                      <th>Ngày</th>
                      <th>Giờ</th>
                      <th>Bác sĩ</th>
                      <th>Phòng</th>
                      <th>STT</th>
                      <th>Trạng thái</th>
                      <th />
                    </tr>
                  </thead>

                  <tbody>
                    {filteredVisits.map(
                      (item) => (
                        <tr key={item.id}>
                          <td className="fw-semibold text-primary">
                            {item.appointmentId ||
                              item.id}
                          </td>

                          <td>
                            {formatDate(
                              item.date
                            )}
                          </td>

                          <td>
                            {formatTime(
                              item.time
                            )}
                          </td>

                          <td>
                            {item.doctorName ||
                              '—'}
                          </td>

                          <td>
                            {item.roomName ||
                              '—'}
                          </td>

                          <td>
                            {item.queueNumber ??
                              '—'}
                          </td>

                          <td>
                            <span
                              className={`badge ${statusClass(
                                item.status
                              )}`}
                            >
                              {getTrackingStatusLabel(
                                item.status
                              )}
                            </span>
                          </td>

                          <td className="text-end">
                            <button
                              type="button"
                              className="btn btn-sm btn-outline-primary"
                              onClick={() =>
                                loadTracking(
                                  item.id
                                )
                              }
                            >
                              Xem tiến trình
                            </button>
                          </td>
                        </tr>
                      )
                    )}
                  </tbody>
                </table>
              </div>
            </div>
          </div>
        )}

      {loadingList &&
        !tracking && (
          <Loading text="Đang tải lượt khám..." />
        )}

      {/* =====================================================
          DETAIL
      ====================================================== */}

      {loadingDetail ? (
        <Loading text="Đang tải tiến trình khám..." />
      ) : tracking ? (
        <>
          <div className="card border-0 shadow-sm rounded-4 mb-4">
            <div className="card-body p-4">
              <div className="d-flex flex-wrap justify-content-between align-items-start gap-3">
                <div>
                  <div className="small text-secondary">
                    Mã đặt lịch
                  </div>

                  <h4 className="fw-bold text-primary mb-1">
                    {tracking.appointmentId ||
                      '—'}
                  </h4>

                  <div className="text-secondary">
                    Lượt khám #{tracking.id}
                  </div>
                </div>

                <span
                  className={`badge fs-6 ${statusClass(
                    tracking.status
                  )}`}
                >
                  {getTrackingStatusLabel(
                    tracking.status
                  )}
                </span>
              </div>

              <hr />

              <div className="row g-3">
                <Info
                  label="Người bệnh"
                  value={
                    tracking.customerName
                  }
                />

                <Info
                  label="Mã khách hàng"
                  value={
                    tracking.patientCode
                  }
                />

                <Info
                  label="Ngày khám"
                  value={formatDate(
                    tracking.date
                  )}
                />

                <Info
                  label="Giờ khám"
                  value={formatTime(
                    tracking.time
                  )}
                />

                <Info
                  label="Bác sĩ"
                  value={
                    tracking.doctorName
                  }
                />

                <Info
                  label="Phòng"
                  value={
                    tracking.roomName
                  }
                />

                <Info
                  label="Số thứ tự"
                  value={
                    tracking.queueNumber
                  }
                />

                <Info
                  label="Check-in"
                  value={formatDateTime(
                    tracking.receivedAt
                  )}
                />
              </div>
            </div>
          </div>

          {/* TIMELINE */}

          <div className="card border-0 shadow-sm rounded-4">
            <div className="card-body p-4">
              <h5 className="fw-bold mb-4">
                Tiến trình khám bệnh
              </h5>

              <Timeline
                items={
                  tracking.timeline ||
                  []
                }
              />
            </div>
          </div>
        </>
      ) : (
        !loadingList && (
          <div className="card border-0 shadow-sm rounded-4">
            <div className="card-body text-center py-5 text-secondary">
              <i className="fa-solid fa-stethoscope fs-1 mb-3 d-block opacity-50" />

              Chọn một lượt khám hoặc nhập mã để xem tiến trình.
            </div>
          </div>
        )
      )}
    </div>
  );
}

// ============================================================
// INFO
// ============================================================

function Info({
  label,
  value,
}) {
  return (
    <div className="col-md-6 col-xl-3">
      <div className="small text-secondary mb-1">
        {label}
      </div>

      <div className="fw-semibold">
        {value ??
          '—'}
      </div>
    </div>
  );
}

// ============================================================
// TIMELINE
// ============================================================

function Timeline({
  items = [],
}) {
  if (!items.length) {
    return (
      <div className="text-center text-secondary py-4">
        Chưa có dữ liệu tiến trình.
      </div>
    );
  }

  return (
    <div>
      {items.map(
        (item, index) => (
          <div
            key={
              item.id ||
              `${item.time}-${index}`
            }
            className="d-flex gap-3"
          >
            <div
              className="d-flex flex-column align-items-center"
              style={{
                width: 42,
              }}
            >
              <div
                className="rounded-circle bg-primary text-white d-flex align-items-center justify-content-center flex-shrink-0"
                style={{
                  width: 36,
                  height: 36,
                }}
              >
                <i
                  className={`fa-solid ${getEventIcon(
                    item.code
                  )}`}
                />
              </div>

              {index <
                items.length - 1 && (
                <div
                  className="bg-secondary-subtle"
                  style={{
                    width: 2,
                    minHeight: 70,
                    flex: 1,
                  }}
                />
              )}
            </div>

            <div className="pb-4 flex-grow-1">
              <div className="d-flex flex-wrap justify-content-between gap-2">
                <div className="fw-bold">
                  {getTrackingEventLabel(
                    item.code,
                    item.event
                  )}
                </div>

                <small className="text-secondary">
                  {formatDateTime(
                    item.time
                  )}
                </small>
              </div>

              {item.description && (
                <div className="text-secondary small mt-1">
                  {item.description}
                </div>
              )}

              {(item.oldStatus ||
                item.newStatus) && (
                <div className="small mt-2">
                  {item.oldStatus && (
                    <span className="badge bg-secondary-subtle text-secondary me-2">
                      {getTrackingStatusLabel(
                        item.oldStatus
                      )}
                    </span>
                  )}

                  {item.oldStatus &&
                    item.newStatus && (
                      <i className="fa-solid fa-arrow-right text-secondary me-2" />
                    )}

                  {item.newStatus && (
                    <span className="badge bg-primary-subtle text-primary">
                      {getTrackingStatusLabel(
                        item.newStatus
                      )}
                    </span>
                  )}
                </div>
              )}
            </div>
          </div>
        )
      )}
    </div>
  );
}