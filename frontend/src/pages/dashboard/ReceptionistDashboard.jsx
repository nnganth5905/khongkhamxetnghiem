import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';

import Loading from '../../components/Loading';
import Notification from '../../components/Notification';
import { getReceptionistDashboard } from '../../services/dashboardService';
import { getApiErrorMessage } from '../../services/api';

const FALLBACK = {
  todayAppointments: 0,
  checkedIn: 0,
  waiting: 0,
  walkIns: 0,
  upcomingAppointments: [],
};

export default function ReceptionistDashboard() {
  const [data, setData] = useState(FALLBACK);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [refreshVersion, setRefreshVersion] = useState(0);

  useEffect(() => {
    let active = true;

    const load = async () => {
      try {
        const response = await getReceptionistDashboard();
        if (!active) return;

        setData({
          ...FALLBACK,
          ...(response || {}),
        });
        setError('');
      } catch (err) {
        if (active) {
          setError(getApiErrorMessage(err, 'Không thể tải Dashboard lễ tân.'));
        }
      } finally {
        if (active) setLoading(false);
      }
    };

    load();
    return () => { active = false; };
  }, [refreshVersion]);

  if (loading) {
    return <Loading text="Đang tải Dashboard lễ tân..." />;
  }

  const cards = [
    {
      label: 'Lịch hôm nay',
      value: data.todayAppointments,
      icon: 'fa-regular fa-calendar-check',
      to: '/reception/check-in',
    },
    {
      label: 'Đã check-in',
      value: data.checkedIn,
      icon: 'fa-solid fa-right-to-bracket',
      to: '/reception/danh-sach-cho',
    },
    {
      label: 'Đang chờ',
      value: data.waiting,
      icon: 'fa-solid fa-users',
      to: '/reception/danh-sach-cho',
    },
    {
      label: 'Khách tiếp nhận trực tiếp',
      value: data.walkIns,
      icon: 'fa-solid fa-user-plus',
      to: '/reception/tiep-nhan',
    },
  ];

  return (
    <div>
      <div className="d-flex flex-wrap justify-content-between align-items-center gap-3 mb-4">
        <div>
          <h1 className="dashboard-page-title mb-1">
            Dashboard lễ tân
          </h1>
          <p className="text-secondary mb-0">
            Quản lý check-in và hàng chờ khách hàng trong ngày.
          </p>
        </div>

        <div className="d-flex gap-2">
          <Link
            to="/reception/qr"
            className="btn btn-primary"
          >
            <i className="fa-solid fa-qrcode me-2" />
            Check-in QR
          </Link>

          <button
            className="btn btn-outline-primary"
            type="button"
            onClick={() => {
              setLoading(true);
              setRefreshVersion((version) => version + 1);
            }}
          >
            <i className="fa-solid fa-rotate" />
          </button>
        </div>
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
          <div className="col-md-6 col-xl-3" key={card.label}>
            <Link
              to={card.to}
              className="card border-0 shadow-sm rounded-4 h-100 text-decoration-none text-dark"
            >
              <div className="card-body p-4">
                <div className="d-flex justify-content-between">
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

      <div className="card border-0 shadow-sm rounded-4">
        <div className="card-body p-4">
          <div className="d-flex justify-content-between align-items-center mb-4">
            <h5 className="fw-bold mb-0">
              Lịch hẹn sắp đến
            </h5>

            <Link
              to="/reception/check-in"
              className="small text-decoration-none"
            >
              Mở màn hình check-in
            </Link>
          </div>

          <div className="table-responsive">
            <table className="table table-hover align-middle">
              <thead className="table-light">
                <tr>
                  <th>Ngày</th>
                  <th>Giờ</th>
                  <th>Khách hàng</th>
                  <th>Loại lịch</th>
                  <th>Dịch vụ</th>
                  <th>Trạng thái</th>
                  <th />
                </tr>
              </thead>

              <tbody>
                {(data.upcomingAppointments || []).map(
                  (item, index) => (
                    <tr key={item.id ?? index}>
                      <td>
                        {item.date
                          ? String(item.date).slice(0, 10).split('-').reverse().join('/')
                          : '—'}
                      </td>
                      <td className="fw-semibold">
                        {item.time ?? item.gioHen ?? '—'}
                      </td>

                      <td>
                        {item.patientName ??
                          item.tenKhachHang ??
                          '—'}
                      </td>

                      <td>
                        {item.type ?? item.loaiLich ?? '—'}
                      </td>

                      <td>
                        {item.serviceName ??
                          item.dichVu ??
                          '—'}
                      </td>

                      <td>
                        <span className={`badge ${
                          (item.status === 'pending' || item.trangThai === 'pending') ? 'bg-warning-subtle text-warning' : 
                          (item.status === 'confirmed' || item.trangThai === 'confirmed') ? 'bg-success-subtle text-success' : 
                          'bg-secondary-subtle text-secondary'
                        }`}>
                          {
                            {
                              'pending': 'Chờ xác nhận',
                              'confirmed': 'Đã xác nhận',
                              'checked_in': 'Đã check-in',
                              'cancelled': 'Đã hủy',
                              'no_show': 'Vắng mặt',
                              'completed': 'Hoàn tất'
                            }[item.status ?? item.trangThai] ?? 'Đã đặt'
                          }
                        </span>
                      </td>

                      <td className="text-end">
                        <Link
                          to={`/reception/check-in?q=${encodeURIComponent(
                            item.id ?? ''
                          )}&date=${encodeURIComponent(item.date ?? '')}`}
                          className="btn btn-sm btn-outline-primary"
                        >
                          Check-in
                        </Link>
                      </td>
                    </tr>
                  )
                )}

                {(data.upcomingAppointments || []).length === 0 && (
                  <tr>
                    <td
                      colSpan="7"
                      className="text-center text-secondary py-4"
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