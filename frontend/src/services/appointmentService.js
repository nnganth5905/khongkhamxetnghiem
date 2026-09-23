// src/services/appointmentService.js
import api, { unwrap } from './api';

// --- HÀM XỬ LÝ LỖI ---
export const getApiErrorMessage = (
  error,
  defaultMessage = 'Có lỗi xảy ra khi xử lý thông tin lịch hẹn.'
) => {
  if (!error) return defaultMessage;
  if (typeof error === 'string') return error;

  return (
    error.response?.data?.message ||
    error.response?.data?.error ||
    error.message ||
    defaultMessage
  );
};

// --- TÙY CHỌN & GIỜ ĐÃ ĐẶT ---
export const getAppointmentOptions = async (params = {}) => {
  const response = await api.get('/appointments/options', { params });
  return unwrap(response);
};
export const getExaminationOptions = getAppointmentOptions;
export const getTestingOptions = getAppointmentOptions;

export const getTakenTimes = async (type, doctorId, date) => {
  const response = await api.get('/appointments/taken-times', {
    params: { type, doctorId, date },
  });
  return unwrap(response);
};

// --- TẠO ĐẶT LỊCH ---
export const createAppointment = async (payload) => {
  const response = await api.post('/appointments', payload);
  return unwrap(response);
};

export const createQuickAppointment = async (payload) => {
  const response = await api.post('/appointments/quick', payload);
  return unwrap(response);
};

// ĐÃ SỬA ĐƯỜNG DẪN Ở ĐÂY
export const createExaminationAppointment = async (payload) => {
  const response = await api.post('/appointments/examinations', payload);
  return unwrap(response);
};

// ĐÃ SỬA ĐƯỜNG DẪN Ở ĐÂY
export const createTestAppointment = async (payload) => {
  const response = await api.post('/appointments/tests', payload);
  return unwrap(response);
};
export const createTestingAppointment = createTestAppointment;

// Tiếp nhận vãng lai tại quầy (QuanLyTiepNhan.jsx)
export const createWalkInVisit = async (payload) => {
  const response = await api.post('/appointments/walk-in', payload);
  return unwrap(response);
};

// --- DÀNH CHO KHÁCH HÀNG (MY APPOINTMENTS) ---
export const getMyAppointments = async (params = {}) => {
  const response = await api.get('/appointments/my', { params });
  return unwrap(response);
};

export const getMyAppointmentDetail = async (id) => {
  const response = await api.get(`/appointments/my/${id}`);
  return unwrap(response);
};

// --- QUẢN LÝ LỊCH HẸN CHUNG ---
export const getAppointments = async (params = {}) => {
  const response = await api.get('/appointments', { params });
  return unwrap(response);
};

export const getAppointmentById = async (id) => {
  const response = await api.get(`/appointments/${id}`);
  return unwrap(response);
};

export const getAppointment = getAppointmentById;

export const updateAppointment = async (id, payload) => {
  const response = await api.put(`/appointments/${id}`, payload);
  return unwrap(response);
};

export const cancelAppointment = async (id, reason = '') => {
  const response = await api.post(`/appointments/${id}/cancel`, { reason });
  return unwrap(response);
};

// --- TIẾP NHẬN & CHECK-IN (LỄ TÂN) ---
export const getReceptionAppointments = async (params = {}) => {
  const response = await api.get('/appointments/reception', { params });
  return unwrap(response);
};

export const checkInAppointment = async (id, data = {}) => {
  const response = await api.post(`/appointments/${id}/check-in`, data);
  return unwrap(response);
};

export const checkInByQr = async (qrCode, data = {}) => {
  const response = await api.post('/appointments/check-in-qr', { qrCode, ...data });
  return unwrap(response);
};

// --- HÀNG ĐỢI & DANH SÁCH CHỜ ---
export const getWaitingQueue = async (params = {}) => {
  const response = await api.get('/appointments/waiting-queue', { params });
  return unwrap(response);
};

export const getWaitingList = async (params = {}) => {
  const response = await api.get('/appointments/waiting-list', { params });
  return unwrap(response);
};

export const callPatient = async (id, data = {}) => {
  const response = await api.post(`/appointments/queue/${id}/call`, data);
  return unwrap(response);
};

export const skipPatient = async (id, reason = '') => {
  const response = await api.post(`/appointments/queue/${id}/skip`, { reason });
  return unwrap(response);
};

// --- BỔ TRỢ DỮ LIỆU ---
export const getAvailableTimeSlots = async (params = {}) => {
  const response = await api.get('/appointments/time-slots', { params });
  return unwrap(response);
};

export const getAvailableDoctors = async (params = {}) => {
  const response = await api.get('/doctors', { params });
  return unwrap(response);
};

export const getSpecialties = async (params = {}) => {
  const response = await api.get('/specialties', { params });
  return unwrap(response);
};

export const getAppointmentDetail = async (id) => {
  try {
    const response = await api.get(`/appointments/${id}`);
    return response.data;
  } catch (error) {
    throw error;
  }
};

// --- ALIASES ---
export const checkIn = checkInAppointment;
export const bookAppointment = createAppointment;