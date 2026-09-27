import React, {
  useState,
} from 'react';

import {
  Outlet,
} from 'react-router-dom';

import Sidebar from '../components/Sidebar';

const getUser = () => {
  try {
    const raw =
      localStorage.getItem(
        'user'
      );

    return raw
      ? JSON.parse(raw)
      : null;
  } catch {
    return null;
  }
};

export default function AdminLayout() {
  const [
    sidebarOpen,
    setSidebarOpen,
  ] = useState(false);

  const user =
    getUser();

  const name =
    user?.fullName ||
    user?.FullName ||
    user?.name ||
    user?.email ||
    'Admin';

  const avatar =
    `https://ui-avatars.com/api/?name=${encodeURIComponent(
      name
    )}&background=0B63E5&color=fff`;

  return (
    <div className="dashboard-layout">
      <Sidebar
        role="ADMIN"
        open={
          sidebarOpen
        }
        onClose={() =>
          setSidebarOpen(
            false
          )
        }
      />

      {sidebarOpen && (
        <button
          type="button"
          className="dashboard-backdrop"
          aria-label="Đóng menu"
          onClick={() =>
            setSidebarOpen(
              false
            )
          }
        />
      )}

      <div className="dashboard-main">
        <header className="dashboard-topbar">
          <div className="d-flex align-items-center gap-3">
            <button
              type="button"
              className="btn btn-outline-primary dashboard-mobile-menu"
              onClick={() =>
                setSidebarOpen(
                  true
                )
              }
            >
              <i className="fa-solid fa-bars" />
            </button>

            <div>
              <div className="fw-bold">
                Hệ thống quản trị
              </div>

              <div className="small text-secondary">
                Bio Medic Center
              </div>
            </div>
          </div>

          <div className="d-flex align-items-center gap-2">
            <img
              src={avatar}
              alt={name}
              className="dashboard-user-avatar"
            />

            <div className="d-none d-sm-block">
              <div className="small fw-semibold">
                {name}
              </div>

              <div className="text-secondary small">
                Quản trị viên
              </div>
            </div>
          </div>
        </header>

        <main className="dashboard-content">
          <Outlet />
        </main>
      </div>
    </div>
  );
}