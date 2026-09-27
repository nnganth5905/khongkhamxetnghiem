import React, {
  useEffect,
  useRef,
  useState,
} from 'react';

import { Link } from 'react-router-dom';

import {
  confirmEmail,
  getAuthErrorMessage,
} from '../../services/authService';

export default function ConfirmEmail() {
  const [status, setStatus] =
    useState('loading');

  const [message, setMessage] =
    useState('');

  /*
   * React StrictMode có thể gọi useEffect
   * hai lần trong môi trường development.
   */
  const hasStarted = useRef(false);

  useEffect(() => {
    if (hasStarted.current) {
      return;
    }

    hasStarted.current = true;

    const params =
      new URLSearchParams(
        window.location.search
      );

    const token =
      params.get('token') || '';

    /*
     * Giữ nguyên quy tắc token cũ:
     * chuỗi HEX 64 ký tự.
     */
    if (
      !/^[a-f0-9]{64}$/i.test(token)
    ) {
      setStatus('error');
      setMessage(
        'Token không hợp lệ'
      );

      return;
    }

    const executeConfirm =
      async () => {
        try {
          const data =
            await confirmEmail(
              token
            );

          setStatus('success');

          setMessage(
            data?.message ||
              'Xác nhận thành công! Hẹn gặp bạn tại Bio Medic Center.'
          );
        } catch (error) {
          setStatus('error');

          setMessage(
            getAuthErrorMessage(
              error,
              'Có lỗi khi xác nhận.'
            )
          );
        }
      };

    executeConfirm();
  }, []);

  return (
    <div className="confirm-email-page bg-light min-vh-100 d-flex align-items-center py-5">
      <div
        className="container"
        style={{
          maxWidth: '520px',
        }}
      >
        <div className="card shadow-sm border-0 rounded-3 p-4 text-center">

          {status === 'loading' && (
            <div>
              <div
                className="spinner-border text-primary mb-3"
                role="status"
              />

              <p className="text-secondary mb-0">
                Đang tiến hành xác nhận đặt lịch...
              </p>
            </div>
          )}

          {status === 'success' && (
            <div>
              <div className="text-success fs-1 mb-3">
                <i className="fa-solid fa-circle-check" />
              </div>

              <h4 className="fw-bold mb-3">
                Xác nhận thành công
              </h4>

              <div className="alert alert-success">
                {message}
              </div>

              <Link
                to="/"
                className="btn btn-primary mt-2"
                style={{
                  backgroundColor:
                    'var(--primary, #0b63e5)',

                  borderColor:
                    'var(--primary, #0b63e5)',
                }}
              >
                Về trang chủ
              </Link>
            </div>
          )}

          {status === 'error' && (
            <div>
              <div className="text-danger fs-1 mb-3">
                <i className="fa-solid fa-circle-xmark" />
              </div>

              <h4 className="fw-bold mb-3">
                Xác nhận thất bại
              </h4>

              <div className="alert alert-danger">
                {message}
              </div>

              <Link
                to="/"
                className="btn btn-outline-secondary mt-2"
              >
                Quay lại trang chủ
              </Link>
            </div>
          )}

        </div>
      </div>
    </div>
  );
}