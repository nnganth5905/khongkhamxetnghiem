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

const AuthContext = createContext(null);

const TOKEN_KEY = 'biomedic_access_token';
const USER_KEY = 'biomedic_user';

export const normalizeRole = (role) => {
  const raw = String(role || '')
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

  return roleMap[raw] || String(role || '').toUpperCase();
};

export const getUserRole = (user) => {
  if (!user) return '';

  return normalizeRole(
    user.role ??
      user.Role ??
      user.roleName ??
      user.RoleName ??
      user.tenVaiTro ??
      user.TenVaiTro ??
      user.vaiTro ??
      user.VaiTro
  );
};

export const getRoleHome = (role) => {
  switch (normalizeRole(role)) {
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

const readStoredUser = () => {
  try {
    const raw = localStorage.getItem(USER_KEY);

    if (!raw) return null;

    const parsed = JSON.parse(raw);

    return {
      ...parsed,
      role: getUserRole(parsed),
    };
  } catch {
    return null;
  }
};

const persistUser = (user) => {
  if (!user) {
    localStorage.removeItem(USER_KEY);
    return;
  }

  localStorage.setItem(
    USER_KEY,
    JSON.stringify(user)
  );
};

export function AuthProvider({ children }) {
  const [user, setUser] = useState(() => readStoredUser());
  const [loading, setLoading] = useState(true);

  const saveAuth = useCallback((data) => {
    const token =
      data?.accessToken ??
      data?.token ??
      data?.jwt ??
      data?.access_token ??
      null;

    if (token) {
      localStorage.setItem(TOKEN_KEY, token);
    }

    const sourceUser =
      data?.user ??
      data?.account ??
      data?.profile ??
      data;

    const normalizedUser = {
      ...sourceUser,
      role: normalizeRole(
        sourceUser?.role ??
          sourceUser?.Role ??
          data?.role ??
          data?.Role
      ),
    };

    setUser(normalizedUser);
    persistUser(normalizedUser);

    return {
      ...data,
      user: normalizedUser,
      accessToken: token,
    };
  }, []);

  const clearAuth = useCallback(() => {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    setUser(null);
  }, []);

  const refreshUser = useCallback(async () => {
    const token = localStorage.getItem(TOKEN_KEY);
    const storedUser = readStoredUser();

    if (!token && !storedUser) {
      setUser(null);
      setLoading(false);
      return null;
    }

    if (storedUser) {
      setUser(storedUser);
    }

    try {
      const data = await getCurrentUserRequest();

      if (data) {
        const normalizedUser = {
          ...data,
          role: getUserRole(data),
        };

        setUser(normalizedUser);
        persistUser(normalizedUser);

        return normalizedUser;
      }

      return storedUser;
    } catch (error) {
      const status = error?.response?.status;

      if (status === 401 || status === 403) {
        clearAuth();
        return null;
      }

      return storedUser;
    } finally {
      setLoading(false);
    }
  }, [clearAuth]);

  useEffect(() => {
    refreshUser();
  }, [refreshUser]);

  const login = useCallback(
    async (credentials) => {
      const data = await loginRequest(credentials);
      return saveAuth(data);
    },
    [saveAuth]
  );

  const logout = useCallback(async () => {
    try {
      await logoutRequest();
    } catch {
      // Nếu backend logout chưa sẵn sàng, vẫn logout phía frontend.
    } finally {
      clearAuth();
    }
  }, [clearAuth]);

  const hasRole = useCallback(
    (...roles) => {
      if (!user) return false;

      const currentRole = getUserRole(user);

      return roles
        .flat()
        .map(normalizeRole)
        .includes(currentRole);
    },
    [user]
  );

  const value = useMemo(
    () => ({
      user,
      loading,
      isAuthenticated: Boolean(user),
      role: getUserRole(user),
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
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  );
}

export const useAuth = () => {
  const context = useContext(AuthContext);

  if (!context) {
    throw new Error(
      'useAuth phải được sử dụng bên trong <AuthProvider>.'
    );
  }

  return context;
};

export default AuthContext;