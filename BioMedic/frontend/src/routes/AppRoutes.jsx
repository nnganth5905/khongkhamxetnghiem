import React from 'react';
import {
  Navigate,
  Route,
  Routes,
} from 'react-router-dom';

import ProtectedRoute from '../components/ProtectedRoute';

import MainLayout from '../layouts/MainLayout';
import AdminLayout from '../layouts/AdminLayout';
import DoctorLayout from '../layouts/DoctorLayout';
import CustomerLayout from '../layouts/CustomerLayout';
import ReceptionistLayout from '../layouts/ReceptionistLayout';
import TechnicianLayout from '../layouts/TechnicianLayout';

import {
  getRoleHome,
  useAuth,
} from '../context/AuthContext';

// HOME
import Home from '../pages/home/Home';
import HomeAlt from '../pages/home/HomeAlt';

// ABOUT
import About1 from '../pages/about/About1';
import About2 from '../pages/about/About2';

// AUTH
import Login from '../pages/auth/Login';
import Register from '../pages/auth/Register';
import ForgotPassword from '../pages/auth/ForgotPassword';
import ResetPassword from '../pages/auth/ResetPassword';
import Confirm from '../pages/auth/Confirm';

// APPOINTMENT
import Book from '../pages/appointment/Book';
import DatLich from '../pages/appointment/DatLich';
import DatLichKham from '../pages/appointment/DatLichKham';
import DatLichXetNghiem from '../pages/appointment/DatLichXetNghiem';
import LichHen from '../pages/appointment/LichHen';
import LichHenChiTiet from '../pages/appointment/LichHenChiTiet';
import LichHenSua from '../pages/appointment/LichHenSua';

// DOCTOR
import BacSiTQ from '../pages/doctor/BacSiTQ';
import GioLamViec from '../pages/doctor/GioLamViec';
import DoctorDanhSachCho from '../pages/doctor/DanhSachCho';
import KhamBenh from '../pages/doctor/KhamBenh';
import ChiDinhXetNghiem from '../pages/doctor/ChiDinhXetNghiem';
import LayMau from '../pages/doctor/LayMau';
import BanGiaoMau from '../pages/doctor/BanGiaoMau';
import DuyetKetQua from '../pages/doctor/DuyetKetQua';
import DocKetQua from '../pages/doctor/DocKetQua';

// TECHNICIAN
import DanhSachMau from '../pages/technician/DanhSachMau';
import TiepNhanMau from '../pages/technician/TiepNhanMau';
import ThucHienXetNghiem from '../pages/technician/ThucHienXetNghiem';
import NhapKetQua from '../pages/technician/NhapKetQua';

// RECEPTION
import CheckIn from '../pages/reception/CheckIn';
import CheckInQR from '../pages/reception/CheckInQR';
import ReceptionDanhSachCho from '../pages/reception/DanhSachCho';
import QuanLyTiepNhan from '../pages/reception/QuanLyTiepNhan';

// TRACKING
import TheoDoiLuotKham from '../pages/tracking/TheoDoiLuotKham';
import TheoDoiXetNghiem from '../pages/tracking/TheoDoiXetNghiem';
import LichSuTruyVet from '../pages/tracking/LichSuTruyVet';

// NOTIFICATION
import ThongBao from '../pages/notification/ThongBao';

// TEST / RESULT
import KetQua from '../pages/test/KetQua';
import ChiTietKetQua from '../pages/test/ChiTietKetQua';
import ChiTietXetNghiemKDN from '../pages/test/ChiTietXetNghiemKDN';
import TraCuuXnoKDN from '../pages/test/TraCuuXnoKDN';
import TestFinal from '../pages/test/TestFinal';

// TEST CATEGORIES
import BloodClot from '../pages/testCategories/BloodClot';
import Hematology from '../pages/testCategories/Hematology';
import HighUp from '../pages/testCategories/HighUp';
import Hormones from '../pages/testCategories/Hormones';
import Kidney from '../pages/testCategories/Kidney';
import Lipid from '../pages/testCategories/Lipid';
import Tumor from '../pages/testCategories/Tumor';
import Urine from '../pages/testCategories/Urine';
import Virus from '../pages/testCategories/Virus';

// MEDICAL
import Medical from '../pages/medical/Medical';
import MedicalArticle from '../pages/medical/MedicalArticle';
import Handbook from '../pages/medical/Handbook';
import QuyTrinh from '../pages/medical/QuyTrinh';

// NEWS
import NewsADN from '../pages/news/NewsADN';
import NewsCovid from '../pages/news/NewsCovid';
import NewsTimMach from '../pages/news/NewsTimMach';
import NewsVoucher from '../pages/news/NewsVoucher';
import NewsXetNghiem from '../pages/news/NewsXetNghiem';

