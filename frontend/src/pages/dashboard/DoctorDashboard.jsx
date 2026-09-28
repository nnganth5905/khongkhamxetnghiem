// src/pages/dashboard/DoctorDashboard.jsx

import React, {
  useEffect,
  useState,
} from 'react';

import {
  Link,
} from 'react-router-dom';

import Loading from '../../components/Loading';
import Notification from '../../components/Notification';

import {
  getDoctorDashboard,
} from '../../services/dashboardService';

import {
  getApiErrorMessage,
} from '../../services/api';

import {
  useAuth,
} from '../../context/AuthContext';

// ============================================================
// DEFAULT DATA
// ============================================================

const FALLBACK = {
  waitingCount: 0,

  todayVisits: 0,
  todayExamsCount: 0,

  pendingApprovals: 0,
  pendingResultsCount: 0,

  completedToday: 0,
  completedTodayCount: 0,

  currentRoom: '—',

  nextPatients: [],

  pendingResults: [],
};

// ============================================================
// STATUS
// ============================================================

const getVisitStatusText = (status) => {
  switch (
    String(status || '').toLowerCase()
  ) {
    case 'da_tiep_nhan':
      return 'Đã tiếp nhận';

    case 'cho_kham':
      return 'Chờ khám';

    case 'da_den_luot':
      return 'Đã gọi';

    case 'cho_goi_lai':
      return 'Chờ gọi lại';

    case 'dang_kham':
      return 'Đang khám';

    case 'da_chi_dinh_xn':
      return 'Đã chỉ định XN';

    case 'moi_doc_kq':
      return 'Mời đọc kết quả';

    case 'dang_tu_van_kq':
      return 'Đang tư vấn';

    case 'hoan_tat':
      return 'Hoàn tất';

    case 'bo_luot':
      return 'Bỏ lượt';

    default:
      return status || 'Chưa xác định';
  }
};

const getVisitStatusClass = (status) => {
  switch (
    String(status || '').toLowerCase()
  ) {
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

    case 'hoan_tat':
      return 'bg-success text-white';

    case 'bo_luot':
      return 'bg-danger-subtle text-danger';

    default:
      return 'bg-secondary-subtle text-secondary';
  }
};

// ============================================================
// DATE
// ============================================================

const formatDateTime = (value) => {
  if (!value) {
    return '—';
  }

  const date =
    new Date(value);

  if (
    Number.isNaN(
      date.getTime()
    )
  ) {
    return String(value);
  }

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
};

// ============================================================
// COMPONENT
// ============================================================

