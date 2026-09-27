import React from 'react';

import { Link } from 'react-router-dom';

export default function Footer() {
  const year =
    new Date().getFullYear();

  return (
    <footer className="bmc-footer">
      <div className="container py-5">
        <div className="row g-4">
          <div className="col-md-4">
            <h5
              className="fw-bold"
              style={{
                color:
                  'var(--primary)',
              }}
            >
              Bio Medic Center
            </h5>

            <p>
              Trụ sở chính: 24 Xuân
              Tảo, Xuân Đỉnh, Bắc Từ
              Liêm, Hà Nội
              <br />

              Email:
              biomediccenter@.vn
              <br />

              Hotline:
              1900 56 56 56
            </p>

            <div className="d-flex gap-3 fs-5">
              <a
                href="#"
                aria-label="Facebook"
                style={{
                  color:
                    'var(--primary)',
                }}
              >
                <i className="fab fa-facebook" />
              </a>

              <a
                href="#"
                aria-label="Instagram"
                style={{
                  color:
                    'var(--primary)',
                }}
              >
                <i className="fab fa-instagram" />
              </a>

              <a
                href="#"
                aria-label="Twitter"
                style={{
                  color:
                    'var(--primary)',
                }}
              >
                <i className="fab fa-twitter" />
              </a>
            </div>
          </div>

          <div className="col-md-4">
            <h6
              className="fw-bold"
              style={{
                color:
                  'var(--primary)',
              }}
            >
              Về chúng tôi
            </h6>

            <ul className="list-unstyled">
              <li>
                <Link
                  to="/"
                  className="link-dark text-decoration-none"
                >
                  Trang chủ
                </Link>
              </li>

              <li>
                <Link
                  to="/dich-vu"
                  className="link-dark text-decoration-none"
                >
                  Dịch vụ xét nghiệm
                </Link>
              </li>

              <li>
                <Link
                  to="/quy-trinh"
                  className="link-dark text-decoration-none"
                >
                  Hướng dẫn khách hàng
                </Link>
              </li>

              <li>
                <Link
                  to="/medical"
                  className="link-dark text-decoration-none"
                >
                  Tin tức
                </Link>
              </li>

              <li>
                <Link
                  to="/lien-he"
                  className="link-dark text-decoration-none"
                >
                  Liên hệ
                </Link>
              </li>
            </ul>
          </div>

          <div className="col-md-4">
            <h6
              className="fw-bold"
              style={{
                color:
                  'var(--primary)',
              }}
            >
              Địa chỉ
            </h6>

            <iframe
              title="Bio Medic Center Map"
              src="https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3724.699199560068!2d105.800951!3d21.005017!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1"
              className="footer-map rounded-3"
              loading="lazy"
              referrerPolicy="no-referrer-when-downgrade"
              allowFullScreen
            />
          </div>
        </div>

        <hr className="my-4" />

        <div
          className="text-center small"
          style={{
            color:
              'var(--primary)',
          }}
        >
          <i className="fa-regular fa-copyright me-1" />

          {year} All Right Reserved
        </div>
      </div>
    </footer>
  );
}