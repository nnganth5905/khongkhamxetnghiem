import api, { unwrap } from './api';

export const searchSite = async (query) =>
  unwrap(
    await api.get('/search', {
      params: {
        q: query,
      },
    })
  );