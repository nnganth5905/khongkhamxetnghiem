import api, { unwrap } from './api';

/**
 * Lấy danh sách thông báo của người dùng hiện tại.
 */
export async function getNotifications(params = {}) {
  const response = await api.get('/notifications', {
    params,
  });

  return unwrap(response);
}

/**
 * Đánh dấu một thông báo là đã đọc.
 */
export async function markNotificationRead(id) {
  const response = await api.patch(
    `/notifications/${id}/read`,
  );

  return unwrap(response);
}

/**
 * Đánh dấu toàn bộ thông báo là đã đọc.
 */
export async function markAllNotificationsRead() {
  const response = await api.patch(
    '/notifications/read-all',
  );

  return unwrap(response);
}


/*
 * =========================================================
 * ALIAS
 * Giữ tương thích với các page/component đã viết trước đó.
 * =========================================================
 */

export const getMyNotifications =
  getNotifications;

export const markAsRead =
  markNotificationRead;

export const markAllAsRead =
  markAllNotificationsRead;


/**
 * Default export
 */
const notificationService = {
  getNotifications,
  markNotificationRead,
  markAllNotificationsRead,

  getMyNotifications,
  markAsRead,
  markAllAsRead,
};

export default notificationService;