import api, {
  unwrap,
} from './api';

// =====================================================
// RECEPTION
// =====================================================

export const getReceptionWaitingList =
  async (
    type = 'ALL'
  ) => {
    const response =
      await api.get(
        '/reception/visits/waiting',
        {
          params: {
            type,
          },
        }
      );

    return unwrap(response);
  };

export const checkInAppointment =
  async (
    appointmentId,
    type,
    data = {}
  ) => {
    const response =
      await api.post(
        `/appointments/${encodeURIComponent(
          appointmentId
        )}/check-in`,
        {
          type,
          ...data,
        }
      );

    return unwrap(response);
  };

export const checkInVisit =
  async ({
    appointmentId,
    customerId = null,
    visitType,
    note = null,
  }) => {
    const response =
      await api.post(
        '/reception/visits/check-in',
        {
          appointmentId,
          customerId,
          visitType,
          note,
        }
      );

    return unwrap(response);
  };

export const checkInByQr =
  async (
    qrCode,
    note = ''
  ) => {
    const response =
      await api.post(
        '/appointments/check-in-qr',
        {
          qrCode,
          note,
        }
      );

    return unwrap(response);
  };

export const createWalkInVisit =
  async (payload) => {
    const response =
      await api.post(
        '/appointments/walk-in',
        payload
      );

    return unwrap(response);
  };

// =====================================================
// VISIT
// =====================================================

export const getVisit =
  async (
    id,
    type
  ) => {
    const response =
      await api.get(
        `/visits/${encodeURIComponent(
          id
        )}`,
        {
          params: {
            type,
          },
        }
      );

    return unwrap(response);
  };

export const updateVisitStatus =
  async (
    id,
    type,
    status,
    note = ''
  ) => {
    const response =
      await api.patch(
        `/visits/${encodeURIComponent(
          id
        )}/status`,
        {
          status,
          note,
        },
        {
          params: {
            type,
          },
        }
      );

    return unwrap(response);
  };

// =====================================================
// RECEPTION QUEUE ACTIONS
// =====================================================

export const callReceptionPatient =
  async (
    appointmentId
  ) => {
    const response =
      await api.post(
        `/appointments/queue/${encodeURIComponent(
          appointmentId
        )}/call`
      );

    return unwrap(response);
  };

export const holdReceptionPatient =
  async (
    appointmentId
  ) => {
    const response =
      await api.post(
        `/appointments/queue/${encodeURIComponent(
          appointmentId
        )}/hold`
      );

    return unwrap(response);
  };

export const skipReceptionPatient =
  async (
    appointmentId
  ) => {
    const response =
      await api.post(
        `/appointments/queue/${encodeURIComponent(
          appointmentId
        )}/skip`
      );

    return unwrap(response);
  };

// =====================================================
// DOCTOR
// =====================================================

export const getDoctorQueue =
  async () => {
    const response =
      await api.get(
        '/doctor/queue'
      );

    return unwrap(response);
  };

export const callPatient =
  async (id) => {
    const response =
      await api.post(
        `/doctor/visits/${encodeURIComponent(
          id
        )}/call`
      );

    return unwrap(response);
  };

export const holdPatient =
  async (id) => {
    const response =
      await api.post(
        `/doctor/visits/${encodeURIComponent(
          id
        )}/hold`
      );

    return unwrap(response);
  };

// Compatibility:
// skip cũ của bác sĩ = chuyển gọi lại sau.
export const skipPatient =
  async (id) => {
    const response =
      await api.post(
        `/doctor/visits/${encodeURIComponent(
          id
        )}/skip`
      );

    return unwrap(response);
  };

export const startExam =
  async (id) => {
    const response =
      await api.post(
        `/doctor/visits/${encodeURIComponent(
          id
        )}/start`
      );

    return unwrap(response);
  };

// =====================================================
// EXAMINATION
// Giữ lại vì các trang Doctor hiện đang dùng.
// =====================================================

export const getExamination =
  async (
    luotKhamId
  ) => {
    const response =
      await api.get(
        `/doctor/examinations/visit/${encodeURIComponent(
          luotKhamId
        )}`
      );

    return unwrap(response);
  };

export const saveExamination =
  async (
    idKham,
    data,
    luotKhamId
  ) => {
    const payload = {
      visitId:
        luotKhamId,

      symptoms:
        data.trieuChung,

      history:
        data.tienSuBenh,

      diagnosis:
        data.chanDoan,

      conclusion:
        data.ketLuan,

      advice:
        data.huongDieuTri,
    };

    const response =
      await api.put(
        `/doctor/examinations/${encodeURIComponent(
          idKham
        )}`,
        payload
      );

    return unwrap(response);
  };

export const completeExamination =
  async (
    luotKhamId
  ) => {
    const response =
      await api.post(
        `/doctor/examinations/${encodeURIComponent(
          luotKhamId
        )}/complete`
      );

    return unwrap(response);
  };

// =====================================================
// DEFAULT EXPORT
// =====================================================

const visitService = {
  getReceptionWaitingList,

  checkInAppointment,
  checkInVisit,
  checkInByQr,
  createWalkInVisit,

  getVisit,
  updateVisitStatus,

  callReceptionPatient,
  holdReceptionPatient,
  skipReceptionPatient,

  getDoctorQueue,
  callPatient,
  holdPatient,
  skipPatient,
  startExam,

  getExamination,
  saveExamination,
  completeExamination,
};

export default visitService;