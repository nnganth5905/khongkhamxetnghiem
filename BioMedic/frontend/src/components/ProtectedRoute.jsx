import React from 'react';
import {
  Navigate,
  Outlet,
  useLocation,
} from 'react-router-dom';

import Loading from './Loading';

import {
  getRoleHome,
  getUserRole,
  normalizeRole,
  useAuth,
} from '../context/AuthContext';

export default function ProtectedRoute({
  allowedRoles = null,
}) {
  const location = useLocation();

  const {
    user,
    loading,
    isAuthenticated,
  } = useAuth();

  if (loading) {
    return <Loading text="Đang kiểm tra đăng nhập..." />;
  }

  if (!isAuthenticated) {
    return (
      <Navigate
        to="/login"
        replace
        state={{
          from: location.pathname + location.search,
        }}
      />
    );
  }

  if (
    Array.isArray(allowedRoles) &&
    allowedRoles.length > 0
  ) {
    const currentRole = getUserRole(user);

    const accepted = allowedRoles
      .map(normalizeRole)
      .includes(currentRole);

    if (!accepted) {
      return (
        <Navigate
          to={getRoleHome(currentRole)}
          replace
        />
      );
    }
  }

  return <Outlet />;
}