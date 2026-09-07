import React from 'react';

import {
  Link,
  useLocation,
} from 'react-router-dom';

const getLoggedIn =
  () => {
    return Boolean(
      localStorage.getItem(
        'accessToken'
      ) ||
        localStorage.getItem(
          'token'
        ) ||
        localStorage.getItem(
          'user'
        )
    );
  };

export default function Navbar() {
  const location =
    useLocation();

  const loggedIn =
    getLoggedIn();

  const resultsPath =
    loggedIn
      ? '/ket-qua'
      : '/tra-cuu-ket-qua';

  const active =
    (path) => {
      return (
        location.pathname ===
          path ||
        location.pathname.startsWith(
          `${path}/`
        )
      );
    };

  return (
    <div className="bmc-nav border-top bg-white">
      <div className="container">
        <div
          className="collapse d-lg-block"
          id="mainNav"
        >
          <nav className="d-flex flex-column flex-lg-row gap-2 gap-lg-4 py-2 py-lg-3 align-items-center">
            <Link
              className={`bmc-link ${
                location.pathname ===
                '/'
                  ? 'active'
                  : ''
              }`}
              to="/"
            >
              Trang chủ
            </Link>

            {/* GIỚI THIỆU */}

            <div className="dropdown">
              <button
                type="button"
                className={`bmc-link bmc-link-button dropdown-toggle ${
                  active(
                    '/gioi-thieu'
                  )
                    ? 'active'
                    : ''
                }`}
                data-bs-toggle="dropdown"
              >
                Giới thiệu
              </button>

              <ul className="dropdown-menu shadow-sm">
                <li>
                  <Link
                    className="dropdown-item"
                    to="/gioi-thieu/lich-su"
                  >
                    Lịch sử hình thành
                  </Link>
                </li>

                <li>
                  <Link
                    className="dropdown-item"
                    to="/gioi-thieu/co-so-vat-chat"
                  >
                    Cơ sở vật chất
                  </Link>
                </li>
              </ul>
            </div>

            <Link
              className={`bmc-link ${
                active(
                  '/dich-vu'
                )
                  ? 'active'
                  : ''
              }`}
              to="/dich-vu"
            >
              Dịch vụ xét nghiệm
            </Link>

            {/* TRA CỨU */}

            <div className="dropdown">
              <button
                type="button"
                className="bmc-link bmc-link-button dropdown-toggle"
                data-bs-toggle="dropdown"
              >
                Tra cứu
              </button>

              <ul className="dropdown-menu shadow-sm">
                <li>
                  <Link
                    className="dropdown-item"
                    to="/lich-hen"
                  >
                    Lịch hẹn
                  </Link>
                </li>

                <li>
                  <Link
                    className="dropdown-item"
                    to={
                      resultsPath
                    }
                  >
                    Kết quả
                  </Link>
                </li>
              </ul>
            </div>

            {/* HƯỚNG DẪN */}

            <div className="dropdown">
              <button
                type="button"
                className="bmc-link bmc-link-button dropdown-toggle"
                data-bs-toggle="dropdown"
              >
                Hướng dẫn khách hàng
              </button>

              <ul className="dropdown-menu shadow-sm">
                <li>
                  <Link
                    className="dropdown-item"
                    to="/goi-y-chuyen-khoa"
                  >
                    Tư vấn đặt lịch
                  </Link>
                </li>

                <li>
                  <Link
                    className="dropdown-item"
                    to="/dat-lich-kham"
                  >
                    Đặt lịch khám
                  </Link>
                </li>

                <li>
                  <Link
                    className="dropdown-item"
                    to="/dat-lich-xet-nghiem"
                  >
                    Đặt lịch xét nghiệm
                  </Link>
                </li>

                <li>
                  <Link
                    className="dropdown-item"
                    to="/quy-trinh"
                  >
                    Quy trình khám chữa bệnh
                  </Link>
                </li>

                <li>
                  <Link
                    className="dropdown-item"
                    to="/gio-lam-viec"
                  >
                    Giờ làm việc
                  </Link>
                </li>
              </ul>
            </div>

            {/* TIN TỨC */}

            <div className="dropdown">
              <button
                type="button"
                className="bmc-link bmc-link-button dropdown-toggle"
                data-bs-toggle="dropdown"
              >
                Tin tức
              </button>

              <ul className="dropdown-menu shadow-sm">
                <li>
                  <Link
                    className="dropdown-item"
                    to="/khuyen-mai"
                  >
                    Ưu đãi
                  </Link>
                </li>

                <li>
                  <Link
                    className="dropdown-item"
                    to="/cam-nang"
                  >
                    Cẩm nang bệnh học
                  </Link>
                </li>

                <li>
                  <Link
                    className="dropdown-item"
                    to="/medical"
                  >
                    Kiến thức y khoa
                  </Link>
                </li>

                <li>
                  <Link
                    className="dropdown-item"
                    to="/tin-tuc"
                  >
                    Hoạt động xã hội
                  </Link>
                </li>
              </ul>
            </div>

            <Link
              className={`bmc-link ${
                active(
                  '/lien-he'
                )
                  ? 'active'
                  : ''
              }`}
              to="/lien-he"
            >
              Liên hệ
            </Link>

            <Link
              className={`bmc-link ${
                active(
                  '/bac-si'
                )
                  ? 'active'
                  : ''
              }`}
              to="/bac-si"
            >
              Đội ngũ bác sĩ
            </Link>
          </nav>

          {!loggedIn && (
            <div className="d-lg-none mt-3 pt-3 border-top w-100 pb-3">
              <div className="d-flex gap-2">
                <Link
                  to="/register"
                  className="btn btn-outline-primary flex-fill"
                >
                  Đăng ký
                </Link>

                <Link
                  to="/login"
                  className="btn btn-primary flex-fill"
                >
                  Đăng nhập
                </Link>
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}