// PROMOTION
import Promotions from '../pages/promotion/Promotions';
import PromotionDetail from '../pages/promotion/PromotionDetail';

// SERVICES
import Services from '../pages/services/Services';

// SOCIAL
import Social from '../pages/social/Social';
import SocialArticle from '../pages/social/SocialArticle';

// SPECIALIST
import GoiYChuyenKhoa from '../pages/specialist/GoiYChuyenKhoa';

// SEARCH / CONTACT
import TimKiem from '../pages/search/TimKiem';
import LienHe from '../pages/contact/LienHe';

// ADMIN
import QuanLyKhachHang from '../pages/admin/QuanLyKhachHang';
import QuanLyNhanVien from '../pages/admin/QuanLyNhanVien';
import QuanLyBacSi from '../pages/admin/QuanLyBacSi';
import QuanLyKTV from '../pages/admin/QuanLyKTV';
import QuanLyXetNghiem from '../pages/admin/QuanLyXetNghiem';
import QuanLyChuyenKhoa from '../pages/admin/QuanLyChuyenKhoa';
import QuanLyPhong from '../pages/admin/QuanLyPhong';
import QuanLyLichLamViec from '../pages/admin/QuanLyLichLamViec';
import BaoCaoThongKe from '../pages/admin/BaoCaoThongKe';

// DASHBOARDS
import AdminDashboard from '../pages/dashboard/AdminDashboard';
import DoctorDashboard from '../pages/dashboard/DoctorDashboard';
import CustomerDashboard from '../pages/dashboard/CustomerDashboard';
import ReceptionistDashboard from '../pages/dashboard/ReceptionistDashboard';
import TechnicianDashboard from '../pages/dashboard/TechnicianDashboard';

const ALL_ROLES = [
  'ADMIN',
  'DOCTOR',
  'CUSTOMER',
  'RECEPTIONIST',
  'TECHNICIAN',
];

function DashboardRedirect() {
  const {
    role,
    loading,
    isAuthenticated,
  } = useAuth();

  if (loading) {
    return null;
  }

  if (!isAuthenticated) {
    return (
      <Navigate
        to="/login"
        replace
      />
    );
  }

  return (
    <Navigate
      to={getRoleHome(role)}
      replace
    />
  );
}

