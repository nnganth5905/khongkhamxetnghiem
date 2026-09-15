import api, { unwrap } from './api';

export const loginRequest = async (credentials) => {
  const response = await api.post('/auth/login', credentials);
  return unwrap(response);
};

export const getCurrentUserRequest = async () => {
  const response = await api.get('/auth/me');
  return unwrap(response);
};

export const logoutRequest = async () => {
  const response = await api.post('/auth/logout');
  return unwrap(response);
};