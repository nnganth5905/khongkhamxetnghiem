// src/layouts/MainLayout.jsx
import React from 'react';
import { Outlet } from 'react-router-dom';
import Header from '../components/Header';
import Navbar from '../components/Navbar';
import Footer from '../components/Footer';

export default function MainLayout() {
  return (
    <div className="bmc-site-wrapper d-flex flex-column min-vh-100">
      {/* 1. Header trên cùng */}
      <Header />

      {/* 2. Menu điều hướng */}
      <Navbar />

      {/* 3. Vùng hiển thị nội dung các trang con (Home, Đặt lịch, Dịch vụ...) */}
      <main className="bmc-main flex-grow-1">
        <Outlet />
      </main>

      {/* 4. Chân trang Footer */}
      <Footer />
    </div>
  );
}