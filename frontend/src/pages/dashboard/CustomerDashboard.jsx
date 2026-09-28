// src/pages/dashboard/CustomerDashboard.jsx

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
  getCustomerDashboard,
} from '../../services/dashboardService';

import {
  getApiErrorMessage,
} from '../../services/api';

import {
  useAuth,
} from '../../context/AuthContext';

// ============================================================
// FALLBACK
// ============================================================

const FALLBACK = {
  todayAppointments: 0,

  upcomingAppointmentsCount: 0,

  activeVisits: 0,

  approvedResults: 0,

  upcomingAppointments: [],
};

// ============================================================
// FORMAT
// ============================================================

const formatDate = (value) => {
  if (!value) {
    return '—';
  }

  const text =
    String(value).substring(
      0,
      10
    );

  const parts =
    text.split('-');

  return parts.length === 3
    ? `${parts[2]}/${parts[1]}/${parts[0]}`
    : text;
};

const statusText = (status) => {
  switch (
    String(status || '').toLowerCase()
  ) {
    case 'pending':
      return 'Chờ xác nhận';

    case 'confirmed':
      return 'Đã xác nhận';

    case 'checked_in':
      return 'Đã check-in';

    case 'completed':
      return 'Hoàn tất';

    case 'cancelled':
      return 'Đã hủy';

    case 'no_show':
      return 'Vắng mặt';

    default:
      return status || '—';
  }
};

const statusClass = (status) => {
  switch (
    String(status || '').toLowerCase()
  ) {
    case 'pending':
      return 'bg-warning-subtle text-warning-emphasis';

    case 'confirmed':
      return 'bg-primary-subtle text-primary';

    case 'checked_in':
      return 'bg-info-subtle text-info-emphasis';

    case 'completed':
      return 'bg-success-subtle text-success';

    case 'cancelled':
    case 'no_show':
      return 'bg-danger-subtle text-danger';

    default:
      return 'bg-secondary-subtle text-secondary';
  }
};

// ============================================================
// COMPONENT
// ============================================================

