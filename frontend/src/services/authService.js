// src/services/authService.js
import api, { unwrap } from './api';

// --- HÀM XỬ LÝ LỖI ---
export const getApiErrorMessage = (
  error,
  defaultMessage = 'Có lỗi xảy ra khi xác thực tài khoản.'
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

export const getAuthErrorMessage = getApiErrorMessage;

// --- QUẢN LÝ PHIÊN ĐĂNG NHẬP / CLIENT STATE ---
export const clearAuthSession = () => {
  localStorage.removeItem('token');
  localStorage.removeItem('user');
  localStorage.removeItem('auth');
  localStorage.removeItem('role');
  sessionStorage.clear();
};

export const clearSession = clearAuthSession;
export const clearAuth = clearAuthSession;

// --- CÁC HÀM XÁC THỰC ---
// Đăng nhập
export const loginRequest = async (payload) => {
  const response = await api.post('/auth/login', payload);
  return unwrap(response);
};

// Đăng ký
export const registerRequest = async (payload) => {
  const response = await api.post('/auth/register', payload);
  return unwrap(response);
};

// Đăng xuất an toàn (bỏ qua 404 từ backend)
export const logoutRequest = async () => {
  try {
    await api.post('/auth/logout');
  } catch (error) {
    console.warn('API /auth/logout chưa được hỗ trợ trên server:', error?.message);
  } finally {
    clearAuthSession();
  }
};

// Quên mật khẩu
export const forgotPasswordRequest = async (payload) => {
  const response = await api.post('/auth/forgot-password', payload);
  return unwrap(response);
};

// Xác thực token đặt lại mật khẩu (cho ResetPassword.jsx)
export const verifyResetTokenRequest = async (token) => {
  const response = await api.get(`/auth/reset-password/verify?token=${token}`);
  return unwrap(response);
};

// Đặt lại mật khẩu
export const resetPasswordRequest = async (payload) => {
  const response = await api.post('/auth/reset-password', payload);
  return unwrap(response);
};

// Xác nhận email
export const confirmEmailRequest = async (token) => {
  const response = await api.get(`/auth/confirm-email?token=${token}`);
  return unwrap(response);
};

// Lấy thông tin tài khoản hiện tại
export const getCurrentUserRequest = async () => {
  const response = await api.get('/auth/me');
  return unwrap(response);
};

// --- ALIASES ĐẦY ĐỦ ---
export const login = loginRequest;
export const register = registerRequest;
export const logout = logoutRequest;
export const forgotPassword = forgotPasswordRequest;
export const verifyResetToken = verifyResetTokenRequest;
export const resetPassword = resetPasswordRequest;
export const confirmEmail = confirmEmailRequest;
export const getCurrentUser = getCurrentUserRequest;