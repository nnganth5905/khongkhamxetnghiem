// src/pages/doctor/LayMau.jsx
import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { getApiErrorMessage } from '../../services/api';

export default function LayMau() {
  const navigate = useNavigate();

  const [formData, setFormData] = useState({
    patientCode: '',
    patientName: '',
    sampleType: 'MauToanPhan',
    barcode: '',
    samplingTime: new Date().toISOString().slice(0, 16),
    notes: '',
  });

  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState({ type: '', text: '' });

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setMessage({ type: '', text: '' });

    try {
      // Logic lưu thông tin lấy mẫu bệnh phẩm
      setMessage({
        type: 'success',
        text: 'Ghi nhận lấy mẫu bệnh phẩm thành công!',
      });
      setFormData({
        patientCode: '',
        patientName: '',
        sampleType: 'MauToanPhan',
        barcode: '',
        samplingTime: new Date().toISOString().slice(0, 16),
        notes: '',
      });
    } catch (err) {
      setMessage({
        type: 'danger',
        text: getApiErrorMessage(err, 'Có lỗi xảy ra khi lưu thông tin lấy mẫu.'),
      });
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
        <button
          type="button"
          className="btn btn-outline-secondary btn-sm"
          onClick={() => navigate(-1)}
        >
          Quay lại
        </button>
      </div>

      {message.text && (
        <div className={`alert alert-${message.type} mb-3`} role="alert">
          {message.text}
        </div>
      )}

      <form className="card border-0 shadow-sm rounded-4 p-4" onSubmit={handleSubmit}>
        <div className="row g-3">
          <div className="col-md-6">
            <label className="form-label fw-semibold">Mã bệnh nhân *</label>
            <input
              type="text"
              name="patientCode"
              className="form-control"
              placeholder="VD: BN00123"
              value={formData.patientCode}
              onChange={handleChange}
              required
            />
          </div>

          <div className="col-md-6">
            <label className="form-label fw-semibold">Họ tên bệnh nhân *</label>
            <input
              type="text"
              name="patientName"
              className="form-control"
              placeholder="Nguyễn Văn A"
              value={formData.patientName}
              onChange={handleChange}
              required
            />
          </div>

          <div className="col-md-6">
            <label className="form-label fw-semibold">Loại bệnh phẩm *</label>
            <select
              name="sampleType"
              className="form-select"
              value={formData.sampleType}
              onChange={handleChange}
            >
              <option value="MauToanPhan">Máu toàn phần (EDTA)</option>
              <option value="HuyetThanh">Huyết thanh (Serum)</option>
              <option value="NuocTieu">Nước tiểu</option>
              <option value="DichQuet">Dịch quẹt họng / mũi</option>
              <option value="Khac">Khác</option>
            </select>
          </div>

          <div className="col-md-6">
            <label className="form-label fw-semibold">Mã vạch (Barcode / Tube ID) *</label>
            <input
              type="text"
              name="barcode"
              className="form-control"
              placeholder="Quét hoặc nhập mã tube"
              value={formData.barcode}
              onChange={handleChange}
              required
            />
          </div>

          <div className="col-md-6">
            <label className="form-label fw-semibold">Thời gian lấy mẫu</label>
            <input
              type="datetime-local"
              name="samplingTime"
              className="form-control"
              value={formData.samplingTime}
              onChange={handleChange}
            />
          </div>

          <div className="col-12">
            <label className="form-label fw-semibold">Ghi chú lâm sàng / Tình trạng mẫu</label>
            <textarea
              name="notes"
              rows="3"
              className="form-control"
              placeholder="Ví dụ: Bệnh nhân lấy máu lúc đói, mẫu có tiêu huyết nhẹ..."
              value={formData.notes}
              onChange={handleChange}
            />
          </div>
        </div>

        <div className="mt-4">
          <button
            type="submit"
            className="btn btn-primary px-4"
            disabled={loading}
          >
            {loading ? 'Đang lưu...' : 'Xác nhận lấy mẫu'}
          </button>
        </div>
      </form>
    </div>
  );
}