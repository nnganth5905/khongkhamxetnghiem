import React, {
  useEffect,
  useRef,
  useState,
} from 'react';

import {
  Link,
  useNavigate,
  useParams,
} from 'react-router-dom';

import {
  cancelAppointment,
  getApiErrorMessage,
} from '../../services/appointmentService';

export default function CancelAppointment({
  appointmentId = null,
}) {
  const params =
    useParams();

  const navigate =
    useNavigate();

  const hasStarted =
    useRef(false);

  const [
    status,
    setStatus,
  ] = useState(
    'loading'
  );

  const [
    message,
    setMessage,
  ] = useState('');

  const id =
    appointmentId ||
    params.id;

  useEffect(() => {
    /*
     * React StrictMode có thể chạy
     * useEffect 2 lần ở development.
     * Guard này tránh gọi API hủy 2 lần.
     */
    if (
      hasStarted.current
    ) {
      return;
    }

    hasStarted.current =
      true;

    if (
      !id ||
      Number(id) <= 0
    ) {
      setStatus(
        'error'
      );

      setMessage(
        'Thiếu ID lịch hẹn hợp lệ.'
      );

      return;
    }

    let timeoutId;

    const executeCancel =
      async () => {
        try {
          await cancelAppointment(
            id
          );

          setStatus(
            'success'
          );

          setMessage(
            'Đã hủy lịch hẹn thành công.'
          );

          timeoutId =
            setTimeout(() => {
              navigate(
                `/lich-hen/${id}`
              );
            }, 1500);
        } catch (error) {
          setStatus(
            'error'
          );

          setMessage(
            getApiErrorMessage(
              error,
              'Không thể hủy lịch (chỉ trước ≥ 3 ngày & lịch còn hiệu lực).'
            )
          );
        }
      };

    executeCancel();

    return () => {
      if (timeoutId) {
        clearTimeout(
          timeoutId
        );
      }
    };
  }, [
    id,
    navigate,
  ]);

  return (
    <div className="bg-light min-vh-100 d-flex align-items-center justify-content-center py-5">
      <div
        className="container"
        style={{
          maxWidth: '520px',
        }}
      >
        <div className="card shadow-sm border-0 rounded-3 p-4 text-center">
          {status ===
            'loading' && (
            <div>
              <div
                className="spinner-border text-primary mb-3"
                role="status"
              />

              <p className="text-secondary mb-0">
                Đang xử lý yêu
                cầu hủy lịch
                hẹn...
              </p>
            </div>
          )}

          {status ===
            'success' && (
            <div>
              <div className="text-success fs-1 mb-3">
                <i className="fa-solid fa-circle-check" />
              </div>

              <h4 className="fw-bold mb-2">
                Hủy lịch thành
                công
              </h4>

              <div className="alert alert-success">
                {message}
              </div>

              <p className="small text-muted mb-0">
                Đang chuyển
                hướng về chi
                tiết lịch hẹn...
              </p>
            </div>
          )}

          {status ===
            'error' && (
            <div>
              <div className="text-danger fs-1 mb-3">
                <i className="fa-solid fa-circle-xmark" />
              </div>

              <h4 className="fw-bold mb-2">
                Không thể hủy
                lịch
              </h4>

              <div className="alert alert-danger">
                {message}
              </div>

              <div className="d-flex justify-content-center gap-2 mt-3">
                <Link
                  to="/lich-hen"
                  className="btn btn-outline-secondary"
                >
                  Danh sách
                  lịch hẹn
                </Link>

                <Link
                  to="/"
                  className="btn btn-primary"
                  style={{
                    backgroundColor:
                      'var(--primary, #0b63e5)',

                    borderColor:
                      'var(--primary, #0b63e5)',
                  }}
                >
                  Trang chủ
                </Link>
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}