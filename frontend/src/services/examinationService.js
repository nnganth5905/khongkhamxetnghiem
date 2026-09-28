import api, { unwrap } from './api';

export const getMyExaminations = async () => {
  const response = await api.get('/examinations/mine');
  return unwrap(response);
};

export const getMyExamination = async (id) => {
  const response = await api.get(`/examinations/mine/${id}`);
  return unwrap(response);
};

export const getExaminationErrorMessage = (
  error,
  fallback = 'Không thể tải kết quả khám.'
) => {
  return (
    error?.response?.data?.message ||
    error?.response?.data?.error ||
    error?.message ||
    fallback
  );
};