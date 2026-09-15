import api, { unwrap } from './api';

export const getPromotions = async (params = {}) =>
  unwrap(
    await api.get('/promotions', {
      params,
    })
  );

export const getPromotion = async (id) =>
  unwrap(
    await api.get(
      `/promotions/${id}`
    )
  );

export const getSocialPosts = async (params = {}) =>
  unwrap(
    await api.get('/social-posts', {
      params,
    })
  );

export const getSocialPost = async (id) =>
  unwrap(
    await api.get(
      `/social-posts/${id}`
    )
  );