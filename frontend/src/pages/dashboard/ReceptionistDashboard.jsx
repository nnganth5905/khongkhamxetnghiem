// src/pages/dashboard/ReceptionistDashboard.jsx

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
  getReceptionistDashboard,
} from '../../services/dashboardService';

import {
  getApiErrorMessage,
} from '../../services/api';

// ============================================================
// DEFAULT
// ============================================================

const FALLBACK = {
  todayAppointments: 0,
  checkedIn: 0,
  waiting: 0,
  walkIns: 0,
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

  if (parts.length !== 3) {
    return text;
  }

  return `${parts[2]}/${parts[1]}/${parts[0]}`;
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

export default function ReceptionistDashboard() {
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
        await getReceptionistDashboard();

      setData({
        ...FALLBACK,
        ...(response || {}),
      });
    } catch (err) {
      setError(
        getApiErrorMessage(
          err,
          'Không thể tải Dashboard lễ tân.'
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
      <Loading text="Đang tải Dashboard lễ tân..." />
    );
  }

  const cards = [
    {
      label:
        'Lịch hẹn hôm nay',

      value:
        data.todayAppointments ??
        0,

      icon:
        'fa-solid fa-calendar-day',

      to:
        '/reception/tiep-nhan',

      bg:
        '#eef4ff',

      color:
        '#0d6efd',
    },

    {
      label:
        'Đã check-in',

      value:
        data.checkedIn ??
        0,

      icon:
        'fa-solid fa-user-check',

      to:
        '/reception/danh-sach-cho',

      bg:
        '#eefaf2',

      color:
        '#198754',
    },

    {
      label:
        'Đang chờ',

      value:
        data.waiting ??
        0,

      icon:
        'fa-solid fa-users',

      to:
        '/reception/danh-sach-cho',

      bg:
        '#fff8e6',

      color:
        '#b7791f',
    },

    {
      label:
        'Khách vãng lai',

      value:
        data.walkIns ??
        0,

      icon:
        'fa-solid fa-person-walking',

      to:
        '/reception/check-in',

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
            Dashboard lễ tân
          </h1>

          <p className="text-secondary mb-0">
            Quản lý lịch hẹn, tiếp nhận và hàng chờ trong ngày.
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
        <div className="col-md-4">
          <Link
            to="/reception/check-in"
            className="btn btn-primary w-100 py-3 rounded-4"
          >
            <i className="fa-solid fa-user-check me-2" />
            Check-in
          </Link>
        </div>

        <div className="col-md-4">
          <Link
            to="/reception/qr"
            className="btn btn-outline-primary w-100 py-3 rounded-4"
          >
            <i className="fa-solid fa-qrcode me-2" />
            Quét QR
          </Link>
        </div>

        <div className="col-md-4">
          <Link
            to="/reception/danh-sach-cho"
            className="btn btn-outline-secondary w-100 py-3 rounded-4"
          >
            <i className="fa-solid fa-list-ol me-2" />
            Danh sách chờ
          </Link>
        </div>
      </div>

      {/* UPCOMING */}

      <div className="card border-0 shadow-sm rounded-4">
        <div className="card-body p-4">
          <div className="d-flex flex-wrap justify-content-between align-items-center gap-3 mb-4">
            <div>
              <h5 className="fw-bold mb-1">
                Lịch hẹn sắp tới
              </h5>

              <small className="text-secondary">
                Lịch khám và xét nghiệm trong 7 ngày tới.
              </small>
            </div>

            <Link
              to="/reception/tiep-nhan"
              className="text-decoration-none small"
            >
              Quản lý tiếp nhận
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
                    Người bệnh
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
                          {item.id ??
                            '—'}
                        </td>

                        <td>
                          {formatDate(
                            item.date
                          )}
                        </td>

                        <td>
                          {item.time ??
                            '—'}
                        </td>

                        <td className="fw-semibold">
                          {item.patientName ??
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
                          {item.serviceName ??
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
                      </tr>
                    )
                  )
                ) : (
                  <tr>
                    <td
                      colSpan="7"
                      className="text-center text-secondary py-5"
                    >
                      Chưa có lịch hẹn sắp tới.
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