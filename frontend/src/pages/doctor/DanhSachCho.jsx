// src/pages/doctor/DanhSachCho.jsx
import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { getApiErrorMessage } from '../../services/api';
import { getWaitingQueue, callPatient, skipPatient } from '../../services/appointmentService';

export default function DoctorDanhSachCho() {
  const navigate = useNavigate();
  const [queue, setQueue] = useState([]);
  const [loading, setLoading] = useState(true);
  const [message, setMessage] = useState({ type: '', text: '' });

  const fetchQueue = async () => {
    try {
      setLoading(true);
      const data = await getWaitingQueue();
      setQueue(Array.isArray(data) ? data : data?.queue || data?.items || []);
    } catch (err) {
      setMessage({
        type: 'danger',
        text: getApiErrorMessage(err, 'Không thể tải danh sách hàng chờ.'),
      });
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchQueue();
  }, []);

  const handleCall = async (id) => {
    try {
      await callPatient(id);
      setMessage({ type: 'success', text: 'Đã gọi bệnh nhân vào phòng khám!' });
      fetchQueue();
    } catch (err) {
      setMessage({
        type: 'danger',
        text: getApiErrorMessage(err, 'Không thể gọi bệnh nhân.'),
      });
    }
  };

  const handleSkip = async (id) => {
    try {
      await skipPatient(id, 'Bệnh nhân vắng mặt');
      setMessage({ type: 'warning', text: 'Đã bỏ qua số thứ tự này.' });
      fetchQueue();
    } catch (err) {
      setMessage({
        type: 'danger',
        text: getApiErrorMessage(err, 'Không thể chuyển lượt.'),
      });
    }
  };

  return (
    <div className="container py-4">
      <div className="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h2 className="h4 fw-bold mb-1">Danh sách chờ khám bệnh</h2>
          <p className="text-secondary mb-0">Hàng đợi tiếp nhận bệnh nhân tại phòng khám</p>
        </div>
        <button
          type="button"
          className="btn btn-outline-primary btn-sm"
          onClick={fetchQueue}
          disabled={loading}
        >
          Làm mới danh sách
        </button>
      </div>

      {message.text && (
        <div className={`alert alert-${message.type} mb-3`} role="alert">
          {message.text}
        </div>
      )}

      <div className="card border-0 shadow-sm rounded-4 overflow-hidden">
        <div className="table-responsive">
          <table className="table table-hover align-middle mb-0">
            <thead className="table-light">
              <tr>
                <th className="px-3">STT</th>
                <th>Mã bệnh nhân</th>
                <th>Họ tên</th>
                <th>Giờ hẹn</th>
                <th>Trạng thái</th>
                <th className="text-end px-3">Hành động</th>
              </tr>
            </thead>
            <tbody>
              {loading ? (
                <tr>
                  <td colSpan="6" className="text-center py-4 text-muted">
                    Đang tải dữ liệu hàng chờ...
                  </td>
                </tr>
              ) : queue.length === 0 ? (
                <tr>
                  <td colSpan="6" className="text-center py-4 text-muted">
                    Hiện không có bệnh nhân nào trong hàng chờ.
                  </td>
                </tr>
              ) : (
                queue.map((item, index) => {
                  const id = item.id || item.IDDatLich || item.appointmentId;
                  const patientName = item.hoten || item.fullName || item.HoTen || 'N/A';
                  const patientCode = item.maBN || item.patientCode || '---';
                  const appointmentTime = item.gio || item.time || '---';
                  const status = item.status || item.TrangThai || 'Chờ khám';

                  return (
                    <tr key={id || index}>
                      <td className="px-3 fw-bold">{index + 1}</td>
                      <td>{patientCode}</td>
                      <td className="fw-semibold">{patientName}</td>
                      <td>{appointmentTime}</td>
                      <td>
                        <span className="badge bg-warning-subtle text-warning-emphasis px-2 py-1">
                          {status}
                        </span>
                      </td>
                      <td className="text-end px-3">
                        <button
                          type="button"
                          className="btn btn-primary btn-sm me-2"
                          onClick={() => handleCall(id)}
                        >
                          Gọi vào khám
                        </button>
                        <button
                          type="button"
                          className="btn btn-outline-secondary btn-sm"
                          onClick={() => handleSkip(id)}
                        >
                          Bỏ qua
                        </button>
                      </td>
                    </tr>
                  );
                })
              )}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}