import React, {
  useState,
} from 'react';

import Notification from '../../components/Notification';

import { sendContactMessage } from '../../services/contactService';
import { getApiErrorMessage } from '../../services/api';

const EMPTY_FORM = {
  fullName: '',
  phone: '',
  email: '',
  subject: '',
  message: '',
};

export default function LienHe() {
  const [form, setForm] =
    useState(EMPTY_FORM);

  const [sending, setSending] =
    useState(false);

  const [notice, setNotice] =
    useState({
      type: '',
      text: '',
    });

  const setField = (
    name,
    value
  ) => {
    setForm((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleSubmit =
    async (e) => {
      e.preventDefault();

      if (
        !form.fullName.trim() ||
        !form.phone.trim() ||
        !form.message.trim()
      ) {
        setNotice({
          type: 'warning',

          text:
            'Vui lòng nhập họ tên, số điện thoại và nội dung liên hệ.',
        });

        return;
      }

      try {
        setSending(true);

        const data =
          await sendContactMessage(
            form
          );

        setNotice({
          type: 'success',

          text:
            data?.message ||
            'Bio Medic Center đã nhận được thông tin của bạn. Chúng tôi sẽ liên hệ sớm nhất có thể.',
        });

        setForm(
          EMPTY_FORM
        );
      } catch (error) {
        setNotice({
          type: 'danger',

          text: getApiErrorMessage(
            error,
            'Không thể gửi thông tin liên hệ.'
          ),
        });
      } finally {
        setSending(false);
      }
    };

  return (
    <div className="bg-light min-vh-100">
      <section className="py-5 bg-white text-center">
        <div className="container">
          <h1
            className="fw-bold mb-2"
            style={{
              color:
                'var(--primary)',
            }}
          >
            Liên hệ Bio Medic Center
          </h1>

          <p className="text-secondary mb-0">
            Chúng tôi sẵn sàng hỗ trợ bạn về dịch vụ xét nghiệm, đặt lịch và kết quả.
          </p>
        </div>
      </section>

      <div
        className="container py-5"
        style={{
          maxWidth: 1150,
        }}
      >
        {notice.text && (
          <Notification
            type={notice.type}
            message={notice.text}
            onClose={() =>
              setNotice({
                type: '',
                text: '',
              })
            }
          />
        )}

        <div className="row g-4">
          <div className="col-lg-5">
            <div className="card border-0 shadow-sm rounded-4 h-100">
              <div className="card-body p-4 p-lg-5">
                <h4
                  className="fw-bold mb-4"
                  style={{
                    color:
                      'var(--primary)',
                  }}
                >
                  Thông tin liên hệ
                </h4>

                <div className="d-flex gap-3 mb-4">
                  <div
                    className="d-flex align-items-center justify-content-center rounded-circle flex-shrink-0"
                    style={{
                      width: 44,
                      height: 44,
                      background:
                        '#eaf2ff',
                      color:
                        'var(--primary)',
                    }}
                  >
                    <i className="fa-solid fa-location-dot" />
                  </div>

                  <div>
                    <div className="fw-bold">
                      Địa chỉ
                    </div>

                    <div className="text-secondary">
                      24 Xuân Tảo, Xuân Đỉnh, Bắc Từ Liêm, Hà Nội
                    </div>
                  </div>
                </div>

                <div className="d-flex gap-3 mb-4">
                  <div
                    className="d-flex align-items-center justify-content-center rounded-circle flex-shrink-0"
                    style={{
                      width: 44,
                      height: 44,
                      background:
                        '#eaf2ff',
                      color:
                        'var(--primary)',
                    }}
                  >
                    <i className="fa-solid fa-phone" />
                  </div>

                  <div>
                    <div className="fw-bold">
                      Hotline
                    </div>

                    <a
                      href="tel:1900565656"
                      className="text-decoration-none"
                    >
                      1900 56 56 56
                    </a>
                  </div>
                </div>

                <div className="d-flex gap-3 mb-4">
                  <div
                    className="d-flex align-items-center justify-content-center rounded-circle flex-shrink-0"
                    style={{
                      width: 44,
                      height: 44,
                      background:
                        '#eaf2ff',
                      color:
                        'var(--primary)',
                    }}
                  >
                    <i className="fa-regular fa-envelope" />
                  </div>

                  <div>
                    <div className="fw-bold">
                      Email
                    </div>

                    <a
                      href="mailto:contact@biomediccenter.vn"
                      className="text-decoration-none"
                    >
                      contact@biomediccenter.vn
                    </a>
                  </div>
                </div>

                <div className="d-flex gap-3">
                  <div
                    className="d-flex align-items-center justify-content-center rounded-circle flex-shrink-0"
                    style={{
                      width: 44,
                      height: 44,
                      background:
                        '#eaf2ff',
                      color:
                        'var(--primary)',
                    }}
                  >
                    <i className="fa-regular fa-clock" />
                  </div>

                  <div>
                    <div className="fw-bold">
                      Giờ làm việc
                    </div>

                    <div className="text-secondary">
                      Thứ hai - Thứ sáu: 7:00 - 19:00
                      <br />
                      Thứ bảy - Chủ nhật: 7:00 - 16:00
                    </div>
                  </div>
                </div>

                <div className="ratio ratio-4x3 mt-4">
                  <iframe
                    title="Bio Medic Center Map"
                    src="https://www.google.com/maps?q=24%20Xuan%20Tao%20Bac%20Tu%20Liem%20Ha%20Noi&output=embed"
                    className="rounded-4 border-0"
                    loading="lazy"
                    referrerPolicy="no-referrer-when-downgrade"
                  />
                </div>
              </div>
            </div>
          </div>

          <div className="col-lg-7">
            <div className="card border-0 shadow-sm rounded-4">
              <div className="card-body p-4 p-lg-5">
                <h4 className="fw-bold mb-1">
                  Gửi yêu cầu hỗ trợ
                </h4>

                <p className="text-secondary mb-4">
                  Điền thông tin bên dưới, đội ngũ Bio Medic Center sẽ phản hồi sớm nhất.
                </p>

                <form
                  onSubmit={
                    handleSubmit
                  }
                >
                  <div className="row g-3">
                    <div className="col-md-6">
                      <label className="form-label fw-semibold">
                        Họ và tên *
                      </label>

                      <input
                        className="form-control"
                        value={
                          form.fullName
                        }
                        onChange={(e) =>
                          setField(
                            'fullName',
                            e
                              .target
                              .value
                          )
                        }
                        required
                      />
                    </div>

                    <div className="col-md-6">
                      <label className="form-label fw-semibold">
                        Số điện thoại *
                      </label>

                      <input
                        type="tel"
                        className="form-control"
                        value={
                          form.phone
                        }
                        onChange={(e) =>
                          setField(
                            'phone',
                            e
                              .target
                              .value
                          )
                        }
                        required
                      />
                    </div>

                    <div className="col-md-6">
                      <label className="form-label fw-semibold">
                        Email
                      </label>

                      <input
                        type="email"
                        className="form-control"
                        value={
                          form.email
                        }
                        onChange={(e) =>
                          setField(
                            'email',
                            e
                              .target
                              .value
                          )
                        }
                      />
                    </div>

                    <div className="col-md-6">
                      <label className="form-label fw-semibold">
                        Chủ đề
                      </label>

                      <select
                        className="form-select"
                        value={
                          form.subject
                        }
                        onChange={(e) =>
                          setField(
                            'subject',
                            e
                              .target
                              .value
                          )
                        }
                      >
                        <option value="">
                          -- Chọn --
                        </option>

                        <option value="APPOINTMENT">
                          Đặt lịch
                        </option>

                        <option value="TEST">
                          Dịch vụ xét nghiệm
                        </option>

                        <option value="RESULT">
                          Kết quả xét nghiệm
                        </option>

                        <option value="COMPLAINT">
                          Góp ý / phản ánh
                        </option>

                        <option value="OTHER">
                          Khác
                        </option>
                      </select>
                    </div>

                    <div className="col-12">
                      <label className="form-label fw-semibold">
                        Nội dung *
                      </label>

                      <textarea
                        className="form-control"
                        rows="6"
                        value={
                          form.message
                        }
                        onChange={(e) =>
                          setField(
                            'message',
                            e
                              .target
                              .value
                          )
                        }
                        required
                      />
                    </div>
                  </div>

                  <button
                    type="submit"
                    className="btn btn-primary mt-4 px-4"
                    disabled={
                      sending
                    }
                  >
                    <i className="fa-regular fa-paper-plane me-2" />

                    {sending
                      ? 'Đang gửi...'
                      : 'Gửi liên hệ'}
                  </button>
                </form>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}