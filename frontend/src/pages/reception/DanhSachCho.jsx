import React, {
  useEffect,
  useMemo,
  useState,
} from 'react';

import Notification from '../../components/Notification';

import {
  getAppointmentOptions,
} from '../../services/appointmentService';

import visitService from '../../services/visitService';

import {
  getApiErrorMessage,
} from '../../services/api';

// =====================================================
// EMPTY
// =====================================================

const EMPTY_FORM = {
  fullName: '',
  phone: '',
  email: '',
  dateOfBirth: '',
  gender: '',
  address: '',

  serviceType:
    'EXAMINATION',

  specialtyId:
    '',

  testId:
    '',

  reason:
    '',

  notes:
    '',
};

// =====================================================
// HELPERS
// =====================================================

const getLocalToday =
  () => {
    const now =
      new Date();

    return [
      now.getFullYear(),

      String(
        now.getMonth() + 1
      ).padStart(
        2,
        '0'
      ),

      String(
        now.getDate()
      ).padStart(
        2,
        '0'
      ),
    ].join('-');
  };

const getTestId =
  (item) =>
    item?.id ??
    item?.IDXetNghiem;

const getTestName =
  (item) =>
    item?.name ??
    item?.TenXetNghiem ??
    'Xét nghiệm';

const getTestSpecialty =
  (item) =>
    item?.specialtyId ??
    item?.ChuyenKhoaID;

const getTestPrice =
  (item) =>
    Number(
      item?.price ??
      item?.Gia ??
      0
    ) || 0;

// =====================================================
// COMPONENT
// =====================================================

