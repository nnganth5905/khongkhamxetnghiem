export const getDoctors = async (params = {}) =>
  unwrap(
    await api.get(
      '/admin/doctors',
      {
        params,
      }
    )
  );

export const createDoctor = async (payload) =>
  unwrap(
    await api.post(
      '/admin/doctors',
      payload
    )
  );

export const updateDoctor = async (id, payload) =>
  unwrap(
    await api.put(
      `/admin/doctors/${id}`,
      payload
    )
  );

export const deleteDoctor = async (id) =>
  unwrap(
    await api.delete(
      `/admin/doctors/${id}`
    )
  );