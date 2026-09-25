import api, { unwrap } from './api';

export const getApiErrorMessage = (
  error,
  defaultMessage = 'Có lỗi xảy ra khi tải dữ liệu xét nghiệm.'
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

export const getTests = async (params = {}) => {
  const response = await api.get('/tests', { params });
  return unwrap(response);
};

export const getTestById = async (id) => {
  const response = await api.get(`/tests/${id}`);
  return unwrap(response);
};

export const getTest = getTestById;

export const getTestCategories = async () => {
  const response = await api.get('/test-categories');
  return unwrap(response);
};

export const searchResultPublic = async (params = {}) => {
  const response = await api.get('/tests/public/search', { params });
  return unwrap(response);
};

export const getPublicResultDetail = async (tokenOrId) => {
  const response = await api.get(
    `/tests/public/results/${encodeURIComponent(tokenOrId)}`
  );
  return unwrap(response);
};

export const searchTestResult = searchResultPublic;

export const getMyResults = async () => {
  const response = await api.get('/results/mine');
  return unwrap(response);
};

export const getMyResultDetail = async (id) => {
  const response = await api.get(
    `/results/${encodeURIComponent(id)}`
  );
  return unwrap(response);
};

export const getResult = getMyResultDetail;

export const downloadResultPdf = async (id) => {
  const response = await api.get(
    `/tests/results/${encodeURIComponent(id)}/pdf`,
    {
      responseType: 'blob',
    }
  );
  return response.data;
};

export const printResult = async (id) => {
  const response = await api.get(
    `/tests/results/${encodeURIComponent(id)}/print`
  );
  return unwrap(response);
};

export const getResultDetail = getMyResultDetail;
export const getTestResult = getMyResultDetail;