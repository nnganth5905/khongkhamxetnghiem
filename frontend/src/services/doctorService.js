import api, {
  unwrap,
} from './api';

// =====================================================
// ERROR
// =====================================================

export const getDoctorErrorMessage = (
  error,
  fallback =
    'Có lỗi xảy ra khi tải dữ liệu bác sĩ.',
) => {
  return (
    error?.response
      ?.data?.message ||
    error?.response
      ?.data?.error ||
    error?.message ||
    fallback
  );
};

// =====================================================
// PUBLIC DOCTORS
// =====================================================

export const getDoctors =
  async ({
    q = '',
    star = 0,
    specialtyId = '',
  } = {}) => {
    const response =
      await api.get(
        '/doctors',
        {
          params: {
            q:
              q || undefined,

            star:
              Number(star) > 0
                ? Number(star)
                : undefined,

            specialtyId:
              specialtyId ||
              undefined,
          },
        },
      );

    return unwrap(
      response,
    );
  };

// =====================================================
// DOCTOR DETAIL
// =====================================================

export const getDoctorById =
  async (
    doctorId,
  ) => {
    const response =
      await api.get(
        `/doctors/${encodeURIComponent(
          doctorId,
        )}`,
      );

    return unwrap(
      response,
    );
  };

// =====================================================
// SLOT
// =====================================================

export const getDoctorSlots =
  async (
    doctorId,
    date,
  ) => {
    /*
     * Endpoint này sẽ được hoàn thiện
     * ở module WorkSchedule/Appointment.
     */
    const response =
      await api.get(
        `/doctors/${encodeURIComponent(
          doctorId,
        )}/slots`,
        {
          params: {
            date,
          },
        },
      );

    return unwrap(
      response,
    );
  };

// =====================================================
// DOCTOR RESULT
// =====================================================

export const getPendingDoctorResults =
  async () => {
    const response =
      await api.get(
        '/doctor/results/pending',
      );

    return unwrap(
      response,
    );
  };

export const getDoctorResultDetail =
  async (
    resultId,
  ) => {
    const response =
      await api.get(
        `/doctor/results/${encodeURIComponent(
          resultId,
        )}`,
      );

    return unwrap(
      response,
    );
  };

export const approveDoctorResult =
  async (
    resultId,
    conclusion,
  ) => {
    const response =
      await api.post(
        `/doctor/results/${encodeURIComponent(
          resultId,
        )}/approve`,

        {
          conclusion:
            conclusion || '',
        },
      );

    return unwrap(
      response,
    );
  };