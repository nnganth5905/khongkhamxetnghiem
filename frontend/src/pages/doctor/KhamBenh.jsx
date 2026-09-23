// src/pages/doctor/KhamBenh.jsx
import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { getApiErrorMessage } from '../../services/api';
import { getAppointmentById } from '../../services/appointmentService';

export default function KhamBenh() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [appointment, setAppointment] = useState(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [message, setMessage] = useState({ type: '', text: '' });

  const [medicalRecord, setMedicalRecord] = useState({
    symptoms: '',
    diagnosis: '',
    treatmentPlan: '',
    prescription: '',
  });

  useEffect(() => {
    let active = true;

    const fetchAppointment = async () => {
      try {
        setLoading(true);
        if (id) {
          const data = await getAppointmentById(id);
          if (active && data) {
            setAppointment(data);
          }
        }
      } catch (err) {
        if (active) {
          setMessage({
            type: 'danger',
            text: getApiErrorMessage(err, 'Không thể tải thông tin lượt khám.'),
          });
        }
      } finally {
        if (active) setLoading(false);
      }
    };

    fetchAppointment();
    return () => {
      active = false;
    };
  }, [id]);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setMedicalRecord((prev) => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSaving(true);
    setMessage({ type: '', text: '' });

    try {
      setMessage({
        type: 'success',
        text: 'Lưu hồ sơ bệnh án thành công!',
      });
      setTimeout(() => {
        navigate('/doctor/danh-sach-cho');
      }, 800);
    } catch (err) {
      setMessage({
        type: 'danger',
        text: getApiErrorMessage(err, 'Có lỗi xảy ra khi lưu kết quả khám.'),
      });
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="container py-4" style={{ maxWidth: '860px' }}>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h2 className="h4 fw-bold mb-1">Khám bệnh & Kê đơn</h2>
          <p className="text-secondary mb-0">Hồ sơ thăm khám lâm sàng của bệnh nhân</p>
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

      {loading ? (
        <div className="text-center py-5 text-secondary">Đang tải thông tin...</div>
      ) : (
        <form className="card border-0 shadow-sm rounded-4 p-4" onSubmit={handleSubmit}>
          <div className="mb-3">
            <label className="form-label fw-semibold">Triệu chứng lâm sàng *</label>
            <textarea
              name="symptoms"
              rows="3"
              className="form-control"
              placeholder="Ghi nhận triệu chứng, tiền sử bệnh..."
              value={medicalRecord.symptoms}
              onChange={handleChange}
              required
            />
          </div>

          <div className="mb-3">
            <label className="form-label fw-semibold">Chẩn đoán xác định *</label>
            <input
              type="text"
              name="diagnosis"
              className="form-control"
              placeholder="Chẩn đoán bệnh học..."
              value={medicalRecord.diagnosis}
              onChange={handleChange}
              required
            />
          </div>

          <div className="mb-3">
            <label className="form-label fw-semibold">Chỉ định điều trị / Lời dặn</label>
            <textarea
              name="treatmentPlan"
              rows="2"
              className="form-control"
              placeholder="Chế độ sinh hoạt, tái khám..."
              value={medicalRecord.treatmentPlan}
              onChange={handleChange}
            />
          </div>

          <div className="mb-3">
            <label className="form-label fw-semibold">Đơn thuốc (nếu có)</label>
            <textarea
              name="prescription"
              rows="3"
              className="form-control"
              placeholder="Tên thuốc, liều dùng, cách dùng..."
              value={medicalRecord.prescription}
              onChange={handleChange}
            />
          </div>

          <div className="mt-4 d-flex gap-2">
            <button
              type="submit"
              className="btn btn-primary px-4"
              disabled={saving}
            >
              {saving ? 'Đang lưu...' : 'Hoàn thành khám'}
            </button>
            <button
              type="button"
              className="btn btn-outline-primary px-3"
              onClick={() => navigate('/doctor/chi-dinh-xet-nghiem')}
            >
              Chỉ định xét nghiệm
            </button>
          </div>
        </form>
      )}
    </div>
  );
}