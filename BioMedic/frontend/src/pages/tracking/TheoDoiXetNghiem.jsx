import React, { useState } from 'react';

import Loading from '../../components/Loading';
import Notification from '../../components/Notification';
import StatusTimeline from '../../components/StatusTimeline';

import { getTestTracking } from '../../services/trackingService';
import { getApiErrorMessage } from '../../services/api';

const DEFAULT_STEPS = [
  { title: 'Đã tạo phiếu xét nghiệm' },
  { title: 'Đã lấy mẫu' },
  { title: 'Đã bàn giao mẫu' },
  { title: 'KTV đã tiếp nhận mẫu' },
  { title: 'Đang thực hiện xét nghiệm' },
  { title: 'Đã nhập kết quả' },
  { title: 'Bác sĩ đã duyệt kết quả' },
  { title: 'Đã có kết luận' },
  { title: 'Hoàn tất' },
];

const normalizeTracking = (data = {}, code = '') => ({
  code:
    data.code ??
    data.maPhieu ??
    data.MaPhieu ??
    code,

  patientName:
    data.patientName ??
    data.tenKhachHang ??
    data.TenKhachHang ??
    '—',

  specimenCode:
    data.specimenCode ??
    data.maMau ??
    data.MaMau ??
    '—',

  testName:
    data.testName ??
    data.tenXetNghiem ??
    data.TenXetNghiem ??
    '—',

  status:
    data.status ??
    data.trangThai ??
    data.TrangThai ??
    '—',

  doctorName:
    data.doctorName ??
    data.tenBacSi ??
    data.TenBacSi ??
    '—',

  currentStep: Number(
    data.currentStep ??
    data.stepIndex ??
    data.buocHienTai ??
    0
  ),

  steps:
    Array.isArray(data.steps) && data.steps.length > 0
      ? data.steps
      : Array.isArray(data.timeline) && data.timeline.length > 0
      ? data.timeline
      : DEFAULT_STEPS,
});

export default function TheoDoiXetNghiem() {
  const [code, setCode] = useState('');
  const [tracking, setTracking] = useState(null);

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const handleSearch = async (e) => {
    e.preventDefault();

    const value = code.trim();

    if (!value) {
      setError(
        'Vui lòng nhập mã phiếu xét nghiệm hoặc mã mẫu.'
      );
      return;
    }

    try {
      setLoading(true);
      setError('');
      setTracking(null);

      const data = await getTestTracking(value);

      setTracking(
        normalizeTracking(data, value)
      );
    } catch (err) {
      setError(
        getApiErrorMessage(
          err,
          'Không tìm thấy lượt xét nghiệm hoặc không thể tải tiến trình.'
        )
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <div>
      <div className="mb-4">
        <h1 className="dashboard-page-title mb-1">
          Theo dõi xét nghiệm
        </h1>

        <p className="text-secondary mb-0">
          Theo dõi chuỗi lấy mẫu, bàn giao, thực hiện, nhập và duyệt kết quả.
        </p>
      </div>

      {error && (
        <Notification
          type="danger"
          message={error}
          onClose={() => setError('')}
        />
      )}

      <div className="card border-0 shadow-sm rounded-4 mb-4">
        <div className="card-body p-4">
          <form
            className="row g-3 align-items-end"
            onSubmit={handleSearch}
          >
            <div className="col-lg-9">
              <label className="form-label fw-semibold">
                Mã phiếu xét nghiệm / mã mẫu
              </label>

              <div className="input-group">
                <span className="input-group-text bg-white">
                  <i className="fa-solid fa-vial-circle-check text-secondary" />
                </span>

                <input
                  type="search"
                  className="form-control"
                  placeholder="Ví dụ: PXN000123 hoặc MBP000456"
                  value={code}
                  onChange={(e) => setCode(e.target.value)}
                />
              </div>
            </div>

            <div className="col-lg-3">
              <button
                type="submit"
                className="btn btn-primary w-100"
                disabled={loading}
              >
                <i className="fa-solid fa-route me-2" />
                Theo dõi
              </button>
            </div>
          </form>
        </div>
      </div>

      {loading && (
        <Loading text="Đang tải tiến trình xét nghiệm..." />
      )}

      {!loading && tracking && (
        <div className="row g-4">
          <div className="col-lg-4">
            <div className="card border-0 shadow-sm rounded-4 h-100">
              <div className="card-body p-4">
                <div
                  className="d-inline-flex align-items-center justify-content-center rounded-circle mb-3"
                  style={{
                    width: 52,
                    height: 52,
                    background: '#eaf2ff',
                    color: 'var(--primary)',
                  }}
                >
                  <i className="fa-solid fa-flask-vial" />
                </div>

                <h5 className="fw-bold mb-4">
                  Thông tin xét nghiệm
                </h5>

                <div className="mb-3">
                  <div className="small text-secondary">
                    Mã phiếu
                  </div>

                  <div className="fw-semibold">
                    {tracking.code}
                  </div>
                </div>

                <div className="mb-3">
                  <div className="small text-secondary">
                    Người bệnh
                  </div>

                  <div className="fw-semibold">
                    {tracking.patientName}
                  </div>
                </div>

                <div className="mb-3">
                  <div className="small text-secondary">
                    Xét nghiệm
                  </div>

                  <div className="fw-semibold">
                    {tracking.testName}
                  </div>
                </div>

                <div className="mb-3">
                  <div className="small text-secondary">
                    Mã mẫu
                  </div>

                  <div className="fw-semibold">
                    {tracking.specimenCode}
                  </div>
                </div>

                <div className="mb-3">
                  <div className="small text-secondary">
                    Bác sĩ
                  </div>

                  <div className="fw-semibold">
                    {tracking.doctorName}
                  </div>
                </div>

                <div>
                  <div className="small text-secondary">
                    Trạng thái hiện tại
                  </div>

                  <span className="badge bg-primary mt-1">
                    {tracking.status}
                  </span>
                </div>
              </div>
            </div>
          </div>

          <div className="col-lg-8">
            <div className="card border-0 shadow-sm rounded-4">
              <div className="card-body p-4">
                <h5 className="fw-bold mb-4">
                  Tiến trình xét nghiệm
                </h5>

                <StatusTimeline
                  steps={tracking.steps}
                  currentStep={tracking.currentStep}
                />
              </div>
            </div>
          </div>
        </div>
      )}

      {!loading && !tracking && !error && (
        <div className="alert alert-light border text-center">
          Nhập mã phiếu xét nghiệm hoặc mã mẫu để xem tiến trình.
        </div>
      )}
    </div>
  );
}