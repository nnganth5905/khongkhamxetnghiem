// src/context/AuthContext.jsx

import React, {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
} from 'react';

import {
  getCurrentUserRequest,
  loginRequest,
  logoutRequest,
} from '../services/authService';

const AuthContext =
  createContext(null);

const TOKEN_KEY =
  'biomedic_access_token';

const USER_KEY =
  'biomedic_user';

// =====================================================
// ROLE
// =====================================================

export const normalizeRole =
  (role) => {

    const raw =
      String(role || '')
        .trim()
        .toLowerCase();

    const roleMap = {
      admin: 'ADMIN',

      bacsi: 'DOCTOR',
      doctor: 'DOCTOR',
      bs: 'DOCTOR',

      khachhang: 'CUSTOMER',
      customer: 'CUSTOMER',
      patient: 'CUSTOMER',

      letan: 'RECEPTIONIST',
      tieptan: 'RECEPTIONIST',
      receptionist: 'RECEPTIONIST',
      reception: 'RECEPTIONIST',

      ktv: 'TECHNICIAN',
      technician: 'TECHNICIAN',
      kythuatvien: 'TECHNICIAN',
    };

    return (
      roleMap[raw] ||
      String(role || '')
        .trim()
        .toUpperCase()
    );
  };

export const getUserRole =
  (user) => {

    if (!user) {
      return '';
    }

    return normalizeRole(
      user.role ??
        user.Role ??
        user.roleName ??
        user.RoleName ??
        user.tenVaiTro ??
        user.TenVaiTro ??
        user.vaiTro ??
        user.VaiTro ??
        user.user?.Role ??
        user.user?.role ??
        user.data?.Role ??
        user.data?.role
    );
  };

export const getRoleHome =
  (role) => {

    switch (
      normalizeRole(role)
    ) {
      case 'ADMIN':
        return '/admin';

      case 'DOCTOR':
        return '/doctor';

      case 'RECEPTIONIST':
        return '/reception';

      case 'TECHNICIAN':
        return '/technician';

      case 'CUSTOMER':
        return '/customer';

      default:
        return '/';
    }
  };

// =====================================================
// STORAGE
// =====================================================

const readStoredUser =
  () => {

    try {
      const raw =
        localStorage.getItem(
          USER_KEY
        ) ||
        localStorage.getItem(
          'user'
        );

      if (!raw) {
        return null;
      }

      const parsed =
        JSON.parse(raw);

      if (
        !parsed ||
        parsed.authenticated === false
      ) {
        return null;
      }

      return {
        ...parsed,
        role:
          getUserRole(parsed),
      };

    } catch {
      return null;
    }
  };

const persistUser =
  (user) => {

    if (
      !user ||
      user.authenticated === false
    ) {
      localStorage.removeItem(
        USER_KEY
      );

      localStorage.removeItem(
        'user'
      );

      localStorage.removeItem(
        'role'
      );

      return;
    }

    localStorage.setItem(
      USER_KEY,
      JSON.stringify(user)
    );

    localStorage.setItem(
      'user',
      JSON.stringify(user)
    );

    localStorage.setItem(
      'role',
      getUserRole(user)
    );
  };

// =====================================================
// PROVIDER
// =====================================================

