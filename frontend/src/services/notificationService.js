import api from './api';

export const getNotifications = async () => {
  const response = await api.get('/notifications');
  // Trả về trực tiếp mảng dữ liệu
  return response.data;
};

export const markNotificationRead = async (id) => {
  const response = await api.patch(`/notifications/${id}/read`);
  return response.data;
};

export const markAllNotificationsRead = async () => {
  const response = await api.post('/notifications/read-all');
  return response.data;
};