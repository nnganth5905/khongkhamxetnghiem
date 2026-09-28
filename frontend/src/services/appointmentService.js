// src/services/appointmentService.js

import api, { unwrap } from './api';

// =====================================================
// ERROR
// =====================================================

export const getApiErrorMessage = (
  error,
  defaultMessage =
    'Có lỗi xảy ra khi xử lý thông tin lịch hẹn.'
) => {
  if (!error) {
    return defaultMessage;
  }

  if (typeof error === 'string') {
    return error;
  }

  return (
    error.response?.data?.message ||
    error.response?.data?.error ||
    error.message ||
    defaultMessage
  );
};

// =====================================================
// OPTIONS
// =====================================================

export const getAppointmentOptions =
  async (params = {}) => {
    const response =
      await api.get(
        '/appointments/options',
        {
          params,
        }
      );

    return unwrap(response);
  };

export const getExaminationOptions =
  getAppointmentOptions;

export const getTestingOptions =
  getAppointmentOptions;

// =====================================================
// OLD TAKEN-TIMES COMPATIBILITY
// =====================================================

export const getTakenTimes =
  async (
    type,
    doctorId,
    date
  ) => {
    const response =
      await api.get(
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

// =====================================================
// AVAILABLE SLOTS FROM WORK SCHEDULE
// =====================================================

export const getAvailableTimeSlots =
  async ({
    type,
    doctorId,
    date,
    excludeId = null,
  } = {}) => {
    if (
      !type ||
      !doctorId ||
      !date
    ) {
      return {
        doctorId,
        date,
        slots: [],
        takenTimes: [],
      };
    }

    const response =
      await api.get(
        '/appointments/time-slots',
        {
          params: {
            type,
            doctorId,
            date,
            excludeId:
              excludeId || undefined,
          },
        }
      );

    return unwrap(response);
  };

// =====================================================
// CREATE
// =====================================================

export const createExaminationAppointment =
  async (payload) => {
    const response =
      await api.post(
        '/appointments/examinations',
        payload
      );

    return unwrap(response);
  };

export const createTestAppointment =
  async (payload) => {
    const response =
      await api.post(
        '/appointments/tests',
        payload
      );

    return unwrap(response);
  };

export const createTestingAppointment =
  createTestAppointment;

export const createQuickAppointment =
  async (payload) => {
    const response =
      await api.post(
        '/appointments/quick',
        payload
      );

    return unwrap(response);
  };

// Compatibility.
export const createAppointment =
  async (payload = {}) => {
    const type =
      String(
        payload.type ||
        payload.appointmentType ||
        ''
      ).toUpperCase();

    if (
      type === 'TEST'
      ||
      payload.idxetnghiem
    ) {
      return createTestAppointment(
        payload
      );
    }

    return createExaminationAppointment(
      payload
    );
  };

// =====================================================
// MY APPOINTMENTS
// =====================================================

export const getMyAppointments =
  async () => {
    const response =
      await api.get(
        '/appointments/my'
      );

    return unwrap(response);
  };

// Detail route đã có sẵn.
// Không gọi /appointments/my/{id} vì backend không có route đó.
export const getMyAppointmentDetail =
  async (id) => {
    return getAppointmentById(id);
  };

// =====================================================
// STAFF LIST
// =====================================================

export const getAppointments =
  async (params = {}) => {
    const response =
      await api.get(
        '/appointments',
        {
          params,
        }
      );

    return unwrap(response);
  };

// =====================================================
// DETAIL
// =====================================================

export const getAppointmentById =
  async (id) => {
    const response =
      await api.get(
        `/appointments/${encodeURIComponent(
          id
        )}`
      );

    return unwrap(response);
  };

export const getAppointment =
  getAppointmentById;

export const getAppointmentDetail =
  getAppointmentById;

// =====================================================
// UPDATE
// =====================================================

export const updateAppointment =
  async (
    id,
    payload
  ) => {
    const response =
      await api.put(
        `/appointments/${encodeURIComponent(
          id
        )}`,
        payload
      );

    return unwrap(response);
  };

// =====================================================
// CANCEL
// =====================================================

export const cancelAppointment =
  async (
    id
  ) => {
    const response =
      await api.post(
        `/appointments/${encodeURIComponent(
          id
        )}/cancel`
      );

    return unwrap(response);
  };

// =====================================================
// MODULE 9 - RECEPTION / CHECK-IN
//
// Giữ export vì các trang hiện tại đang import.
// Backend tương ứng sẽ được hoàn thiện ở Module 9.
// =====================================================

export const createWalkInVisit =
  async (payload) => {
    const response =
      await api.post(
        '/appointments/walk-in',
        payload
      );

    return unwrap(response);
  };

export const getReceptionAppointments =
  async (params = {}) => {
    const response =
      await api.get(
        '/appointments/reception',
        {
          params,
        }
      );

    return unwrap(response);
  };

export const checkInAppointment =
  async (
    id,
    data = {}
  ) => {
    const response =
      await api.post(
        `/appointments/${encodeURIComponent(
          id
        )}/check-in`,
        data
      );

    return unwrap(response);
  };

export const checkInByQr =
  async (
    qrCode,
    data = {}
  ) => {
    const response =
      await api.post(
        '/appointments/check-in-qr',
        {
          qrCode,
          ...data,
        }
      );

    return unwrap(response);
  };

export const getWaitingQueue =
  async (params = {}) => {
    const normalized =
      typeof params === 'string'
        ? {
            type: params,
          }
        : params;

    const response =
      await api.get(
        '/appointments/waiting-queue',
        {
          params: normalized,
        }
      );

    return unwrap(response);
  };

export const getWaitingList =
  getWaitingQueue;

export const callPatient =
  async (
    id,
    data = {}
  ) => {
    const response =
      await api.post(
        `/appointments/queue/${encodeURIComponent(
          id
        )}/call`,
        data
      );

    return unwrap(response);
  };

export const holdPatient =
  async (id) => {
    const response =
      await api.post(
        `/appointments/queue/${encodeURIComponent(
          id
        )}/hold`
      );

    return unwrap(response);
  };

export const skipPatient =
  async (
    id,
    reason = ''
  ) => {
    const response =
      await api.post(
        `/appointments/queue/${encodeURIComponent(
          id
        )}/skip`,
        {
          reason,
        }
      );

    return unwrap(response);
  };

// =====================================================
// OTHER DATA
// =====================================================

export const getAvailableDoctors =
  async (params = {}) => {
    const response =
      await api.get(
        '/doctors',
        {
          params,
        }
      );

    return unwrap(response);
  };

export const getSpecialties =
  async (params = {}) => {
    const response =
      await api.get(
        '/specialties',
        {
          params,
        }
      );

    return unwrap(response);
  };

// =====================================================
// ALIASES
// =====================================================

export const checkIn =
  checkInAppointment;

export const bookAppointment =
  createAppointment;