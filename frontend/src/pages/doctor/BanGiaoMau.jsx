// src/pages/doctor/BanGiaoMau.jsx
import React, { useState, useEffect } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import api, { getApiErrorMessage } from '../../services/api';

export default function BanGiaoMau() {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  
  const [formData, setFormData] = useState({
    specimenCode: '',
    receiverName: '',
    notes: '',
    appointmentId: '',
    patientCode: ''
  });
  
  const [technicians, setTechnicians] = useState([]);
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState({ type: '', text: '' });

  useEffect(() => {
    const code = searchParams.get('specimenCode');
    const aptId = searchParams.get('appointmentId');
    const pCode = searchParams.get('patientCode');
    if (code) {
      setFormData(prev => ({ ...prev, specimenCode: code, appointmentId: aptId, patientCode: pCode }));
    }

    const fetchKTVs = async () => {
      try {
        const response = await api.get('/technicians');
        setTechnicians(response.data || []);
      } catch (error) {
        console.error("Lỗi lấy danh sách KTV:", error);
      }
    };
    fetchKTVs();
  }, [searchParams]);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!formData.receiverName) {
        setMessage({ type: 'danger', text: 'Vui lòng chọn kỹ thuật viên tiếp nhận.' });
        return;
    }

    setLoading(true);
    setMessage({ type: '', text: '' });

    try {
      const res = await api.post(`/doctor/specimens/${formData.specimenCode}/handover`, {
          receiverId: formData.receiverName,
          note: formData.notes,
          appointmentId: formData.appointmentId,
          patientCode: formData.patientCode
      });
      
      if (res.data.error) {
        throw new Error(res.data.error);
      }
      
      setMessage({ type: 'success', text: 'Bàn giao mẫu bệnh phẩm thành công! Khách hàng đã chuyển sang trạng thái chờ kết quả.' });
      setTimeout(() => navigate('/doctor/danh-sach-cho'), 2000);
    } catch (error) {
      setMessage({ type: 'danger', text: getApiErrorMessage(error, 'Lỗi hệ thống khi bàn giao.') });
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="container py-4" style={{ maxWidth: '720px' }}>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h2 className="h4 fw-bold mb-1">Bàn giao mẫu bệnh phẩm</h2>
          <p className="text-secondary mb-0">Chuyển mẫu sang phòng xét nghiệm</p>
        </div>
        <button type="button" className="btn btn-outline-secondary btn-sm" onClick={() => navigate(-1)}>Quay lại</button>
      </div>

      {message.text && (
        <div className={`alert alert-${message.type} mb-3`} role="alert">{message.text}</div>
      )}

      <form className="card border-0 shadow-sm rounded-4 p-4" onSubmit={handleSubmit}>
        <div className="mb-3">
          <label className="form-label fw-semibold">Mã mẫu bệnh phẩm *</label>
          <input type="text" name="specimenCode" className="form-control bg-light" value={formData.specimenCode} readOnly required />
        </div>

        <div className="mb-3">
          <label className="form-label fw-semibold">Kỹ thuật viên tiếp nhận *</label>
          <select name="receiverName" className="form-control form-select" value={formData.receiverName} onChange={handleChange} required>
            <option value="">-- Chọn kỹ thuật viên --</option>
            {technicians.map((ktv) => (
              <option key={ktv.id} value={ktv.id}>{ktv.name} (KTV)</option>
            ))}
          </select>
        </div>

        <div className="mb-3">
          <label className="form-label fw-semibold">Ghi chú bàn giao</label>
          <textarea name="notes" rows="3" className="form-control" value={formData.notes} onChange={handleChange} />
        </div>

        <div className="mt-3">
          <button type="submit" className="btn btn-primary px-4" disabled={loading}>
            {loading ? 'Đang bàn giao...' : 'Xác nhận bàn giao'}
          </button>
        </div>
      </form>
    </div>
  );
}