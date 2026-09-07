import api from './api';

const unwrap = (response) => {
  if (response?.data?.data !== undefined) {
    return response.data.data;
  }

  return response?.data;
};

export const getHomeData = async () => {
  const response = await api.get('/home');
  return unwrap(response);
};

export const submitConsultation = async (payload) => {
  const response = await api.post(
    '/contact/consultation',
    payload
  );

  return unwrap(response);
};

export const getHomeErrorMessage = (
  error,
  fallback = 'Có lỗi xảy ra, vui lòng thử lại.'
) => {
  return (
    error?.response?.data?.message ||
    error?.response?.data?.error ||
    error?.message ||
    fallback
  );
};