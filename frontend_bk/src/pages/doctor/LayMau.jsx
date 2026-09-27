// src/pages/doctor/LayMau.jsx
import React, { useState, useEffect } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import api, { getApiErrorMessage } from '../../services/api';
import { getAppointmentDetail } from '../../services/appointmentService'; 

export default function LayMau() {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const appointmentId = searchParams.get('id'); 

  const [formData, setFormData] = useState({
    patientCode: '',
    patientName: '',
    sampleType: 'Máu toàn phần',
    barcode: '',
    samplingTime: new Date().toISOString().slice(0, 16),
    notes: '',
  });

  const [loading, setLoading] = useState(false);
  const [fetching, setFetching] = useState(false);
  const [isCollected, setIsCollected] = useState(false); 
  const [message, setMessage] = useState({ type: '', text: '' });

  useEffect(() => {
    if (appointmentId) fetchAppointmentData(appointmentId);
  }, [appointmentId]);

  const fetchAppointmentData = async (id) => {
    try {
      setFetching(true);
      const data = await getAppointmentDetail(id);
      if (data && !data.error) {
        setFormData(prev => ({
          ...prev,
          patientCode: data.patientCode || '',
          patientName: data.customerName || '',
          notes: data.registeredTests && data.registeredTests.length > 0
            ? `Chỉ định xét nghiệm: ${data.registeredTests.join(', ')}`
            : ''
        }));
      }
    } catch (error) {
      setMessage({ type: 'warning', text: 'Không thể tải thông tin bệnh nhân.' });
    } finally {
      setFetching(false);
    }
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setMessage({ type: '', text: '' });

    try {
      let finalBarcode = formData.barcode;
      if (!finalBarcode || !finalBarcode.trim()) {
        const timestamp = Date.now().toString(); 
        const randomNum = Math.floor(Math.random() * 90 + 10).toString(); 
        finalBarcode = timestamp + randomNum;
      }

      const dataToSubmit = {
        ...formData,
        barcode: finalBarcode,
        appointmentId: appointmentId
      };

      const res = await api.post(`/doctor/specimens`, dataToSubmit);
      
      if (res.data.error) {
        throw new Error(res.data.error);
      }

      setFormData(prev => ({ ...prev, barcode: finalBarcode }));
      setIsCollected(true); 
      setMessage({ type: 'success', text: 'Đã lưu mẫu vào CSDL! Vui lòng bàn giao mẫu.' });
      
    } catch (err) {
      setMessage({ type: 'danger', text: getApiErrorMessage(err, 'Lỗi khi lưu mẫu vào cơ sở dữ liệu.') });
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="container py-4" style={{ maxWidth: '780px' }}>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h2 className="h4 fw-bold mb-1">Thu thập mẫu bệnh phẩm</h2>
          <p className="text-secondary mb-0">Ghi nhận thông tin mẫu và gán barcode trước khi chuyển lab</p>
        </div>
        <button type="button" className="btn btn-outline-secondary btn-sm" onClick={() => navigate('/doctor/danh-sach-cho')}>
          Quay lại danh sách chờ
        </button>
      </div>

      {message.text && (
        <div className={`alert alert-${message.type} mb-3`} role="alert">{message.text}</div>
      )}

      {fetching ? (
        <div className="text-center py-5 text-muted"><div className="spinner-border spinner-border-sm me-2"></div>Đang tải...</div>
      ) : (
        <form className="card border-0 shadow-sm rounded-4 p-4" onSubmit={handleSubmit}>
          <div className="row g-3">
            <div className="col-md-6">
              <label className="form-label fw-semibold">Mã bệnh nhân *</label>
              <input type="text" name="patientCode" className="form-control bg-light" value={formData.patientCode} readOnly required />
            </div>
            <div className="col-md-6">
              <label className="form-label fw-semibold">Họ tên bệnh nhân *</label>
              <input type="text" name="patientName" className="form-control bg-light" value={formData.patientName} readOnly required />
            </div>
            <div className="col-md-6">
              <label className="form-label fw-semibold">Loại bệnh phẩm *</label>
              <select name="sampleType" className="form-select" value={formData.sampleType} onChange={handleChange} disabled={isCollected}>
                <option value="Máu toàn phần">Máu toàn phần (EDTA)</option>
                <option value="Huyết thanh">Huyết thanh (Serum)</option>
                <option value="Nước tiểu">Nước tiểu</option>
                <option value="Khác">Khác</option>
              </select>
            </div>
            <div className="col-md-6">
              <label className="form-label fw-semibold">Mã vạch (Barcode)</label>
              <input type="text" name="barcode" className="form-control" placeholder="Để trống hệ thống sẽ tự sinh" value={formData.barcode} onChange={handleChange} disabled={isCollected} />
            </div>
            <div className="col-12">
              <label className="form-label fw-semibold">Ghi chú lâm sàng</label>
              <textarea name="notes" rows="3" className="form-control" value={formData.notes} onChange={handleChange} disabled={isCollected} />
            </div>
          </div>

          <div className="mt-4 d-flex gap-3">
            {!isCollected ? (
              <button type="submit" className="btn btn-primary px-4" disabled={loading}>
                {loading ? 'Đang lưu CSDL...' : 'Xác nhận lấy mẫu'}
              </button>
            ) : (
              <button 
                type="button" 
                className="btn btn-warning px-4 shadow-sm"
                onClick={() => navigate(`/doctor/ban-giao-mau?specimenCode=${formData.barcode}&appointmentId=${appointmentId}&patientCode=${formData.patientCode}`)}
              >
                <i className="fa-solid fa-truck-fast me-2"></i> Bàn giao mẫu
              </button>
            )}
          </div>
        </form>
      )}
    </div>
  );
}