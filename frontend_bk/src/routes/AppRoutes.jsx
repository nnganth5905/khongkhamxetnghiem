// src/routes/AppRoutes.jsx
import { Routes, Route, Navigate, useParams } from "react-router-dom";

import ProtectedRoute from "../components/ProtectedRoute";

import MainLayout from "../layouts/MainLayout";
import AdminLayout from "../layouts/AdminLayout";
import DoctorLayout from "../layouts/DoctorLayout";
import CustomerLayout from "../layouts/CustomerLayout";
import ReceptionistLayout from "../layouts/ReceptionistLayout";
import TechnicianLayout from "../layouts/TechnicianLayout";

import { getRoleHome, useAuth } from "../context/AuthContext";

// HOME
import Home from "../pages/home/Home";
import HomeAlt from "../pages/home/HomeAlt";

// ABOUT
import About1 from "../pages/about/About1";
import About2 from "../pages/about/About2";

// AUTH
import Login from "../pages/auth/Login";
import Register from "../pages/auth/Register";
import ForgotPassword from "../pages/auth/ForgotPassword";
import ResetPassword from "../pages/auth/ResetPassword";
import Confirm from "../pages/auth/Confirm";

// APPOINTMENT
import Book from "../pages/appointment/Book";
import DatLich from "../pages/appointment/DatLich";
import DatLichKham from "../pages/appointment/DatLichKham";
import DatLichXetNghiem from "../pages/appointment/DatLichXetNghiem";
import LichHen from "../pages/appointment/LichHen";
import LichHenChiTiet from "../pages/appointment/LichHenChiTiet";
import LichHenSua from "../pages/appointment/LichHenSua";

// DOCTOR
import BacSiTQ from "../pages/doctor/BacSiTQ";
import GioLamViec from "../pages/doctor/GioLamViec";
import DoctorDanhSachCho from "../pages/doctor/DanhSachCho";
import KhamBenh from "../pages/doctor/KhamBenh";
import ChiDinhXetNghiem from "../pages/doctor/ChiDinhXetNghiem";
import LayMau from "../pages/doctor/LayMau";
import BanGiaoMau from "../pages/doctor/BanGiaoMau";
import DuyetKetQua from "../pages/doctor/DuyetKetQua";
import DocKetQua from "../pages/doctor/DocKetQua";

// TECHNICIAN
import DanhSachMau from "../pages/technician/DanhSachMau";
import TiepNhanMau from "../pages/technician/TiepNhanMau";
import ThucHienXetNghiem from "../pages/technician/ThucHienXetNghiem";
import NhapKetQua from "../pages/technician/NhapKetQua";

// RECEPTION
import CheckIn from "../pages/reception/CheckIn";
import CheckInQR from "../pages/reception/CheckInQR";
import ReceptionDanhSachCho from "../pages/reception/DanhSachCho";
import QuanLyTiepNhan from "../pages/reception/QuanLyTiepNhan";

// TRACKING
import TheoDoiLuotKham from "../pages/tracking/TheoDoiLuotKham";
import TheoDoiXetNghiem from "../pages/tracking/TheoDoiXetNghiem";
import LichSuTruyVet from "../pages/tracking/LichSuTruyVet";

// NOTIFICATION
import ThongBao from "../pages/notification/ThongBao";

// TEST
import KetQua from "../pages/test/KetQua";
import ChiTietKetQua from "../pages/test/ChiTietKetQua";
import ChiTietXetNghiemKDN from "../pages/test/ChiTietXetNghiemKDN";
import TraCuuXnoKDN from "../pages/test/TraCuuXnoKDN";
import TestFinal from "../pages/test/TestFinal";

