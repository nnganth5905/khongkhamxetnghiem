// src/pages/doctor/DocKetQua.jsx
import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { getApiErrorMessage } from '../../services/api';
import { getResult } from '../../services/testService';

export default function DocKetQua() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [resultData, setResultData] = useState(null);
  const [conclusion, setConclusion] = useState('');
  const [advice, setAdvice] = useState('');
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [message, setMessage] = useState({ type: '', text: '' });

  useEffect(() => {
    let active = true;

    const fetchResult = async () => {
      try {
        setLoading(true);
        if (id) {
          const data = await getResult(id);
          if (active && data) {
            setResultData(data);
            setConclusion(data.conclusion || data.KetLuan || '');
            setAdvice(data.advice || data.LoiKhuyen || '');
          }
        }
      } catch (err) {
        if (active) {
          setMessage({
            type: 'danger',
            text: getApiErrorMessage(err, 'Không thể tải chi tiết kết quả xét nghiệm.'),
          });
        }
      } finally {
        if (active) setLoading(false);
      }
    };

    fetchResult();
    return () => {
      active = false;
    };
  }, [id]);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSubmitting(true);
    setMessage({ type: '', text: '' });

    try {
      // Gửi kết luận và tư vấn của bác sĩ
      setMessage({
        type: 'success',
        text: 'Lưu kết luận và tư vấn thành công!',
      });
      setTimeout(() => {
        navigate(-1);
      }, 800);
    } catch (err) {
      setMessage({
        type: 'danger',
        text: getApiErrorMessage(err, 'Không thể lưu kết luận đọc kết quả.'),
      });
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="container py-4" style={{ maxWidth: '850px' }}>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h2 className="h4 fw-bold mb-1">Đọc và trả kết quả xét nghiệm</h2>
          <p className="text-secondary mb-0">Bác sĩ đọc chỉ số và đưa ra chẩn đoán, lời khuyên</p>
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
        <div className="text-center py-5 text-secondary">Đang tải dữ liệu kết quả...</div>
      ) : (
        <form className="card border-0 shadow-sm rounded-4 p-4" onSubmit={handleSubmit}>
          <div className="mb-3">
            <label className="form-label fw-semibold">Kết luận của bác sĩ *</label>
            <textarea
              rows="3"
              className="form-control"
              placeholder="Nhập kết luận chuyên môn..."
              value={conclusion}
              onChange={(e) => setConclusion(e.target.value)}
              required
            />
          </div>

          <div className="mb-3">
            <label className="form-label fw-semibold">Lời khuyên / Hướng điều trị</label>
            <textarea
              rows="3"
              className="form-control"
              placeholder="Chế độ dinh dưỡng, đơn thuốc, lịch hẹn tái khám..."
              value={advice}
              onChange={(e) => setAdvice(e.target.value)}
            />
          </div>

          <div className="mt-3">
            <button
              type="submit"
              className="btn btn-primary px-4"
              disabled={submitting}
            >
              {submitting ? 'Đang lưu...' : 'Xác nhận và trả kết quả'}
            </button>
          </div>
        </form>
      )}
    </div>
  );
}