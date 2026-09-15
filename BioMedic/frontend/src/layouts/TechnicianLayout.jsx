import React, {
  useState,
} from 'react';

import {
  Outlet,
} from 'react-router-dom';

import Sidebar from '../components/Sidebar';

export default function TechnicianLayout() {
  const [
    sidebarOpen,
    setSidebarOpen,
  ] = useState(false);

  return (
    <div className="dashboard-layout">
      <Sidebar
        role="TECHNICIAN"
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
                Kỹ thuật viên
              </div>

              <div className="small text-secondary">
                Bio Medic Center
              </div>
            </div>
          </div>

          <span className="badge bg-primary">
            TECHNICIAN
          </span>
        </header>

        <main className="dashboard-content">
          <Outlet />
        </main>
      </div>
    </div>
  );
}