// TEST CATEGORIES
import BloodClot from "../pages/testCategories/BloodClot";
import Hematology from "../pages/testCategories/Hematology";
import HighUp from "../pages/testCategories/HighUp";
import Hormones from "../pages/testCategories/Hormones";
import Kidney from "../pages/testCategories/Kidney";
import Lipid from "../pages/testCategories/Lipid";
import Tumor from "../pages/testCategories/Tumor";
import Urine from "../pages/testCategories/Urine";
import Virus from "../pages/testCategories/Virus";

// MEDICAL
import Medical from "../pages/medical/Medical";
import MedicalArticle from "../pages/medical/MedicalArticle";
import Handbook from "../pages/medical/Handbook";
import QuyTrinh from "../pages/medical/QuyTrinh";

// NEWS
import NewsADN from "../pages/news/NewsADN";
import NewsCovid from "../pages/news/NewsCovid";
import NewsTimMach from "../pages/news/NewsTimMach";
import NewsVoucher from "../pages/news/NewsVoucher";
import NewsXetNghiem from "../pages/news/NewsXetNghiem";

// PROMOTION
import Promotions from "../pages/promotion/Promotions";
import PromotionDetail from "../pages/promotion/PromotionDetail";

// SERVICES
import Services from "../pages/services/Services";

// SOCIAL
import Social from "../pages/social/Social";
import SocialArticle from "../pages/social/SocialArticle";

// SEARCH
import TimKiem from "../pages/search/TimKiem";
import LienHe from "../pages/contact/LienHe";

// ADMIN
import QuanLyKhachHang from "../pages/admin/QuanLyKhachHang";
import QuanLyNhanVien from "../pages/admin/QuanLyNhanVien";
import QuanLyBacSi from "../pages/admin/QuanLyBacSi";
import QuanLyKTV from "../pages/admin/QuanLyKTV";
import QuanLyXetNghiem from "../pages/admin/QuanLyXetNghiem";
import QuanLyChuyenKhoa from "../pages/admin/QuanLyChuyenKhoa";
import QuanLyPhong from "../pages/admin/QuanLyPhong";
import QuanLyLichLamViec from "../pages/admin/QuanLyLichLamViec";
import BaoCaoThongKe from "../pages/admin/BaoCaoThongKe";
import QuanLyLichHen from "../pages/admin/QuanLyLichHen";

// DASHBOARD
import AdminDashboard from "../pages/dashboard/AdminDashboard";
import DoctorDashboard from "../pages/dashboard/DoctorDashboard";
import CustomerDashboard from "../pages/dashboard/CustomerDashboard";
import ReceptionistDashboard from "../pages/dashboard/ReceptionistDashboard";
import TechnicianDashboard from "../pages/dashboard/TechnicianDashboard";

const ALL_ROLES = [
  "ADMIN",
  "DOCTOR",
  "CUSTOMER",
  "RECEPTIONIST",
  "TECHNICIAN",
];

function DashboardRedirect() {
  const { role, loading, isAuthenticated } = useAuth();

  if (loading) return null;

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  return <Navigate to={getRoleHome(role)} replace />;
}

function ResultRouteRedirect() {
  const { role } = useAuth();
  const { id } = useParams();

  const destination = role === "CUSTOMER"
    ? "/customer/ket-qua"
    : role === "ADMIN"
      ? "/admin/ket-qua"
      : getRoleHome(role);

  const target = role === "CUSTOMER" || role === "ADMIN"
    ? `${destination}${id ? `/${id}` : ""}`
    : destination;

  return <Navigate to={target} replace />;
}

