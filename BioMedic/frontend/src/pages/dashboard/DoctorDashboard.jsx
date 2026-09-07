import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';

import Loading from '../../components/Loading';
import Notification from '../../components/Notification';
import { getDoctorDashboard } from '../../services/dashboardService';
import { getApiErrorMessage } from '../../services/api';
import { useAuth } from '../../context/AuthContext';

const FALLBACK = {
  waitingCount: 0,
  todayVisits: 0,
  pendingApprovals: 0,
  completedToday: 0,
  currentRoom: '—',
  nextPatients: [],
  pendingResults: [],
};

export default function DoctorDashboard() {
  const { user } = useAuth();

  const [data, setData] = useState(FALLBACK);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const load = async () => {
    try {
      setLoading(true);
      setError('');
      const response = await getDoctorDashboard();
      setData({ ...FALLBACK, ...(response || {}) });
    } catch (err) {
      setError(
        getApiErrorMessage(
          err,
          'Không thể tải Dashboard bác sĩ.'
        )
      );
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    load();
  }, []);

  if (loading) {
    return <Loading text="Đang tải Dashboard bác sĩ..." />;
  }

  const cards = [
    {
      label: 'Đang chờ khám',
      value: data.waitingCount,
      icon: 'fa-solid fa-users',
      to: '/doctor/danh-sach-cho',
    },
    {
      label: 'Lượt khám hôm nay',
      value: data.todayVisits,
      icon: 'fa-solid fa-stethoscope',
      to: '/doctor/danh-sach-cho',
    },
    {
      label: 'Kết quả chờ duyệt',
      value: data.pendingApprovals,
      icon: 'fa-solid fa-file-circle-check',
      to: '/doctor/duyet-ket-qua',
    },
    {
      label: 'Hoàn tất hôm nay',
      value: data.completedToday,
      icon: 'fa-solid fa-circle-check',
      to: '/doctor/doc-ket-qua',
    },
  ];

  return (
    <div>
      <div className="d-flex flex-wrap justify-content-between align-items-center gap-3 mb-4">
        <div>
          <h1 className="dashboard-page-title mb-1">
            Xin chào,{' '}
            {user?.fullName ??
              user?.FullName ??
              user?.name ??
              'Bác sĩ'}
          </h1>
          <p className="text-secondary mb-0">
            Phòng làm việc hiện tại:{' '}
            <strong>{data.currentRoom}</strong>
          </p>
        </div>

        <button
          className="btn btn-outline-primary"
          type="button"
          onClick={load}
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

      <div className="row g-4">
        <div className="col-xl-7">
          <div className="card border-0 shadow-sm rounded-4 h-100">
            <div className="card-body p-4">
              <div className="d-flex justify-content-between align-items-center mb-4">
                <h5 className="fw-bold mb-0">
                  Bệnh nhân tiếp theo
                </h5>
                <Link
                  to="/doctor/danh-sach-cho"
                  className="small text-decoration-none"
                >
                  Xem tất cả
                </Link>
              </div>

              <div className="table-responsive">
                <table className="table align-middle">
                  <thead className="table-light">
                    <tr>
                      <th>STT</th>
                      <th>Người bệnh</th>
                      <th>Giờ</th>
                      <th>Lý do khám</th>
                      <th />
                    </tr>
                  </thead>

                  <tbody>
                    {(data.nextPatients || []).map((item, index) => (
                      <tr key={item.id ?? index}>
                        <td>
                          <span className="badge bg-primary">
                            {item.queueNumber ?? index + 1}
                          </span>
                        </td>
                        <td className="fw-semibold">
                          {item.patientName ??
                            item.tenKhachHang ??
                            '—'}
                        </td>
                        <td>
                          {item.time ?? item.gioHen ?? '—'}
                        </td>
                        <td>
                          {item.reason ?? item.lyDo ?? '—'}
                        </td>
                        <td className="text-end">
                          <Link
                            to={`/doctor/kham-benh?id=${encodeURIComponent(
                              item.id ?? ''
                            )}`}
                            className="btn btn-sm btn-primary"
                          >
                            Khám
                          </Link>
                        </td>
                      </tr>
                    ))}

                    {(data.nextPatients || []).length === 0 && (
                      <tr>
                        <td
                          colSpan="5"
                          className="text-center text-secondary py-4"
                        >
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

        <div className="col-xl-5">
          <div className="card border-0 shadow-sm rounded-4 h-100">
            <div className="card-body p-4">
              <h5 className="fw-bold mb-4">
                Kết quả cần xử lý
              </h5>

              <div className="d-flex flex-column gap-3">
                {(data.pendingResults || []).map((item, index) => (
                  <div
                    className="p-3 rounded-3 border"
                    key={item.id ?? index}
                  >
                    <div className="fw-semibold">
                      {item.patientName ??
                        item.tenKhachHang ??
                        'Người bệnh'}
                    </div>
                    <div className="small text-secondary">
                      {item.testName ??
                        item.tenXetNghiem ??
                        'Xét nghiệm'}
                    </div>

                    <div className="d-flex gap-2 mt-3">
                      <Link
                        to={`/doctor/duyet-ket-qua?id=${encodeURIComponent(
                          item.id ?? ''
                        )}`}
                        className="btn btn-sm btn-outline-primary"
                      >
                        Duyệt
                      </Link>

                      <Link
                        to={`/doctor/doc-ket-qua?id=${encodeURIComponent(
                          item.id ?? ''
                        )}`}
                        className="btn btn-sm btn-primary"
                      >
                        Đọc kết quả
                      </Link>
                    </div>
                  </div>
                ))}

                {(data.pendingResults || []).length === 0 && (
                  <div className="text-secondary">
                    Không có kết quả đang chờ.
                  </div>
                )}
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}