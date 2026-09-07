import React, { useState } from 'react';
import {
  Link,
  useLocation,
  useNavigate,
} from 'react-router-dom';

import {
  getRoleHome,
  useAuth,
} from '../../context/AuthContext';

import {
  getApiErrorMessage,
} from '../../services/api';

export default function Login() {
  const navigate = useNavigate();
  const location = useLocation();
  const { login } = useAuth();

  const [formData, setFormData] = useState({
    email: '',
    password: '',
  });

  const [showPassword, setShowPassword] = useState(false);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleInputChange = (e) => {
    const { name, value } = e.target;

    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');

    const emailOrUser = formData.email.trim();

    if (!emailOrUser || !formData.password) {
      setError(
        'Vui lòng nhập email/username và mật khẩu.'
      );
      return;
    }

    try {
      setLoading(true);

      const data = await login({
        email: emailOrUser,
        username: emailOrUser,
        password: formData.password,
      });

      const destination =
        location.state?.from ||
        getRoleHome(data?.user?.role);

      navigate(destination, {
        replace: true,
      });
    } catch (err) {
      setError(
        getApiErrorMessage(
          err,
          'Email/Username hoặc mật khẩu không đúng, hoặc tài khoản đã bị khóa.'
        )
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="login-page py-4">
      <div
        className="auth mx-auto"
        style={{
          maxWidth: '520px',
          margin: '7vh auto',
          background: '#fff',
          border: '1px solid #eef2ff',
          borderRadius: '16px',
          boxShadow:
            '0 10px 40px rgba(11,99,229,.12)',
        }}
      >
        <div
          className="hd"
          style={{
            padding: '18px 22px',
            borderBottom:
              '1px solid #f0f3fb',
            fontWeight: '700',
          }}
        >
          Đăng nhập
        </div>

        <div
          className="bd"
          style={{
            padding: '22px',
          }}
        >
          {error && (
            <div className="alert alert-danger">
              {error}
            </div>
          )}

          <form onSubmit={handleSubmit} noValidate>
            <div className="mb-3">
              <label className="form-label">
                Email hoặc Username
              </label>

              <input
                type="text"
                name="email"
                value={formData.email}
                onChange={handleInputChange}
                className="form-control"
                placeholder="Nhập email hoặc username"
                required
              />
            </div>

            <div className="mb-3">
              <label className="form-label">
                Mật khẩu
              </label>

              <div className="input-group">
                <input
                  type={
                    showPassword
                      ? 'text'
                      : 'password'
                  }
                  name="password"
                  value={formData.password}
                  onChange={handleInputChange}
                  className="form-control"
                  placeholder="Nhập mật khẩu"
                  required
                />

                <button
                  className="btn btn-outline-secondary"
                  type="button"
                  onClick={() =>
                    setShowPassword(
                      (prev) => !prev
                    )
                  }
                >
                  <i
                    className={
                      showPassword
                        ? 'fa-regular fa-eye-slash'
                        : 'fa-regular fa-eye'
                    }
                  />
                </button>
              </div>
            </div>

            <button
              type="submit"
              className="btn btn-primary w-100 py-2"
              disabled={loading}
              style={{
                backgroundColor:
                  'var(--primary, #0360D9)',
                border: 'none',
                borderRadius: '12px',
              }}
            >
              {loading
                ? 'Đang xử lý...'
                : 'Đăng nhập'}
            </button>

            <div className="d-flex justify-content-between mt-3 small">
              <Link
                to="/forgot-password"
                className="text-decoration-none"
              >
                Quên mật khẩu?
              </Link>

              <span>
                Chưa có tài khoản?{' '}
                <Link
                  to="/register"
                  style={{
                    color: 'var(--primary)',
                  }}
                >
                  Đăng ký
                </Link>
              </span>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}