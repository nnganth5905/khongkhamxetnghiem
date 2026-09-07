import api, { unwrap } from './api';

export const sendContactMessage = async (payload) =>
  unwrap(
    await api.post(
      '/contact',
      payload
    )
  );