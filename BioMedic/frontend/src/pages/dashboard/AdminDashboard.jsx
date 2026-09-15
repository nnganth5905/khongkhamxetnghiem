import React, { useEffect, useMemo, useState } from 'react';
import { Link } from 'react-router-dom';

import Loading from '../../components/Loading';
import Notification from '../../components/Notification';
import { getAdminDashboard } from '../../services/dashboardService';
import { getApiErrorMessage } from '../../services/api';

const FALLBACK = {
  totalCustomers: 0,
  totalEmployees: 0,
  totalDoctors: 0,
  totalTechnicians: 0,
  totalTestOrders: 0,
  totalResults: 0,
  todayAppointments: 0,
  revenueToday: 0,
  pendingResults: 0,
  waitingSpecimens: 0,
  recentActivities: [],
};

const formatVnd = (value) =>
  `${Number(value || 0).toLocaleString('vi-VN')} đ`;

export default function AdminDashboard() {
  const [data, setData] = useState(FALLBACK);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const loadDashboard = async () => {
    try {
      setLoading(true);
      setError('');

      const response = await getAdminDashboard();

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
    }
  };

  useEffect(() => {
    loadDashboard();
  }, []);

  const cards = useMemo(
    () => [
      {
        title: 'Khách hàng',
        value: data.totalCustomers,
        icon: 'fa-solid fa-users',
        to: '/admin/khach-hang',
      },
      {
        title: 'Nhân viên',
        value: data.totalEmployees,
        icon: 'fa-solid fa-users-gear',
        to: '/admin/nhan-vien',
      },
      {
        title: 'Bác sĩ',
        value: data.totalDoctors,
        icon: 'fa-solid fa-user-doctor',
        to: '/admin/bac-si',
      },
      {
        title: 'Kỹ thuật viên',
        value: data.totalTechnicians,
        icon: 'fa-solid fa-microscope',
        to: '/admin/ktv',
      },
      {
        title: 'Phiếu xét nghiệm',
        value: data.totalTestOrders,
        icon: 'fa-solid fa-flask-vial',
        to: '/admin/xet-nghiem',
      },
      {
        title: 'Kết quả',
        value: data.totalResults,
        icon: 'fa-solid fa-square-poll-horizontal',
        to: '/admin/bao-cao',
      },
      {
        title: 'Lịch hôm nay',
        value: data.todayAppointments,
        icon: 'fa-regular fa-calendar-check',
        to: '/admin/lich-lam-viec',
      },
      {
        title: 'Chờ duyệt kết quả',
        value: data.pendingResults,
        icon: 'fa-solid fa-file-circle-check',
        to: '/tracking/lich-su',
      },
    ],
    [data]
  );

  if (loading) {
    return <Loading text="Đang tải Dashboard quản trị..." />;
  }

  return (
    <div>
      <div className="d-flex flex-wrap justify-content-between align-items-center gap-3 mb-4">
        <div>
          <h1 className="dashboard-page-title mb-1">
            Tổng quan hệ thống
          </h1>
          <p className="text-secondary mb-0">
            Theo dõi hoạt động vận hành Bio Medic Center.
          </p>
        </div>

        <button
          type="button"
          className="btn btn-outline-primary"
          onClick={loadDashboard}
        >
          <i className="fa-solid fa-rotate me-2" />
          Làm mới
        </button>
      </div>

      {error && (
        <Notification
          type="danger"
          message={error}
          onClose={() => setError('')}
        />
      )}

      <div className="row g-4 mb-4">
        {cards.map((card) => (
          <div className="col-md-6 col-xl-3" key={card.title}>
            <Link
              to={card.to}
              className="card border-0 shadow-sm rounded-4 h-100 text-decoration-none text-dark"
            >
              <div className="card-body p-4">
                <div className="d-flex justify-content-between align-items-start gap-3">
                  <div>
                    <div className="small text-secondary">
                      {card.title}
                    </div>
                    <div className="fs-2 fw-bold mt-2">
                      {Number(card.value || 0).toLocaleString('vi-VN')}
                    </div>
                  </div>

                  <div
                    className="d-flex align-items-center justify-content-center rounded-3"
                    style={{
                      width: 48,
                      height: 48,
                      background: '#eaf2ff',
                      color: 'var(--primary)',
                    }}
                  >
                    <i className={card.icon} />
                  </div>
                </div>
              </div>
            </Link>
          </div>
        ))}
      </div>

      <div className="row g-4">
        <div className="col-lg-4">
          <div
            className="card border-0 shadow-sm rounded-4 h-100 text-white"
            style={{ background: 'var(--primary)' }}
          >
            <div className="card-body p-4">
              <div className="small opacity-75">
                Doanh thu hôm nay
              </div>
              <div className="display-6 fw-bold mt-2">
                {formatVnd(data.revenueToday)}
              </div>

              <hr className="border-white opacity-25" />

              <div className="d-flex justify-content-between">
                <span>Mẫu đang chờ</span>
                <strong>{data.waitingSpecimens}</strong>
              </div>

              <Link to="/admin/bao-cao" className="btn btn-light mt-4">
                Xem báo cáo chi tiết
              </Link>
            </div>
          </div>
        </div>

        <div className="col-lg-8">
          <div className="card border-0 shadow-sm rounded-4 h-100">
            <div className="card-body p-4">
              <div className="d-flex justify-content-between align-items-center mb-4">
                <h5 className="fw-bold mb-0">
                  Hoạt động gần đây
                </h5>

                <Link
                  to="/tracking/lich-su"
                  className="small text-decoration-none"
                >
                  Xem truy vết
                </Link>
              </div>

              {(data.recentActivities || []).length > 0 ? (
                <div className="d-flex flex-column gap-3">
                  {data.recentActivities.map((item, index) => (
                    <div
                      key={item.id ?? index}
                      className="d-flex gap-3 pb-3 border-bottom"
                    >
                      <div
                        className="d-flex align-items-center justify-content-center rounded-circle flex-shrink-0"
                        style={{
                          width: 40,
                          height: 40,
                          background: '#eaf2ff',
                          color: 'var(--primary)',
                        }}
                      >
                        <i className="fa-solid fa-clock-rotate-left" />
                      </div>

                      <div className="flex-grow-1">
                        <div className="fw-semibold">
                          {item.title ??
                            item.action ??
                            'Hoạt động hệ thống'}
                        </div>
                        <div className="small text-secondary">
                          {item.description ?? item.note ?? ''}
                        </div>
                      </div>

                      <div className="small text-secondary text-nowrap">
                        {item.time ?? item.createdAt ?? ''}
                      </div>
                    </div>
                  ))}
                </div>
              ) : (
                <div className="text-secondary">
                  Chưa có hoạt động gần đây.
                </div>
              )}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}