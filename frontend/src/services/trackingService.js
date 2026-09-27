import api, { unwrap } from './api';

export const getVisitTracking = async (code) =>
  unwrap(
    await api.get(
      `/tracking/visits/${encodeURIComponent(code)}`
    )
  );

export const getTestTracking = async (code) =>
  unwrap(
    await api.get(
      `/tracking/tests/${encodeURIComponent(code)}`
    )
  );

export const getTrackingHistory = async (params = {}) =>
  unwrap(
    await api.get('/tracking/history', {
      params,
    })
  );