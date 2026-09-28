import React, {
  useEffect,
  useState,
} from 'react';

import {
  useNavigate,
  useSearchParams,
} from 'react-router-dom';

import {
  cancelAppointment,
  getApiErrorMessage,
  getAppointmentById,
  getAvailableTimeSlots,
  updateAppointment,
} from '../../services/appointmentService';

const getLocalToday = () => {
  const now = new Date();

  return [
    now.getFullYear(),
    String(
      now.getMonth() + 1
    ).padStart(2, '0'),
    String(
      now.getDate()
    ).padStart(2, '0'),
  ].join('-');
};

export default function LichHenSua() {
  const [searchParams] =
    useSearchParams();

  const id =
    searchParams.get('id');

  const navigate =
    useNavigate();

  const [
    appointment,
    setAppointment,
  ] = useState(null);

  const [
    loading,
    setLoading,
  ] = useState(true);

  const [
    saving,
    setSaving,
  ] = useState(false);

  const [
    slotsLoading,
    setSlotsLoading,
  ] = useState(false);

  const [
    availableSlots,
    setAvailableSlots,
  ] = useState([]);

  const [
    message,
    setMessage,
  ] = useState({
    type: '',
    text: '',
  });

  const [
    formData,
    setFormData,
  ] = useState({
    new_date: '',
    new_time: '',
    ghichu: '',
  });

  // =====================================================
  // LOAD DETAIL
  // =====================================================

  useEffect(() => {
    let active = true;

    const fetchDetail =
      async () => {
        try {
          setLoading(true);

          const data =
            await getAppointmentById(
              id
            );

          if (!active) {
            return;
          }

          setAppointment(data);

          setFormData({
            new_date:
              data?.appointmentDate
                ?.substring(0, 10)
              ||
              data?.date
                ?.substring(0, 10)
              ||
              '',

            new_time:
              String(
                data?.appointmentTime
                ||
                data?.time
                ||
                ''
              ).substring(0, 5),

            ghichu:
              data?.note || '',
          });
        } catch (error) {
          if (!active) {
            return;
          }

          setMessage({
            type: 'danger',

            text:
              getApiErrorMessage(
                error,
                'Không thể tải thông tin lịch hẹn.'
              ),
          });
        } finally {
          if (active) {
            setLoading(false);
          }
        }
      };

    if (id) {
      fetchDetail();
    } else {
      setMessage({
        type: 'danger',
        text:
          'Thiếu mã lịch hẹn hợp lệ trên URL.',
      });

      setLoading(false);
    }

    return () => {
      active = false;
    };
  }, [id]);

  // =====================================================
  // LOAD REAL SLOTS
  // =====================================================

  useEffect(() => {
    const doctorId =
      appointment?.doctorId;

    const date =
      formData.new_date;

    if (
      !doctorId ||
      !date
    ) {
      setAvailableSlots([]);
      return;
    }

    let active = true;

    const loadSlots =
      async () => {
        try {
          setSlotsLoading(true);

          const data =
            await getAvailableTimeSlots({
              type:
                appointment.type,

              doctorId,

              date,

              /*
               * Loại trừ chính lịch đang sửa,
               * nếu không giờ hiện tại sẽ bị
               * xem như đã kín.
               */
              excludeId:
                appointment.id,
            });

          if (!active) {
            return;
          }

          const slots =
            Array.isArray(data)
              ? data
              : data?.slots || [];

          setAvailableSlots(
            slots
          );

          setFormData(
            (prev) => {
              if (
                slots.includes(
                  prev.new_time
                )
              ) {
                return prev;
              }

              return {
                ...prev,
                new_time: '',
              };
            }
          );
        } catch (error) {
          if (!active) {
            return;
          }

          setAvailableSlots([]);

          setMessage({
            type: 'danger',

            text:
              getApiErrorMessage(
                error,
                'Không thể tải khung giờ làm việc của bác sĩ.'
              ),
          });
        } finally {
          if (active) {
            setSlotsLoading(false);
          }
        }
      };

    loadSlots();

    return () => {
      active = false;
    };
  }, [
    appointment,
    formData.new_date,
  ]);

  // =====================================================
  // FORM
  // =====================================================

  const handleChange = (e) => {
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

  const handleSubmit =
    async (e) => {
      e.preventDefault();

      setSaving(true);

      setMessage({
        type: '',
        text: '',
      });

      try {
        await updateAppointment(
          id,
          formData
        );

        setMessage({
          type: 'success',
          text:
            'Đổi lịch hẹn thành công!',
        });

        navigate(
          `/lich-hen/${id}`
        );
      } catch (error) {
        setMessage({
          type: 'danger',

          text:
            getApiErrorMessage(
              error,
              'Không thể đổi lịch hẹn lúc này.'
            ),
        });
      } finally {
        setSaving(false);
      }
    };

  const handleCancel =
    async () => {
      if (
        !window.confirm(
          'Bạn có chắc chắn muốn hủy lịch hẹn này?'
        )
      ) {
        return;
      }

      setSaving(true);

      try {
        await cancelAppointment(
          id
        );

        navigate('/lich-hen');
      } catch (error) {
        setMessage({
          type: 'danger',

          text:
            getApiErrorMessage(
              error,
              'Không thể hủy lịch hẹn.'
            ),
        });
      } finally {
        setSaving(false);
      }
    };

  // =====================================================
  // RENDER
  // =====================================================

  if (loading) {
    return (
      <div className="bg-light min-vh-100 d-flex align-items-center justify-content-center">
        <div className="spinner-border text-primary" />
      </div>
    );
  }

  return (
    <div className="bg-light min-vh-100 py-5">
      <div
        className="container"
        style={{
          maxWidth: '600px',
        }}
      >
        <div className="d-flex align-items-center mb-4">
          <button
            type="button"
            className="btn btn-light border-0 shadow-sm rounded-circle me-3"
            onClick={() =>
              navigate(-1)
            }
          >
            <i className="fa-solid fa-arrow-left" />
          </button>

          <h3 className="fw-bold mb-0 text-primary">
            Thay đổi lịch hẹn
          </h3>
        </div>

        {message.text && (
          <div
            className={
              `alert alert-${message.type} shadow-sm rounded-3`
            }
          >
            {message.text}
          </div>
        )}

        {appointment && (
          <div className="card border-0 shadow-sm rounded-4">
            <div className="card-body p-4">
              <div className="alert alert-info py-2 px-3 mb-4 rounded-3">
                Đang chỉnh sửa:{' '}

                <strong>
                  {appointment.id}
                </strong>

                {' — '}

                {appointment.type
                  === 'EXAMINATION'
                  ? 'Khám bệnh'
                  : 'Xét nghiệm'}
              </div>

              <form
                onSubmit={
                  handleSubmit
                }
              >
                <div className="mb-3">
                  <label className="form-label fw-semibold">
                    Bác sĩ phụ trách
                  </label>

                  <input
                    type="text"
                    className="form-control bg-light"
                    value={
                      appointment.doctorName
                      ||
                      'Chưa xác định'
                    }
                    disabled
                  />
                </div>

                <div className="row">
                  <div className="col-md-6 mb-3">
                    <label className="form-label fw-semibold">
                      Ngày mới
                    </label>

                    <input
                      type="date"
                      className="form-control"
                      name="new_date"
                      value={
                        formData.new_date
                      }
                      onChange={
                        handleChange
                      }
                      min={
                        getLocalToday()
                      }
                      required
                    />
                  </div>

                  <div className="col-md-6 mb-3">
                    <label className="form-label fw-semibold">
                      Giờ mới
                    </label>

                    <select
                      className="form-select"
                      name="new_time"
                      value={
                        formData.new_time
                      }
                      onChange={
                        handleChange
                      }
                      disabled={
                        slotsLoading
                        ||
                        !formData.new_date
                      }
                      required
                    >
                      <option value="">
                        {slotsLoading
                          ? 'Đang tải...'
                          : '-- Chọn giờ --'}
                      </option>

                      {availableSlots.map(
                        (time) => (
                          <option
                            key={time}
                            value={time}
                          >
                            {time}
                          </option>
                        )
                      )}
                    </select>

                    {!slotsLoading
                      &&
                      formData.new_date
                      &&
                      availableSlots.length
                        === 0
                      && (
                        <div className="form-text text-danger">
                          Không có khung giờ trống.
                        </div>
                      )}
                  </div>
                </div>

                <div className="mb-4">
                  <label className="form-label fw-semibold">
                    Ghi chú
                  </label>

                  <textarea
                    className="form-control"
                    rows="3"
                    name="ghichu"
                    value={
                      formData.ghichu
                    }
                    onChange={
                      handleChange
                    }
                  />
                </div>

                <div className="d-flex justify-content-between pt-3 border-top">
                  <button
                    type="button"
                    className="btn btn-outline-danger rounded-pill px-4"
                    onClick={
                      handleCancel
                    }
                    disabled={
                      saving
                    }
                  >
                    Hủy lịch
                  </button>

                  <button
                    type="submit"
                    className="btn btn-primary rounded-pill px-4"
                    disabled={
                      saving
                      ||
                      slotsLoading
                      ||
                      !formData.new_time
                    }
                  >
                    {saving
                      ? 'Đang xử lý...'
                      : 'Lưu thay đổi'}
                  </button>
                </div>
              </form>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}