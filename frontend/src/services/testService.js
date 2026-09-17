// src/services/testService.js
import api, { unwrap } from './api';

// --- HÀM XỬ LÝ LỖI ---
export const getApiErrorMessage = (
  error,
  defaultMessage = 'Có lỗi xảy ra khi tải dữ liệu xét nghiệm.'
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

// --- QUẢN LÝ DANH MỤC XÉT NGHIỆM ---
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

// --- TRA CỨU CÔNG KHAI (KHÔNG CẦN ĐĂNG NHẬP) ---
export const searchResultPublic = async (params = {}) => {
  const response = await api.get('/tests/public/search', { params });
  return unwrap(response);
};

export const getPublicResultDetail = async (tokenOrId) => {
  const response = await api.get(`/tests/public/results/${tokenOrId}`);
  return unwrap(response);
};

export const searchTestResult = searchResultPublic;

// --- DÀNH CHO BỆNH NHÂN (ĐÃ ĐĂNG NHẬP) ---
export const getMyResults = async (params = {}) => {
  const response = await api.get('/tests/my-results', { params });
  return unwrap(response);
};

export const getMyResultDetail = async (id) => {
  const response = await api.get(`/tests/my-results/${id}`);
  return unwrap(response);
};

export const getResult = async (id) => {
  const response = await api.get(`/tests/results/${id}`);
  return unwrap(response);
};

export const downloadResultPdf = async (id) => {
  const response = await api.get(`/tests/results/${id}/pdf`, {
    responseType: 'blob',
  });
  return response.data;
};

export const printResult = async (id) => {
  const response = await api.get(`/tests/results/${id}/print`);
  return unwrap(response);
};

// --- ALIASES HỖ TRỢ ---
export const getResultDetail = getResult;
export const getTestResult = getResult;