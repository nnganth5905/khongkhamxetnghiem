// src/pages/doctor/DuyetKetQua.jsx
import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { getApiErrorMessage } from '../../services/api';
import { getTests } from '../../services/testService';

export default function DuyetKetQua() {
  const navigate = useNavigate();
  const [results, setResults] = useState([]);
  const [loading, setLoading] = useState(true);
  const [processingId, setProcessingId] = useState(null);
  const [message, setMessage] = useState({ type: '', text: '' });

  const fetchPendingResults = async () => {
    try {
      setLoading(true);
      const data = await getTests({ status: 'PENDING_APPROVAL' });
      setResults(Array.isArray(data) ? data : data?.results || data?.items || []);
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

  const handleApprove = async (id) => {
    try {
      setProcessingId(id);
      setMessage({ type: '', text: '' });
      // Logic duyệt kết quả xét nghiệm
      setMessage({
        type: 'success',
        text: `Đã duyệt và phát hành kết quả #${id} thành công!`,
      });
      fetchPendingResults();
    } catch (err) {
      setMessage({
        type: 'danger',
        text: getApiErrorMessage(err, 'Không thể duyệt kết quả xét nghiệm.'),
      });
    } finally {
      setProcessingId(null);
    }
  };

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
                <th>Mã bệnh nhân</th>
                <th>Họ tên</th>
                <th>Tên xét nghiệm</th>
                <th>Kỹ thuật viên thực hiện</th>
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
                  const id = item.id || item.IDXetNghiem || item.resultId;
                  const patientCode = item.maBN || item.patientCode || '---';
                  const patientName = item.hoten || item.patientName || 'N/A';
                  const testName = item.tenXN || item.testName || 'Xét nghiệm tổng hợp';
                  const techName = item.kyThuatVien || item.technicianName || 'KTV';
                  const createdAt = item.ngayTao || item.createdAt || '---';

                  return (
                    <tr key={id || index}>
                      <td className="px-3 fw-bold">{index + 1}</td>
                      <td>{patientCode}</td>
                      <td className="fw-semibold">{patientName}</td>
                      <td>{testName}</td>
                      <td>{techName}</td>
                      <td className="text-muted small">{createdAt}</td>
                      <td className="text-end px-3">
                        <button
                          type="button"
                          className="btn btn-outline-info btn-sm me-2"
                          onClick={() => navigate(`/doctor/doc-ket-qua/${id}`)}
                        >
                          Xem chi tiết
                        </button>
                        <button
                          type="button"
                          className="btn btn-success btn-sm"
                          disabled={processingId === id}
                          onClick={() => handleApprove(id)}
                        >
                          {processingId === id ? 'Đang duyệt...' : 'Duyệt'}
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