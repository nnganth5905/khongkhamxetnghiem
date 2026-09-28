// src/pages/doctor/LayMau.jsx
import React, { useEffect, useState } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { getApiErrorMessage } from '../../services/api';
import {
  collectDoctorOrderedSpecimen,
  getDoctorTestOrder,
} from '../../services/testService';

export default function LayMau() {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();

  const orderId = searchParams.get('orderId');
  const luotKhamId = searchParams.get('luotKhamId');

  const [formData, setFormData] = useState({
    patientCode: '',
    patientName: '',
    sampleType: 'Máu toàn phần',
    barcode: '',
    notes: '',
  });

  const [tests, setTests] = useState([]);
  const [loading, setLoading] = useState(false);
  const [fetching, setFetching] = useState(false);
  const [isCollected, setIsCollected] = useState(false);
  const [specimenCode, setSpecimenCode] = useState('');
  const [message, setMessage] = useState({ type: '', text: '' });

  useEffect(() => {
    if (!orderId) {
      setMessage({
        type: 'warning',
        text: 'Không tìm thấy mã phiếu xét nghiệm. Hãy vào Lấy mẫu từ bước Chỉ định xét nghiệm.',
      });
      return;
    }

    const loadOrder = async () => {
      try {
        setFetching(true);
        const data = await getDoctorTestOrder(orderId);

        setTests(Array.isArray(data?.tests) ? data.tests : []);

        setFormData((prev) => ({
          ...prev,
          patientCode: data?.patientCode || '',
          patientName: data?.patientName || '',
          sampleType:
            data?.defaultSampleType ||
            data?.tests?.[0]?.sampleType ||
            'Máu toàn phần',
          notes:
            data?.notes ||
            (Array.isArray(data?.tests) && data.tests.length > 0
              ? `Chỉ định xét nghiệm: ${data.tests.map((x) => x.name).join(', ')}`
              : ''),
        }));
      } catch (err) {
        setMessage({
          type: 'danger',
          text: getApiErrorMessage(err, 'Không thể tải thông tin phiếu xét nghiệm.'),
        });
      } finally {
        setFetching(false);
      }
    };

    loadOrder();
  }, [orderId]);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!orderId) {
      setMessage({
        type: 'danger',
        text: 'Thiếu mã phiếu xét nghiệm.',
      });
      return;
    }

    setLoading(true);
    setMessage({ type: '', text: '' });

    try {
      let finalBarcode = formData.barcode.trim();

      if (!finalBarcode) {
        finalBarcode = `${Date.now()}${Math.floor(Math.random() * 90 + 10)}`;
      }

      const result = await collectDoctorOrderedSpecimen(orderId, {
        barcode: finalBarcode,
        sampleType: formData.sampleType,
        notes: formData.notes,
      });

      const code =
        result?.specimenId ||
        result?.id ||
        result?.code ||
        `M${finalBarcode}`;

      setSpecimenCode(code);

      setFormData((prev) => ({
        ...prev,
        barcode: finalBarcode,
      }));

      setIsCollected(true);

      setMessage({
        type: 'success',
        text: 'Đã lưu mẫu vào CSDL. Bây giờ có thể bàn giao mẫu cho kỹ thuật viên.',
      });
    } catch (err) {
      setMessage({
        type: 'danger',
        text: getApiErrorMessage(err, 'Lỗi khi lưu mẫu vào cơ sở dữ liệu.'),
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
          <p className="text-secondary mb-0">
            Ghi nhận thông tin mẫu và gán barcode trước khi chuyển lab
          </p>
        </div>

        <button
          type="button"
          className="btn btn-outline-secondary btn-sm"
          onClick={() =>
            luotKhamId
              ? navigate(`/doctor/kham-benh?luotKhamId=${luotKhamId}`)
              : navigate('/doctor')
          }
        >
          Quay lại
        </button>
      </div>

      {message.text && (
        <div className={`alert alert-${message.type} mb-3`} role="alert">
          {message.text}
        </div>
      )}

      {fetching ? (
        <div className="text-center py-5 text-muted">
          <div className="spinner-border spinner-border-sm me-2"></div>
          Đang tải...
        </div>
      ) : (
        <form
          className="card border-0 shadow-sm rounded-4 p-4"
          onSubmit={handleSubmit}
        >
          {orderId && (
            <div className="alert alert-light border small">
              Mã phiếu xét nghiệm: <strong>{orderId}</strong>
            </div>
          )}

          <div className="row g-3">
            <div className="col-md-6">
              <label className="form-label fw-semibold">Mã bệnh nhân *</label>
              <input
                type="text"
                name="patientCode"
                className="form-control bg-light"
                value={formData.patientCode}
                readOnly
                required
              />
            </div>

            <div className="col-md-6">
              <label className="form-label fw-semibold">Họ tên bệnh nhân *</label>
              <input
                type="text"
                name="patientName"
                className="form-control bg-light"
                value={formData.patientName}
                readOnly
                required
              />
            </div>

            {tests.length > 0 && (
              <div className="col-12">
                <label className="form-label fw-semibold">Xét nghiệm đã chỉ định</label>
                <div className="border rounded p-3 bg-light">
                  {tests.map((test) => (
                    <div key={test.id}>
                      {test.name}
                    </div>
                  ))}
                </div>
              </div>
            )}

            <div className="col-md-6">
              <label className="form-label fw-semibold">Loại bệnh phẩm *</label>
              <select
                name="sampleType"
                className="form-select"
                value={formData.sampleType}
                onChange={handleChange}
                disabled={isCollected}
              >
                <option value="Máu toàn phần">Máu toàn phần (EDTA)</option>
                <option value="Huyết thanh">Huyết thanh (Serum)</option>
                <option value="Nước tiểu">Nước tiểu</option>
                <option value="Khác">Khác</option>
              </select>
            </div>

            <div className="col-md-6">
              <label className="form-label fw-semibold">Mã vạch (Barcode)</label>
              <input
                type="text"
                name="barcode"
                className="form-control"
                placeholder="Để trống hệ thống sẽ tự sinh"
                value={formData.barcode}
                onChange={handleChange}
                disabled={isCollected}
              />
            </div>

            <div className="col-12">
              <label className="form-label fw-semibold">Ghi chú lâm sàng</label>
              <textarea
                name="notes"
                rows="3"
                className="form-control"
                value={formData.notes}
                onChange={handleChange}
                disabled={isCollected}
              />
            </div>
          </div>

          <div className="mt-4 d-flex gap-3">
            {!isCollected ? (
              <button
                type="submit"
                className="btn btn-primary px-4"
                disabled={loading || fetching || !orderId}
              >
                {loading ? 'Đang lưu CSDL...' : 'Xác nhận lấy mẫu'}
              </button>
            ) : (
              <button
                type="button"
                className="btn btn-warning px-4 shadow-sm"
                onClick={() =>
                  navigate(
                    `/doctor/ban-giao-mau?specimenCode=${encodeURIComponent(
                      specimenCode || formData.barcode
                    )}&orderId=${encodeURIComponent(orderId)}&patientCode=${encodeURIComponent(
                      formData.patientCode
                    )}`
                  )
                }
              >
                <i className="fa-solid fa-truck-fast me-2"></i>
                Bàn giao mẫu
              </button>
            )}
          </div>
        </form>
      )}
    </div>
  );
}