export default function AppRoutes() {
  return (
    <Routes>
      {/* =====================================================
          PUBLIC WEBSITE
      ====================================================== */}
      <Route element={<MainLayout />}>
        <Route
          path="/"
          element={<Home />}
        />

        <Route
          path="/home-alt"
          element={<HomeAlt />}
        />

        {/* ABOUT */}
        <Route
          path="/about/history"
          element={<About1 />}
        />

        <Route
          path="/about/facilities"
          element={<About2 />}
        />

        <Route
          path="/about1"
          element={<About1 />}
        />

        <Route
          path="/about2"
          element={<About2 />}
        />

        {/* AUTH */}
        <Route
          path="/login"
          element={<Login />}
        />

        <Route
          path="/register"
          element={<Register />}
        />

        <Route
          path="/forgot-password"
          element={<ForgotPassword />}
        />

        <Route
          path="/forgot_password"
          element={
            <Navigate
              to="/forgot-password"
              replace
            />
          }
        />

        <Route
          path="/reset-password"
          element={<ResetPassword />}
        />

        <Route
          path="/confirm-email"
          element={<Confirm />}
        />

        {/* APPOINTMENT */}
        <Route
          path="/book"
          element={<Book />}
        />

        <Route
          path="/dat-lich"
          element={<DatLich />}
        />

        <Route
          path="/dat-lich-kham"
          element={<DatLichKham />}
        />

        <Route
          path="/dat-lich-xet-nghiem"
          element={<DatLichXetNghiem />}
        />

        {/* DOCTOR PUBLIC */}
        <Route
          path="/doi-ngu-bac-si"
          element={<BacSiTQ />}
        />

        <Route
          path="/gio-lam-viec"
          element={<GioLamViec />}
        />

        {/* SERVICES */}
        <Route
          path="/dich-vu"
          element={<Services />}
        />

        <Route
          path="/services"
          element={
            <Navigate
              to="/dich-vu"
              replace
            />
          }
        />

        <Route
          path="/danh-muc-xet-nghiem"
          element={<TestFinal />}
        />

        <Route
          path="/xet-nghiem/:testId"
          element={<ChiTietXetNghiemKDN />}
        />

        {/* TEST CATEGORIES */}
        <Route
          path="/xet-nghiem/dong-mau"
          element={<BloodClot />}
        />

        <Route
          path="/xet-nghiem/huyet-hoc"
          element={<Hematology />}
        />

        <Route
          path="/xet-nghiem/cao-cap"
          element={<HighUp />}
        />

        <Route
          path="/xet-nghiem/hormone"
          element={<Hormones />}
        />

        <Route
          path="/xet-nghiem/than"
          element={<Kidney />}
        />

        <Route
          path="/xet-nghiem/lipid"
          element={<Lipid />}
        />

        <Route
          path="/xet-nghiem/dau-an-ung-thu"
          element={<Tumor />}
        />

        <Route
          path="/xet-nghiem/nuoc-tieu"
          element={<Urine />}
        />

        <Route
          path="/xet-nghiem/virus-vi-khuan"
          element={<Virus />}
        />

        {/* MEDICAL */}
        <Route
          path="/kien-thuc-y-khoa"
          element={<Medical />}
        />

        <Route
          path="/kien-thuc-y-khoa/:id"
          element={<MedicalArticle />}
        />

        <Route
          path="/cam-nang-benh-hoc"
          element={<Handbook />}
        />

        <Route
          path="/quy-trinh"
          element={<QuyTrinh />}
        />

        {/* NEWS */}
        <Route
          path="/tin-tuc/adn"
          element={<NewsADN />}
        />

        <Route
          path="/tin-tuc/covid"
          element={<NewsCovid />}
        />

        <Route
          path="/tin-tuc/tim-mach"
          element={<NewsTimMach />}
        />

        <Route
          path="/tin-tuc/voucher"
          element={<NewsVoucher />}
        />

        <Route
          path="/tin-tuc/xet-nghiem"
          element={<NewsXetNghiem />}
        />

        {/* PROMOTION */}
        <Route
          path="/khuyen-mai"
          element={<Promotions />}
        />

        <Route
          path="/khuyen-mai/:id"
          element={<PromotionDetail />}
        />

        {/* SOCIAL */}
        <Route
          path="/hoat-dong-xa-hoi"
          element={<Social />}
        />

        <Route
          path="/hoat-dong-xa-hoi/:id"
          element={<SocialArticle />}
        />

        {/* GUIDE */}
        <Route
          path="/goi-y-chuyen-khoa"
          element={<GoiYChuyenKhoa />}
        />

        {/* SEARCH / CONTACT */}
        <Route
          path="/tim-kiem"
          element={<TimKiem />}
        />

        <Route
          path="/lien-he"
          element={<LienHe />}
        />

        {/* PUBLIC RESULT LOOKUP */}
        <Route
          path="/tra-cuu-ket-qua"
          element={<TraCuuXnoKDN />}
        />

        {/* AUTHENTICATED COMMON PAGES */}
        <Route
          element={
            <ProtectedRoute
              allowedRoles={ALL_ROLES}
            />
          }
        >
          <Route
            path="/thong-bao"
            element={<ThongBao />}
          />

          <Route
            path="/tracking/luot-kham"
            element={<TheoDoiLuotKham />}
          />

          <Route
            path="/tracking/xet-nghiem"
            element={<TheoDoiXetNghiem />}
          />
        </Route>
      </Route>

      {/* =====================================================
          CUSTOMER
      ====================================================== */}
      <Route
        element={
          <ProtectedRoute
            allowedRoles={['CUSTOMER']}
          />
        }
      >
        <Route element={<CustomerLayout />}>
          <Route
            path="/customer"
            element={<CustomerDashboard />}
          />

          <Route
            path="/lich-hen"
            element={<LichHen />}
          />

          <Route
            path="/lichhen"
            element={<LichHen />}
          />

          <Route
            path="/lich-hen/chi-tiet"
            element={<LichHenChiTiet />}
          />

          <Route
            path="/lich-hen/sua"
            element={<LichHenSua />}
          />

          <Route
            path="/ket-qua"
            element={<KetQua />}
          />

          <Route
            path="/ket-qua/:resultId"
            element={<ChiTietKetQua />}
          />
        </Route>
      </Route>

      {/* DOCTOR / ADMIN internal result view */}
      <Route
        element={
          <ProtectedRoute
            allowedRoles={[
              'DOCTOR',
              'ADMIN',
            ]}
          />
        }
      >
        <Route
          path="/internal/ket-qua/:resultId"
          element={<ChiTietKetQua />}
        />
      </Route>

      {/* =====================================================
          DOCTOR
      ====================================================== */}
      <Route
        element={
          <ProtectedRoute
            allowedRoles={['DOCTOR']}
          />
        }
      >
        <Route element={<DoctorLayout />}>
          <Route
            path="/doctor"
            element={<DoctorDashboard />}
          />

          <Route
            path="/doctor/danh-sach-cho"
            element={<DoctorDanhSachCho />}
          />

          <Route
            path="/doctor/kham-benh"
            element={<KhamBenh />}
          />

          <Route
            path="/doctor/chi-dinh-xet-nghiem"
            element={<ChiDinhXetNghiem />}
          />

          <Route
            path="/doctor/lay-mau"
            element={<LayMau />}
          />

          <Route
            path="/doctor/ban-giao-mau"
            element={<BanGiaoMau />}
          />

          <Route
            path="/doctor/duyet-ket-qua"
            element={<DuyetKetQua />}
          />

          <Route
            path="/doctor/doc-ket-qua"
            element={<DocKetQua />}
          />
        </Route>
      </Route>

      {/* =====================================================
          RECEPTIONIST
      ====================================================== */}
      <Route
        element={
          <ProtectedRoute
            allowedRoles={['RECEPTIONIST']}
          />
        }
      >
        <Route element={<ReceptionistLayout />}>
          <Route
            path="/reception"
            element={<ReceptionistDashboard />}
          />

          <Route
            path="/reception/check-in"
            element={<CheckIn />}
          />

          <Route
            path="/reception/qr"
            element={<CheckInQR />}
          />

          <Route
            path="/reception/danh-sach-cho"
            element={<ReceptionDanhSachCho />}
          />

          <Route
            path="/reception/tiep-nhan"
            element={<QuanLyTiepNhan />}
          />
        </Route>
      </Route>

      {/* =====================================================
          TECHNICIAN
      ====================================================== */}
      <Route
        element={
          <ProtectedRoute
            allowedRoles={['TECHNICIAN']}
          />
        }
      >
        <Route element={<TechnicianLayout />}>
          <Route
            path="/technician"
            element={<TechnicianDashboard />}
          />

          <Route
            path="/technician/mau-benh-pham"
            element={<DanhSachMau />}
          />

          <Route
            path="/technician/tiep-nhan-mau/:specimenId"
            element={<TiepNhanMau />}
          />

          <Route
            path="/technician/worklist"
            element={<ThucHienXetNghiem />}
          />

          <Route
            path="/technician/nhap-ket-qua/:worklistId"
            element={<NhapKetQua />}
          />
        </Route>
      </Route>

      {/* =====================================================
          ADMIN
      ====================================================== */}
      <Route
        element={
          <ProtectedRoute
            allowedRoles={['ADMIN']}
          />
        }
      >
        <Route element={<AdminLayout />}>
          <Route
            path="/admin"
            element={<AdminDashboard />}
          />

          <Route
            path="/admin/khach-hang"
            element={<QuanLyKhachHang />}
          />

          <Route
            path="/admin/nhan-vien"
            element={<QuanLyNhanVien />}
          />

          <Route
            path="/admin/bac-si"
            element={<QuanLyBacSi />}
          />

          <Route
            path="/admin/ktv"
            element={<QuanLyKTV />}
          />

          <Route
            path="/admin/xet-nghiem"
            element={<QuanLyXetNghiem />}
          />

          <Route
            path="/admin/chuyen-khoa"
            element={<QuanLyChuyenKhoa />}
          />

          <Route
            path="/admin/phong"
            element={<QuanLyPhong />}
          />

          <Route
            path="/admin/lich-lam-viec"
            element={<QuanLyLichLamViec />}
          />

          <Route
            path="/admin/bao-cao"
            element={<BaoCaoThongKe />}
          />

          <Route
            path="/tracking/lich-su"
            element={<LichSuTruyVet />}
          />
        </Route>
      </Route>

      {/* =====================================================
          ROLE REDIRECT
      ====================================================== */}
      <Route
        path="/dashboard"
        element={<DashboardRedirect />}
      />

      {/* Alias từ Login PHP cũ */}
      <Route
        path="/dashboard/bacsi/baocao"
        element={
          <Navigate
            to="/doctor"
            replace
          />
        }
      />

      <Route
        path="/dashboard/letan/baocao"
        element={
          <Navigate
            to="/reception"
            replace
          />
        }
      />

      <Route
        path="/dashboard/ad/ketqua"
        element={
          <Navigate
            to="/admin"
            replace
          />
        }
      />

      {/* NOT FOUND */}
      <Route
        path="*"
        element={
          <Navigate
            to="/"
            replace
          />
        }
      />
    </Routes>
  );
}