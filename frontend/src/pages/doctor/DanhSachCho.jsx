// src/pages/doctor/DanhSachCho.jsx
import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { getApiErrorMessage } from '../../services/api';
import { getWaitingQueue, callPatient, holdPatient, skipPatient } from '../../services/appointmentService';

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
      setMessage({ type: 'danger', text: getApiErrorMessage(err, 'Không thể tải danh sách hàng chờ.') });
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchQueue();
  }, []);

  const handleCall = async (id, type) => {
    try {
      await callPatient(id);
      setMessage({ type: 'success', text: 'Đang gọi bệnh nhân... Vui lòng chờ bệnh nhân vào phòng.' });
      fetchQueue();
    } catch (err) {
      setMessage({ type: 'danger', text: getApiErrorMessage(err, 'Không thể gọi bệnh nhân.') });
    }
  };

  const handleStartExam = (id, type) => {
    if (type === 'TEST') {
      navigate(`/doctor/lay-mau?id=${id}`);
    } else {
      navigate(`/doctor/kham-benh?luotKhamId=${id}`); 
    }
  };

  const handleHold = async (id) => {
    try {
      await holdPatient(id);
      setMessage({ type: 'warning', text: 'Bệnh nhân chưa có mặt. Đã chuyển xuống cuối hàng chờ!' });
      fetchQueue();
    } catch (err) {
      setMessage({ type: 'danger', text: getApiErrorMessage(err, 'Không thể thực hiện thao tác.') });
    }
  };

  const handleSkip = async (id) => {
    if (!window.confirm('Bệnh nhân này sẽ bị tính là Bỏ lượt (Không đến). Bạn có chắc chắn?')) return;
    try {
      await skipPatient(id, 'Bệnh nhân vắng mặt');
      setMessage({ type: 'secondary', text: 'Bệnh nhân đã bị đánh dấu bỏ lượt.' });
      fetchQueue();
    } catch (err) {
      setMessage({ type: 'danger', text: getApiErrorMessage(err, 'Không thể chuyển lượt.') });
    }
  };

  return (
    <div className="container py-4">
      <div className="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h2 className="h4 fw-bold mb-1">Danh sách hàng chờ</h2>
          <p className="text-secondary mb-0">Tiếp nhận bệnh nhân Khám bệnh & Lấy mẫu xét nghiệm</p>
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
                <th style={{ width: '25%' }}>Dịch vụ chỉ định</th>
                <th>Giờ hẹn</th>
                <th>Trạng thái</th>
                <th className="text-end px-3">Hành động</th>
              </tr>
            </thead>
            <tbody>
              {loading ? (
                <tr>
                  <td colSpan="7" className="text-center py-4 text-muted">
                    Đang tải dữ liệu hàng chờ...
                  </td>
                </tr>
              ) : queue.length === 0 ? (
                <tr>
                  <td colSpan="7" className="text-center py-4 text-muted">
                    Hiện không có bệnh nhân nào trong hàng chờ.
                  </td>
                </tr>
              ) : (
                queue.map((item, index) => {
                  const id = item.id || item.IDDatLich || item.appointmentId;
                  const patientName = item.hoten || item.fullName || item.HoTen || 'N/A';
                  const patientCode = item.patientCode || item.maBN || '---';
                  const appointmentTime = item.gio || item.time || '---';
                  const status = item.status || 'Chờ tiếp nhận';
                  const statusCode = item.statusCode || '';
                  const type = item.type || 'EXAMINATION';
                  const dichVu = item.dichVu || '---';

                  const isExam = type === 'EXAMINATION';
                  const isCalling = statusCode === 'da_den_luot';

                  return (
                    <tr key={id || index} className={isCalling ? "table-primary" : ""}>
                      <td className="px-3 fw-bold">{item.stt || index + 1}</td>
                      <td>{patientCode}</td>
                      <td className="fw-semibold">
                        {patientName}
                        <br />
                        <small className={isExam ? 'text-primary' : 'text-success'}>
                          {isExam ? 'Khám bệnh' : 'Xét nghiệm'}
                        </small>
                      </td>
                      
                      {/* MỚI: Hiển thị dịch vụ (lý do khám / danh sách test) */}
                      <td className="text-wrap">
                        <span className="text-muted small">{dichVu}</span>
                      </td>

                      <td>{appointmentTime}</td>
                      <td>
                        <span className={`badge ${isCalling ? 'bg-primary text-white' : (status === 'Chờ gọi lại' ? 'bg-danger-subtle text-danger-emphasis' : (isExam ? 'bg-warning-subtle text-warning-emphasis' : 'bg-info-subtle text-info-emphasis'))} px-2 py-1`}>
                          {status}
                        </span>
                      </td>
                      <td className="text-end px-3 text-nowrap">
                        {!isCalling ? (
                          <>
                            <button
                              type="button"
                              className="btn btn-primary btn-sm me-2"
                              onClick={() => handleCall(id, type)}
                            >
                              {isExam ? 'Gọi vào phòng' : 'Gọi lấy mẫu'}
                            </button>
                            <button
                              type="button"
                              className="btn btn-outline-danger btn-sm"
                              onClick={() => handleSkip(id)}
                              title="Đánh dấu bệnh nhân không đến"
                            >
                              Bỏ qua
                            </button>
                          </>
                        ) : (
                          <>
                            <button
                              type="button"
                              className="btn btn-success btn-sm me-2 shadow-sm"
                              onClick={() => handleStartExam(id, type)}
                            >
                              <i className="fa-solid fa-play me-1"></i> {isExam ? 'Khám bệnh' : 'Lấy mẫu'}
                            </button>
                            <button
                              type="button"
                              className="btn btn-warning btn-sm shadow-sm"
                              onClick={() => handleHold(id)}
                              title="Bệnh nhân chưa tới, gọi lại sau"
                            >
                              <i className="fa-solid fa-clock-rotate-left me-1"></i> Gọi lại sau
                            </button>
                          </>
                        )}
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