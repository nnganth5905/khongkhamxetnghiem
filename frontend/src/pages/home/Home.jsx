// src/pages/home/Home.jsx
import React from 'react';
import { Link } from 'react-router-dom';

export default function Home() {
  return (
    <div className="min-vh-100 bg-light">
      {/* Hero Section */}
      <div className="bg-primary text-white py-5 shadow-sm" style={{ backgroundColor: '#0b63e5' }}>
        <div className="container py-4 text-center">
          <h1 className="display-5 fw-bold mb-3">Trung Tâm Y Tế Bio Medic</h1>
          <p className="lead mb-4 col-md-8 mx-auto opacity-90">
            Dịch vụ khám chữa bệnh và xét nghiệm y khoa kỹ thuật số chất lượng cao, nhanh chóng và chính xác.
          </p>
          <div className="d-flex justify-content-center gap-3">
            <Link to="/dat-lich-kham" className="btn btn-light btn-lg px-4 fw-semibold text-primary">
              Đặt lịch khám
            </Link>
            <Link to="/dat-lich-xet-nghiem" className="btn btn-outline-light btn-lg px-4 fw-semibold">
              Đặt xét nghiệm
            </Link>
          </div>
        </div>
      </div>

      {/* Quick Action Grid */}
      <div className="container py-5">
        <div className="row g-4">
          <div className="col-md-4">
            <div className="card h-100 border-0 shadow-sm rounded-4 p-4 text-center">
              <div className="fs-1 mb-3">🩺</div>
              <h5 className="fw-bold mb-2">Khám Chuyên Khoa</h5>
              <p className="text-muted small mb-4">
                Đội ngũ bác sĩ giàu kinh nghiệm thuộc nhiều chuyên khoa hỗ trợ thăm khám tận tâm.
              </p>
              <Link to="/dat-lich-kham" className="btn btn-outline-primary mt-auto rounded-3">
                Đăng ký khám
              </Link>
            </div>
          </div>

          <div className="col-md-4">
            <div className="card h-100 border-0 shadow-sm rounded-4 p-4 text-center">
              <div className="fs-1 mb-3">🧪</div>
              <h5 className="fw-bold mb-2">Xét Nghiệm Chuẩn Xác</h5>
              <p className="text-muted small mb-4">
                Hệ thống phòng lab đạt chuẩn, tiếp nhận và phân tích bệnh phẩm tự động.
              </p>
              <Link to="/dat-lich-xet-nghiem" className="btn btn-outline-primary mt-auto rounded-3">
                Đặt lịch xét nghiệm
              </Link>
            </div>
          </div>

          <div className="col-md-4">
            <div className="card h-100 border-0 shadow-sm rounded-4 p-4 text-center">
              <div className="fs-1 mb-3">📄</div>
              <h5 className="fw-bold mb-2">Tra Cứu Kết Quả</h5>
              <p className="text-muted small mb-4">
                Tra cứu kết quả xét nghiệm trực tuyến nhanh chóng thông qua mã bệnh phẩm hoặc tài khoản.
              </p>
              <Link to="/tra-cuu" className="btn btn-outline-primary mt-auto rounded-3">
                Tra cứu ngay
              </Link>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}