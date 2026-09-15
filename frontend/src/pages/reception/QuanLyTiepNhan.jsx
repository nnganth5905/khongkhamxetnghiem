import React, { useState } from 'react';

import Notification from '../../components/Notification';

import { createWalkInVisit } from '../../services/appointmentService';
import { getApiErrorMessage } from '../../services/api';

const EMPTY_FORM = {
  fullName: '',
  phone: '',
  email: '',
  dateOfBirth: '',
  gender: '',
  address: '',
  serviceType: 'EXAMINATION',
  specialtyId: '',
  reason: '',
  notes: '',
};

export default function QuanLyTiepNhan() {
  const [form, setForm] = useState(EMPTY_FORM);
  const [saving, setSaving] = useState(false);

  const [message, setMessage] = useState({
    type: '',
    text: '',
  });

  const setField = (name, value) => {
    setForm((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!form.fullName.trim()) {
      setMessage({
        type: 'warning',
        text: 'Vui lòng nhập họ và tên khách hàng.',
      });

      return;
    }

    if (!form.phone.trim()) {
      setMessage({
        type: 'warning',
        text: 'Vui lòng nhập số điện thoại.',
      });

      return;
    }

    try {
      setSaving(true);

      const data =
        await createWalkInVisit(form);

      setMessage({
        type: 'success',
        text:
          data?.message ||
          `Tiếp nhận thành công${
            data?.queueNumber
              ? ` - Số thứ tự: ${data.queueNumber}`
              : ''
          }.`,
      });

      setForm(EMPTY_FORM);
    } catch (err) {
      setMessage({
        type: 'danger',
        text: getApiErrorMessage(
          err,
          'Không thể tạo lượt tiếp nhận.'
        ),
      });
    } finally {
      setSaving(false);
    }
  };

  return (
    <div>
      <h1 className="dashboard-page-title">
        Quản lý tiếp nhận
      </h1>

      <p className="text-secondary mb-4">
        Tạo lượt khám/xét nghiệm cho khách đến trực tiếp chưa có lịch hẹn.
      </p>

      {message.text && (
        <Notification
          type={message.type}
          message={message.text}
          onClose={() =>
            setMessage({
              type: '',
              text: '',
            })
          }
        />
      )}

      <form
        onSubmit={handleSubmit}
        className="card border-0 shadow-sm rounded-4"
      >
        <div className="card-body p-4">
          <div className="d-flex align-items-center gap-3 mb-4">
            <div
              className="d-flex align-items-center justify-content-center rounded-circle"
              style={{
                width: 48,
                height: 48,
                background:
                  '#eaf2ff',
                color:
                  'var(--primary)',
              }}
            >
              <i className="fa-solid fa-user-plus" />
            </div>

            <div>
              <h5 className="fw-bold mb-1">
                Thông tin người bệnh
              </h5>

              <div className="small text-secondary">
                Nhập thông tin cơ bản để tạo lượt tiếp nhận.
              </div>
            </div>
          </div>

          <div className="row g-3">
            <div className="col-lg-6">
              <label className="form-label fw-semibold">
                Họ và tên
                <span className="text-danger">
                  {' '}*
                </span>
              </label>

              <input
                type="text"
                className="form-control"
                value={form.fullName}
                onChange={(e) =>
                  setField(
                    'fullName',
                    e.target.value
                  )
                }
                required
              />
            </div>

            <div className="col-lg-3">
              <label className="form-label fw-semibold">
                Số điện thoại
                <span className="text-danger">
                  {' '}*
                </span>
              </label>

              <input
                type="tel"
                className="form-control"
                value={form.phone}
                onChange={(e) =>
                  setField(
                    'phone',
                    e.target.value
                  )
                }
                required
              />
            </div>

            <div className="col-lg-3">
              <label className="form-label fw-semibold">
                Email
              </label>

              <input
                type="email"
                className="form-control"
                value={form.email}
                onChange={(e) =>
                  setField(
                    'email',
                    e.target.value
                  )
                }
              />
            </div>

            <div className="col-lg-3">
              <label className="form-label fw-semibold">
                Ngày sinh
              </label>

              <input
                type="date"
                className="form-control"
                value={form.dateOfBirth}
                onChange={(e) =>
                  setField(
                    'dateOfBirth',
                    e.target.value
                  )
                }
              />
            </div>

            <div className="col-lg-3">
              <label className="form-label fw-semibold">
                Giới tính
              </label>

              <select
                className="form-select"
                value={form.gender}
                onChange={(e) =>
                  setField(
                    'gender',
                    e.target.value
                  )
                }
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

            <div className="col-lg-6">
              <label className="form-label fw-semibold">
                Địa chỉ
              </label>

              <input
                type="text"
                className="form-control"
                value={form.address}
                onChange={(e) =>
                  setField(
                    'address',
                    e.target.value
                  )
                }
              />
            </div>
          </div>

          <hr className="my-4 opacity-25" />

          <h5 className="fw-bold mb-3">
            Thông tin tiếp nhận
          </h5>

          <div className="row g-3">
            <div className="col-lg-4">
              <label className="form-label fw-semibold">
                Loại dịch vụ
              </label>

              <select
                className="form-select"
                value={form.serviceType}
                onChange={(e) =>
                  setField(
                    'serviceType',
                    e.target.value
                  )
                }
              >
                <option value="EXAMINATION">
                  Khám bệnh
                </option>

                <option value="TEST">
                  Xét nghiệm
                </option>
              </select>
            </div>

            <div className="col-lg-4">
              <label className="form-label fw-semibold">
                Mã chuyên khoa
              </label>

              <input
                type="text"
                className="form-control"
                placeholder="VD: CK008"
                value={form.specialtyId}
                onChange={(e) =>
                  setField(
                    'specialtyId',
                    e.target.value
                  )
                }
              />
            </div>

            <div className="col-lg-4">
              <label className="form-label fw-semibold">
                Lý do đến khám/xét nghiệm
              </label>

              <input
                type="text"
                className="form-control"
                placeholder="Triệu chứng hoặc nhu cầu chính"
                value={form.reason}
                onChange={(e) =>
                  setField(
                    'reason',
                    e.target.value
                  )
                }
              />
            </div>

            <div className="col-12">
              <label className="form-label fw-semibold">
                Ghi chú
              </label>

              <textarea
                className="form-control"
                rows="4"
                placeholder="Thông tin bổ sung cho bác sĩ/kỹ thuật viên..."
                value={form.notes}
                onChange={(e) =>
                  setField(
                    'notes',
                    e.target.value
                  )
                }
              />
            </div>
          </div>

          <div className="d-flex flex-wrap gap-2 mt-4">
            <button
              type="submit"
              className="btn btn-primary"
              disabled={saving}
            >
              <i className="fa-solid fa-user-check me-2" />

              {saving
                ? 'Đang tiếp nhận...'
                : 'Tạo lượt tiếp nhận'}
            </button>

            <button
              type="button"
              className="btn btn-outline-secondary"
              disabled={saving}
              onClick={() =>
                setForm(EMPTY_FORM)
              }
            >
              Xóa biểu mẫu
            </button>
          </div>
        </div>
      </form>
    </div>
  );
}