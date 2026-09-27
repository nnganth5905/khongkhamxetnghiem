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
        // SỬA LỖI Ở ĐÂY: Trích xuất chính xác mảng 'data' bên trong response
        const dataArray = response.data?.data || response.data || [];
        
        // Đảm bảo state luôn là một mảng để tránh lỗi .map() is not a function
        setTechnicians(Array.isArray(dataArray) ? dataArray : []);
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
      
      // Bắt thêm trường hợp backend trả về false nhưng HTTP status vẫn là 200
      if (res.data?.error || res.data?.success === false) {
        throw new Error(res.data?.error || res.data?.message || 'Lỗi từ máy chủ');
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
    <div className="container mt-5">
      <h2>Bàn giao mẫu bệnh phẩm</h2>
      {message.text && (
        <div className={`alert alert-${message.type}`} role="alert">
          {message.text}
        </div>
      )}
      <form onSubmit={handleSubmit}>
        <div className="mb-3">
          <label htmlFor="specimenCode" className="form-label">Mã mẫu bệnh phẩm</label>
          <input type="text" className="form-control" id="specimenCode" name="specimenCode" value={formData.specimenCode} readOnly />
        </div>
        <div className="mb-3">
          <label htmlFor="receiverName" className="form-label">Kỹ thuật viên tiếp nhận</label>
          <select className="form-select" id="receiverName" name="receiverName" value={formData.receiverName} onChange={handleChange} required>
            <option value="">Chọn kỹ thuật viên</option>
            {technicians.map((tech) => (
              <option key={tech.id} value={tech.id}>{tech.name}</option>
            ))}
          </select>
        </div>
        <div className="mb-3">
          <label htmlFor="notes" className="form-label">Ghi chú</label>
          <textarea className="form-control" id="notes" name="notes" value={formData.notes} onChange={handleChange}></textarea>
        </div>
        <button type="submit" className="btn btn-primary" disabled={loading}>
          {loading ? 'Đang xử lý...' : 'Bàn giao'}
        </button>
      </form> 
    </div>
  );
}
