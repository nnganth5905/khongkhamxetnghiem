import React, { useEffect, useState } from 'react';
import { Link, useNavigate, useSearchParams } from 'react-router-dom';
import { getApiErrorMessage, getAppointmentById, cancelAppointment } from '../../services/appointmentService';
import api from '../../services/api';

export default function LichHenSua() {
  const [searchParams] = useSearchParams();
  const id = searchParams.get('id');
  
  const navigate = useNavigate();

  const [appointment, setAppointment] = useState(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [message, setMessage] = useState({ type: '', text: '' });

  const [formData, setFormData] = useState({
    new_date: '',
    new_time: '',
    ghichu: ''
  });

  // Danh sách các khung giờ theo format mới
  const timeSlots = [
    '07:00', '08:00', '09:00', '10:00', '11:00',
    '12:00', '13:00', '14:00', '15:00', '16:00'
  ];

  useEffect(() => {
    const fetchAppointmentDetail = async () => {
      try {
        setLoading(true);
        const data = await getAppointmentById(id);
        
        setAppointment(data);
        
        // Xử lý lấy giờ từ data (VD: "08:30:00" -> "08:00" để match với select box)
        let timeValue = '';
        if (data.appointmentTime) {
          const hour = data.appointmentTime.split(':')[0];
          timeValue = `${hour}:00`;
        }

        setFormData({
          new_date: data.appointmentDate ? data.appointmentDate.substring(0, 10) : '',
          new_time: timeValue,
          ghichu: data.note || ''
        });
      } catch (error) {
        setMessage({ type: 'danger', text: getApiErrorMessage(error, 'Không thể tải thông tin lịch hẹn.') });
      } finally {
        setLoading(false);
      }
    };

    if (id) {
      fetchAppointmentDetail();
    } else {
      setMessage({ type: 'danger', text: 'Thiếu mã lịch hẹn hợp lệ trên URL.' });
      setLoading(false);
    }
  }, [id]);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSaving(true);
    setMessage({ type: '', text: '' });

    try {
      await api.put(`/appointments/${id}`, formData);
      setMessage({ type: 'success', text: 'Đổi lịch hẹn thành công!' });
      setTimeout(() => navigate(`/lich-hen/${id}`), 1500); 
    } catch (error) {
      setMessage({ type: 'danger', text: getApiErrorMessage(error, 'Không thể đổi lịch hẹn lúc này.') });
      setSaving(false);
    }
  };

  const handleCancel = async () => {
    if (!window.confirm("Bạn có chắc chắn muốn hủy lịch hẹn này? Hành động này không thể hoàn tác.")) {
      return;
    }
    setSaving(true);
    try {
      await cancelAppointment(id);
      setMessage({ type: 'success', text: 'Đã hủy lịch hẹn thành công!' });
      setTimeout(() => navigate('/lich-hen'), 1500);
    } catch (error) {
      setMessage({ type: 'danger', text: getApiErrorMessage(error, 'Không thể hủy lịch hẹn.') });
      setSaving(false);
    }
  };

  if (loading) {
    return <div className="bg-light min-vh-100 d-flex align-items-center justify-content-center"><div className="spinner-border text-primary"></div></div>;
  }

  return (
    <div className="bg-light min-vh-100 py-5">
      <div className="container" style={{ maxWidth: '600px' }}>
        <div className="d-flex align-items-center mb-4">
          <button className="btn btn-light border-0 shadow-sm rounded-circle me-3" onClick={() => navigate(-1)}>
            <i className="fa-solid fa-arrow-left"></i>
          </button>
          <h3 className="fw-bold mb-0 text-primary">Thay đổi lịch hẹn</h3>
        </div>

        {message.text && <div className={`alert alert-${message.type} shadow-sm rounded-3`}>{message.text}</div>}

        {appointment && (
          <div className="card border-0 shadow-sm rounded-4">
            <div className="card-body p-4">
              <div className="alert alert-info py-2 px-3 mb-4 rounded-3 d-flex align-items-center">
                <i className="fa-solid fa-circle-info me-2"></i>
                <small>Đang chỉnh sửa lịch hẹn: <strong>{id}</strong> ({appointment.type === 'EXAMINATION' ? 'Khám bệnh' : 'Xét nghiệm'})</small>
              </div>

              <form onSubmit={handleSubmit}>
                <div className="mb-3">
                  <label className="form-label fw-semibold">Bác sĩ phụ trách / Phân loại</label>
                  <input type="text" className="form-control bg-light" value={appointment.doctorName || (appointment.type === 'EXAMINATION' ? 'Khám bệnh' : 'Xét nghiệm')} disabled />
                </div>

                <div className="row">
                  <div className="col-md-6 mb-3">
                    <label className="form-label fw-semibold">Ngày khám mới <span className="text-danger">*</span></label>
                    <input 
                      type="date" 
                      className="form-control" 
                      name="new_date"
                      value={formData.new_date}
                      onChange={handleChange}
                      min={new Date().toISOString().split("T")[0]} 
                      required 
                    />
                  </div>
                  <div className="col-md-6 mb-3">
                    <label className="form-label fw-semibold">Giờ khám mới <span className="text-danger">*</span></label>
                    <select 
                      className="form-select" 
                      name="new_time"
                      value={formData.new_time}
                      onChange={handleChange}
                      required
                    >
                      <option value="">-- Chọn giờ --</option>
                      {timeSlots.map(time => (
                        <option key={time} value={time}>{time}</option>
                      ))}
                    </select>
                  </div>
                </div>

                <div className="mb-4">
                  <label className="form-label fw-semibold">Ghi chú thêm</label>
                  <textarea 
                    className="form-control" 
                    rows="3" 
                    name="ghichu"
                    value={formData.ghichu}
                    onChange={handleChange}
                    placeholder="Lý do đổi lịch..."
                  ></textarea>
                </div>

                <div className="d-flex justify-content-between mt-2 pt-3 border-top">
                  <button type="button" className="btn btn-outline-danger px-4 rounded-pill fw-medium" onClick={handleCancel} disabled={saving}>
                    <i className="fa-solid fa-trash me-2"></i>Hủy lịch hẹn này
                  </button>
                  <div className="d-flex gap-2">
                    <button type="submit" className="btn btn-primary px-4 rounded-pill" disabled={saving}>
                      {saving ? 'Đang xử lý...' : 'Lưu thay đổi'}
                    </button>
                  </div>
                </div>
              </form>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}