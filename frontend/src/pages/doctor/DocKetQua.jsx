// src/pages/doctor/DocKetQua.jsx
import React, { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import api, { getApiErrorMessage } from '../../services/api';

export default function DocKetQua() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [results, setResults] = useState([]);
  const [resultData, setResultData] = useState(null);
  const [conclusion, setConclusion] = useState('');
  const [advice, setAdvice] = useState('');
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [message, setMessage] = useState({ type: '', text: '' });

  useEffect(() => {
    let active = true;

    const load = async () => {
      try {
        setLoading(true);
        setMessage({ type: '', text: '' });

        // Không có id: hiển thị danh sách kết quả đang chờ bác sĩ đọc.
        if (!id) {
          const response = await api.get('/doctor/results/pending');
          const data = response.data ?? response;
          if (active) {
            setResults(Array.isArray(data) ? data : data?.items || data?.content || []);
          }
          return;
        }

        // Có id: hiển thị chi tiết kết quả.
        const response = await api.get(`/doctor/results/${id}`);
        const data = response.data ?? response;
        if (active) setResultData(data);
      } catch (err) {
        if (active) {
          setMessage({
            type: 'danger',
            text: getApiErrorMessage(
              err,
              id
                ? 'Không thể tải chi tiết kết quả xét nghiệm.'
                : 'Không thể tải danh sách kết quả xét nghiệm.'
            ),
          });
        }
      } finally {
        if (active) setLoading(false);
      }
    };

    load();
    return () => {
      active = false;
    };
  }, [id]);

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!id) return;

    setSubmitting(true);
    setMessage({ type: '', text: '' });

    try {
      await api.post(`/doctor/results/${id}/approve`, {
        conclusion: conclusion + (advice ? `\nLời khuyên: ${advice}` : ''),
      });

      setMessage({
        type: 'success',
        text: 'Lưu kết luận và duyệt kết quả thành công!',
      });

      setTimeout(() => navigate('/doctor/duyet-ket-qua'), 800);
    } catch (err) {
      setMessage({
        type: 'danger',
        text: getApiErrorMessage(err, 'Không thể lưu kết luận đọc kết quả.'),
      });
    } finally {
      setSubmitting(false);
    }
  };

  // Trang danh sách khi bấm trực tiếp "Đọc kết quả" trên sidebar.
  if (!id) {
    return (
      <div className="container py-4">
        <div className="d-flex justify-content-between align-items-center mb-4">
          <div>
            <h2 className="h4 fw-bold mb-1">Đọc kết quả xét nghiệm</h2>
            <p className="text-secondary mb-0">
              Chọn một kết quả đang chờ để xem các chỉ số xét nghiệm.
            </p>
          </div>
        </div>

        {message.text && (
          <div className={`alert alert-${message.type}`} role="alert">
            {message.text}
          </div>
        )}

        <div className="card border-0 shadow-sm rounded-4 overflow-hidden">
          <div className="table-responsive">
            <table className="table table-hover align-middle mb-0">
              <thead className="table-light">
                <tr>
                  <th className="px-3">STT</th>
                  <th>Mã mẫu</th>
                  <th>Người bệnh</th>
                  <th>Xét nghiệm</th>
                  <th>KTV thực hiện</th>
                  <th>Thời gian gửi</th>
                  <th className="text-end px-3">Thao tác</th>
                </tr>
              </thead>
              <tbody>
                {loading ? (
                  <tr>
                    <td colSpan="7" className="text-center py-4 text-muted">
                      Đang tải kết quả...
                    </td>
                  </tr>
                ) : results.length === 0 ? (
                  <tr>
                    <td colSpan="7" className="text-center py-4 text-muted">
                      Không có kết quả nào đang chờ đọc.
                    </td>
                  </tr>
                ) : (
                  results.map((item, index) => (
                    <tr key={item.id || index}>
                      <td className="px-3 fw-bold">{index + 1}</td>
                      <td className="fw-semibold text-primary">{item.specimenCode || '—'}</td>
                      <td>{item.patientName || '—'}</td>
                      <td>{item.testName || '—'}</td>
                      <td>{item.technicianName || '—'}</td>
                      <td>{item.submittedAt || '—'}</td>
                      <td className="text-end px-3">
                        <button
                          type="button"
                          className="btn btn-outline-primary btn-sm"
                          onClick={() => navigate(`/doctor/doc-ket-qua/${item.id}`)}
                        >
                          Đọc kết quả
                        </button>
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>
        </div>
      </div>
    );
  }

  // Trang chi tiết.
  return (
    <div className="container py-4" style={{ maxWidth: '900px' }}>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h2 className="h4 fw-bold mb-1">Đọc và trả kết quả xét nghiệm</h2>
          <p className="text-secondary mb-0">
            Bác sĩ đọc chỉ số và đưa ra kết luận, lời khuyên.
          </p>
        </div>
        <button
          type="button"
          className="btn btn-outline-secondary btn-sm"
          onClick={() => navigate('/doctor/doc-ket-qua')}
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
      ) : !resultData ? (
        <div className="alert alert-warning">Không tìm thấy dữ liệu kết quả.</div>
      ) : (
        <>
          <div className="card border-0 shadow-sm rounded-4 p-4 mb-4">
            <h5 className="fw-bold mb-3 text-primary">
              Thông tin xét nghiệm: {resultData.testName || '—'}
            </h5>

            <div className="row mb-4">
              <div className="col-md-6">
                <p className="mb-1"><strong>Người bệnh:</strong> {resultData.patientName || '—'}</p>
                <p className="mb-1"><strong>Mã mẫu:</strong> {resultData.specimenCode || '—'}</p>
              </div>
              <div className="col-md-6">
                <p className="mb-1">
                  <strong>Ghi chú từ KTV:</strong>{' '}
                  {resultData.technicianNotes || 'Không có ghi chú'}
                </p>
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
                    <th>Đánh giá</th>
                  </tr>
                </thead>
                <tbody>
                  {resultData.indicators?.length > 0 ? (
                    resultData.indicators.map((indicator, index) => (
                      <tr key={index}>
                        <td className="fw-semibold">{indicator.name}</td>
                        <td className={indicator.abnormal ? 'text-danger fw-bold' : ''}>
                          {indicator.value || '---'}
                        </td>
                        <td>{indicator.unit || '—'}</td>
                        <td>
                          {indicator.abnormal ? (
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

            <div className="text-end">
              <button type="submit" className="btn btn-primary" disabled={submitting}>
                {submitting ? 'Đang lưu...' : 'Lưu kết luận & duyệt'}
              </button>
            </div>
          </form>
        </>
      )}
    </div>
  );
}
