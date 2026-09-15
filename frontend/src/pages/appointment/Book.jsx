import React, { useState } from 'react';

import {
  createQuickAppointment,
  getApiErrorMessage,
} from '../../services/appointmentService';

export default function BookAppointment() {
  const [formData, setFormData] = useState({
    hoten: '',
    email: '',
    ngay: '',
    gio: '',
    idbacsi: '',
    ghichu: '',
  });

  const [loading, setLoading] = useState(false);

  const [message, setMessage] = useState({
    type: '',
    text: '',
  });

  const handleChange = (e) => {
    const { name, value } = e.target;

    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    setLoading(true);
    setMessage({
      type: '',
      text: '',
    });

    try {
      await createQuickAppointment(formData);

      setMessage({
        type: 'success',
        text:
          'Đặt lịch thành công! Vui lòng kiểm tra email xác nhận.',
      });

      setFormData({
        hoten: '',
        email: '',
        ngay: '',
        gio: '',
        idbacsi: '',
        ghichu: '',
      });
    } catch (error) {
      setMessage({
        type: 'danger',
        text: getApiErrorMessage(
          error,
          'Có lỗi xảy ra khi gửi thông tin.'
        ),
      });
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="bg-light min-vh-100 py-4">
      <div
        className="container"
        style={{ maxWidth: '800px' }}
      >
        <h1 className="h4 mb-3 fw-bold">
          Đặt lịch xét nghiệm
        </h1>

        {message.text && (
          <div
            className={`alert alert-${message.type} mb-3`}
            role="alert"
          >
            {message.text}
          </div>
        )}

        <form
          className="card p-4 shadow-sm border-0 rounded-3"
          onSubmit={handleSubmit}
        >
          <div className="row g-3">
            <div className="col-md-6">
              <label className="form-label">
                Họ tên
              </label>

              <input
                type="text"
                name="hoten"
                value={formData.hoten}
                onChange={handleChange}
                className="form-control"
                required
              />
            </div>

            <div className="col-md-6">
              <label className="form-label">
                Email
              </label>

              <input
                type="email"
                name="email"
                value={formData.email}
                onChange={handleChange}
                className="form-control"
                required
              />
            </div>

            <div className="col-md-4">
              <label className="form-label">
                Ngày
              </label>

              <input
                type="date"
                name="ngay"
                value={formData.ngay}
                onChange={handleChange}
                className="form-control"
                required
              />
            </div>

            <div className="col-md-4">
              <label className="form-label">
                Giờ
              </label>

              <input
                type="time"
                name="gio"
                value={formData.gio}
                onChange={handleChange}
                className="form-control"
                required
              />
            </div>

            <div className="col-md-4">
              <label className="form-label">
                ID Bác sĩ
              </label>

              <input
                type="text"
                name="idbacsi"
                value={formData.idbacsi}
                onChange={handleChange}
                className="form-control"
                placeholder="VD: BS001 hoặc 12"
                required
              />
            </div>

            <div className="col-12">
              <label className="form-label">
                Ghi chú
              </label>

              <textarea
                name="ghichu"
                value={formData.ghichu}
                onChange={handleChange}
                className="form-control"
                rows="3"
              />
            </div>
          </div>

          <div className="mt-4">
            <button
              type="submit"
              className="btn btn-primary px-4"
              disabled={loading}
              style={{
                backgroundColor:
                  'var(--primary, #0b63e5)',
                borderColor:
                  'var(--primary, #0b63e5)',
              }}
            >
              {loading
                ? 'Đang xử lý...'
                : 'Đặt lịch'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}