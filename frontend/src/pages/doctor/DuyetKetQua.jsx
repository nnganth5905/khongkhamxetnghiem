// src/pages/doctor/DuyetKetQua.jsx
import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { getApiErrorMessage } from '../../services/api';
import api from '../../services/api'; 

export default function DuyetKetQua() {
  const navigate = useNavigate();
  const [results, setResults] = useState([]);
  const [loading, setLoading] = useState(true);
  const [message, setMessage] = useState({ type: '', text: '' });

  const fetchPendingResults = async () => {
    try {
      setLoading(true);
      const response = await api.get('/doctor/results/pending');
      const data = response.data ?? response; 
      
      setResults(Array.isArray(data) ? data : data?.content || data?.items || []);
    } catch (err) {
      setMessage({
        type: 'danger',
        text: getApiErrorMessage(err, 'Không thể tải danh sách kết quả cần duyệt.'),
      });
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchPendingResults();
  }, []);

  return (
    <div className="container py-4">
      <div className="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h2 className="h4 fw-bold mb-1">Duyệt kết quả xét nghiệm</h2>
          <p className="text-secondary mb-0">
            Xem xét và ký duyệt các phiếu xét nghiệm trước khi trả cho bệnh nhân
          </p>
        </div>
        <button
          type="button"
          className="btn btn-outline-primary btn-sm"
          onClick={fetchPendingResults}
          disabled={loading}
        >
          <i className="fa-solid fa-rotate me-2"></i>
          Làm mới
        </button>
      </div>

      {message.text && (
        <div className={`alert alert-${message.type} mb-3`} role="alert">
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
                <th>Tên xét nghiệm</th>
                <th>KTV thực hiện</th>
                <th>Thời gian gửi</th>
                <th className="text-end px-3">Thao tác</th>
              </tr>
            </thead>
            <tbody>
              {loading ? (
                <tr>
                  <td colSpan="7" className="text-center py-4 text-muted">
                    Đang tải danh sách kết quả chờ duyệt...
                  </td>
                </tr>
              ) : results.length === 0 ? (
                <tr>
                  <td colSpan="7" className="text-center py-4 text-muted">
                    Không có kết quả nào đang chờ duyệt.
                  </td>
                </tr>
              ) : (
                results.map((item, index) => {
                  return (
                    <tr key={item.id || index}>
                      <td className="px-3 fw-bold">{index + 1}</td>
                      <td className="fw-semibold text-primary">{item.specimenCode}</td>
                      <td className="fw-semibold">{item.patientName}</td>
                      <td>{item.testName}</td>
                      <td>{item.technicianName || 'KTV'}</td>
                      <td className="text-muted small">{item.submittedAt}</td>
                      <td className="text-end px-3">
                        <button
                          type="button"
                          className="btn btn-outline-info btn-sm"
                          onClick={() => navigate(`/doctor/doc-ket-qua/${item.id}`)}
                        >
                          Đọc & Duyệt
                        </button>
                      </td>
                    </tr>
                  );
                })
              )}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}