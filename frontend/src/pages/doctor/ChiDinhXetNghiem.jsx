// src/pages/doctor/ChiDinhXetNghiem.jsx
import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { getApiErrorMessage } from '../../services/api';
import { getTests } from '../../services/testService';

export default function ChiDinhXetNghiem() {
  const navigate = useNavigate();
  const { appointmentId } = useParams();

  const [testList, setTestList] = useState([]);
  const [selectedTests, setSelectedTests] = useState([]);
  const [diagnosis, setDiagnosis] = useState('');
  const [notes, setNotes] = useState('');
  const [loading, setLoading] = useState(false);
  const [fetching, setFetching] = useState(true);
  const [message, setMessage] = useState({ type: '', text: '' });

  useEffect(() => {
    let active = true;
    const fetchAvailableTests = async () => {
      try {
        setFetching(true);
        const data = await getTests();
        if (active) {
          setTestList(Array.isArray(data) ? data : data?.tests || []);
        }
      } catch (err) {
        if (active) {
          setMessage({
            type: 'danger',
            text: getApiErrorMessage(err, 'Không thể tải danh sách xét nghiệm.'),
          });
        }
      } finally {
        if (active) setFetching(false);
      }
    };

    fetchAvailableTests();
    return () => {
      active = false;
    };
  }, []);

  const handleToggleTest = (testId) => {
    setSelectedTests((prev) =>
      prev.includes(testId)
        ? prev.filter((id) => id !== testId)
        : [...prev, testId]
    );
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (selectedTests.length === 0) {
      setMessage({
        type: 'warning',
        text: 'Vui lòng chọn ít nhất một xét nghiệm cần chỉ định.',
      });
      return;
    }

    setLoading(true);
    setMessage({ type: '', text: '' });

    try {
      // Logic gửi yêu cầu chỉ định xét nghiệm
      setMessage({
        type: 'success',
        text: 'Chỉ định xét nghiệm thành công!',
      });
      setTimeout(() => {
        navigate(-1);
      }, 800);
    } catch (err) {
      setMessage({
        type: 'danger',
        text: getApiErrorMessage(err, 'Không thể tạo phiếu chỉ định xét nghiệm.'),
      });
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="container py-4" style={{ maxWidth: '820px' }}>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h2 className="h4 fw-bold mb-1">Chỉ định xét nghiệm</h2>
          <p className="text-secondary mb-0">
            Tạo phiếu yêu cầu xét nghiệm cho bệnh nhân
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
          <label className="form-label fw-semibold">Chẩn đoán sơ bộ</label>
          <input
            type="text"
            className="form-control"
            placeholder="Ví dụ: Theo dõi sốt xuất huyết Dengue ngày 3..."
            value={diagnosis}
            onChange={(e) => setDiagnosis(e.target.value)}
            required
          />
        </div>

        <div className="mb-3">
          <label className="form-label fw-semibold">Chọn xét nghiệm *</label>
          {fetching ? (
            <div className="text-secondary small">Đang tải danh mục xét nghiệm...</div>
          ) : (
            <div
              className="border rounded p-3 overflow-auto"
              style={{ maxHeight: '240px' }}
            >
              {testList.map((test) => {
                const id = test.id || test.IDXetNghiem;
                const name = test.name || test.TenXetNghiem;
                const price = test.price || test.Gia;
                return (
                  <div key={id} className="form-check mb-2">
                    <input
                      type="checkbox"
                      id={`test-${id}`}
                      className="form-check-input"
                      checked={selectedTests.includes(id)}
                      onChange={() => handleToggleTest(id)}
                    />
                    <label
                      className="form-check-label d-flex justify-content-between"
                      htmlFor={`test-${id}`}
                      style={{ cursor: 'pointer' }}
                    >
                      <span>{name}</span>
                      {price && (
                        <span className="text-muted ms-2">
                          {Number(price).toLocaleString()} đ
                        </span>
                      )}
                    </label>
                  </div>
                );
              })}
            </div>
          )}
        </div>

        <div className="mb-3">
          <label className="form-label fw-semibold">Ghi chú lâm sàng / dặn dò</label>
          <textarea
            rows="3"
            className="form-control"
            placeholder="Ví dụ: Lấy máu lúc đói, cần kết quả khẩn..."
            value={notes}
            onChange={(e) => setNotes(e.target.value)}
          />
        </div>

        <div className="mt-3">
          <button
            type="submit"
            className="btn btn-primary px-4"
            disabled={loading || fetching}
          >
            {loading ? 'Đang gửi chỉ định...' : 'Xác nhận chỉ định'}
          </button>
        </div>
      </form>
    </div>
  );
}