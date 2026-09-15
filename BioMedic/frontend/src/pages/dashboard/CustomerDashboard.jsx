import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';

import Loading from '../../components/Loading';
import Notification from '../../components/Notification';
import { getCustomerDashboard } from '../../services/dashboardService';
import { getApiErrorMessage } from '../../services/api';
import { useAuth } from '../../context/AuthContext';

const FALLBACK = {
  upcomingAppointment: null,
  totalAppointments: 0,
  availableResults: 0,
  unreadNotifications: 0,
  recentResults: [],
};

export default function CustomerDashboard() {
  const { user } = useAuth();

  const [data, setData] = useState(FALLBACK);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    let active = true;

    const load = async () => {
      try {
        const response = await getCustomerDashboard();

        if (active) {
          setData({
            ...FALLBACK,
            ...(response || {}),
          });
        }
      } catch (err) {
        if (active) {
          setError(
            getApiErrorMessage(
              err,
              'Không thể tải Dashboard khách hàng.'
            )
          );
        }
      } finally {
        if (active) {
          setLoading(false);
        }
      }
    };

    load();

    return () => {
      active = false;
    };
  }, []);

  if (loading) {
    return <Loading text="Đang tải thông tin của bạn..." />;
  }

  return (
    <div>
      <div className="mb-4">
        <h1 className="dashboard-page-title mb-1">
          Xin chào,{' '}
          {user?.fullName ??
            user?.FullName ??
            user?.name ??
            'Khách hàng'}
        </h1>
        <p className="text-secondary mb-0">
          Quản lý lịch hẹn, kết quả xét nghiệm và thông báo của bạn.
        </p>
      </div>

      {error && (
        <Notification
          type="danger"
          message={error}
          onClose={() => setError('')}
        />
      )}

      <div className="row g-4 mb-4">
        <div className="col-md-4">
          <Link
            to="/lich-hen"
            className="card border-0 shadow-sm rounded-4 h-100 text-decoration-none text-dark"
          >
            <div className="card-body p-4">
              <div className="small text-secondary">
                Tổng lịch hẹn
              </div>
              <div className="fs-2 fw-bold mt-2">
                {data.totalAppointments}
              </div>
              <div className="text-primary small mt-3">
                Xem lịch hẹn
                <i className="fa-solid fa-arrow-right ms-2" />
              </div>
            </div>
          </Link>
        </div>

        <div className="col-md-4">
          <Link
            to="/ket-qua"
            className="card border-0 shadow-sm rounded-4 h-100 text-decoration-none text-dark"
          >
            <div className="card-body p-4">
              <div className="small text-secondary">
                Kết quả đã có
              </div>
              <div className="fs-2 fw-bold mt-2">
                {data.availableResults}
              </div>
              <div className="text-primary small mt-3">
                Xem kết quả
                <i className="fa-solid fa-arrow-right ms-2" />
              </div>
            </div>
          </Link>
        </div>

        <div className="col-md-4">
          <Link
            to="/thong-bao"
            className="card border-0 shadow-sm rounded-4 h-100 text-decoration-none text-dark"
          >
            <div className="card-body p-4">
              <div className="small text-secondary">
                Thông báo chưa đọc
              </div>
              <div className="fs-2 fw-bold mt-2">
                {data.unreadNotifications}
              </div>
              <div className="text-primary small mt-3">
                Mở thông báo
                <i className="fa-solid fa-arrow-right ms-2" />
              </div>
            </div>
          </Link>
        </div>
      </div>

      <div className="row g-4">
        <div className="col-lg-5">
          <div className="card border-0 shadow-sm rounded-4 h-100">
            <div className="card-body p-4">
              <h5 className="fw-bold mb-4">
                Lịch hẹn sắp tới
              </h5>

              {data.upcomingAppointment ? (
                <>
                  <div className="small text-secondary">
                    Dịch vụ
                  </div>
                  <div className="fw-bold fs-5 mb-3">
                    {data.upcomingAppointment.serviceName ??
                      data.upcomingAppointment.testName ??
                      data.upcomingAppointment.type ??
                      'Lịch hẹn Bio Medic'}
                  </div>

                  <div className="mb-2">
                    <i className="fa-regular fa-calendar me-2 text-primary" />
                    {data.upcomingAppointment.date ??
                      data.upcomingAppointment.ngay ??
                      '—'}
                  </div>

                  <div className="mb-2">
                    <i className="fa-regular fa-clock me-2 text-primary" />
                    {data.upcomingAppointment.time ??
                      data.upcomingAppointment.gio ??
                      '—'}
                  </div>

                  <div className="mb-4">
                    <i className="fa-solid fa-user-doctor me-2 text-primary" />
                    {data.upcomingAppointment.doctorName ??
                      'Theo phân công'}
                  </div>

                  <Link
                    to="/lich-hen"
                    className="btn btn-outline-primary"
                  >
                    Xem lịch hẹn
                  </Link>
                </>
              ) : (
                <>
                  <p className="text-secondary">
                    Bạn chưa có lịch hẹn sắp tới.
                  </p>

                  <Link
                    to="/dat-lich-xet-nghiem"
                    className="btn btn-primary"
                  >
                    Đặt lịch ngay
                  </Link>
                </>
              )}
            </div>
          </div>
        </div>

        <div className="col-lg-7">
          <div className="card border-0 shadow-sm rounded-4 h-100">
            <div className="card-body p-4">
              <div className="d-flex justify-content-between align-items-center mb-4">
                <h5 className="fw-bold mb-0">
                  Kết quả gần đây
                </h5>
                <Link
                  to="/ket-qua"
                  className="small text-decoration-none"
                >
                  Xem tất cả
                </Link>
              </div>

              <div className="table-responsive">
                <table className="table align-middle">
                  <thead className="table-light">
                    <tr>
                      <th>Xét nghiệm</th>
                      <th>Ngày</th>
                      <th>Trạng thái</th>
                      <th />
                    </tr>
                  </thead>

                  <tbody>
                    {(data.recentResults || []).map((item, index) => (
                      <tr key={item.id ?? index}>
                        <td className="fw-semibold">
                          {item.testName ??
                            item.tenXetNghiem ??
                            '—'}
                        </td>
                        <td>
                          {item.date ??
                            item.ngayXetNghiem ??
                            '—'}
                        </td>
                        <td>
                          <span className="badge bg-success-subtle text-success">
                            {item.status ?? 'Có kết quả'}
                          </span>
                        </td>
                        <td className="text-end">
                          <Link
                            to={`/ket-qua/${item.id}`}
                            className="btn btn-sm btn-outline-primary"
                          >
                            Xem
                          </Link>
                        </td>
                      </tr>
                    ))}

                    {(data.recentResults || []).length === 0 && (
                      <tr>
                        <td
                          colSpan="4"
                          className="text-center text-secondary py-4"
                        >
                          Chưa có kết quả gần đây.
                        </td>
                      </tr>
                    )}
                  </tbody>
                </table>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}