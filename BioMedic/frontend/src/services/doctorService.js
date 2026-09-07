import api from './api';

const unwrap = (response) => {
  if (response?.data?.data !== undefined) {
    return response.data.data;
  }

  return response?.data;
};

export const getDoctorErrorMessage = (
  error,
  fallback = 'Có lỗi xảy ra khi tải dữ liệu bác sĩ.'
) => {
  return (
    error?.response?.data?.message ||
    error?.response?.data?.error ||
    error?.message ||
    fallback
  );
};

export const getDoctors = async ({
  q = '',
  star = '0',
  page = 1,
} = {}) => {
  const response = await api.get('/doctors', {
    params: {
      q,
      star,
      page,
    },
  });

  return unwrap(response);
};

export const getDoctorSlots = async (
  doctorId,
  date
) => {
  const response = await api.get(
    `/doctors/${doctorId}/slots`,
    {
      params: {
        date,
      },
    }
  );

  return unwrap(response);
};