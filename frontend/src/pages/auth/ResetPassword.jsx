import React, {
  useEffect,
  useRef,
  useState,
} from 'react';

import { Link } from 'react-router-dom';

import {
  getAuthErrorMessage,
  resetPassword,
  verifyResetToken,
} from '../../services/authService';

export default function ResetPassword() {
  const [token, setToken] =
    useState('');

  const [email, setEmail] =
    useState('');

  const [
    password,
    setPassword,
  ] = useState('');

  const [
    password2,
    setPassword2,
  ] = useState('');

  const [valid, setValid] =
    useState(false);

  const [msg, setMsg] =
    useState('');

  const [done, setDone] =
    useState(false);

  const [loading, setLoading] =
    useState(true);

  const [
    submitting,
    setSubmitting,
  ] = useState(false);

  const hasStarted =
    useRef(false);

  useEffect(() => {
    if (hasStarted.current) {
      return;
    }

    hasStarted.current = true;

    const params =
      new URLSearchParams(
        window.location.search
      );

    const tokenParam =
      params.get('token') ||
      '';

    const emailParam =
      params.get('email') ||
      '';

    setToken(
      tokenParam
    );

    setEmail(
      emailParam
    );

    if (
      !tokenParam ||
      !emailParam
    ) {
      setMsg(
        'Thiếu tham số liên kết.'
      );

      setValid(false);
      setLoading(false);

      return;
    }

    const executeVerify =
      async () => {
        try {
          const data =
            await verifyResetToken(
              tokenParam,
              emailParam
            );

          if (
            data?.valid === true
          ) {
            setValid(true);
            setMsg('');
          } else {
            setValid(false);

            setMsg(
              data?.message ||
                'Liên kết không hợp lệ hoặc đã hết hạn.'
            );
          }
        } catch (error) {
          setValid(false);

          setMsg(
            getAuthErrorMessage(
              error,
              'Liên kết không hợp lệ hoặc đã hết hạn.'
            )
          );
        } finally {
          setLoading(false);
        }
      };

    executeVerify();
  }, []);

  const handleSubmit =
    async (e) => {
      e.preventDefault();

      setMsg('');

      if (
        password.length < 6
      ) {
        setMsg(
          'Mật khẩu tối thiểu 6 ký tự.'
        );

        return;
      }

      if (
        password !== password2
      ) {
        setMsg(
          'Xác nhận mật khẩu không khớp.'
        );

        return;
      }

      try {
        setSubmitting(true);

        const data =
          await resetPassword({
            token,
            email,
            password,
          });

        setDone(true);
        setValid(false);

        setMsg(
          data?.message ||
            'Đặt lại mật khẩu thành công. Bạn có thể đăng nhập bằng mật khẩu mới.'
        );
      } catch (error) {
        setMsg(
          getAuthErrorMessage(
            error,
            'Có lỗi xảy ra, vui lòng thử lại.'
          )
        );
      } finally {
        setSubmitting(false);
      }
    };

  if (loading) {
    return (
      <div className="bg-light min-vh-100 d-flex align-items-center justify-content-center">
        <div className="text-secondary">
          Đang kiểm tra liên kết...
        </div>
      </div>
    );
  }

  const alertClass =
    done
      ? 'alert-success'
      : valid
      ? 'alert-warning'
      : 'alert-danger';

  return (
    <div className="bg-light min-vh-100 py-5">
      <div
        className="container"
        style={{
          maxWidth:
            '520px',
        }}
      >
        <h3 className="mb-4 text-center fw-bold">
          Đặt lại mật khẩu
        </h3>

        {msg && (
          <div
            className={`alert ${alertClass}`}
          >
            {msg}
          </div>
        )}

        {valid ? (
          <div className="card shadow-sm p-4 border-0 rounded-3">
            <form
              onSubmit={
                handleSubmit
              }
              autoComplete="off"
            >
              <div className="mb-3">
                <label className="form-label">
                  Mật khẩu mới
                </label>

                <input
                  type="password"
                  name="password"
                  value={
                    password
                  }
                  onChange={(e) =>
                    setPassword(
                      e.target
                        .value
                    )
                  }
                  className="form-control"
                  required
                  minLength={6}
                  placeholder="Tối thiểu 6 ký tự"
                />
              </div>

              <div className="mb-3">
                <label className="form-label">
                  Nhập lại mật khẩu
                </label>

                <input
                  type="password"
                  name="password2"
                  value={
                    password2
                  }
                  onChange={(e) =>
                    setPassword2(
                      e.target
                        .value
                    )
                  }
                  className="form-control"
                  required
                  minLength={6}
                  placeholder="Nhập lại mật khẩu mới"
                />
              </div>

              <button
                type="submit"
                className="btn btn-primary w-100 py-2 fw-semibold"
                disabled={
                  submitting
                }
                style={{
                  backgroundColor:
                    'var(--primary, #0b63e5)',

                  borderColor:
                    'var(--primary, #0b63e5)',
                }}
              >
                {submitting
                  ? 'Đang cập nhật...'
                  : 'Cập nhật mật khẩu'}
              </button>
            </form>
          </div>
        ) : (
          <div className="text-center mt-3">

            {!done && (
              <Link
                to="/forgot-password"
                className="btn btn-outline-primary me-2"
              >
                Gửi lại liên kết
              </Link>
            )}

            <Link
              to="/login"
              className="btn btn-link text-decoration-none"
            >
              Đăng nhập
            </Link>

          </div>
        )}
      </div>
    </div>
  );
}