export default function CustomerDashboard() {
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
    full = true
  ) => {
    try {
      if (full) {
        setLoading(true);
      } else {
        setRefreshing(true);
      }

      setError('');

      const response =
        await getCustomerDashboard();

      setData({
        ...FALLBACK,
        ...(response || {}),
      });
    } catch (err) {
      setError(
        getApiErrorMessage(
          err,
          'Không thể tải Dashboard khách hàng.'
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

  if (loading) {
    return (
      <Loading text="Đang tải Dashboard..." />
    );
  }

  const cards = [
    {
      label:
        'Lịch hôm nay',

      value:
        data.todayAppointments ??
        0,

      icon:
        'fa-solid fa-calendar-day',

      to:
        '/lich-hen',

      bg:
        '#eef4ff',

      color:
        '#0d6efd',
    },

    {
      label:
        'Lịch sắp tới',

      value:
        data.upcomingAppointmentsCount ??
        0,

      icon:
        'fa-solid fa-calendar-check',

      to:
        '/lich-hen',

      bg:
        '#eefaf2',

      color:
        '#198754',
    },

    {
      label:
        'Đang xử lý',

      value:
        data.activeVisits ??
        0,

      icon:
        'fa-solid fa-spinner',

      to:
        '/customer/theo-doi',

      bg:
        '#fff8e6',

      color:
        '#b7791f',
    },

    {
      label:
        'Kết quả đã duyệt',

      value:
        data.approvedResults ??
        0,

      icon:
        'fa-solid fa-file-medical',

      to:
        '/customer/ket-qua',

      bg:
        '#f2efff',

      color:
        '#6f42c1',
    },
  ];

  const upcoming =
    Array.isArray(
      data.upcomingAppointments
    )
      ? data.upcomingAppointments
      : [];

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
              'Khách hàng'}
          </h1>

          <p className="text-secondary mb-0">
            Theo dõi lịch hẹn, tiến trình khám và kết quả xét nghiệm.
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
                      className="rounded-3 d-flex align-items-center justify-content-center"
                      style={{
                        width: 50,
                        height: 50,
                        background:
                          card.bg,
                        color:
                          card.color,
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

      {/* QUICK ACTION */}

      <div className="row g-4 mb-4">
        <div className="col-md-6 col-xl-3">
          <Link
            to="/dat-lich"
            className="btn btn-primary w-100 py-3 rounded-4"
          >
            <i className="fa-solid fa-calendar-plus me-2" />

            Đặt lịch
          </Link>
        </div>

        <div className="col-md-6 col-xl-3">
          <Link
            to="/customer/theo-doi"
            className="btn btn-outline-primary w-100 py-3 rounded-4"
          >
            <i className="fa-solid fa-stethoscope me-2" />

            Theo dõi khám
          </Link>
        </div>

        <div className="col-md-6 col-xl-3">
          <Link
            to="/customer/theo-doi-xn"
            className="btn btn-outline-success w-100 py-3 rounded-4"
          >
            <i className="fa-solid fa-flask-vial me-2" />

            Theo dõi XN
          </Link>
        </div>

        <div className="col-md-6 col-xl-3">
          <Link
            to="/customer/ket-qua"
            className="btn btn-outline-secondary w-100 py-3 rounded-4"
          >
            <i className="fa-solid fa-file-medical me-2" />

            Kết quả
          </Link>
        </div>
      </div>

      {/* UPCOMING */}

      <div className="card border-0 shadow-sm rounded-4">
        <div className="card-body p-4">
          <div className="d-flex justify-content-between align-items-center mb-4">
            <div>
              <h5 className="fw-bold mb-1">
                Lịch hẹn sắp tới
              </h5>

              <small className="text-secondary">
                Các lịch đang chờ hoặc đã được xác nhận.
              </small>
            </div>

            <Link
              to="/lich-hen"
              className="small text-decoration-none"
            >
              Xem tất cả
            </Link>
          </div>

          <div className="table-responsive">
            <table className="table table-hover align-middle mb-0">
              <thead className="table-light">
                <tr>
                  <th>
                    Mã lịch
                  </th>

                  <th>
                    Ngày
                  </th>

                  <th>
                    Giờ
                  </th>

                  <th>
                    Loại
                  </th>

                  <th>
                    Dịch vụ
                  </th>

                  <th>
                    Trạng thái
                  </th>

                  <th />
                </tr>
              </thead>

              <tbody>
                {upcoming.length >
                0 ? (
                  upcoming.map(
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
                        <td className="fw-semibold text-primary">
                          {item.id ||
                            '—'}
                        </td>

                        <td>
                          {formatDate(
                            item.date
                          )}
                        </td>

                        <td>
                          {item.time ||
                            '—'}
                        </td>

                        <td>
                          {String(
                            item.type ||
                              ''
                          ).toUpperCase() ===
                          'TEST'
                            ? 'Xét nghiệm'
                            : 'Khám bệnh'}
                        </td>

                        <td>
                          {item.serviceName ||
                            '—'}
                        </td>

                        <td>
                          <span
                            className={`badge ${statusClass(
                              item.status
                            )}`}
                          >
                            {statusText(
                              item.status
                            )}
                          </span>
                        </td>

                        <td className="text-end">
                          <Link
                            to={`/lich-hen/${encodeURIComponent(
                              item.id ??
                                ''
                            )}`}
                            className="btn btn-sm btn-outline-primary"
                          >
                            Chi tiết
                          </Link>
                        </td>
                      </tr>
                    )
                  )
                ) : (
                  <tr>
                    <td
                      colSpan="7"
                      className="text-center text-secondary py-5"
                    >
                      <i className="fa-solid fa-calendar-xmark fs-2 d-block mb-3 opacity-50" />

                      Bạn chưa có lịch hẹn sắp tới.
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
  );
}