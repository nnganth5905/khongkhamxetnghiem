// src/pages/doctor/ChiDinhXetNghiem.jsx
import React, { useEffect, useState } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { getApiErrorMessage } from '../../services/api';
import { createDoctorTestOrder, getTests } from '../../services/testService';

export default function ChiDinhXetNghiem() {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();

  const luotKhamId = searchParams.get('luotKhamId');
  const idKham = searchParams.get('idKham');

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

    if (!luotKhamId) {
      setMessage({
        type: 'danger',
        text: 'Không tìm thấy mã lượt khám. Hãy mở Chỉ định xét nghiệm từ ca khám đang xử lý.',
      });
      return;
    }

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
      const result = await createDoctorTestOrder({
        visitId: luotKhamId,
        examinationId: idKham ? Number(idKham) : null,
        diagnosis: diagnosis.trim(),
        notes: notes.trim(),
        testIds: selectedTests,
      });

      const orderId =
        result?.orderId ||
        result?.idPhieuXetNghiem ||
        result?.id ||
        result?.IDPhieuXetNghiem;

      setMessage({
        type: 'success',
        text: orderId
          ? `Chỉ định xét nghiệm thành công. Mã phiếu: ${orderId}`
          : 'Chỉ định xét nghiệm thành công.',
      });

      if (orderId) {
        setTimeout(() => {
          navigate(
            `/doctor/lay-mau?orderId=${encodeURIComponent(orderId)}&luotKhamId=${encodeURIComponent(luotKhamId)}`
          );
        }, 900);
      }
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
          onClick={() => {
            if (luotKhamId) {
              navigate(`/doctor/kham-benh?luotKhamId=${encodeURIComponent(luotKhamId)}`);
            } else {
              navigate('/doctor/danh-sach-cho');
            }
          }}
        >
          Quay lại
        </button>
      </div>

      {!luotKhamId && (
        <div className="alert alert-warning d-flex flex-wrap justify-content-between align-items-center gap-2">
          <span>
            Trang này cần <strong>luotKhamId</strong>. Hãy chọn bệnh nhân từ Danh sách chờ.
          </span>
          <button
            type="button"
            className="btn btn-sm btn-warning"
            onClick={() => navigate('/doctor/danh-sach-cho')}
          >
            Mở danh sách chờ
          </button>
        </div>
      )}

      {message.text && (
        <div className={`alert alert-${message.type} mb-3`} role="alert">
          {message.text}
        </div>
      )}

      <form
        className="card border-0 shadow-sm rounded-4 p-4"
        onSubmit={handleSubmit}
      >
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
            <div className="text-secondary small">
              Đang tải danh mục xét nghiệm...
            </div>
          ) : (
            <div
              className="border rounded p-3 overflow-auto"
              style={{ maxHeight: '240px' }}
            >
              {testList.length === 0 ? (
                <div className="text-muted">Không có xét nghiệm khả dụng.</div>
              ) : (
                testList.map((test) => {
                  const id = test.id || test.IDXetNghiem;
                  const name = test.name || test.TenXetNghiem;
                  const price = test.price ?? test.Gia;

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

                        {price !== undefined && price !== null && (
                          <span className="text-muted ms-2">
                            {Number(price).toLocaleString('vi-VN')} đ
                          </span>
                        )}
                      </label>
                    </div>
                  );
                })
              )}
            </div>
          )}
        </div>

        <div className="mb-3">
          <label className="form-label fw-semibold">
            Ghi chú lâm sàng / dặn dò
          </label>

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
            disabled={loading || fetching || !luotKhamId}
          >
            {loading ? 'Đang gửi chỉ định...' : 'Xác nhận chỉ định'}
          </button>
        </div>
      </form>
    </div>
  );
}
