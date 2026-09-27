import api, { unwrap } from './api';

export const getAdminDashboard = async () =>
  unwrap(await api.get('/dashboard/admin'));

export const getDoctorDashboard = async () =>
  unwrap(await api.get('/dashboard/doctor'));

export const getCustomerDashboard = async () =>
  unwrap(await api.get('/dashboard/customer'));

export const getReceptionistDashboard = async () =>
  unwrap(await api.get('/dashboard/receptionist'));

export const getTechnicianDashboard = async () =>
  unwrap(await api.get('/dashboard/technician'));