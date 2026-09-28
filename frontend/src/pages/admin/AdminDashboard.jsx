// src/pages/dashboard/AdminDashboard.jsx

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
  getAdminDashboard,
} from '../../services/dashboardService';

import {
  getApiErrorMessage,
} from '../../services/api';

// ============================================================
// FALLBACK
// ============================================================

const FALLBACK = {
  totalCustomers: 0,
  totalDoctors: 0,
  totalEmployees: 0,
  totalTechnicians: 0,

  todayAppointments: 0,

  checkedInToday: 0,

  waitingNow: 0,

  pendingResults: 0,

  activeWorklists: 0,

  recentAppointments: [],
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

const appointmentStatus = (
  status
) => {
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

export default function AdminDashboard() {
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
        await getAdminDashboard();

      setData({
        ...FALLBACK,
        ...(response || {}),
      });
    } catch (err) {
      setError(
        getApiErrorMessage(
          err,
          'Không thể tải Dashboard quản trị.'
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
      <Loading text="Đang tải Dashboard quản trị..." />
    );
  }

  const cards = [
    {
      label:
        'Khách hàng',

      value:
        data.totalCustomers ??
        0,

      icon:
        'fa-solid fa-users',

      to:
        '/admin/khach-hang',
    },

    {
      label:
        'Bác sĩ',

      value:
        data.totalDoctors ??
        0,

      icon:
        'fa-solid fa-user-doctor',

      to:
        '/admin/bac-si',
    },

    {
      label:
        'Nhân viên',

      value:
        data.totalEmployees ??
        0,

      icon:
        'fa-solid fa-id-card',

      to:
        '/admin/nhan-vien',
    },

    {
      label:
        'Kỹ thuật viên',

      value:
        data.totalTechnicians ??
        0,

      icon:
        'fa-solid fa-microscope',

      to:
        '/admin/ktv',
    },
  ];

  const operationCards = [
    {
      label:
        'Lịch hẹn hôm nay',

      value:
        data.todayAppointments ??
        0,

      icon:
        'fa-solid fa-calendar-day',

      to:
        '/admin/lich-hen',
    },

    {
      label:
        'Check-in hôm nay',

      value:
        data.checkedInToday ??
        0,

      icon:
        'fa-solid fa-user-check',

      to:
        '/admin/lich-hen',
    },

    {
      label:
        'Đang trong quy trình',

      value:
        data.waitingNow ??
        0,

      icon:
        'fa-solid fa-clock',

      to:
        '/admin/lich-hen',
    },

    {
      label:
        'Kết quả chờ duyệt',

      value:
        data.pendingResults ??
        0,

      icon:
        'fa-solid fa-file-circle-exclamation',

      to:
        '/admin/bao-cao',
    },

    {
      label:
        'Worklist đang hoạt động',

      value:
        data.activeWorklists ??
        0,

      icon:
        'fa-solid fa-list-check',

      to:
        '/admin/xet-nghiem',
    },
  ];

  const appointments =
    Array.isArray(
      data.recentAppointments
    )
      ? data.recentAppointments
      : [];

  return (
    <div>
      {/* HEADER */}

      <div className="d-flex flex-wrap justify-content-between align-items-center gap-3 mb-4">
        <div>
          <h1 className="dashboard-page-title mb-1">
            Tổng quan hệ thống
          </h1>

          <p className="text-secondary mb-0">
            Theo dõi hoạt động khám bệnh và xét nghiệm.
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

      {/* MASTER DATA */}

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
                      className="rounded-3 bg-primary-subtle text-primary d-flex align-items-center justify-content-center"
                      style={{
                        width: 50,
                        height: 50,
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

      {/* OPERATION */}

      <div className="card border-0 shadow-sm rounded-4 mb-4">
        <div className="card-body p-4">
          <h5 className="fw-bold mb-4">
            Hoạt động nghiệp vụ
          </h5>

          <div className="row g-3">
            {operationCards.map(
              (card) => (
                <div
                  className="col-md-6 col-xl"
                  key={card.label}
                >
                  <Link
                    to={card.to}
                    className="border rounded-4 p-3 d-flex justify-content-between align-items-center text-decoration-none text-dark h-100"
                  >
                    <div>
                      <div className="small text-secondary">
                        {card.label}
                      </div>

                      <div className="fs-4 fw-bold mt-1">
                        {card.value}
                      </div>
                    </div>

                    <i
                      className={`${card.icon} text-primary fs-4`}
                    />
                  </Link>
                </div>
              )
            )}
          </div>
        </div>
      </div>

      {/* QUICK MANAGEMENT */}

      <div className="card border-0 shadow-sm rounded-4 mb-4">
        <div className="card-body p-4">
          <h5 className="fw-bold mb-3">
            Quản lý nhanh
          </h5>

          <div className="d-flex flex-wrap gap-2">
            <Link
              to="/admin/chuyen-khoa"
              className="btn btn-outline-primary"
            >
              Chuyên khoa
            </Link>

            <Link
              to="/admin/phong"
              className="btn btn-outline-primary"
            >
              Phòng
            </Link>

            <Link
              to="/admin/lich-lam-viec"
              className="btn btn-outline-primary"
            >
              Lịch làm việc
            </Link>

            <Link
              to="/admin/xet-nghiem"
              className="btn btn-outline-primary"
            >
              Xét nghiệm
            </Link>

            <Link
              to="/admin/bao-cao"
              className="btn btn-outline-primary"
            >
              Báo cáo
            </Link>
          </div>
        </div>
      </div>

      {/* APPOINTMENTS */}

      <div className="card border-0 shadow-sm rounded-4">
        <div className="card-body p-4">
          <div className="d-flex justify-content-between align-items-center mb-4">
            <div>
              <h5 className="fw-bold mb-1">
                Lịch hẹn sắp tới
              </h5>

              <small className="text-secondary">
                Lịch khám và xét nghiệm gần nhất.
              </small>
            </div>

            <Link
              to="/admin/lich-hen"
              className="small text-decoration-none"
            >
              Quản lý lịch hẹn
            </Link>
          </div>

          <div className="table-responsive">
            <table className="table table-hover align-middle mb-0">
              <thead className="table-light">
                <tr>
                  <th>
                    Mã
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
                {appointments.length >
                0 ? (
                  appointments.map(
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

                        <td className="fw-semibold">
                          {item.patientName ||
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
                            {appointmentStatus(
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
                      Chưa có lịch hẹn.
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