export default function DoctorDashboard() {
  const {
    user,
  } = useAuth();

  const [
    data,
    setData,
  ] = useState(FALLBACK);

  const [
    loading,
    setLoading,
  ] = useState(true);

  const [
    refreshing,
    setRefreshing,
  ] = useState(false);

  const [
    error,
    setError,
  ] = useState('');

  // ==========================================================
  // LOAD
  // ==========================================================

  const load = async (
    showFullLoading = true
  ) => {
    try {
      if (showFullLoading) {
        setLoading(true);
      } else {
        setRefreshing(true);
      }

      setError('');

      const response =
        await getDoctorDashboard();

      setData({
        ...FALLBACK,
        ...(response || {}),
      });
    } catch (err) {
      setError(
        getApiErrorMessage(
          err,
          'Không thể tải Dashboard bác sĩ.'
        )
      );
    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  };

  useEffect(() => {
    load();
  }, []);

  // ==========================================================
  // LOADING
  // ==========================================================

  if (loading) {
    return (
      <Loading text="Đang tải Dashboard bác sĩ..." />
    );
  }

  // ==========================================================
  // CARDS
  // ==========================================================

  const cards = [
    {
      label:
        'Đang chờ khám',

      value:
        data.waitingCount ??
        0,

      icon:
        'fa-solid fa-users',

      to:
        '/doctor/danh-sach-cho',

      bg:
        '#eef4ff',

      iconColor:
        '#0d6efd',
    },

    {
      label:
        'Lượt khám hôm nay',

      value:
        data.todayExamsCount ??
        data.todayVisits ??
        0,

      icon:
        'fa-solid fa-stethoscope',

      to:
        '/doctor/danh-sach-cho',

      bg:
        '#eefaf2',

      iconColor:
        '#198754',
    },

    {
      label:
        'Kết quả chờ duyệt',

      value:
        data.pendingResultsCount ??
        data.pendingApprovals ??
        0,

      icon:
        'fa-solid fa-file-circle-check',

      to:
        '/doctor/duyet-ket-qua',

      bg:
        '#fff8e6',

      iconColor:
        '#b7791f',
    },

    {
      label:
        'Hoàn tất hôm nay',

      value:
        data.completedTodayCount ??
        data.completedToday ??
        0,

      icon:
        'fa-solid fa-circle-check',

      to:
        '/doctor/danh-sach-cho',

      bg:
        '#f2efff',

      iconColor:
        '#6f42c1',
    },
  ];

  const nextPatients =
    Array.isArray(
      data.nextPatients
    )
      ? data.nextPatients
      : [];

  const pendingResults =
    Array.isArray(
      data.pendingResults
    )
      ? data.pendingResults
      : [];

  // ==========================================================
  // RENDER
  // ==========================================================

  return (
    <div>
      {/* HEADER */}

      <div className="d-flex flex-wrap justify-content-between align-items-center gap-3 mb-4">
        <div>
          <h1 className="dashboard-page-title mb-1">
            Xin chào,{' '}

            {user?.fullName ??
              user?.FullName ??
              user?.name ??
              user?.username ??
              'Bác sĩ'}
          </h1>

          <p className="text-secondary mb-0">
            Phòng làm việc hiện tại:{' '}

            <strong>
              {data.currentRoom ||
                '—'}
            </strong>
          </p>
        </div>

        <button
          type="button"
          className="btn btn-outline-primary"
          disabled={refreshing}
          onClick={() =>
            load(false)
          }
        >
          <i
            className={`fa-solid fa-rotate me-2 ${
              refreshing
                ? 'fa-spin'
                : ''
            }`}
          />

          Làm mới
        </button>
      </div>

      {/* ERROR */}

      {error && (
        <Notification
          type="danger"
          message={error}
          onClose={() =>
            setError('')
          }
        />
      )}

      {/* CARDS */}

      <div className="row g-4 mb-4">
        {cards.map(
          (card) => (
            <div
              className="col-md-6 col-xl-3"
              key={card.label}
            >
              <Link
                to={card.to}
                className="card border-0 shadow-sm rounded-4 h-100 text-decoration-none text-dark"
              >
                <div className="card-body p-4">
                  <div className="d-flex justify-content-between align-items-start">
                    <div>
                      <div className="small text-secondary">
                        {card.label}
                      </div>

                      <div className="fs-2 fw-bold mt-2">
                        {card.value}
                      </div>
                    </div>

                    <div
                      className="d-flex align-items-center justify-content-center rounded-3"
                      style={{
                        width: 50,
                        height: 50,
                        background:
                          card.bg,
                        color:
                          card.iconColor,
                      }}
                    >
                      <i
                        className={`${card.icon} fs-5`}
                      />
                    </div>
                  </div>
                </div>
              </Link>
            </div>
          )
        )}
      </div>

      <div className="row g-4">
        {/* ===================================================
            NEXT PATIENTS
        ==================================================== */}

        <div className="col-xl-7">
          <div className="card border-0 shadow-sm rounded-4 h-100">
            <div className="card-body p-4">
              <div className="d-flex justify-content-between align-items-center mb-4">
                <div>
                  <h5 className="fw-bold mb-1">
                    Bệnh nhân tiếp theo
                  </h5>

                  <small className="text-secondary">
                    Hàng chờ của bác sĩ trong hôm nay.
                  </small>
                </div>

                <Link
                  to="/doctor/danh-sach-cho"
                  className="small text-decoration-none"
                >
                  Xem tất cả
                </Link>
              </div>

              <div className="table-responsive">
                <table className="table table-hover align-middle mb-0">
                  <thead className="table-light">
                    <tr>
                      <th>STT</th>

                      <th>
                        Người bệnh
                      </th>

                      <th>
                        Giờ
                      </th>

                      <th>
                        Lý do khám
                      </th>

                      <th>
                        Trạng thái
                      </th>

                      <th />
                    </tr>
                  </thead>

                  <tbody>
                    {nextPatients.length >
                    0 ? (
                      nextPatients.map(
                        (
                          item,
                          index
                        ) => (
                          <tr
                            key={
                              item.id ??
                              index
                            }
                          >
                            <td>
                              <span className="badge bg-primary fs-6">
                                {item.queueNumber ??
                                  index +
                                    1}
                              </span>
                            </td>

                            <td>
                              <div className="fw-semibold">
                                {item.patientName ??
                                  '—'}
                              </div>

                              <small className="text-secondary">
                                {item.patientCode ??
                                  ''}
                              </small>
                            </td>

                            <td>
                              {item.time ??
                                '—'}
                            </td>

                            <td
                              style={{
                                maxWidth:
                                  220,
                              }}
                            >
                              {item.reason ??
                                'Khám bệnh'}
                            </td>

                            <td>
                              <span
                                className={`badge ${getVisitStatusClass(
                                  item.status
                                )}`}
                              >
                                {getVisitStatusText(
                                  item.status
                                )}
                              </span>
                            </td>

                            <td className="text-end">
                              <Link
                                to={`/doctor/kham-benh?luotKhamId=${encodeURIComponent(
                                  item.id ??
                                    ''
                                )}`}
                                className="btn btn-sm btn-primary"
                              >
                                Khám
                              </Link>
                            </td>
                          </tr>
                        )
                      )
                    ) : (
                      <tr>
                        <td
                          colSpan="6"
                          className="text-center text-secondary py-5"
                        >
                          <i className="fa-solid fa-user-clock fs-2 d-block mb-3 opacity-50" />

                          Không có bệnh nhân đang chờ.
                        </td>
                      </tr>
                    )}
                  </tbody>
                </table>
              </div>
            </div>
          </div>
        </div>

        {/* ===================================================
            RESULTS
        ==================================================== */}

        <div className="col-xl-5">
          <div className="card border-0 shadow-sm rounded-4 h-100">
            <div className="card-body p-4">
              <div className="d-flex justify-content-between align-items-center mb-4">
                <div>
                  <h5 className="fw-bold mb-1">
                    Kết quả chờ duyệt
                  </h5>

                  <small className="text-secondary">
                    Ưu tiên kết quả bất thường.
                  </small>
                </div>

                <Link
                  to="/doctor/duyet-ket-qua"
                  className="small text-decoration-none"
                >
                  Xem tất cả
                </Link>
              </div>

              {pendingResults.length >
              0 ? (
                <div className="list-group list-group-flush">
                  {pendingResults.map(
                    (
                      item,
                      index
                    ) => (
                      <Link
                        key={
                          item.id ??
                          index
                        }
                        to={`/doctor/doc-ket-qua/${encodeURIComponent(
                          item.id ??
                            ''
                        )}`}
                        className="list-group-item list-group-item-action px-0 py-3 border-bottom"
                      >
                        <div className="d-flex justify-content-between gap-3">
                          <div>
                            <div className="fw-semibold">
                              {item.patientName ??
                                '—'}
                            </div>

                            <div className="small text-secondary mt-1">
                              {item.testName ??
                                'Xét nghiệm'}
                            </div>

                            <div className="small text-secondary mt-1">
                              {formatDateTime(
                                item.submittedAt
                              )}
                            </div>
                          </div>

                          {item.hasAbnormalIndicator && (
                            <span className="badge bg-danger-subtle text-danger align-self-start">
                              Bất thường
                            </span>
                          )}
                        </div>
                      </Link>
                    )
                  )}
                </div>
              ) : (
                <div className="text-center text-secondary py-5">
                  <i className="fa-solid fa-file-circle-check fs-2 d-block mb-3 opacity-50" />

                  Không có kết quả chờ duyệt.
                </div>
              )}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}