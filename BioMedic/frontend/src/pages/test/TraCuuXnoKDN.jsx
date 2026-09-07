import React, { useState } from 'react';

import { Link } from 'react-router-dom';

import Loading from '../../components/Loading';
import Notification from '../../components/Notification';

import { searchResultPublic } from '../../services/testService';
import { getApiErrorMessage } from '../../services/api';

export default function TraCuuXnoKDN() {
  const [form, setForm] = useState({
    code: '',
    phone: '',
    email: '',
  });

  const [result, setResult] = useState(null);

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const setField = (
    name,
    value
  ) => {
    setForm((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!form.code.trim()) {
      setError(
        'Vui lòng nhập mã tra cứu kết quả.'
      );
      return;
    }

    if (
      !form.phone.trim() &&
      !form.email.trim()
    ) {
      setError(
        'Vui lòng nhập số điện thoại hoặc email để xác minh.'
      );
      return;
    }

    try {
      setLoading(true);
      setError('');
      setResult(null);

      const data =
        await searchResultPublic({
          code: form.code.trim(),
          phone:
            form.phone.trim() ||
            null,
          email:
            form.email.trim() ||
            null,
        });

      setResult(data);
    } catch (err) {
      setError(
        getApiErrorMessage(
          err,
          'Không tìm thấy kết quả phù hợp.'
        )
      );
    } finally {
      setLoading(false);
    }
  };

  const resultId =
    result?.id ??
    result?.idKetQua ??
    result?.IDKetQua;

  return (
    <div className="bg-light min-vh-100 py-5">
      <div
        className="container"
        style={{
          maxWidth: 760,
        }}
      >
        <div className="text-center mb-5">
          <div
            className="d-inline-flex align-items-center justify-content-center rounded-circle mb-3"
            style={{
              width: 70,
              height: 70,
              background:
                '#eaf2ff',
              color:
                'var(--primary)',
              fontSize:
                28,
            }}
          >
            <i className="fa-solid fa-file-medical" />
          </div>

          <h1
            className="fw-bold"
            style={{
              color:
                'var(--primary)',
            }}
          >
            Tra cứu kết quả xét nghiệm
          </h1>

          <p className="text-secondary">
            Nhập mã tra cứu cùng thông tin xác minh để kiểm tra kết quả.
          </p>
        </div>

        {error && (
          <Notification
            type="danger"
            message={error}
            onClose={() => setError('')}
          />
        )}

        <div className="card border-0 shadow-sm rounded-4">
          <div className="card-body p-4 p-md-5">
            <form onSubmit={handleSubmit}>
              <div className="mb-3">
                <label className="form-label fw-semibold">
                  Mã tra cứu
                  <span className="text-danger">
                    {' '}*
                  </span>
                </label>

                <input
                  type="text"
                  className="form-control form-control-lg"
                  placeholder="Ví dụ: KQ000123"
                  value={form.code}
                  onChange={(e) =>
                    setField(
                      'code',
                      e.target.value
                    )
                  }
                />
              </div>

              <div className="row g-3">
                <div className="col-md-6">
                  <label className="form-label fw-semibold">
                    Số điện thoại
                  </label>

                  <input
                    type="tel"
                    className="form-control"
                    placeholder="Số điện thoại đã đăng ký"
                    value={form.phone}
                    onChange={(e) =>
                      setField(
                        'phone',
                        e.target.value
                      )
                    }
                  />
                </div>

                <div className="col-md-6">
                  <label className="form-label fw-semibold">
                    Email
                  </label>

                  <input
                    type="email"
                    className="form-control"
                    placeholder="Email đã đăng ký"
                    value={form.email}
                    onChange={(e) =>
                      setField(
                        'email',
                        e.target.value
                      )
                    }
                  />
                </div>
              </div>

              <div className="small text-secondary mt-2">
                Chỉ cần nhập một trong hai: số điện thoại hoặc email.
              </div>

              <button
                type="submit"
                className="btn btn-primary w-100 mt-4 py-2"
                disabled={loading}
              >
                <i className="fa-solid fa-magnifying-glass me-2" />

                {loading
                  ? 'Đang tra cứu...'
                  : 'Tra cứu kết quả'}
              </button>
            </form>
          </div>
        </div>

        {loading && (
          <div className="mt-4">
            <Loading text="Đang kiểm tra kết quả..." />
          </div>
        )}

        {!loading && result && (
          <div className="card border-0 shadow-sm rounded-4 mt-4">
            <div className="card-body p-4">
              <div className="d-flex justify-content-between align-items-start gap-3">
                <div>
                  <div className="small text-secondary">
                    Người bệnh
                  </div>

                  <h5 className="fw-bold mb-3">
                    {result.patientName ??
                      result.tenKhachHang ??
                      result.TenKhachHang ??
                      '—'}
                  </h5>
                </div>

                <span className="badge bg-success">
                  {result.status ??
                    result.trangThai ??
                    result.TrangThai ??
                    'Có kết quả'}
                </span>
              </div>

              <div className="row g-3">
                <div className="col-md-6">
                  <div className="small text-secondary">
                    Mã kết quả
                  </div>

                  <div className="fw-semibold">
                    {result.code ??
                      result.maKetQua ??
                      result.MaKetQua ??
                      form.code}
                  </div>
                </div>

                <div className="col-md-6">
                  <div className="small text-secondary">
                    Xét nghiệm
                  </div>

                  <div className="fw-semibold">
                    {result.testName ??
                      result.tenXetNghiem ??
                      result.TenXetNghiem ??
                      '—'}
                  </div>
                </div>
              </div>

              {resultId && (
                <Link
                  to={`/ket-qua/${resultId}`}
                  className="btn btn-outline-primary mt-4"
                >
                  <i className="fa-solid fa-eye me-2" />
                  Xem chi tiết kết quả
                </Link>
              )}
            </div>
          </div>
        )}
      </div>
    </div>
  );
}