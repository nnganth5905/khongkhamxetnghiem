// src/pages/appointment/DatLich.jsx
import React from 'react';
import { Link } from 'react-router-dom';

export default function DatLich() {
  return (
    <div className="container py-5" style={{ maxWidth: '840px' }}>
      <div className="text-center mb-5">
        <h2 className="fw-bold mb-2">ĐẶT LỊCH DỊCH VỤ</h2>
        <p className="text-muted">
          Vui lòng chọn hình thức đặt lịch phù hợp với nhu cầu của bạn
        </p>
      </div>

      <div className="row g-4">
        <div className="col-md-6">
          <div className="card h-100 border-0 shadow-sm rounded-4 p-4 text-center">
            <div className="mb-3">
              <span className="badge bg-primary-subtle text-primary p-3 rounded-circle fs-3">
                🩺
              </span>
            </div>
            <h4 className="fw-bold mb-2">Đặt Lịch Khám</h4>
            <p className="text-muted small mb-4">
              Khám tổng quát, khám chuyên khoa với đội ngũ bác sĩ chuyên môn cao.
            </p>
            <Link
              to="/dat-lich-kham"
              className="btn btn-primary mt-auto py-2 rounded-3"
              style={{
                backgroundColor: 'var(--primary, #0b63e5)',
                borderColor: 'var(--primary, #0b63e5)',
              }}
            >
              Đặt lịch khám ngay
            </Link>
          </div>
        </div>

        <div className="col-md-6">
          <div className="card h-100 border-0 shadow-sm rounded-4 p-4 text-center">
            <div className="mb-3">
              <span className="badge bg-success-subtle text-success p-3 rounded-circle fs-3">
                🧪
              </span>
            </div>
            <h4 className="fw-bold mb-2">Đặt Lịch Xét Nghiệm</h4>
            <p className="text-muted small mb-4">
              Xét nghiệm sinh hóa, huyết học, miễn dịch, di truyền nhanh chóng, chính xác.
            </p>
            <Link
              to="/dat-lich-xet-nghiem"
              className="btn btn-success mt-auto py-2 rounded-3"
            >
              Đặt lịch xét nghiệm ngay
            </Link>
          </div>
        </div>
      </div>
    </div>
  );
}