export default function QuanLyTiepNhan() {
  const [
    form,
    setForm,
  ] = useState(
    EMPTY_FORM
  );

  const [
    specialties,
    setSpecialties,
  ] = useState({});

  const [
    tests,
    setTests,
  ] = useState([]);

  const [
    optionsLoading,
    setOptionsLoading,
  ] = useState(true);

  const [
    saving,
    setSaving,
  ] = useState(false);

  const [
    message,
    setMessage,
  ] = useState({
    type: '',
    text: '',
  });

  // =====================================================
  // LOAD OPTIONS
  // =====================================================

  useEffect(() => {
    let active = true;

    const loadOptions =
      async () => {
        try {
          setOptionsLoading(
            true
          );

          const data =
            await getAppointmentOptions();

          if (!active) {
            return;
          }

          setSpecialties(
            data?.departments ||
            {}
          );

          setTests(
            data?.tests ||
            []
          );
        } catch (err) {
          if (!active) {
            return;
          }

          setMessage({
            type:
              'danger',

            text:
              getApiErrorMessage(
                err,
                'Không thể tải danh mục chuyên khoa/xét nghiệm.'
              ),
          });
        } finally {
          if (active) {
            setOptionsLoading(
              false
            );
          }
        }
      };

    loadOptions();

    return () => {
      active = false;
    };
  }, []);

  // =====================================================
  // FILTER TESTS
  // =====================================================

  const filteredTests =
    useMemo(
      () => {
        if (
          !form.specialtyId
        ) {
          return tests;
        }

        return tests.filter(
          (item) =>
            String(
              getTestSpecialty(
                item
              ) ||
              ''
            ) ===
            String(
              form.specialtyId
            )
        );
      },
      [
        tests,
        form.specialtyId,
      ]
    );

  // =====================================================
  // SET FIELD
  // =====================================================

  const setField =
    (
      name,
      value
    ) => {
      setForm(
        (prev) => ({
          ...prev,
          [name]:
            value,
        })
      );
    };

  const handleServiceTypeChange =
    (value) => {
      setForm(
        (prev) => ({
          ...prev,

          serviceType:
            value,

          testId:
            '',

          specialtyId:
            '',
        })
      );
    };

  // =====================================================
  // SUBMIT
  // =====================================================

  const handleSubmit =
    async (e) => {
      e.preventDefault();

      if (
        !form.fullName
          .trim()
      ) {
        setMessage({
          type:
            'warning',

          text:
            'Vui lòng nhập họ và tên khách hàng.',
        });

        return;
      }

      if (
        !form.phone
          .trim()
      ) {
        setMessage({
          type:
            'warning',

          text:
            'Vui lòng nhập số điện thoại.',
        });

        return;
      }

      if (
        form.serviceType ===
          'EXAMINATION'
        &&
        !form.specialtyId
      ) {
        setMessage({
          type:
            'warning',

          text:
            'Vui lòng chọn chuyên khoa khám.',
        });

        return;
      }

      if (
        form.serviceType ===
          'TEST'
        &&
        !form.testId
      ) {
        setMessage({
          type:
            'warning',

          text:
            'Vui lòng chọn loại xét nghiệm.',
        });

        return;
      }

      try {
        setSaving(
          true
        );

        setMessage({
          type: '',
          text: '',
        });

        const data =
          await visitService
            .createWalkInVisit(
              {
                ...form,

                fullName:
                  form.fullName
                    .trim(),

                phone:
                  form.phone
                    .trim(),

                email:
                  form.email
                    .trim(),

                address:
                  form.address
                    .trim(),

                reason:
                  form.reason
                    .trim(),

                notes:
                  form.notes
                    .trim(),
              }
            );

        const roomText =
          data?.roomName
            ? ` - Phòng: ${data.roomName}`
            : '';

        const orderText =
          data?.testOrderId
            ? ` - Phiếu XN: ${data.testOrderId}`
            : '';

        setMessage({
          type:
            'success',

          text:
            data?.message
            ||
            `Tiếp nhận thành công - STT: ${
              data?.queueNumber ??
              '—'
            }${roomText}${orderText}`,
        });

        setForm(
          EMPTY_FORM
        );
      } catch (err) {
        setMessage({
          type:
            'danger',

          text:
            getApiErrorMessage(
              err,
              'Không thể tạo lượt tiếp nhận.'
            ),
        });
      } finally {
        setSaving(
          false
        );
      }
    };

  const today =
    getLocalToday();

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
          type={
            message.type
          }
          message={
            message.text
          }
          onClose={() =>
            setMessage({
              type: '',
              text: '',
            })
          }
        />
      )}

      <form
        onSubmit={
          handleSubmit
        }
        className="card border-0 shadow-sm rounded-4"
      >
        <div className="card-body p-4">
          {/* PATIENT */}

          <div className="d-flex align-items-center gap-3 mb-4">
            <div
              className="d-flex align-items-center justify-content-center rounded-circle"
              style={{
                width:
                  48,

                height:
                  48,

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
                Họ và tên{' '}
                <span className="text-danger">
                  *
                </span>
              </label>

              <input
                type="text"
                className="form-control"
                value={
                  form.fullName
                }
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
                Số điện thoại{' '}
                <span className="text-danger">
                  *
                </span>
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
                value={
                  form.email
                }
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
                max={today}
                value={
                  form.dateOfBirth
                }
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
                value={
                  form.gender
                }
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

                <option value="nam">
                  Nam
                </option>

                <option value="nu">
                  Nữ
                </option>

                <option value="khac">
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
                value={
                  form.address
                }
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

          {/* SERVICE */}

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
                value={
                  form.serviceType
                }
                onChange={(e) =>
                  handleServiceTypeChange(
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

            {/* SPECIALTY */}

            <div className="col-lg-4">
              <label className="form-label fw-semibold">
                Chuyên khoa
                {form.serviceType ===
                  'EXAMINATION' && (
                  <span className="text-danger">
                    {' '}*
                  </span>
                )}
              </label>

              <select
                className="form-select"
                value={
                  form.specialtyId
                }
                disabled={
                  optionsLoading
                }
                required={
                  form.serviceType ===
                  'EXAMINATION'
                }
                onChange={(e) => {
                  setField(
                    'specialtyId',
                    e.target.value
                  );

                  setField(
                    'testId',
                    ''
                  );
                }}
              >
                <option value="">
                  -- Chọn chuyên khoa --
                </option>

                {Object.entries(
                  specialties
                ).map(
                  ([
                    id,
                    name,
                  ]) => (
                    <option
                      key={
                        id
                      }
                      value={
                        id
                      }
                    >
                      {
                        name
                      }
                    </option>
                  )
                )}
              </select>
            </div>

            {/* TEST */}

            {form.serviceType ===
              'TEST' && (
              <div className="col-lg-4">
                <label className="form-label fw-semibold">
                  Loại xét nghiệm{' '}
                  <span className="text-danger">
                    *
                  </span>
                </label>

                <select
                  className="form-select"
                  value={
                    form.testId
                  }
                  required
                  disabled={
                    optionsLoading
                  }
                  onChange={(e) => {
                    const testId =
                      e.target.value;

                    const selected =
                      tests.find(
                        (item) =>
                          String(
                            getTestId(
                              item
                            )
                          ) ===
                          String(
                            testId
                          )
                      );

                    setForm(
                      (prev) => ({
                        ...prev,

                        testId,

                        specialtyId:
                          selected
                            ? String(
                                getTestSpecialty(
                                  selected
                                ) ||
                                prev.specialtyId
                              )
                            : prev.specialtyId,
                      })
                    );
                  }}
                >
                  <option value="">
                    -- Chọn xét nghiệm --
                  </option>

                  {filteredTests.map(
                    (item) => {
                      const id =
                        getTestId(
                          item
                        );

                      const price =
                        getTestPrice(
                          item
                        );

                      return (
                        <option
                          key={
                            id
                          }
                          value={
                            id
                          }
                        >
                          {getTestName(
                            item
                          )}
                          {' - '}
                          {price.toLocaleString(
                            'vi-VN'
                          )}
                          {' đ'}
                        </option>
                      );
                    }
                  )}
                </select>
              </div>
            )}

            {/* REASON */}

            <div
              className={
                form.serviceType ===
                'TEST'
                  ? 'col-lg-12'
                  : 'col-lg-4'
              }
            >
              <label className="form-label fw-semibold">
                Lý do đến khám/xét nghiệm
              </label>

              <input
                type="text"
                className="form-control"
                placeholder="Triệu chứng hoặc nhu cầu chính"
                value={
                  form.reason
                }
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
                maxLength={500}
                placeholder="Thông tin bổ sung cho bác sĩ/kỹ thuật viên..."
                value={
                  form.notes
                }
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
              disabled={
                saving ||
                optionsLoading
              }
            >
              <i className="fa-solid fa-user-check me-2" />

              {saving
                ? 'Đang tiếp nhận...'
                : 'Tạo lượt tiếp nhận'}
            </button>

            <button
              type="button"
              className="btn btn-outline-secondary"
              disabled={
                saving
              }
              onClick={() =>
                setForm(
                  EMPTY_FORM
                )
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