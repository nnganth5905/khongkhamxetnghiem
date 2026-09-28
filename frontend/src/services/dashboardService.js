// src/services/dashboardService.js

import api, {
  unwrap,
} from './api';

// ============================================================
// ERROR
// ============================================================

export const getDashboardErrorMessage = (
  error,
  fallback =
    'Không thể tải dữ liệu Dashboard.'
) => {
  return (
    error?.response?.data?.message ||
    error?.response?.data?.error ||
    error?.message ||
    fallback
  );
};

// ============================================================
// RECEPTIONIST
// ============================================================

export const getReceptionistDashboard =
  async () => {
    const response =
      await api.get(
        '/dashboard/receptionist'
      );

    return unwrap(
      response
    );
  };

// ============================================================
// DOCTOR
// ============================================================

export const getDoctorDashboard =
  async () => {
    const response =
      await api.get(
        '/dashboard/doctor'
      );

    return unwrap(
      response
    );
  };

// ============================================================
// TECHNICIAN
// ============================================================

export const getTechnicianDashboard =
  async () => {
    const response =
      await api.get(
        '/dashboard/technician'
      );

    return unwrap(
      response
    );
  };

// ============================================================
// ADMIN
// ============================================================

export const getAdminDashboard =
  async () => {
    const response =
      await api.get(
        '/dashboard/admin'
      );

    return unwrap(
      response
    );
  };

// ============================================================
// CUSTOMER
// ============================================================

export const getCustomerDashboard =
  async () => {
    const response =
      await api.get(
        '/dashboard/customer'
      );

    return unwrap(
      response
    );
  };

// ============================================================
// ALIASES
// ============================================================

export const loadReceptionistDashboard =
  getReceptionistDashboard;

export const loadDoctorDashboard =
  getDoctorDashboard;

export const loadTechnicianDashboard =
  getTechnicianDashboard;

export const loadAdminDashboard =
  getAdminDashboard;

export const loadCustomerDashboard =
  getCustomerDashboard;

// ============================================================
// DEFAULT
// ============================================================

const dashboardService = {
  getReceptionistDashboard,
  getDoctorDashboard,
  getTechnicianDashboard,
  getAdminDashboard,
  getCustomerDashboard,

  loadReceptionistDashboard,
  loadDoctorDashboard,
  loadTechnicianDashboard,
  loadAdminDashboard,
  loadCustomerDashboard,

  getDashboardErrorMessage,
};

export default dashboardService;