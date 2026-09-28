// src/pages/tracking/TheoDoiXetNghiem.jsx

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
  getMyTestVisits,
  getTestTracking,
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

  return parts.length === 3
    ? `${parts[2]}/${parts[1]}/${parts[0]}`
    : text;
};

const formatTime = (value) => {
  if (!value) {
    return '—';
  }

  return String(value)
    .substring(0, 5);
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
  switch (
    String(status || '').toLowerCase()
  ) {
    case 'da_tiep_nhan':
      return 'bg-primary-subtle text-primary';

    case 'cho_xet_nghiem':
      return 'bg-warning-subtle text-warning-emphasis';

    case 'da_den_luot':
      return 'bg-info-subtle text-info-emphasis';

    case 'dang_lay_mau':
      return 'bg-info text-white';

    case 'da_lay_mau':
      return 'bg-primary text-white';

    case 'ktv_tiep_nhan':
      return 'bg-primary-subtle text-primary';

    case 'dang_xet_nghiem':
      return 'bg-warning text-dark';

    case 'cho_duyet_kq':
      return 'bg-warning-subtle text-warning-emphasis';

    case 'da_co_kq':
      return 'bg-success-subtle text-success';

    case 'moi_doc_kq':
      return 'bg-success-subtle text-success';

    case 'dang_tu_van_kq':
      return 'bg-info-subtle text-info-emphasis';

    case 'hoan_tat':
      return 'bg-success text-white';

    case 'huy':
      return 'bg-danger-subtle text-danger';

    default:
      return 'bg-secondary-subtle text-secondary';
  }
};

const eventIcon = (code) => {
  switch (
    String(code || '').toUpperCase()
  ) {
    case 'BOOKED':
      return 'fa-calendar-check';

    case 'CHECKED_IN':
      return 'fa-clipboard-check';

    case 'CALLED':
      return 'fa-bell';

    case 'STARTED':
      return 'fa-play';

    case 'SPECIMEN_COLLECTED':
      return 'fa-vial';

    case 'SPECIMEN_HANDED_OVER':
      return 'fa-truck-fast';

    case 'SPECIMEN_RECEIVED':
      return 'fa-box-open';

    case 'WORKLIST_RECEIVED':
      return 'fa-list-check';

    case 'TEST_IN_PROGRESS':
      return 'fa-flask-vial';

    case 'TEST_FINISHED':
      return 'fa-circle-check';

    case 'RESULT_ENTERED':
      return 'fa-keyboard';

    case 'RESULT_SUBMITTED':
      return 'fa-paper-plane';

    case 'RESULT_APPROVED':
      return 'fa-user-doctor';

    case 'RESULT_AVAILABLE':
      return 'fa-file-medical';

    case 'COMPLETED':
      return 'fa-flag-checkered';

    default:
      return 'fa-circle';
  }
};

// ============================================================
// COMPONENT
// ============================================================

