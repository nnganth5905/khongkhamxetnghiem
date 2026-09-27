import React, {
  useState,
} from 'react';

import { Link } from 'react-router-dom';

import {
  forgotPassword,
} from '../../services/authService';

export default function ForgotPassword() {
  const [email, setEmail] =
    useState('');

  const [sent, setSent] =
    useState(false);

  const [devLink, setDevLink] =
    useState('');

  const [loading, setLoading] =
    useState(false);

  const [error, setError] =
    useState('');

  const handleSubmit =
    async (e) => {
      e.preventDefault();

      setError('');

      const trimmedEmail =
        email
          .trim()
          .toLowerCase();

      if (!trimmedEmail) {
        setError(
          'Vui lòng nhập email.'
        );

        return;
      }

      try {
        setLoading(true);

        const data =
          await forgotPassword(
            trimmedEmail
          );

        /*
         * Chỉ dùng khi backend development
         * chủ động trả devLink.
         */
        if (data?.devLink) {
          setDevLink(
            data.devLink
          );
        }

        /*
         * Luôn dùng thông báo chung để tránh
         * tiết lộ email có tồn tại hay không.
         */
        setSent(true);
      } catch {
        /*
         * Giữ nguyên logic bảo mật cũ:
         * không tiết lộ tài khoản có tồn tại.
         */
        setSent(true);
      } finally {
        setLoading(false);
      }
    };

  return (
    <div className="bg-light min-vh-100 d-flex align-items-center py-5">
      <div
        className="container"
        style={{
          maxWidth: '520px',
        }}
      >
        <h3 className="mb-4 text-center fw-bold">
          Quên mật khẩu
        </h3>

        {sent ? (
          <div>
            <div className="alert alert-success">
              Nếu email tồn tại trong hệ thống,
              chúng tôi đã gửi liên kết đặt lại
              mật khẩu. Vui lòng kiểm tra hộp thư
              của bạn.
            </div>

            {devLink && (
              <div className="alert alert-info small">
                <strong>
                  DEV:
                </strong>{' '}
                Mail chưa gửi được trên máy của bạn.
                Dùng link này để test:

                <br />

                <a
                  href={devLink}
                  className="text-break"
                >
                  {devLink}
                </a>
              </div>
            )}

            <div className="text-center mt-3">
              <Link
                to="/login"
                className="text-decoration-none"
              >
                Quay lại đăng nhập
              </Link>
            </div>
          </div>
        ) : (
          <div className="card shadow-sm p-4 border-0 rounded-3">

            {error && (
              <div className="alert alert-danger mb-3">
                {error}
              </div>
            )}

            <form
              onSubmit={handleSubmit}
              autoComplete="off"
            >
              <div className="mb-3">
                <label className="form-label">
                  Email đăng ký
                </label>

                <input
                  type="email"
                  name="email"
                  value={email}
                  onChange={(e) =>
                    setEmail(
                      e.target.value
                    )
                  }
                  className="form-control"
                  required
                  placeholder="you@example.com"
                />
              </div>

              <button
                type="submit"
                className="btn btn-primary w-100 py-2 fw-semibold"
                disabled={loading}
                style={{
                  backgroundColor:
                    'var(--primary, #0b63e5)',

                  borderColor:
                    'var(--primary, #0b63e5)',
                }}
              >
                {loading
                  ? 'Đang gửi...'
                  : 'Gửi liên kết đặt lại'}
              </button>
            </form>

            <div className="text-center mt-3">
              <Link
                to="/login"
                className="text-decoration-none text-secondary small"
              >
                Quay lại đăng nhập
              </Link>
            </div>

          </div>
        )}
      </div>
    </div>
  );
}