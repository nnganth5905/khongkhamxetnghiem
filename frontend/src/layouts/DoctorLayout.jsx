import React, {
  useState,
} from 'react';

import {
  Outlet,
} from 'react-router-dom';

import Sidebar from '../components/Sidebar';

export default function DoctorLayout() {
  const [
    sidebarOpen,
    setSidebarOpen,
  ] = useState(false);

  return (
    <div className="dashboard-layout">
      <Sidebar
        role="DOCTOR"
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
                Bác sĩ
              </div>

              <div className="small text-secondary">
                Bio Medic Center
              </div>
            </div>
          </div>

          <span className="badge bg-primary">
            DOCTOR
          </span>
        </header>

        <main className="dashboard-content">
          <Outlet />
        </main>
      </div>
    </div>
  );
}