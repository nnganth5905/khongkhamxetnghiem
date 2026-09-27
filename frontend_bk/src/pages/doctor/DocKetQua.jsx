// src/pages/doctor/DocKetQua.jsx
import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { getApiErrorMessage } from '../../services/api';
import api from '../../services/api';

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
          const response = await api.get(`/doctor/results/${id}`);
          const data = response.data ?? response;
          
          if (active && data) {
            setResultData(data);
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
      // Gửi kết luận lên API duyệt kết quả
      await api.post(`/doctor/results/${id}/approve`, {
        conclusion: conclusion + (advice ? `\nLời khuyên: ${advice}` : ''),
      });

      setMessage({
        type: 'success',
        text: 'Lưu kết luận và duyệt thành công!',
      });
      setTimeout(() => {
        navigate(-1); // Quay lại trang danh sách chờ duyệt
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
    <div className="container py-4" style={{ maxWidth: '900px' }}>
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
        <>
          {/* Thông tin bệnh nhân và chỉ số xét nghiệm */}
          <div className="card border-0 shadow-sm rounded-4 p-4 mb-4">
            <h5 className="fw-bold mb-3 text-primary">Thông tin xét nghiệm: {resultData?.testName}</h5>
            <div className="row mb-4">
              <div className="col-md-6">
                <p className="mb-1"><strong>Người bệnh:</strong> {resultData?.patientName}</p>
                <p className="mb-1"><strong>Mã mẫu:</strong> {resultData?.specimenCode}</p>
              </div>
              <div className="col-md-6">
                <p className="mb-1"><strong>Ghi chú từ KTV:</strong> <span className="text-danger">{resultData?.technicianNotes || 'Không có ghi chú'}</span></p>
              </div>
            </div>

            <h6 className="fw-bold mb-2">Bảng chỉ số chi tiết</h6>
            <div className="table-responsive">
              <table className="table table-bordered align-middle">
                <thead className="table-light">
                  <tr>
                    <th>Chỉ số</th>
                    <th>Kết quả đo</th>
                    <th>Đơn vị</th>
                    <th>Đánh giá của KTV</th>
                  </tr>
                </thead>
                <tbody>
                  {resultData?.indicators?.length > 0 ? (
                    resultData.indicators.map((ind, i) => (
                      <tr key={i}>
                        <td className="fw-semibold">{ind.name}</td>
                        <td className={ind.abnormal ? 'text-danger fw-bold' : ''}>{ind.value || '---'}</td>
                        <td>{ind.unit}</td>
                        <td>
                          {ind.abnormal ? (
                            <span className="badge bg-danger">Bất thường</span>
                          ) : (
                            <span className="badge bg-success">Bình thường</span>
                          )}
                        </td>
                      </tr>
                    ))
                  ) : (
                    <tr>
                      <td colSpan="4" className="text-center text-muted">Chưa có chỉ số.</td>
                    </tr>
                  )}
                </tbody>
              </table>
            </div>
          </div>

          {/* Form nhập kết luận */}
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

            <div className="mt-3 text-end">
              <button
                type="submit"
                className="btn btn-primary px-4"
                disabled={submitting}
              >
                <i className="fa-solid fa-check-double me-2"></i>
                {submitting ? 'Đang lưu...' : 'Ký Duyệt & Trả Kết Quả'}
              </button>
            </div>
          </form>
        </>
      )}
    </div>
  );
}