export function AuthProvider({
  children,
}) {
  const [
    user,
    setUser,
  ] = useState(
    () => readStoredUser()
  );

  const [
    loading,
    setLoading,
  ] = useState(true);

  // ===================================================
  // CLEAR AUTH
  // ===================================================

  const clearAuth =
    useCallback(() => {

      localStorage.removeItem(
        TOKEN_KEY
      );

      localStorage.removeItem(
        USER_KEY
      );

      localStorage.removeItem(
        'user'
      );

      localStorage.removeItem(
        'token'
      );

      localStorage.removeItem(
        'role'
      );

      localStorage.removeItem(
        'accessToken'
      );

      setUser(null);

      window.dispatchEvent(
        new Event(
          'auth-change'
        )
      );
    }, []);

  // ===================================================
  // SAVE AUTH
  // ===================================================

  const saveAuth =
    useCallback(
      (data) => {

        if (
          !data ||
          data.authenticated === false
        ) {
          clearAuth();
          return null;
        }

        const token =
          data?.accessToken ??
          data?.token ??
          data?.jwt ??
          data?.access_token ??
          data?.data?.accessToken ??
          data?.data?.token ??
          null;

        if (token) {
          localStorage.setItem(
            TOKEN_KEY,
            token
          );

          localStorage.setItem(
            'token',
            token
          );

          localStorage.setItem(
            'accessToken',
            token
          );
        }

        const sourceUser =
          data?.user ??
          data?.account ??
          data?.profile ??
          data?.data?.user ??
          data?.data ??
          data;

        if (
          !sourceUser ||
          sourceUser.authenticated === false
        ) {
          clearAuth();
          return null;
        }

        const currentRole =
          getUserRole(
            sourceUser
          ) ||
          getUserRole(data);

        const normalizedUser = {
          ...sourceUser,
          role: currentRole,
        };

        setUser(
          normalizedUser
        );

        persistUser(
          normalizedUser
        );

        window.dispatchEvent(
          new Event(
            'auth-change'
          )
        );

        return {
          ...data,

          user:
            normalizedUser,

          accessToken:
            token,

          role:
            currentRole,
        };
      },
      [clearAuth]
    );

  // ===================================================
  // REFRESH USER
  // ===================================================

  const refreshUser =
    useCallback(
      async () => {

        const token =
          localStorage.getItem(
            TOKEN_KEY
          ) ||
          localStorage.getItem(
            'token'
          );

        if (!token) {
          clearAuth();

          setLoading(false);

          return null;
        }

        try {
          const data =
            await getCurrentUserRequest();

          if (
            !data ||
            data.authenticated === false
          ) {
            clearAuth();
            return null;
          }

          const sourceUser =
            data?.user ??
            data;

          const normalizedUser = {
            ...sourceUser,

            role:
              getUserRole(
                sourceUser
              ),
          };

          setUser(
            normalizedUser
          );

          persistUser(
            normalizedUser
          );

          return normalizedUser;

        } catch {
          clearAuth();
          return null;

        } finally {
          setLoading(false);
        }
      },
      [clearAuth]
    );

  // ===================================================
  // INIT
  // ===================================================

  useEffect(() => {
    refreshUser();
  }, [refreshUser]);

  // ===================================================
  // AUTH CHANGE EVENT
  // ===================================================

  useEffect(() => {

    const handleAuthChange =
      () => {

        const token =
          localStorage.getItem(
            TOKEN_KEY
          );

        if (!token) {
          setUser(null);
        }
      };

    window.addEventListener(
      'auth-change',
      handleAuthChange
    );

    return () => {
      window.removeEventListener(
        'auth-change',
        handleAuthChange
      );
    };
  }, []);

  // ===================================================
  // LOGIN
  // ===================================================

  const login =
    useCallback(
      async (
        credentials
      ) => {

        const data =
          await loginRequest(
            credentials
          );

        return saveAuth(
          data
        );
      },
      [saveAuth]
    );

  // ===================================================
  // LOGOUT
  // ===================================================

  const logout =
    useCallback(
      async () => {

        try {
          await logoutRequest();

        } finally {
          clearAuth();
        }
      },
      [clearAuth]
    );

  // ===================================================
  // ROLE CHECK
  // ===================================================

  const hasRole =
    useCallback(
      (...roles) => {

        if (!user) {
          return false;
        }

        const currentRole =
          getUserRole(user);

        return roles
          .flat()
          .map(
            normalizeRole
          )
          .includes(
            currentRole
          );
      },
      [user]
    );

  // ===================================================
  // VALUE
  // ===================================================

  const value =
    useMemo(
      () => ({
        user,

        loading,

        isAuthenticated:
          Boolean(user),

        role:
          getUserRole(user),

        login,

        logout,

        refreshUser,

        hasRole,

        setUser,
      }),

      [
        user,
        loading,
        login,
        logout,
        refreshUser,
        hasRole,
      ]
    );

  return (
    <AuthContext.Provider
      value={value}
    >
      {children}
    </AuthContext.Provider>
  );
}

// =====================================================
// HOOK
// =====================================================

export const useAuth =
  () => {

    const context =
      useContext(
        AuthContext
      );

    if (!context) {
      throw new Error(
        'useAuth phải được sử dụng bên trong <AuthProvider>.'
      );
    }

    return context;
  };

export default AuthContext;