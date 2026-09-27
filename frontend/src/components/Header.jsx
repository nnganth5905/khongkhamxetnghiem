import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

const getDisplayName = (user) =>
  user?.fullName ||
  user?.FullName ||
  user?.hoTen ||
  user?.HoTen ||
  user?.name ||
  user?.email ||
  user?.Email ||
  'Tài khoản';

const formatRoleDisplay = (rawRole) => {
  const role = String(rawRole || '').toLowerCase();
  switch (role) {
    case 'admin': return 'Quản trị viên';
    case 'bacsi':
    case 'doctor': return 'Bác sĩ';
    case 'letan':
    case 'receptionist': return 'Lễ tân';
    case 'ktv':
    case 'technician': return 'Kỹ thuật viên';
    case 'khachhang':
    case 'customer': return 'Khách hàng';
    default: return 'Khách hàng';
  }
};

export default function Header() {
  const navigate = useNavigate();
  const [search, setSearch] = useState('');

  // Sử dụng useAuth thay vì tự đọc localStorage
  const { user, isAuthenticated, role, logout } = useAuth();

  const displayName = getDisplayName(user);

  const avatar = `https://ui-avatars.com/api/?name=${encodeURIComponent(
    displayName
  )}&background=0B63E5&color=fff&size=64`;

  const resultsPath = isAuthenticated ? '/ket-qua' : '/tra-cuu-ket-qua';

  const handleSearch = (e) => {
    e.preventDefault();
    const q = search.trim();
    if (!q) return;
    navigate(`/tim-kiem?q=${encodeURIComponent(q)}`);
  };

  const handleLogout = async () => {
    // Gọi hàm logout chuẩn từ AuthContext
    await logout();
    
    // Điều hướng về trang chủ một cách mượt mà (không load lại cả trang web)
    navigate('/', { replace: true });
  };

  return (
    <header className="bmc-header">
      <div className="container py-3 d-flex align-items-center justify-content-between gap-3 flex-wrap">
        {/* BRAND */}
        <Link
          to="/"
          className="d-flex align-items-center gap-2 text-decoration-none"
        >
          <img
            src="https://nganngan5905-tech.github.io/LTWEB/Screenshot_2025-09-16_080100-removebg-preview.png"
            alt="Bio Medic"
            style={{ height: '40px' }}
          />
          <span
            className="fw-bold fs-5"
            style={{ color: 'var(--primary)' }}
          >
            Bio Medic Center
          </span>
        </Link>

        {/* SEARCH DESKTOP */}
        <form
          className="bmc-search d-none d-lg-block mx-auto"
          role="search"
          onSubmit={handleSearch}
        >
          <div className="search-box position-relative">
            <i
              className="fa-solid fa-magnifying-glass"
              onClick={handleSearch}
            />
            <input
              type="search"
              className="form-control"
              placeholder="Tìm kiếm"
              value={search}
              onChange={(e) => setSearch(e.target.value)}
            />
          </div>
        </form>

        {/* RIGHT */}
        <div className="d-flex align-items-center gap-2">
          <button
            className="btn btn-outline-primary d-lg-none me-1"
            type="button"
            data-bs-toggle="collapse"
            data-bs-target="#mainNav"
            aria-controls="mainNav"
            aria-expanded="false"
            aria-label="Mở menu"
          >
            <i className="fa-solid fa-bars" />
          </button>

          <a
            href="tel:1900565656"
            className="text-decoration-none d-none d-sm-flex align-items-center me-2"
          >
            <div className="me-2 small text-secondary text-end">
              <div>Đường dây nóng</div>
              <div className="fw-bold">1900 56 56 56</div>
            </div>
          </a>

          {isAuthenticated ? (
            <div className="dropdown">
              <button
                type="button"
                className="btn btn-light border rounded-pill d-flex align-items-center gap-2 px-2"
                data-bs-toggle="dropdown"
                aria-expanded="false"
              >
                <img
                  src={avatar}
                  alt="avatar"
                  className="avatar"
                />
                <span className="d-none d-md-inline fw-semibold">
                  {displayName}
                </span>
                <i className="fa-solid fa-chevron-down small ms-1" />
              </button>

              <ul className="dropdown-menu dropdown-menu-end shadow-sm">
                <li className="px-3 py-2 small text-secondary">
                  Đăng nhập với vai trò:{' '}
                  <strong>
                    {formatRoleDisplay(role)}
                  </strong>
                </li>

                <li>
                  <Link className="dropdown-item" to="/lich-hen">
                    <i className="fa-regular fa-calendar-check me-2" />
                    Lịch hẹn của tôi
                  </Link>
                </li>

                <li>
                  <Link className="dropdown-item" to={resultsPath}>
                    <i className="fa-solid fa-magnifying-glass me-2" />
                    Tra cứu kết quả
                  </Link>
                </li>

                <li>
                  <hr className="dropdown-divider" />
                </li>

                <li>
                  <button
                    type="button"
                    className="dropdown-item text-danger"
                    onClick={handleLogout}
                  >
                    <i className="fa-solid fa-right-from-bracket me-2" />
                    Đăng xuất
                  </button>
                </li>
              </ul>
            </div>
          ) : (
            <>
              <Link
                to="/register"
                className="btn btn-outline-primary rounded-pill px-3 fw-semibold d-none d-sm-inline"
                style={{
                  color: 'var(--primary)',
                  borderColor: 'var(--primary)',
                }}
              >
                Đăng ký
              </Link>

              <Link
                to="/login"
                className="btn btn-primary rounded-pill px-3 fw-semibold d-none d-sm-inline"
                style={{
                  backgroundColor: 'var(--primary)',
                  borderColor: 'var(--primary)',
                }}
              >
                Đăng nhập
              </Link>
            </>
          )}
        </div>
      </div>

      {/* SEARCH MOBILE */}
      <div className="container d-lg-none pb-2">
        <form
          className="bmc-search-mobile"
          role="search"
          onSubmit={handleSearch}
        >
          <div className="search-box position-relative">
            <i
              className="fa-solid fa-magnifying-glass"
              onClick={handleSearch}
            />
            <input
              type="search"
              className="form-control"
              placeholder="Tìm kiếm"
              value={search}
              onChange={(e) => setSearch(e.target.value)}
            />
          </div>
        </form>
      </div>
    </header>
  );
}