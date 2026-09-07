import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';

import Loading from '../../components/Loading';
import Notification from '../../components/Notification';
import { getTechnicianDashboard } from '../../services/dashboardService';
import { getApiErrorMessage } from '../../services/api';

const FALLBACK = {
  waitingSpecimens: 0,
  receivedSpecimens: 0,
  inProgress: 0,
  pendingResultEntries: 0,
  worklist: [],
};

export default function TechnicianDashboard() {
  const [data, setData] = useState(FALLBACK);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const load = async () => {
    try {
      setLoading(true);
      setError('');

      const response = await getTechnicianDashboard();

      setData({
        ...FALLBACK,
        ...(response || {}),
      });
    } catch (err) {
      setError(
        getApiErrorMessage(
          err,
          'Không thể tải Dashboard kỹ thuật viên.'
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
    return (
      <Loading text="Đang tải Dashboard kỹ thuật viên..." />
    );
  }

  const cards = [
    {
      label: 'Mẫu chờ tiếp nhận',
      value: data.waitingSpecimens,
      icon: 'fa-solid fa-vials',
      to: '/technician/mau-benh-pham',
    },
    {
      label: 'Mẫu đã tiếp nhận',
      value: data.receivedSpecimens,
      icon: 'fa-solid fa-vial-circle-check',
      to: '/technician/mau-benh-pham',
    },
    {
      label: 'Đang thực hiện',
      value: data.inProgress,
      icon: 'fa-solid fa-microscope',
      to: '/technician/worklist',
    },
    {
      label: 'Chờ nhập kết quả',
      value: data.pendingResultEntries,
      icon: 'fa-solid fa-file-pen',
      to: '/technician/worklist',
    },
  ];

  return (
    <div>
      <div className="d-flex flex-wrap justify-content-between align-items-center gap-3 mb-4">
        <div>
          <h1 className="dashboard-page-title mb-1">
            Dashboard kỹ thuật viên
          </h1>
          <p className="text-secondary mb-0">
            Theo dõi mẫu bệnh phẩm, worklist và tiến độ nhập kết quả.
          </p>
        </div>

        <button
          type="button"
          className="btn btn-outline-primary"
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

      <div className="card border-0 shadow-sm rounded-4">
        <div className="card-body p-4">
          <div className="d-flex justify-content-between align-items-center mb-4">
            <h5 className="fw-bold mb-0">
              Worklist cần xử lý
            </h5>

            <Link
              to="/technician/worklist"
              className="small text-decoration-none"
            >
              Xem toàn bộ
            </Link>
          </div>

          <div className="table-responsive">
            <table className="table table-hover align-middle">
              <thead className="table-light">
                <tr>
                  <th>Mã mẫu</th>
                  <th>Người bệnh</th>
                  <th>Xét nghiệm</th>
                  <th>Ưu tiên</th>
                  <th>Trạng thái</th>
                  <th />
                </tr>
              </thead>

              <tbody>
                {(data.worklist || []).map((item, index) => (
                  <tr key={item.id ?? index}>
                    <td className="fw-semibold">
                      {item.specimenCode ??
                        item.maMau ??
                        '—'}
                    </td>

                    <td>
                      {item.patientName ??
                        item.tenKhachHang ??
                        '—'}
                    </td>

                    <td>
                      {item.testName ??
                        item.tenXetNghiem ??
                        '—'}
                    </td>

                    <td>
                      <span
                        className={`badge ${
                          String(
                            item.priority ?? ''
                          ).toUpperCase() === 'URGENT'
                            ? 'bg-danger'
                            : 'bg-secondary'
                        }`}
                      >
                        {String(
                          item.priority ?? ''
                        ).toUpperCase() === 'URGENT'
                          ? 'Khẩn'
                          : 'Bình thường'}
                      </span>
                    </td>

                    <td>
                      {item.status ??
                        item.trangThai ??
                        '—'}
                    </td>

                    <td className="text-end">
                      <Link
                        to={`/technician/nhap-ket-qua/${item.id}`}
                        className="btn btn-sm btn-outline-primary"
                      >
                        Xử lý
                      </Link>
                    </td>
                  </tr>
                ))}

                {(data.worklist || []).length === 0 && (
                  <tr>
                    <td
                      colSpan="6"
                      className="text-center text-secondary py-4"
                    >
                      Worklist hiện đang trống.
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