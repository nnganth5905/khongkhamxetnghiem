import api from './api';

const unwrap = (response) => {
  if (response?.data?.data !== undefined) {
    return response.data.data;
  }

  return response?.data;
};

export const getMedicalArticles = async (
  page = 1,
  limit = 8
) => {
  const response = await api.get(
    '/medical-knowledge',
    {
      params: {
        page,
        limit,
      },
    }
  );

  return unwrap(response);
};

export const getMedicalArticle = async (id) => {
  const response = await api.get(
    `/medical-knowledge/${id}`
  );

  return unwrap(response);
};

export const getHandbook = async () => {
  const response = await api.get('/handbook');

  return unwrap(response);
};

export const getMedicalErrorMessage = (
  error,
  fallback = 'Không thể tải dữ liệu.'
) => {
  return (
    error?.response?.data?.message ||
    error?.response?.data?.error ||
    error?.message ||
    fallback
  );
};