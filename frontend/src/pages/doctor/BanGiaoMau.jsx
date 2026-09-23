// src/pages/doctor/BanGiaoMau.jsx
import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { getApiErrorMessage } from '../../services/api';
import { handoverSpecimen } from '../../services/technicianService';

export default function BanGiaoMau() {
  const navigate = useNavigate();
  const [formData, setFormData] = useState({
    specimenCode: '',
    receiverName: '',
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
      if (handoverSpecimen) {
        await handoverSpecimen(formData.specimenCode, formData);
      }
      setMessage({
        type: 'success',
        text: 'Bàn giao mẫu bệnh phẩm thành công!',
      });
      setFormData({
        specimenCode: '',
        receiverName: '',
        notes: '',
      });
    } catch (error) {
      setMessage({
        type: 'danger',
        text: getApiErrorMessage(error, 'Có lỗi xảy ra khi bàn giao mẫu.'),
      });
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="container py-4" style={{ maxWidth: '720px' }}>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h2 className="h4 fw-bold mb-1">Bàn giao mẫu bệnh phẩm</h2>
          <p className="text-secondary mb-0">
            Chuyển mẫu sang phòng xét nghiệm / kỹ thuật viên tiếp nhận
          </p>
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
        <div className="mb-3">
          <label className="form-label fw-semibold">Mã mẫu bệnh phẩm *</label>
          <input
            type="text"
            name="specimenCode"
            className="form-control"
            placeholder="Ví dụ: MAU12345"
            value={formData.specimenCode}
            onChange={handleChange}
            required
          />
        </div>

        <div className="mb-3">
          <label className="form-label fw-semibold">Người tiếp nhận / Bộ phận nhận *</label>
          <input
            type="text"
            name="receiverName"
            className="form-control"
            placeholder="Ví dụ: Phòng xét nghiệm Huyết học"
            value={formData.receiverName}
            onChange={handleChange}
            required
          />
        </div>

        <div className="mb-3">
          <label className="form-label fw-semibold">Ghi chú bàn giao</label>
          <textarea
            name="notes"
            rows="3"
            className="form-control"
            placeholder="Tình trạng bảo quản, yêu cầu xét nghiệm khẩn..."
            value={formData.notes}
            onChange={handleChange}
          />
        </div>

        <div className="mt-3">
          <button
            type="submit"
            className="btn btn-primary px-4"
            disabled={loading}
          >
            {loading ? 'Đang bàn giao...' : 'Xác nhận bàn giao'}
          </button>
        </div>
      </form>
    </div>
  );
}