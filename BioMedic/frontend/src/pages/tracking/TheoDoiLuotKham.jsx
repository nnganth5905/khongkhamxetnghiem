import React, { useState } from 'react';

import Loading from '../../components/Loading';
import Notification from '../../components/Notification';
import StatusTimeline from '../../components/StatusTimeline';

import { getVisitTracking } from '../../services/trackingService';
import { getApiErrorMessage } from '../../services/api';

const DEFAULT_STEPS = [
  { title: 'Đặt lịch thành công' },
  { title: 'Đã tiếp nhận' },
  { title: 'Đang chờ khám' },
  { title: 'Đang khám' },
  { title: 'Đã chỉ định xét nghiệm' },
  { title: 'Đang chờ kết quả' },
  { title: 'Bác sĩ đọc kết quả' },
  { title: 'Hoàn tất' },
];

const normalizeTracking = (data = {}, code = '') => ({
  code:
    data.code ??
    data.maLuotKham ??
    data.MaLuotKham ??
    code,

  patientName:
    data.patientName ??
    data.tenKhachHang ??
    data.TenKhachHang ??
    '—',

  doctorName:
    data.doctorName ??
    data.tenBacSi ??
    data.TenBacSi ??
    '—',

  roomName:
    data.roomName ??
    data.tenPhong ??
    data.TenPhong ??
    '—',

  status:
    data.status ??
    data.trangThai ??
    data.TrangThai ??
    '—',

  appointmentDate:
    data.appointmentDate ??
    data.ngayHen ??
    data.NgayHen ??
    '—',

  appointmentTime:
    data.appointmentTime ??
    data.gioHen ??
    data.GioHen ??
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

export default function TheoDoiLuotKham() {
  const [code, setCode] = useState('');
  const [tracking, setTracking] = useState(null);

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const handleSearch = async (e) => {
    e.preventDefault();

    const value = code.trim();

    if (!value) {
      setError('Vui lòng nhập mã lượt khám.');
      return;
    }

    try {
      setLoading(true);
      setError('');
      setTracking(null);

      const data = await getVisitTracking(value);

      setTracking(
        normalizeTracking(data, value)
      );
    } catch (err) {
      setError(
        getApiErrorMessage(
          err,
          'Không tìm thấy lượt khám hoặc không thể tải tiến trình.'
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
          Theo dõi lượt khám
        </h1>

        <p className="text-secondary mb-0">
          Theo dõi trạng thái từ lúc tiếp nhận đến khi hoàn tất lượt khám.
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
                Mã lượt khám
              </label>

              <div className="input-group">
                <span className="input-group-text bg-white">
                  <i className="fa-solid fa-magnifying-glass text-secondary" />
                </span>

                <input
                  type="search"
                  className="form-control"
                  placeholder="Ví dụ: LK000123"
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
        <Loading text="Đang tải tiến trình lượt khám..." />
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
                  <i className="fa-solid fa-stethoscope" />
                </div>

                <h5 className="fw-bold mb-4">
                  Thông tin lượt khám
                </h5>

                <div className="mb-3">
                  <div className="small text-secondary">
                    Mã lượt khám
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
                    Bác sĩ
                  </div>

                  <div className="fw-semibold">
                    {tracking.doctorName}
                  </div>
                </div>

                <div className="mb-3">
                  <div className="small text-secondary">
                    Phòng
                  </div>

                  <div className="fw-semibold">
                    {tracking.roomName}
                  </div>
                </div>

                <div className="mb-3">
                  <div className="small text-secondary">
                    Thời gian hẹn
                  </div>

                  <div className="fw-semibold">
                    {tracking.appointmentDate}
                    {tracking.appointmentTime !== '—'
                      ? ` • ${tracking.appointmentTime}`
                      : ''}
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
                  Tiến trình lượt khám
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
          Nhập mã lượt khám để xem tiến trình.
        </div>
      )}
    </div>
  );
}