export default function AppRoutes() {
  return (
    <Routes>

      {/* AUTH */}
      <Route path="/login" element={<Login />} />
      <Route path="/register" element={<Register />} />
      <Route path="/forgot-password" element={<ForgotPassword />} />
      <Route path="/reset-password" element={<ResetPassword />} />
      <Route path="/confirm" element={<Confirm />} />

      {/* PUBLIC */}
      <Route element={<MainLayout />}>

        <Route index element={<Home />} />
        <Route path="home-alt" element={<HomeAlt />} />

        <Route path="about" element={<About1 />} />
        <Route path="about-2" element={<About2 />} />

        <Route path="services" element={<Services />} />

        <Route path="book" element={<Book />} />
        <Route path="dat-lich" element={<DatLich />} />
        <Route path="dat-lich-kham" element={<DatLichKham />} />
        <Route path="dat-lich-xet-nghiem" element={<DatLichXetNghiem />} />

        <Route path="bac-si" element={<BacSiTQ />} />
        <Route path="gio-lam-viec" element={<GioLamViec />} />

        <Route path="blood-clot" element={<BloodClot />} />
        <Route path="hematology" element={<Hematology />} />
        <Route path="high-up" element={<HighUp />} />
        <Route path="hormones" element={<Hormones />} />
        <Route path="kidney" element={<Kidney />} />
        <Route path="lipid" element={<Lipid />} />
        <Route path="tumor" element={<Tumor />} />
        <Route path="urine" element={<Urine />} />
        <Route path="virus" element={<Virus />} />

        <Route path="medical" element={<Medical />} />
        <Route path="medical/:id" element={<MedicalArticle />} />
        <Route path="medical-article" element={<MedicalArticle />} />
        <Route path="handbook" element={<Handbook />} />
        <Route path="quy-trinh" element={<QuyTrinh />} />

        <Route path="news-adn" element={<NewsADN />} />
        <Route path="news-covid" element={<NewsCovid />} />
        <Route path="news-tim-mach" element={<NewsTimMach />} />
        <Route path="news-voucher" element={<NewsVoucher />} />
        <Route path="news-xet-nghiem" element={<NewsXetNghiem />} />

        <Route path="promotions" element={<Promotions />} />
        <Route path="promotion/:id" element={<PromotionDetail />} />

        <Route path="social" element={<Social />} />
        <Route path="social/:id" element={<SocialArticle />} />

        <Route path="tim-kiem" element={<TimKiem />} />
        <Route path="lien-he" element={<LienHe />} />

        <Route path="tra-cuu-kdn" element={<TraCuuXnoKDN />} />
        <Route path="chi-tiet-kdn/:testId" element={<ChiTietXetNghiemKDN />} />
        <Route path="xet-nghiem/:testId" element={<ChiTietXetNghiemKDN />} />

        <Route element={<ProtectedRoute roles={ALL_ROLES} />}>
          <Route path="ket-qua" element={<ResultRouteRedirect />} />
          <Route path="ket-qua/:id" element={<ResultRouteRedirect />} />
        </Route>

        <Route element={<ProtectedRoute roles={["CUSTOMER"]} />}>
          <Route path="lich-hen" element={<LichHen />} />
          <Route path="lich-hen/:id" element={<LichHenChiTiet />} />
          <Route path="lich-hen/sua" element={<LichHenSua />} />
          <Route path="lich-hen/:id/sua" element={<LichHenSua />} />
        </Route>

        <Route element={<ProtectedRoute roles={ALL_ROLES} />}>
          <Route path="thong-bao" element={<ThongBao />} />
        </Route>

      </Route>

      {/* CUSTOMER */}
      <Route element={<ProtectedRoute roles={["CUSTOMER"]} />}>
        <Route element={<CustomerLayout />}>

          <Route path="customer" element={<CustomerDashboard />} />
          <Route path="customer/ket-qua" element={<KetQua />} />
          <Route path="customer/ket-qua/:resultId" element={<ChiTietKetQua />} />
          <Route path="customer/theo-doi" element={<TheoDoiLuotKham />} />
          <Route path="customer/theo-doi-xn" element={<TheoDoiXetNghiem />} />
          <Route path="customer/lich-su" element={<LichSuTruyVet />} />

        </Route>
      </Route>

      {/* DOCTOR */}
      <Route element={<ProtectedRoute roles={["DOCTOR"]} />}>
        <Route element={<DoctorLayout />}>

          <Route path="doctor" element={<DoctorDashboard />} />
          <Route path="doctor/danh-sach-cho" element={<DoctorDanhSachCho />} />
          <Route path="doctor/kham-benh" element={<KhamBenh />} />
          <Route path="doctor/chi-dinh-xet-nghiem" element={<ChiDinhXetNghiem />} />
          <Route path="doctor/lay-mau" element={<LayMau />} />
          <Route path="doctor/ban-giao-mau" element={<BanGiaoMau />} />
          <Route path="doctor/duyet-ket-qua" element={<DuyetKetQua />} />
          <Route path="doctor/doc-ket-qua" element={<Navigate to="/doctor/duyet-ket-qua" replace />} />
          <Route path="doctor/doc-ket-qua/:id" element={<DocKetQua />} />
          <Route path="doctor/test-final" element={<TestFinal />} />

        </Route>
      </Route>

      {/* RECEPTION */}
      <Route element={<ProtectedRoute roles={["RECEPTIONIST"]} />}>
        <Route element={<ReceptionistLayout />}>

          <Route path="reception" element={<ReceptionistDashboard />} />
          <Route path="reception/check-in" element={<CheckIn />} />
          <Route path="reception/qr" element={<CheckInQR />} />
          <Route path="reception/danh-sach-cho" element={<ReceptionDanhSachCho />} />
          <Route path="reception/tiep-nhan" element={<QuanLyTiepNhan />} />

        </Route>
      </Route>

      {/* TECHNICIAN */}
      <Route element={<ProtectedRoute roles={["TECHNICIAN"]} />}>
        <Route element={<TechnicianLayout />}>

          <Route path="technician" element={<TechnicianDashboard />} />
          <Route path="technician/danh-sach-mau" element={<DanhSachMau />} />
          <Route path="technician/mau-benh-pham" element={<DanhSachMau />} />
          <Route path="technician/tiep-nhan-mau/:specimenId" element={<TiepNhanMau />} />
          <Route path="technician/worklist" element={<ThucHienXetNghiem />} />
          <Route path="technician/thuc-hien" element={<ThucHienXetNghiem />} />
          <Route path="technician/nhap-ket-qua" element={<Navigate to="/technician/worklist" replace />} />
          <Route path="technician/nhap-ket-qua/:worklistId" element={<NhapKetQua />} />

        </Route>
      </Route>

      {/* ADMIN */}
      <Route element={<ProtectedRoute roles={["ADMIN"]} />}>
        <Route element={<AdminLayout />}>

          <Route path="admin" element={<AdminDashboard />} />

          <Route path="admin/khach-hang" element={<QuanLyKhachHang />} />
          <Route path="admin/nhan-vien" element={<QuanLyNhanVien />} />
          <Route path="admin/bac-si" element={<QuanLyBacSi />} />
          <Route path="admin/ktv" element={<QuanLyKTV />} />
          <Route path="admin/xet-nghiem" element={<QuanLyXetNghiem />} />
          <Route path="admin/chuyen-khoa" element={<QuanLyChuyenKhoa />} />
          <Route path="admin/phong" element={<QuanLyPhong />} />
          <Route path="admin/lich-lam-viec" element={<QuanLyLichLamViec />} />
          <Route path="admin/bao-cao" element={<BaoCaoThongKe />} />
          <Route path="admin/lich-hen" element={<QuanLyLichHen />} />

          <Route path="admin/ket-qua" element={<KetQua />} />
          <Route path="admin/ket-qua/:resultId" element={<ChiTietKetQua />} />

          <Route path="admin/theo-doi" element={<TheoDoiLuotKham />} />
          <Route path="admin/theo-doi-xn" element={<TheoDoiXetNghiem />} />
          <Route path="admin/lich-su" element={<LichSuTruyVet />} />

        </Route>
      </Route>

      <Route path="/dashboard" element={<DashboardRedirect />} />

      <Route path="*" element={<Navigate to="/" replace />} />

    </Routes>
  );
}