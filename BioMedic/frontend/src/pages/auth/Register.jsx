import React, {
  useMemo,
  useState,
} from 'react';

import { Link } from 'react-router-dom';

import {
  getAuthErrorMessage,
  register,
} from '../../services/authService';

export default function Register() {
  const [
    formData,
    setFormData,
  ] = useState({
    name: '',
    email: '',
    phone: '',
    gender: '',
    password: '',
    password2: '',
  });

  const [
    showPassword1,
    setShowPassword1,
  ] = useState(false);

  const [
    showPassword2,
    setShowPassword2,
  ] = useState(false);

  const [
    errors,
    setErrors,
  ] = useState([]);

  const [
    done,
    setDone,
  ] = useState(false);

  const [
    loading,
    setLoading,
  ] = useState(false);

  /*
   * Giữ nguyên cách tính độ mạnh
   * mật khẩu của code cũ.
   */
  const passwordStrength =
    useMemo(() => {
      const v =
        formData.password ||
        '';

      if (!v) {
        return 0;
      }

      let s = 0;

      if (v.length >= 6) {
        s++;
      }

      if (/[A-Z]/.test(v)) {
        s++;
      }

      if (/[a-z]/.test(v)) {
        s++;
      }

      if (
        /\d|[^A-Za-z0-9]/.test(
          v
        )
      ) {
        s++;
      }

      return s;
    }, [
      formData.password,
    ]);

  const handleInputChange =
    (e) => {
      const {
        name,
        value,
      } = e.target;

      setFormData(
        (prev) => ({
          ...prev,
          [name]: value,
        })
      );
    };

  const validateForm =
    () => {
      const newErrors = [];

      const name =
        formData.name.trim();

      const email =
        formData.email.trim();

      const pass =
        formData.password;

      const pass2 =
        formData.password2;

      if (!name) {
        newErrors.push(
          'Vui lòng nhập họ và tên'
        );
      }

      if (
        !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(
          email
        )
      ) {
        newErrors.push(
          'Email không hợp lệ'
        );
      }

      if (
        !pass ||
        pass.length < 6
      ) {
        newErrors.push(
          'Mật khẩu tối thiểu 6 ký tự'
        );
      }

      if (
        pass !== pass2
      ) {
        newErrors.push(
          'Xác nhận mật khẩu không khớp'
        );
      }

      return newErrors;
    };

  const handleSubmit =
    async (e) => {
      e.preventDefault();

      setErrors([]);
      setDone(false);

      const validationErrors =
        validateForm();

      if (
        validationErrors.length >
        0
      ) {
        setErrors(
          validationErrors
        );

        return;
      }

      try {
        setLoading(true);

        await register({
          name:
            formData.name.trim(),

          email:
            formData.email
              .trim()
              .toLowerCase(),

          phone:
            formData.phone.trim() ||
            null,

          gender:
            formData.gender ||
            null,

          password:
            formData.password,
        });

        setDone(true);

        setFormData({
          name: '',
          email: '',
          phone: '',
          gender: '',
          password: '',
          password2: '',
        });
      } catch (err) {
        setErrors([
          getAuthErrorMessage(
            err,
            'Không thể tạo tài khoản'
          ),
        ]);
      } finally {
        setLoading(false);
      }
    };

  return (
    <div className="register-page py-4">
      <div
        className="auth mx-auto"
        style={{
          maxWidth:
            '680px',

          margin:
            '6vh auto',

          background:
            '#fff',

          border:
            '1px solid #eef2ff',

          borderRadius:
            '16px',

          boxShadow:
            '0 10px 40px rgba(11,99,229,.12)',
        }}
      >
        <div
          className="hd"
          style={{
            padding:
              '18px 22px',

            borderBottom:
              '1px solid #f0f3fb',

            fontWeight:
              '700',
          }}
        >
          Đăng ký tài khoản
        </div>

        <div
          className="bd"
          style={{
            padding:
              '22px',
          }}
        >
          {done && (
            <div className="alert alert-success">
              Tạo tài khoản thành công!
              Vui lòng{' '}

              <Link
                to="/login"
                className="alert-link"
              >
                đăng nhập
              </Link>
              .
            </div>
          )}

          {errors.length > 0 && (
            <div className="alert alert-danger">
              <ul className="mb-0 ps-3">
                {errors.map(
                  (
                    error,
                    idx
                  ) => (
                    <li key={idx}>
                      {error}
                    </li>
                  )
                )}
              </ul>
            </div>
          )}

          <form
            onSubmit={
              handleSubmit
            }
            noValidate
          >
            <div className="row">
              <div className="col-sm-6 mb-3">
                <label className="form-label">
                  Họ và tên
                </label>

                <input
                  type="text"
                  name="name"
                  value={
                    formData.name
                  }
                  onChange={
                    handleInputChange
                  }
                  className="form-control"
                  placeholder="Nhập họ và tên"
                  required
                />
              </div>

              <div className="col-sm-6 mb-3">
                <label className="form-label">
                  Email
                </label>

                <input
                  type="email"
                  name="email"
                  value={
                    formData.email
                  }
                  onChange={
                    handleInputChange
                  }
                  className="form-control"
                  placeholder="Nhập email"
                  required
                />
              </div>
            </div>

            <div className="row">
              <div className="col-sm-6 mb-3">
                <label className="form-label">
                  Số điện thoại
                </label>

                <input
                  type="tel"
                  name="phone"
                  value={
                    formData.phone
                  }
                  onChange={
                    handleInputChange
                  }
                  className="form-control"
                  placeholder="Nhập số điện thoại"
                />
              </div>

              <div className="col-sm-6 mb-3">
                <label className="form-label">
                  Giới tính
                </label>

                <select
                  name="gender"
                  value={
                    formData.gender
                  }
                  onChange={
                    handleInputChange
                  }
                  className="form-select"
                >
                  <option value="">
                    -- Chọn --
                  </option>

                  <option value="Nam">
                    Nam
                  </option>

                  <option value="Nữ">
                    Nữ
                  </option>

                  <option value="Khác">
                    Khác
                  </option>
                </select>
              </div>
            </div>

            <div className="mb-2">
              <label className="form-label">
                Mật khẩu
              </label>

              <div className="input-group">
                <input
                  type={
                    showPassword1
                      ? 'text'
                      : 'password'
                  }
                  name="password"
                  value={
                    formData.password
                  }
                  onChange={
                    handleInputChange
                  }
                  className="form-control"
                  placeholder="Nhập mật khẩu"
                  required
                />

                <button
                  className="btn btn-outline-secondary"
                  type="button"
                  onClick={() =>
                    setShowPassword1(
                      (prev) =>
                        !prev
                    )
                  }
                  aria-label="Toggle password"
                >
                  <i
                    className={
                      showPassword1
                        ? 'fa-regular fa-eye-slash'
                        : 'fa-regular fa-eye'
                    }
                  />
                </button>
              </div>

              {/* Password Strength Meter */}
              <div
                className="pw-meter mt-2"
                style={{
                  height:
                    '8px',

                  borderRadius:
                    '999px',

                  background:
                    '#eef2f7',

                  overflow:
                    'hidden',
                }}
              >
                <span
                  style={{
                    display:
                      'block',

                    height:
                      '100%',

                    width:
                      `${passwordStrength * 25}%`,

                    backgroundColor:
                      passwordStrength ===
                      1
                        ? '#ff7676'
                        : passwordStrength ===
                          2
                        ? '#ffa84d'
                        : passwordStrength ===
                          3
                        ? '#ffd23f'
                        : passwordStrength ===
                          4
                        ? '#28c76f'
                        : 'transparent',

                    transition:
                      '0.25s',
                  }}
                />
              </div>

              <div className="small text-secondary mt-1">
                Tối thiểu 6 ký tự.
                Khuyến nghị gồm chữ hoa,
                thường và số.
              </div>
            </div>

            <div className="mb-3">
              <label className="form-label">
                Xác nhận mật khẩu
              </label>

              <div className="input-group">
                <input
                  type={
                    showPassword2
                      ? 'text'
                      : 'password'
                  }
                  name="password2"
                  value={
                    formData.password2
                  }
                  onChange={
                    handleInputChange
                  }
                  className="form-control"
                  placeholder="Nhập lại mật khẩu"
                  required
                />

                <button
                  className="btn btn-outline-secondary"
                  type="button"
                  onClick={() =>
                    setShowPassword2(
                      (prev) =>
                        !prev
                    )
                  }
                  aria-label="Toggle confirm password"
                >
                  <i
                    className={
                      showPassword2
                        ? 'fa-regular fa-eye-slash'
                        : 'fa-regular fa-eye'
                    }
                  />
                </button>
              </div>
            </div>

            <button
              type="submit"
              className="btn btn-primary w-100 py-2 fw-semibold"
              disabled={
                loading
              }
              style={{
                backgroundColor:
                  'var(--primary, #0b63e5)',

                borderColor:
                  'var(--primary, #0b63e5)',

                borderRadius:
                  '12px',
              }}
            >
              {loading
                ? 'Đang tạo tài khoản...'
                : 'Tạo tài khoản'}
            </button>

            <div className="text-center small mt-3">
              Đã có tài khoản?{' '}

              <Link
                to="/login"
                style={{
                  color:
                    'var(--primary, #0b63e5)',
                }}
              >
                Đăng nhập
              </Link>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}