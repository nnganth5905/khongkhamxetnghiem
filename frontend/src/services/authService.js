// src/services/authService.js

import api, { unwrap } from './api';

// =====================================================
// ERROR
// =====================================================

export const getApiErrorMessage = (
  error,
  defaultMessage = 'Có lỗi xảy ra khi xác thực tài khoản.'
) => {
  if (!error) {
    return defaultMessage;
  }

  if (typeof error === 'string') {
    return error;
  }

  const validationData =
    error?.response?.data?.data;

  if (
    validationData &&
    typeof validationData === 'object'
  ) {
    const firstValidationError =
      Object.values(validationData)[0];

    if (firstValidationError) {
      return String(firstValidationError);
    }
  }

  return (
    error?.response?.data?.message ||
    error?.response?.data?.error ||
    error?.message ||
    defaultMessage
  );
};

export const getAuthErrorMessage =
  getApiErrorMessage;

// =====================================================
// LOCAL SESSION
// =====================================================

export const clearAuthSession = () => {
  localStorage.removeItem(
    'biomedic_access_token'
  );

  localStorage.removeItem(
    'biomedic_user'
  );

  localStorage.removeItem(
    'accessToken'
  );

  localStorage.removeItem(
    'token'
  );

  localStorage.removeItem(
    'user'
  );

  localStorage.removeItem(
    'role'
  );

  localStorage.removeItem(
    'auth'
  );

  sessionStorage.removeItem(
    'biomedic_access_token'
  );

  sessionStorage.removeItem(
    'biomedic_user'
  );
};

export const clearSession =
  clearAuthSession;

export const clearAuth =
  clearAuthSession;

// =====================================================
// LOGIN
// =====================================================

export const loginRequest =
  async (payload) => {

    const response =
      await api.post(
        '/auth/login',
        payload
      );

    return unwrap(response);
  };

// =====================================================
// REGISTER
// =====================================================

export const registerRequest =
  async (payload) => {

    const response =
      await api.post(
        '/auth/register',
        payload
      );

    return unwrap(response);
  };

// =====================================================
// LOGOUT
// =====================================================

export const logoutRequest =
  async () => {

    try {
      const response =
        await api.post(
          '/auth/logout'
        );

      return unwrap(response);

    } finally {
      clearAuthSession();
    }
  };

// =====================================================
// FORGOT PASSWORD
// =====================================================

export const forgotPasswordRequest =
  async (input) => {

    /*
     * Cho phép gọi:
     *
     * forgotPassword("abc@gmail.com")
     *
     * hoặc:
     *
     * forgotPassword({
     *   email: "abc@gmail.com"
     * })
     */

    const payload =
      typeof input === 'string'
        ? {
            email: input
              .trim()
              .toLowerCase(),
          }
        : {
            ...input,
            email: String(
              input?.email || ''
            )
              .trim()
              .toLowerCase(),
          };

    const response =
      await api.post(
        '/auth/forgot-password',
        payload
      );

    return unwrap(response);
  };

// =====================================================
// VERIFY RESET TOKEN
// =====================================================

export const verifyResetTokenRequest =
  async (
    token,
    email
  ) => {

    const response =
      await api.get(
        '/auth/reset-password/verify',
        {
          params: {
            token,
            email,
          },
        }
      );

    return unwrap(response);
  };

// =====================================================
// RESET PASSWORD
// =====================================================

export const resetPasswordRequest =
  async (payload) => {

    const response =
      await api.post(
        '/auth/reset-password',
        {
          token: payload?.token,
          email: String(
            payload?.email || ''
          )
            .trim()
            .toLowerCase(),

          password:
            payload?.password,
        }
      );

    return unwrap(response);
  };

// =====================================================
// CONFIRM EMAIL
// =====================================================

export const confirmEmailRequest =
  async (token) => {

    const response =
      await api.get(
        '/auth/confirm-email',
        {
          params: {
            token,
          },
        }
      );

    return unwrap(response);
  };

// =====================================================
// CURRENT USER
// =====================================================

export const getCurrentUserRequest =
  async () => {

    const response =
      await api.get(
        '/auth/me'
      );

    return unwrap(response);
  };

// =====================================================
// ALIASES
// =====================================================

export const login =
  loginRequest;

export const register =
  registerRequest;

export const logout =
  logoutRequest;

export const forgotPassword =
  forgotPasswordRequest;

export const verifyResetToken =
  verifyResetTokenRequest;

export const resetPassword =
  resetPasswordRequest;

export const confirmEmail =
  confirmEmailRequest;

export const getCurrentUser =
  getCurrentUserRequest;