import api, { unwrap } from './api';

export const getNotifications = async (params = {}) =>
  unwrap(
    await api.get('/notifications', {
      params,
    })
  );

export const markNotificationRead = async (id) =>
  unwrap(
    await api.patch(
      `/notifications/${id}/read`
    )
  );

export const markAllNotificationsRead = async () =>
  unwrap(
    await api.patch(
      '/notifications/read-all'
    )
  );

src/services/testService.js

import api, { unwrap } from './api';

export const getTests = async (params = {}) =>
  unwrap(
    await api.get('/tests', {
      params,
    })
  );

export const getTest = async (id) =>
  unwrap(
    await api.get(`/tests/${id}`)
  );

export const getTestCategory = async (
  slug,
  params = {}
) =>
  unwrap(
    await api.get(
      `/tests/category/${slug}`,
      {
        params,
      }
    )
  );

export const getMyResults = async (params = {}) =>
  unwrap(
    await api.get('/results/mine', {
      params,
    })
  );

export const getResult = async (id) =>
  unwrap(
    await api.get(`/results/${id}`)
  );

export const searchResultPublic = async (payload) =>
  unwrap(
    await api.post(
      '/results/lookup',
      payload
    )
  );