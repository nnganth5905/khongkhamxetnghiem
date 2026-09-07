import api from './api';

const unwrap = (response) => {
  if (response?.data?.data !== undefined) {
    return response.data.data;
  }

  return response?.data;
};

export const getApiErrorMessage = (
  error,
  fallback = 'Có lỗi xảy ra.'
) => {
  return (
    error?.response?.data?.message ||
    error?.response?.data?.error ||
    error?.message ||
    fallback
  );
};

/**
 * Lấy dữ liệu cần cho form đặt lịch:
 * - chuyên khoa
 * - xét nghiệm
 * - bác sĩ
 * - user hiện tại
 */
export const getAppointmentOptions = async () => {
  const response = await api.get('/appointments/options');
  return unwrap(response);
};

/**
 * Lấy danh sách khung giờ đã kín.
 *
 * type:
 * TEST
 * EXAMINATION
 */
export const getTakenTimes = async (
  type,
  doctorId,
  date
) => {
  const response = await api.get(
    '/appointments/taken-times',
    {
      params: {
        type,
        doctorId,
        date,
      },
    }
  );

  return unwrap(response);
};

/**
 * Book.jsx - form đặt lịch đơn giản
 */
export const createQuickAppointment = async (
  payload
) => {
  const response = await api.post(
    '/appointments/quick',
    payload
  );

  return unwrap(response);
};

/**
 * Đặt lịch xét nghiệm
 */
export const createTestAppointment = async (
  payload
) => {
  const response = await api.post(
    '/appointments/tests',
    payload
  );

  return unwrap(response);
};

/**
 * Đặt lịch khám
 */
export const createExaminationAppointment = async (
  payload
) => {
  const response = await api.post(
    '/appointments/examinations',
    payload
  );

  return unwrap(response);
};

/**
 * Danh sách lịch của khách hàng hiện tại
 */
export const getMyAppointments = async () => {
  const response = await api.get('/appointments/my');
  return unwrap(response);
};

/**
 * Chi tiết lịch
 */
export const getAppointmentDetail = async (id) => {
  const response = await api.get(
    `/appointments/${id}`
  );

  return unwrap(response);
};

/**
 * Cập nhật lịch
 */
export const updateAppointment = async (
  id,
  payload
) => {
  const response = await api.put(
    `/appointments/${id}`,
    payload
  );

  return unwrap(response);
};

/**
 * Hủy lịch
 */
export const cancelAppointment = async (id) => {
  const response = await api.post(
    `/appointments/${id}/cancel`
  );

  return unwrap(response);
};