export default function TheoDoiXetNghiem() {
  const [
    searchParams,
    setSearchParams,
  ] = useSearchParams();

  const queryId =
    searchParams.get('id') ||
    '';

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
  // LOAD LIST
  // ==========================================================

  const loadList = async () => {
    try {
      setLoadingList(true);

      const data =
        await getMyTestVisits();

      setVisits(
        Array.isArray(data)
          ? data
          : []
      );
    } catch (err) {
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
            'Không thể tải danh sách lượt xét nghiệm.'
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
      return;
    }

    try {
      setLoadingDetail(true);

      setMessage({
        type: '',
        text: '',
      });

      const data =
        await getTestTracking(
          normalizedId
        );

      setTracking({
        ...data,

        timeline:
          normalizeTimeline(
            data?.timeline ||
            []
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
          'Không thể tải tiến trình xét nghiệm.'
        ),
      });
    } finally {
      setLoadingDetail(false);
    }
  };

  useEffect(() => {
    loadList();
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

  const filtered =
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
      keyword,
      visits,
    ]);

  const handleSearch = (e) => {
    e.preventDefault();

    if (!selectedId.trim()) {
      setMessage({
        type: 'warning',
        text:
          'Vui lòng nhập mã lượt xét nghiệm hoặc mã đặt lịch.',
      });

      return;
    }

    loadTracking(
      selectedId
    );
  };

  return (
    <div className="container-fluid py-4">
      <div className="mb-4">
        <h1 className="dashboard-page-title mb-1">
          Theo dõi xét nghiệm
        </h1>

        <p className="text-secondary mb-0">
          Theo dõi quá trình từ check-in, lấy mẫu,
          xét nghiệm đến khi bác sĩ duyệt kết quả.
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

      {/* SEARCH */}

      <div className="card border-0 shadow-sm rounded-4 mb-4">
        <div className="card-body p-4">
          <form
            className="row g-3 align-items-end"
            onSubmit={handleSearch}
          >
            <div className="col-lg-9">
              <label className="form-label fw-semibold">
                Mã lượt xét nghiệm / mã đặt lịch
              </label>

              <input
                type="text"
                className="form-control"
                placeholder="Ví dụ: 10 hoặc DLXN..."
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
                className="btn btn-success w-100"
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

      {/* LIST */}

      {!loadingList &&
        visits.length > 0 && (
          <div className="card border-0 shadow-sm rounded-4 mb-4">
            <div className="card-body p-4">
              <div className="d-flex flex-wrap justify-content-between align-items-center gap-3 mb-3">
                <div>
                  <h5 className="fw-bold mb-1">
                    Lượt xét nghiệm của bạn
                  </h5>

                  <span className="small text-secondary">
                    Chọn một lượt để xem chi tiết.
                  </span>
                </div>

                <input
                  type="search"
                  className="form-control"
                  style={{
                    maxWidth: 320,
                  }}
                  placeholder="Tìm lượt xét nghiệm..."
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
                    {filtered.map(
                      (item) => (
                        <tr key={item.id}>
                          <td className="fw-semibold text-success">
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
                              className="btn btn-sm btn-outline-success"
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
          <Loading text="Đang tải lượt xét nghiệm..." />
        )}

      {/* DETAIL */}

      {loadingDetail ? (
        <Loading text="Đang tải tiến trình xét nghiệm..." />
      ) : tracking ? (
        <>
          <div className="card border-0 shadow-sm rounded-4 mb-4">
            <div className="card-body p-4">
              <div className="d-flex flex-wrap justify-content-between align-items-start gap-3">
                <div>
                  <div className="small text-secondary">
                    Mã đặt lịch
                  </div>

                  <h4 className="fw-bold text-success mb-1">
                    {tracking.appointmentId ||
                      '—'}
                  </h4>

                  <span className="text-secondary">
                    Lượt xét nghiệm #{tracking.id}
                  </span>
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
                  label="Ngày xét nghiệm"
                  value={formatDate(
                    tracking.date
                  )}
                />

                <Info
                  label="Giờ"
                  value={formatTime(
                    tracking.time
                  )}
                />

                <Info
                  label="Bác sĩ"
                  value={
                    tracking.doctorName ||
                    'Chưa phân công'
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

          {/* SPECIMEN */}

          {Array.isArray(
            tracking.specimens
          ) &&
            tracking.specimens.length >
              0 && (
              <div className="card border-0 shadow-sm rounded-4 mb-4">
                <div className="card-body p-4">
                  <h5 className="fw-bold mb-3">
                    Mẫu bệnh phẩm
                  </h5>

                  <div className="table-responsive">
                    <table className="table align-middle mb-0">
                      <thead className="table-light">
                        <tr>
                          <th>Mã mẫu</th>
                          <th>Barcode</th>
                          <th>Loại mẫu</th>
                          <th>Thời gian lấy</th>
                          <th>Trạng thái</th>
                        </tr>
                      </thead>

                      <tbody>
                        {tracking.specimens.map(
                          (
                            item,
                            index
                          ) => (
                            <tr
                              key={
                                item.IDMau ||
                                item.idMau ||
                                index
                              }
                            >
                              <td className="fw-semibold">
                                {item.IDMau ||
                                  item.idMau ||
                                  '—'}
                              </td>

                              <td>
                                {item.MaBarcode ||
                                  item.maBarcode ||
                                  '—'}
                              </td>

                              <td>
                                {item.LoaiMau ||
                                  item.loaiMau ||
                                  '—'}
                              </td>

                              <td>
                                {formatDateTime(
                                  item.ThoiGianLayMau ||
                                    item.thoiGianLayMau
                                )}
                              </td>

                              <td>
                                {item.TrangThai ||
                                  item.trangThai ||
                                  '—'}
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

          {/* RESULT */}

          {Array.isArray(
            tracking.results
          ) &&
            tracking.results.length >
              0 && (
              <div className="card border-0 shadow-sm rounded-4 mb-4">
                <div className="card-body p-4">
                  <h5 className="fw-bold mb-3">
                    Kết quả xét nghiệm
                  </h5>

                  <div className="table-responsive">
                    <table className="table align-middle mb-0">
                      <thead className="table-light">
                        <tr>
                          <th>Mã kết quả</th>
                          <th>Trạng thái</th>
                          <th>Thời gian nhập</th>
                          <th>Thời gian duyệt</th>
                          <th>Kết luận</th>
                        </tr>
                      </thead>

                      <tbody>
                        {tracking.results.map(
                          (
                            item,
                            index
                          ) => (
                            <tr
                              key={
                                item.IDKetQua ||
                                item.idKetQua ||
                                index
                              }
                            >
                              <td className="fw-semibold">
                                {item.IDKetQua ||
                                  item.idKetQua ||
                                  '—'}
                              </td>

                              <td>
                                {item.TrangThai ||
                                  item.trangThai ||
                                  '—'}
                              </td>

                              <td>
                                {formatDateTime(
                                  item.ThoiGianNhap ||
                                    item.thoiGianNhap
                                )}
                              </td>

                              <td>
                                {formatDateTime(
                                  item.ThoiGianDuyet ||
                                    item.thoiGianDuyet
                                )}
                              </td>

                              <td>
                                {item.KetLuanBacSi ||
                                  item.ketLuanBacSi ||
                                  '—'}
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

          {/* TIMELINE */}

          <div className="card border-0 shadow-sm rounded-4">
            <div className="card-body p-4">
              <h5 className="fw-bold mb-4">
                Tiến trình xét nghiệm
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
            <div className="card-body text-center text-secondary py-5">
              <i className="fa-solid fa-flask-vial fs-1 mb-3 d-block opacity-50" />

              Chọn một lượt xét nghiệm hoặc nhập mã để theo dõi.
            </div>
          </div>
        )
      )}
    </div>
  );
}

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
                className="rounded-circle bg-success text-white d-flex align-items-center justify-content-center"
                style={{
                  width: 36,
                  height: 36,
                }}
              >
                <i
                  className={`fa-solid ${eventIcon(
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
                <strong>
                  {getTrackingEventLabel(
                    item.code,
                    item.event
                  )}
                </strong>

                <small className="text-secondary">
                  {formatDateTime(
                    item.time
                  )}
                </small>
              </div>

              {item.description && (
                <div className="small text-secondary mt-1">
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
                      <i className="fa-solid fa-arrow-right me-2 text-secondary" />
                    )}

                  {item.newStatus && (
                    <span className="badge bg-success-subtle text-success">
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