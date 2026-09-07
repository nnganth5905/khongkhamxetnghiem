import api, { unwrap } from './api';

// =========================
// CUSTOMER
// =========================

export const getCustomers = async (params = {}) =>
  unwrap(
    await api.get(
      '/admin/customers',
      {
        params,
      }
    )
  );

export const createCustomer = async (payload) =>
  unwrap(
    await api.post(
      '/admin/customers',
      payload
    )
  );

export const updateCustomer = async (id, payload) =>
  unwrap(
    await api.put(
      `/admin/customers/${id}`,
      payload
    )
  );

export const deleteCustomer = async (id) =>
  unwrap(
    await api.delete(
      `/admin/customers/${id}`
    )
  );

// =========================
// EMPLOYEE
// =========================

export const getEmployees = async (params = {}) =>
  unwrap(
    await api.get(
      '/admin/employees',
      {
        params,
      }
    )
  );

export const createEmployee = async (payload) =>
  unwrap(
    await api.post(
      '/admin/employees',
      payload
    )
  );

export const updateEmployee = async (id, payload) =>
  unwrap(
    await api.put(
      `/admin/employees/${id}`,
      payload
    )
  );

export const deleteEmployee = async (id) =>
  unwrap(
    await api.delete(
      `/admin/employees/${id}`
    )
  );

// =========================
// DOCTOR
// =========================

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

// =========================
// TECHNICIAN
// =========================

export const getTechnicians = async (params = {}) =>
  unwrap(
    await api.get(
      '/admin/technicians',
      {
        params,
      }
    )
  );

export const createTechnician = async (payload) =>
  unwrap(
    await api.post(
      '/admin/technicians',
      payload
    )
  );

export const updateTechnician = async (id, payload) =>
  unwrap(
    await api.put(
      `/admin/technicians/${id}`,
      payload
    )
  );

export const deleteTechnician = async (id) =>
  unwrap(
    await api.delete(
      `/admin/technicians/${id}`
    )
  );

// =========================
// TEST CATALOG
// =========================

export const getAdminTests = async (params = {}) =>
  unwrap(
    await api.get(
      '/admin/tests',
      {
        params,
      }
    )
  );

export const createAdminTest = async (payload) =>
  unwrap(
    await api.post(
      '/admin/tests',
      payload
    )
  );

export const updateAdminTest = async (id, payload) =>
  unwrap(
    await api.put(
      `/admin/tests/${id}`,
      payload
    )
  );

export const deleteAdminTest = async (id) =>
  unwrap(
    await api.delete(
      `/admin/tests/${id}`
    )
  );

// =========================
// SPECIALTY
// =========================

export const getSpecialties = async (params = {}) =>
  unwrap(
    await api.get(
      '/admin/specialties',
      {
        params,
      }
    )
  );

export const createSpecialty = async (payload) =>
  unwrap(
    await api.post(
      '/admin/specialties',
      payload
    )
  );

export const updateSpecialty = async (id, payload) =>
  unwrap(
    await api.put(
      `/admin/specialties/${id}`,
      payload
    )
  );

export const deleteSpecialty = async (id) =>
  unwrap(
    await api.delete(
      `/admin/specialties/${id}`
    )
  );

// =========================
// ROOM
// =========================

export const getRooms = async (params = {}) =>
  unwrap(
    await api.get(
      '/admin/rooms',
      {
        params,
      }
    )
  );

export const createRoom = async (payload) =>
  unwrap(
    await api.post(
      '/admin/rooms',
      payload
    )
  );

export const updateRoom = async (id, payload) =>
  unwrap(
    await api.put(
      `/admin/rooms/${id}`,
      payload
    )
  );

export const deleteRoom = async (id) =>
  unwrap(
    await api.delete(
      `/admin/rooms/${id}`
    )
  );

// =========================
// WORK SCHEDULE
// =========================

export const getWorkSchedules = async (params = {}) =>
  unwrap(
    await api.get(
      '/admin/work-schedules',
      {
        params,
      }
    )
  );

export const createWorkSchedule = async (payload) =>
  unwrap(
    await api.post(
      '/admin/work-schedules',
      payload
    )
  );

export const updateWorkSchedule = async (id, payload) =>
  unwrap(
    await api.put(
      `/admin/work-schedules/${id}`,
      payload
    )
  );

export const deleteWorkSchedule = async (id) =>
  unwrap(
    await api.delete(
      `/admin/work-schedules/${id}`
    )
  );

// =========================
// DASHBOARD / REPORT
// =========================

export const getDashboardStats = async (params = {}) =>
  unwrap(
    await api.get(
      '/admin/dashboard',
      {
        params,
      }
    )
  );