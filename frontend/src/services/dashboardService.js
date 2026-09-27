import api, { unwrap } from './api';

export const getAdminDashboard = async () => {
  const data = unwrap(await api.get('/admin/dashboard')) || {};

  return {
    totalCustomers: data.customersCount ?? 0,
    totalEmployees: data.employeesCount ?? 0,
    totalDoctors: data.doctorsCount ?? 0,
    totalTechnicians: data.techniciansCount ?? 0,
    totalTestOrders: data.testOrdersCount ?? 0,
    totalResults: data.resultsCount ?? 0,
    todayAppointments: data.todayAppointments ?? 0,
    pendingResults: data.pendingResults ?? 0,
    revenueToday: data.revenue ?? 0,
    waitingSpecimens: data.waitingSpecimens ?? 0,
    recentActivities: data.recentActivities ?? [],
  };
};

export const getDoctorDashboard = async () =>
  unwrap(await api.get('/dashboard/doctor'));

export const getCustomerDashboard = async () =>
  unwrap(await api.get('/dashboard/customer'));

export const getReceptionistDashboard = async () =>
  unwrap(await api.get('/dashboard/receptionist'));

export const getTechnicianDashboard = async () =>
  unwrap(await api.get('/dashboard/technician'));