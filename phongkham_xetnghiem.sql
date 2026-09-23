-- MySQL dump 10.13  Distrib 8.0.44, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: phongkham_xetnghiem
-- ------------------------------------------------------
-- Server version	8.0.44

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `bacsi`
--

DROP TABLE IF EXISTS `bacsi`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bacsi` (
  `IDBacSi` int NOT NULL AUTO_INCREMENT,
  `TenBacSi` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `HocVi` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ChucDanh` varchar(150) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KhoaID` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL,
  `CoSoID` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `MoTa` text COLLATE utf8mb4_unicode_ci,
  `HinhAnh` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `SoSao` decimal(2,1) NOT NULL DEFAULT '5.0',
  `NamKinhNghiem` tinyint unsigned DEFAULT NULL,
  `TrangThai` enum('active','inactive') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'active',
  `IDNhanVien` varchar(10) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`IDBacSi`),
  UNIQUE KEY `IDNhanVien` (`IDNhanVien`),
  KEY `fk_bacsi_chuyenkhoa` (`KhoaID`),
  KEY `fk_bacsi_coso` (`CoSoID`),
  CONSTRAINT `fk_bacsi_chuyenkhoa` FOREIGN KEY (`KhoaID`) REFERENCES `chuyenkhoa` (`IDChuyenKhoa`),
  CONSTRAINT `fk_bacsi_coso` FOREIGN KEY (`CoSoID`) REFERENCES `coso` (`CoSoID`),
  CONSTRAINT `fk_bacsi_nhanvien` FOREIGN KEY (`IDNhanVien`) REFERENCES `nhanvien` (`IDNhanVien`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bacsi`
--

LOCK TABLES `bacsi` WRITE;
/*!40000 ALTER TABLE `bacsi` DISABLE KEYS */;
/*!40000 ALTER TABLE `bacsi` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `bangiaomau`
--

DROP TABLE IF EXISTS `bangiaomau`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bangiaomau` (
  `IDBanGiao` bigint unsigned NOT NULL AUTO_INCREMENT,
  `IDMau` varchar(30) COLLATE utf8mb4_unicode_ci NOT NULL,
  `IDBacSiBanGiao` int NOT NULL,
  `IDKTVTiepNhan` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL,
  `ThoiGianBanGiao` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `ThoiGianTiepNhan` datetime DEFAULT NULL,
  `TrangThai` enum('da_ban_giao','da_tiep_nhan','tu_choi') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'da_ban_giao',
  `LyDoTuChoi` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `GhiChu` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`IDBanGiao`),
  KEY `fk_bangiao_bacsi` (`IDBacSiBanGiao`),
  KEY `fk_bangiao_ktv` (`IDKTVTiepNhan`),
  KEY `idx_bangiao_mau` (`IDMau`,`ThoiGianBanGiao`),
  CONSTRAINT `fk_bangiao_bacsi` FOREIGN KEY (`IDBacSiBanGiao`) REFERENCES `bacsi` (`IDBacSi`),
  CONSTRAINT `fk_bangiao_ktv` FOREIGN KEY (`IDKTVTiepNhan`) REFERENCES `nhanvien` (`IDNhanVien`),
  CONSTRAINT `fk_bangiao_mau` FOREIGN KEY (`IDMau`) REFERENCES `maubenhpham` (`IDMau`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bangiaomau`
--

LOCK TABLES `bangiaomau` WRITE;
/*!40000 ALTER TABLE `bangiaomau` DISABLE KEYS */;
/*!40000 ALTER TABLE `bangiaomau` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `chisoxetnghiem`
--

DROP TABLE IF EXISTS `chisoxetnghiem`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `chisoxetnghiem` (
  `IDChiSo` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL,
  `IDXetNghiem` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL,
  `TenChiSo` varchar(120) COLLATE utf8mb4_unicode_ci NOT NULL,
  `DonVi` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KieuDuLieu` enum('number','text') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'number',
  `GhiChu` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Status` enum('yes','no') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'yes',
  PRIMARY KEY (`IDChiSo`),
  KEY `idx_chiso_xn` (`IDXetNghiem`),
  CONSTRAINT `fk_chiso_loaixn` FOREIGN KEY (`IDXetNghiem`) REFERENCES `loaixetnghiem` (`IDXetNghiem`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `chisoxetnghiem`
--

LOCK TABLES `chisoxetnghiem` WRITE;
/*!40000 ALTER TABLE `chisoxetnghiem` DISABLE KEYS */;
/*!40000 ALTER TABLE `chisoxetnghiem` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `chitiethoadon`
--

DROP TABLE IF EXISTS `chitiethoadon`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `chitiethoadon` (
  `IDChiTietHoaDon` bigint unsigned NOT NULL AUTO_INCREMENT,
  `MaHoaDon` varchar(30) COLLATE utf8mb4_unicode_ci NOT NULL,
  `LoaiDichVu` enum('kham','xet_nghiem','khac') COLLATE utf8mb4_unicode_ci NOT NULL,
  `MaDichVu` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TenDichVu` varchar(200) COLLATE utf8mb4_unicode_ci NOT NULL,
  `SoLuong` int NOT NULL DEFAULT '1',
  `DonGia` decimal(12,2) NOT NULL DEFAULT '0.00',
  `ThanhTien` decimal(12,2) GENERATED ALWAYS AS ((`SoLuong` * `DonGia`)) STORED,
  PRIMARY KEY (`IDChiTietHoaDon`),
  KEY `fk_cthd_hd` (`MaHoaDon`),
  CONSTRAINT `fk_cthd_hd` FOREIGN KEY (`MaHoaDon`) REFERENCES `hoadon` (`MaHoaDon`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `chitiethoadon`
--

LOCK TABLES `chitiethoadon` WRITE;
/*!40000 ALTER TABLE `chitiethoadon` DISABLE KEYS */;
/*!40000 ALTER TABLE `chitiethoadon` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `chuyenkhoa`
--

DROP TABLE IF EXISTS `chuyenkhoa`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `chuyenkhoa` (
  `IDChuyenKhoa` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL,
  `TenChuyenKhoa` varchar(128) COLLATE utf8mb4_unicode_ci NOT NULL,
  `MoTa` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Status` enum('yes','no') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'yes',
  PRIMARY KEY (`IDChuyenKhoa`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `chuyenkhoa`
--

LOCK TABLES `chuyenkhoa` WRITE;
/*!40000 ALTER TABLE `chuyenkhoa` DISABLE KEYS */;
INSERT INTO `chuyenkhoa` VALUES ('CK001','Nội tổng quát',NULL,'yes'),('CK002','Tim mạch',NULL,'yes'),('CK003','Tai mũi họng',NULL,'yes'),('CK004','Nhi',NULL,'yes'),('CK005','Xét nghiệm',NULL,'yes');
/*!40000 ALTER TABLE `chuyenkhoa` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `coso`
--

DROP TABLE IF EXISTS `coso`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `coso` (
  `CoSoID` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `TenCoSo` varchar(150) COLLATE utf8mb4_unicode_ci NOT NULL,
  `DiaChi` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `SoDienThoai` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Email` varchar(191) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TrangThai` enum('active','inactive') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'active',
  `CreatedAt` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`CoSoID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `coso`
--

LOCK TABLES `coso` WRITE;
/*!40000 ALTER TABLE `coso` DISABLE KEYS */;
INSERT INTO `coso` VALUES ('CS001','Cơ sở chính','Chưa cập nhật',NULL,NULL,'active','2026-08-23 14:14:53',NULL);
/*!40000 ALTER TABLE `coso` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ctdatlichxetnghiem`
--

DROP TABLE IF EXISTS `ctdatlichxetnghiem`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ctdatlichxetnghiem` (
  `IDCTDatLichXN` bigint unsigned NOT NULL AUTO_INCREMENT,
  `IDDatLichXN` bigint unsigned NOT NULL,
  `IDXetNghiem` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL,
  `DonGia` decimal(12,2) NOT NULL DEFAULT '0.00',
  `GhiChu` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`IDCTDatLichXN`),
  UNIQUE KEY `uq_ctdlxn` (`IDDatLichXN`,`IDXetNghiem`),
  KEY `fk_ctdlxn_xn` (`IDXetNghiem`),
  CONSTRAINT `fk_ctdlxn_datlich` FOREIGN KEY (`IDDatLichXN`) REFERENCES `datlichxetnghiem` (`IDDatLichXN`) ON DELETE CASCADE,
  CONSTRAINT `fk_ctdlxn_xn` FOREIGN KEY (`IDXetNghiem`) REFERENCES `loaixetnghiem` (`IDXetNghiem`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ctdatlichxetnghiem`
--

LOCK TABLES `ctdatlichxetnghiem` WRITE;
/*!40000 ALTER TABLE `ctdatlichxetnghiem` DISABLE KEYS */;
/*!40000 ALTER TABLE `ctdatlichxetnghiem` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ctphieuxetnghiem`
--

DROP TABLE IF EXISTS `ctphieuxetnghiem`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ctphieuxetnghiem` (
  `IDCTPhieu` bigint unsigned NOT NULL AUTO_INCREMENT,
  `IDPhieuXetNghiem` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `IDXetNghiem` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL,
  `SoLuong` int NOT NULL DEFAULT '1',
  `DonGia` decimal(12,2) NOT NULL DEFAULT '0.00',
  `GhiChu` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TrangThai` enum('cho_lay_mau','da_lay_mau','ktv_tiep_nhan','dang_xet_nghiem','cho_duyet','da_duyet','can_lam_lai','hoan_tat','huy') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'cho_lay_mau',
  `Status` enum('yes','no') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'yes',
  PRIMARY KEY (`IDCTPhieu`),
  UNIQUE KEY `uq_ctphieu_xn` (`IDPhieuXetNghiem`,`IDXetNghiem`),
  KEY `fk_ctphieu_xn` (`IDXetNghiem`),
  CONSTRAINT `fk_ctphieu_phieu` FOREIGN KEY (`IDPhieuXetNghiem`) REFERENCES `phieuxetnghiem` (`IDPhieuXetNghiem`) ON DELETE CASCADE,
  CONSTRAINT `fk_ctphieu_xn` FOREIGN KEY (`IDXetNghiem`) REFERENCES `loaixetnghiem` (`IDXetNghiem`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ctphieuxetnghiem`
--

LOCK TABLES `ctphieuxetnghiem` WRITE;
/*!40000 ALTER TABLE `ctphieuxetnghiem` DISABLE KEYS */;
/*!40000 ALTER TABLE `ctphieuxetnghiem` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `datlichkham`
--

DROP TABLE IF EXISTS `datlichkham`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `datlichkham` (
  `IDDatLichKham` bigint unsigned NOT NULL AUTO_INCREMENT,
  `MaDatLich` varchar(30) COLLATE utf8mb4_unicode_ci NOT NULL,
  `UserID` int DEFAULT NULL,
  `IDKhachHang` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL,
  `IDChuyenKhoa` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL,
  `IDBacSi` int NOT NULL,
  `CoSoID` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `NgayKham` date NOT NULL,
  `GioKham` time NOT NULL,
  `GhiChu` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TrangThai` enum('pending','confirmed','checked_in','cancelled','no_show','completed') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'pending',
  `StatusMail` enum('pending','sent','failed') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'pending',
  `MaQR` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `CreatedAt` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`IDDatLichKham`),
  UNIQUE KEY `MaDatLich` (`MaDatLich`),
  UNIQUE KEY `MaQR` (`MaQR`),
  KEY `fk_dlk_user` (`UserID`),
  KEY `fk_dlk_chuyenkhoa` (`IDChuyenKhoa`),
  KEY `fk_dlk_coso` (`CoSoID`),
  KEY `idx_dlk_bacsi_time` (`IDBacSi`,`NgayKham`,`GioKham`,`TrangThai`),
  KEY `idx_dlk_khachhang` (`IDKhachHang`,`NgayKham`),
  CONSTRAINT `fk_dlk_bacsi` FOREIGN KEY (`IDBacSi`) REFERENCES `bacsi` (`IDBacSi`),
  CONSTRAINT `fk_dlk_chuyenkhoa` FOREIGN KEY (`IDChuyenKhoa`) REFERENCES `chuyenkhoa` (`IDChuyenKhoa`),
  CONSTRAINT `fk_dlk_coso` FOREIGN KEY (`CoSoID`) REFERENCES `coso` (`CoSoID`),
  CONSTRAINT `fk_dlk_khachhang` FOREIGN KEY (`IDKhachHang`) REFERENCES `khachhang` (`IDKhachHang`),
  CONSTRAINT `fk_dlk_user` FOREIGN KEY (`UserID`) REFERENCES `users` (`UserID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `datlichkham`
--

LOCK TABLES `datlichkham` WRITE;
/*!40000 ALTER TABLE `datlichkham` DISABLE KEYS */;
/*!40000 ALTER TABLE `datlichkham` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `datlichxetnghiem`
--

DROP TABLE IF EXISTS `datlichxetnghiem`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `datlichxetnghiem` (
  `IDDatLichXN` bigint unsigned NOT NULL AUTO_INCREMENT,
  `MaDatLich` varchar(30) COLLATE utf8mb4_unicode_ci NOT NULL,
  `UserID` int DEFAULT NULL,
  `IDKhachHang` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL,
  `CoSoID` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `NgayXetNghiem` date NOT NULL,
  `GioXetNghiem` time NOT NULL,
  `GhiChu` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TrangThai` enum('pending','confirmed','checked_in','cancelled','no_show','completed') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'pending',
  `StatusMail` enum('pending','sent','failed') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'pending',
  `MaQR` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `CreatedAt` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`IDDatLichXN`),
  UNIQUE KEY `MaDatLich` (`MaDatLich`),
  UNIQUE KEY `MaQR` (`MaQR`),
  KEY `fk_dlxn_user` (`UserID`),
  KEY `idx_dlxn_time` (`CoSoID`,`NgayXetNghiem`,`GioXetNghiem`,`TrangThai`),
  KEY `idx_dlxn_khachhang` (`IDKhachHang`,`NgayXetNghiem`),
  CONSTRAINT `fk_dlxn_coso` FOREIGN KEY (`CoSoID`) REFERENCES `coso` (`CoSoID`),
  CONSTRAINT `fk_dlxn_khachhang` FOREIGN KEY (`IDKhachHang`) REFERENCES `khachhang` (`IDKhachHang`),
  CONSTRAINT `fk_dlxn_user` FOREIGN KEY (`UserID`) REFERENCES `users` (`UserID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `datlichxetnghiem`
--

LOCK TABLES `datlichxetnghiem` WRITE;
/*!40000 ALTER TABLE `datlichxetnghiem` DISABLE KEYS */;
/*!40000 ALTER TABLE `datlichxetnghiem` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `hoadon`
--

DROP TABLE IF EXISTS `hoadon`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `hoadon` (
  `MaHoaDon` varchar(30) COLLATE utf8mb4_unicode_ci NOT NULL,
  `IDKhachHang` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL,
  `IDPhieuXetNghiem` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `IDLuotKham` bigint unsigned DEFAULT NULL,
  `NgayTaoHoaDon` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `TongTien` decimal(12,2) NOT NULL DEFAULT '0.00',
  `TrangThaiThanhToan` enum('chua_thanh_toan','da_thanh_toan','hoan_tien','huy') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'chua_thanh_toan',
  `PhuongThuc` enum('tien_mat','chuyen_khoan','the','khac') COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Status` enum('yes','no') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'yes',
  PRIMARY KEY (`MaHoaDon`),
  KEY `fk_hd_khachhang` (`IDKhachHang`),
  KEY `fk_hd_phieu` (`IDPhieuXetNghiem`),
  KEY `fk_hd_luotkham` (`IDLuotKham`),
  CONSTRAINT `fk_hd_khachhang` FOREIGN KEY (`IDKhachHang`) REFERENCES `khachhang` (`IDKhachHang`),
  CONSTRAINT `fk_hd_luotkham` FOREIGN KEY (`IDLuotKham`) REFERENCES `luotkham` (`IDLuotKham`),
  CONSTRAINT `fk_hd_phieu` FOREIGN KEY (`IDPhieuXetNghiem`) REFERENCES `phieuxetnghiem` (`IDPhieuXetNghiem`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `hoadon`
--

LOCK TABLES `hoadon` WRITE;
/*!40000 ALTER TABLE `hoadon` DISABLE KEYS */;
/*!40000 ALTER TABLE `hoadon` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ketquachiso`
--

DROP TABLE IF EXISTS `ketquachiso`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ketquachiso` (
  `IDKetQuaChiSo` bigint unsigned NOT NULL AUTO_INCREMENT,
  `IDKetQua` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `IDChiSo` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL,
  `GiaTriSo` decimal(15,4) DEFAULT NULL,
  `GiaTriText` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NguongMinApDung` decimal(15,4) DEFAULT NULL,
  `NguongMaxApDung` decimal(15,4) DEFAULT NULL,
  `GiaTriThamChieu` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `DanhGia` enum('binh_thuong','thap','cao','bat_thuong','chua_danh_gia') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'chua_danh_gia',
  `GhiChu` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Status` enum('yes','no') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'yes',
  `CreatedAt` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`IDKetQuaChiSo`),
  UNIQUE KEY `uq_kq_chiso` (`IDKetQua`,`IDChiSo`),
  KEY `fk_kqchiso_chiso` (`IDChiSo`),
  CONSTRAINT `fk_kqchiso_chiso` FOREIGN KEY (`IDChiSo`) REFERENCES `chisoxetnghiem` (`IDChiSo`),
  CONSTRAINT `fk_kqchiso_kq` FOREIGN KEY (`IDKetQua`) REFERENCES `ketquaxetnghiem` (`IDKetQua`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ketquachiso`
--

LOCK TABLES `ketquachiso` WRITE;
/*!40000 ALTER TABLE `ketquachiso` DISABLE KEYS */;
/*!40000 ALTER TABLE `ketquachiso` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ketquaxetnghiem`
--

DROP TABLE IF EXISTS `ketquaxetnghiem`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ketquaxetnghiem` (
  `IDKetQua` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `IDCTPhieu` bigint unsigned NOT NULL,
  `IDMau` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `IDKTV` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL,
  `ThoiGianBatDau` datetime DEFAULT NULL,
  `ThoiGianHoanThanh` datetime DEFAULT NULL,
  `ThoiGianNhap` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `KetQuaTongQuat` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TrangThai` enum('dang_thuc_hien','cho_duyet','da_duyet','can_lam_lai','hoan_tat') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'cho_duyet',
  `IDBacSiDuyet` int DEFAULT NULL,
  `ThoiGianDuyet` datetime DEFAULT NULL,
  `KetLuanBacSi` text COLLATE utf8mb4_unicode_ci,
  `GhiChu` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`IDKetQua`),
  UNIQUE KEY `IDCTPhieu` (`IDCTPhieu`),
  KEY `fk_kq_mau` (`IDMau`),
  KEY `fk_kq_ktv` (`IDKTV`),
  KEY `fk_kq_bacsi_duyet` (`IDBacSiDuyet`),
  KEY `idx_kq_trangthai` (`TrangThai`,`ThoiGianNhap`),
  CONSTRAINT `fk_kq_bacsi_duyet` FOREIGN KEY (`IDBacSiDuyet`) REFERENCES `bacsi` (`IDBacSi`),
  CONSTRAINT `fk_kq_ctphieu` FOREIGN KEY (`IDCTPhieu`) REFERENCES `ctphieuxetnghiem` (`IDCTPhieu`),
  CONSTRAINT `fk_kq_ktv` FOREIGN KEY (`IDKTV`) REFERENCES `nhanvien` (`IDNhanVien`),
  CONSTRAINT `fk_kq_mau` FOREIGN KEY (`IDMau`) REFERENCES `maubenhpham` (`IDMau`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ketquaxetnghiem`
--

LOCK TABLES `ketquaxetnghiem` WRITE;
/*!40000 ALTER TABLE `ketquaxetnghiem` DISABLE KEYS */;
/*!40000 ALTER TABLE `ketquaxetnghiem` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `khachhang`
--

DROP TABLE IF EXISTS `khachhang`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `khachhang` (
  `IDKhachHang` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL,
  `TenKhachHang` varchar(120) COLLATE utf8mb4_unicode_ci NOT NULL,
  `NgaySinh` date DEFAULT NULL,
  `SoDienThoai` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `GioiTinh` enum('nam','nu','khac') COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `CCCD` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `DiaChi` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Email` varchar(191) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Status` enum('yes','no') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'yes',
  `CreatedAt` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`IDKhachHang`),
  UNIQUE KEY `uq_khachhang_cccd` (`CCCD`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `khachhang`
--

LOCK TABLES `khachhang` WRITE;
/*!40000 ALTER TABLE `khachhang` DISABLE KEYS */;
/*!40000 ALTER TABLE `khachhang` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `kham`
--

DROP TABLE IF EXISTS `kham`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `kham` (
  `IDKham` bigint unsigned NOT NULL AUTO_INCREMENT,
  `IDLuotKham` bigint unsigned NOT NULL,
  `IDBacSi` int NOT NULL,
  `TrieuChung` text COLLATE utf8mb4_unicode_ci,
  `TienSuBenh` text COLLATE utf8mb4_unicode_ci,
  `ChanDoan` text COLLATE utf8mb4_unicode_ci,
  `KetLuan` text COLLATE utf8mb4_unicode_ci,
  `HuongDieuTri` text COLLATE utf8mb4_unicode_ci,
  `ThoiGianKham` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`IDKham`),
  UNIQUE KEY `IDLuotKham` (`IDLuotKham`),
  KEY `fk_kham_bacsi` (`IDBacSi`),
  CONSTRAINT `fk_kham_bacsi` FOREIGN KEY (`IDBacSi`) REFERENCES `bacsi` (`IDBacSi`),
  CONSTRAINT `fk_kham_luot` FOREIGN KEY (`IDLuotKham`) REFERENCES `luotkham` (`IDLuotKham`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `kham`
--

LOCK TABLES `kham` WRITE;
/*!40000 ALTER TABLE `kham` DISABLE KEYS */;
/*!40000 ALTER TABLE `kham` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `lichlamviec`
--

DROP TABLE IF EXISTS `lichlamviec`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `lichlamviec` (
  `LichID` bigint unsigned NOT NULL AUTO_INCREMENT,
  `IDBacSi` int NOT NULL,
  `IDPhong` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Ngay` date NOT NULL,
  `Ca` enum('Sang','Chieu','Toi','TuyChinh') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'TuyChinh',
  `GioBatDau` time NOT NULL,
  `GioKetThuc` time NOT NULL,
  `NguonTao` enum('bacsi','admin') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'bacsi',
  `NguoiTaoUserID` int DEFAULT NULL,
  `TrangThai` enum('duoc_duyet','huy') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'duoc_duyet',
  `GhiChu` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `CreatedAt` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`LichID`),
  UNIQUE KEY `uq_lich_exact` (`IDBacSi`,`Ngay`,`GioBatDau`,`GioKetThuc`),
  KEY `fk_lich_phong` (`IDPhong`),
  KEY `fk_lich_nguoitao` (`NguoiTaoUserID`),
  KEY `idx_lich_bacsi_ngay` (`IDBacSi`,`Ngay`,`TrangThai`),
  CONSTRAINT `fk_lich_bacsi` FOREIGN KEY (`IDBacSi`) REFERENCES `bacsi` (`IDBacSi`),
  CONSTRAINT `fk_lich_nguoitao` FOREIGN KEY (`NguoiTaoUserID`) REFERENCES `users` (`UserID`),
  CONSTRAINT `fk_lich_phong` FOREIGN KEY (`IDPhong`) REFERENCES `phong` (`IDPhong`),
  CONSTRAINT `ck_lich_gio` CHECK ((`GioBatDau` < `GioKetThuc`))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `lichlamviec`
--

LOCK TABLES `lichlamviec` WRITE;
/*!40000 ALTER TABLE `lichlamviec` DISABLE KEYS */;
/*!40000 ALTER TABLE `lichlamviec` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `loaixetnghiem`
--

DROP TABLE IF EXISTS `loaixetnghiem`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `loaixetnghiem` (
  `IDXetNghiem` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL,
  `TenXetNghiem` varchar(150) COLLATE utf8mb4_unicode_ci NOT NULL,
  `ChuyenKhoaID` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Loai` varchar(80) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `MoTa` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Gia` decimal(12,2) NOT NULL DEFAULT '0.00',
  `LoaiMauMacDinh` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ThoiGianDuKienPhut` int DEFAULT NULL,
  `Status` enum('yes','no') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'yes',
  PRIMARY KEY (`IDXetNghiem`),
  KEY `fk_loaixn_chuyenkhoa` (`ChuyenKhoaID`),
  CONSTRAINT `fk_loaixn_chuyenkhoa` FOREIGN KEY (`ChuyenKhoaID`) REFERENCES `chuyenkhoa` (`IDChuyenKhoa`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `loaixetnghiem`
--

LOCK TABLES `loaixetnghiem` WRITE;
/*!40000 ALTER TABLE `loaixetnghiem` DISABLE KEYS */;
/*!40000 ALTER TABLE `loaixetnghiem` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `luotkham`
--

DROP TABLE IF EXISTS `luotkham`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `luotkham` (
  `IDLuotKham` bigint unsigned NOT NULL AUTO_INCREMENT,
  `IDDatLichKham` bigint unsigned NOT NULL,
  `IDKhachHang` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL,
  `IDBacSi` int NOT NULL,
  `IDPhong` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `SoThuTu` int DEFAULT NULL,
  `ThoiGianTiepNhan` datetime DEFAULT NULL,
  `ThoiGianBatDau` datetime DEFAULT NULL,
  `ThoiGianKetThuc` datetime DEFAULT NULL,
  `TrangThai` enum('da_tiep_nhan','cho_kham','da_den_luot','cho_goi_lai','dang_kham','da_chi_dinh_xn','moi_doc_kq','dang_tu_van_kq','bo_luot','hoan_tat') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'da_tiep_nhan',
  PRIMARY KEY (`IDLuotKham`),
  UNIQUE KEY `IDDatLichKham` (`IDDatLichKham`),
  KEY `fk_luotkham_khachhang` (`IDKhachHang`),
  KEY `fk_luotkham_phong` (`IDPhong`),
  KEY `idx_luotkham_queue` (`IDBacSi`,`TrangThai`,`SoThuTu`),
  CONSTRAINT `fk_luotkham_bacsi` FOREIGN KEY (`IDBacSi`) REFERENCES `bacsi` (`IDBacSi`),
  CONSTRAINT `fk_luotkham_datlich` FOREIGN KEY (`IDDatLichKham`) REFERENCES `datlichkham` (`IDDatLichKham`),
  CONSTRAINT `fk_luotkham_khachhang` FOREIGN KEY (`IDKhachHang`) REFERENCES `khachhang` (`IDKhachHang`),
  CONSTRAINT `fk_luotkham_phong` FOREIGN KEY (`IDPhong`) REFERENCES `phong` (`IDPhong`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `luotkham`
--

LOCK TABLES `luotkham` WRITE;
/*!40000 ALTER TABLE `luotkham` DISABLE KEYS */;
/*!40000 ALTER TABLE `luotkham` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `luotxetnghiem`
--

DROP TABLE IF EXISTS `luotxetnghiem`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `luotxetnghiem` (
  `IDLuotXetNghiem` bigint unsigned NOT NULL AUTO_INCREMENT,
  `IDDatLichXN` bigint unsigned NOT NULL,
  `IDKhachHang` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL,
  `IDPhong` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `SoThuTu` int DEFAULT NULL,
  `ThoiGianTiepNhan` datetime DEFAULT NULL,
  `ThoiGianBatDau` datetime DEFAULT NULL,
  `ThoiGianKetThuc` datetime DEFAULT NULL,
  `TrangThai` enum('da_tiep_nhan','cho_xet_nghiem','da_den_luot','dang_lay_mau','da_lay_mau','ktv_tiep_nhan','dang_xet_nghiem','cho_duyet_kq','da_co_kq','moi_doc_kq','dang_tu_van_kq','hoan_tat','huy') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'da_tiep_nhan',
  PRIMARY KEY (`IDLuotXetNghiem`),
  UNIQUE KEY `IDDatLichXN` (`IDDatLichXN`),
  KEY `fk_luotxn_khachhang` (`IDKhachHang`),
  KEY `fk_luotxn_phong` (`IDPhong`),
  KEY `idx_luotxn_queue` (`TrangThai`,`SoThuTu`),
  CONSTRAINT `fk_luotxn_datlich` FOREIGN KEY (`IDDatLichXN`) REFERENCES `datlichxetnghiem` (`IDDatLichXN`),
  CONSTRAINT `fk_luotxn_khachhang` FOREIGN KEY (`IDKhachHang`) REFERENCES `khachhang` (`IDKhachHang`),
  CONSTRAINT `fk_luotxn_phong` FOREIGN KEY (`IDPhong`) REFERENCES `phong` (`IDPhong`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `luotxetnghiem`
--

LOCK TABLES `luotxetnghiem` WRITE;
/*!40000 ALTER TABLE `luotxetnghiem` DISABLE KEYS */;
/*!40000 ALTER TABLE `luotxetnghiem` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `maubenhpham`
--

DROP TABLE IF EXISTS `maubenhpham`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `maubenhpham` (
  `IDMau` varchar(30) COLLATE utf8mb4_unicode_ci NOT NULL,
  `IDCTPhieu` bigint unsigned NOT NULL,
  `MaBarcode` varchar(64) COLLATE utf8mb4_unicode_ci NOT NULL,
  `LoaiMau` varchar(80) COLLATE utf8mb4_unicode_ci NOT NULL,
  `IDBacSiLayMau` int DEFAULT NULL,
  `ThoiGianLayMau` datetime DEFAULT NULL,
  `TrangThai` enum('moi_tao','da_lay_mau','da_ban_giao','ktv_tiep_nhan','dang_xu_ly','hoan_tat','tu_choi_mau','huy') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'moi_tao',
  `GhiChu` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`IDMau`),
  UNIQUE KEY `MaBarcode` (`MaBarcode`),
  KEY `fk_mau_bacsi` (`IDBacSiLayMau`),
  KEY `idx_mau_ctphieu` (`IDCTPhieu`),
  CONSTRAINT `fk_mau_bacsi` FOREIGN KEY (`IDBacSiLayMau`) REFERENCES `bacsi` (`IDBacSi`),
  CONSTRAINT `fk_mau_ctphieu` FOREIGN KEY (`IDCTPhieu`) REFERENCES `ctphieuxetnghiem` (`IDCTPhieu`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `maubenhpham`
--

LOCK TABLES `maubenhpham` WRITE;
/*!40000 ALTER TABLE `maubenhpham` DISABLE KEYS */;
/*!40000 ALTER TABLE `maubenhpham` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `nguongchisoxetnghiem`
--

DROP TABLE IF EXISTS `nguongchisoxetnghiem`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `nguongchisoxetnghiem` (
  `IDNguong` bigint unsigned NOT NULL AUTO_INCREMENT,
  `IDChiSo` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL,
  `GioiTinhApDung` enum('tatca','nam','nu') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'tatca',
  `TuoiMin` smallint unsigned DEFAULT NULL,
  `TuoiMax` smallint unsigned DEFAULT NULL,
  `GiaTriMin` decimal(15,4) DEFAULT NULL,
  `GiaTriMax` decimal(15,4) DEFAULT NULL,
  `GiaTriTextBinhThuong` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `GhiChu` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Status` enum('yes','no') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'yes',
  PRIMARY KEY (`IDNguong`),
  KEY `idx_nguong_lookup` (`IDChiSo`,`GioiTinhApDung`,`TuoiMin`,`TuoiMax`,`Status`),
  CONSTRAINT `fk_nguong_chiso` FOREIGN KEY (`IDChiSo`) REFERENCES `chisoxetnghiem` (`IDChiSo`) ON DELETE CASCADE,
  CONSTRAINT `ck_nguong_giatri` CHECK (((`GiaTriMin` is null) or (`GiaTriMax` is null) or (`GiaTriMin` <= `GiaTriMax`))),
  CONSTRAINT `ck_nguong_tuoi` CHECK (((`TuoiMin` is null) or (`TuoiMax` is null) or (`TuoiMin` <= `TuoiMax`)))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `nguongchisoxetnghiem`
--

LOCK TABLES `nguongchisoxetnghiem` WRITE;
/*!40000 ALTER TABLE `nguongchisoxetnghiem` DISABLE KEYS */;
/*!40000 ALTER TABLE `nguongchisoxetnghiem` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `nhanvien`
--

DROP TABLE IF EXISTS `nhanvien`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `nhanvien` (
  `IDNhanVien` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL,
  `TenNhanVien` varchar(120) COLLATE utf8mb4_unicode_ci NOT NULL,
  `ViTri` enum('bacsi','letan','ktv','admin','dieu_duong','khac') COLLATE utf8mb4_unicode_ci NOT NULL,
  `SoDienThoai` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Email` varchar(191) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `CoSoID` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Status` enum('yes','no') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'yes',
  `CreatedAt` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`IDNhanVien`),
  KEY `fk_nhanvien_coso` (`CoSoID`),
  CONSTRAINT `fk_nhanvien_coso` FOREIGN KEY (`CoSoID`) REFERENCES `coso` (`CoSoID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `nhanvien`
--

LOCK TABLES `nhanvien` WRITE;
/*!40000 ALTER TABLE `nhanvien` DISABLE KEYS */;
/*!40000 ALTER TABLE `nhanvien` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `password_resets`
--

DROP TABLE IF EXISTS `password_resets`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `password_resets` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `UserID` int NOT NULL,
  `TokenHash` char(64) COLLATE utf8mb4_unicode_ci NOT NULL,
  `ExpiresAt` datetime NOT NULL,
  `Used` tinyint(1) NOT NULL DEFAULT '0',
  `CreatedAt` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `TokenHash` (`TokenHash`),
  KEY `fk_passwordreset_user` (`UserID`),
  CONSTRAINT `fk_passwordreset_user` FOREIGN KEY (`UserID`) REFERENCES `users` (`UserID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `password_resets`
--

LOCK TABLES `password_resets` WRITE;
/*!40000 ALTER TABLE `password_resets` DISABLE KEYS */;
/*!40000 ALTER TABLE `password_resets` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `phieuxetnghiem`
--

DROP TABLE IF EXISTS `phieuxetnghiem`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `phieuxetnghiem` (
  `IDPhieuXetNghiem` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `IDKhachHang` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL,
  `IDKham` bigint unsigned DEFAULT NULL,
  `IDLuotXetNghiem` bigint unsigned DEFAULT NULL,
  `IDBacSiChiDinh` int DEFAULT NULL,
  `IDBacSiPhuTrach` int DEFAULT NULL,
  `NgayTao` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `TongTien` decimal(12,2) NOT NULL DEFAULT '0.00',
  `TrangThaiThanhToan` enum('chua_thanh_toan','da_thanh_toan','mien_phi') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'chua_thanh_toan',
  `TrangThai` enum('moi_tao','cho_lay_mau','da_lay_mau','ktv_tiep_nhan','dang_xet_nghiem','cho_duyet','da_co_kq','hoan_tat','huy') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'moi_tao',
  `GhiChu` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`IDPhieuXetNghiem`),
  KEY `fk_phieu_kham` (`IDKham`),
  KEY `fk_phieu_luotxn` (`IDLuotXetNghiem`),
  KEY `fk_phieu_bacsi_chidinh` (`IDBacSiChiDinh`),
  KEY `fk_phieu_bacsi_phutrach` (`IDBacSiPhuTrach`),
  KEY `idx_phieu_khachhang` (`IDKhachHang`,`NgayTao`),
  CONSTRAINT `fk_phieu_bacsi_chidinh` FOREIGN KEY (`IDBacSiChiDinh`) REFERENCES `bacsi` (`IDBacSi`),
  CONSTRAINT `fk_phieu_bacsi_phutrach` FOREIGN KEY (`IDBacSiPhuTrach`) REFERENCES `bacsi` (`IDBacSi`),
  CONSTRAINT `fk_phieu_khachhang` FOREIGN KEY (`IDKhachHang`) REFERENCES `khachhang` (`IDKhachHang`),
  CONSTRAINT `fk_phieu_kham` FOREIGN KEY (`IDKham`) REFERENCES `kham` (`IDKham`),
  CONSTRAINT `fk_phieu_luotxn` FOREIGN KEY (`IDLuotXetNghiem`) REFERENCES `luotxetnghiem` (`IDLuotXetNghiem`),
  CONSTRAINT `ck_phieu_nguon` CHECK ((((`IDKham` is not null) and (`IDLuotXetNghiem` is null)) or ((`IDKham` is null) and (`IDLuotXetNghiem` is not null))))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `phieuxetnghiem`
--

LOCK TABLES `phieuxetnghiem` WRITE;
/*!40000 ALTER TABLE `phieuxetnghiem` DISABLE KEYS */;
/*!40000 ALTER TABLE `phieuxetnghiem` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `phong`
--

DROP TABLE IF EXISTS `phong`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `phong` (
  `IDPhong` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `CoSoID` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `IDChuyenKhoa` varchar(10) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TenPhong` varchar(120) COLLATE utf8mb4_unicode_ci NOT NULL,
  `LoaiPhong` enum('kham','lay_mau','xet_nghiem','tu_van','khac') COLLATE utf8mb4_unicode_ci NOT NULL,
  `Tang` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TrangThai` enum('active','inactive','maintenance') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'active',
  PRIMARY KEY (`IDPhong`),
  KEY `fk_phong_coso` (`CoSoID`),
  KEY `fk_phong_chuyenkhoa` (`IDChuyenKhoa`),
  CONSTRAINT `fk_phong_chuyenkhoa` FOREIGN KEY (`IDChuyenKhoa`) REFERENCES `chuyenkhoa` (`IDChuyenKhoa`),
  CONSTRAINT `fk_phong_coso` FOREIGN KEY (`CoSoID`) REFERENCES `coso` (`CoSoID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `phong`
--

LOCK TABLES `phong` WRITE;
/*!40000 ALTER TABLE `phong` DISABLE KEYS */;
/*!40000 ALTER TABLE `phong` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `thongbao`
--

DROP TABLE IF EXISTS `thongbao`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `thongbao` (
  `IDThongBao` bigint unsigned NOT NULL AUTO_INCREMENT,
  `UserIDNhan` int NOT NULL,
  `LoaiThongBao` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `TieuDe` varchar(200) COLLATE utf8mb4_unicode_ci NOT NULL,
  `NoiDung` text COLLATE utf8mb4_unicode_ci NOT NULL,
  `LoaiDoiTuong` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `IDDoiTuong` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `DaDoc` tinyint(1) NOT NULL DEFAULT '0',
  `ThoiGianTao` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `ThoiGianDoc` datetime DEFAULT NULL,
  PRIMARY KEY (`IDThongBao`),
  KEY `idx_thongbao_user` (`UserIDNhan`,`DaDoc`,`ThoiGianTao`),
  CONSTRAINT `fk_thongbao_user` FOREIGN KEY (`UserIDNhan`) REFERENCES `users` (`UserID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `thongbao`
--

LOCK TABLES `thongbao` WRITE;
/*!40000 ALTER TABLE `thongbao` DISABLE KEYS */;
/*!40000 ALTER TABLE `thongbao` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `truyvet`
--

DROP TABLE IF EXISTS `truyvet`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `truyvet` (
  `IDTruyVet` bigint unsigned NOT NULL AUTO_INCREMENT,
  `IDKhachHang` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL,
  `LoaiDoiTuong` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `IDDoiTuong` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `HanhDong` varchar(150) COLLATE utf8mb4_unicode_ci NOT NULL,
  `TrangThaiCu` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TrangThaiMoi` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `UserIDThucHien` int DEFAULT NULL,
  `NguonThucHien` enum('user','system') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'user',
  `ThoiGian` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `MoTa` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `IPAddress` varchar(45) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`IDTruyVet`),
  KEY `fk_truyvet_user` (`UserIDThucHien`),
  KEY `idx_truyvet_khachhang` (`IDKhachHang`,`ThoiGian`),
  KEY `idx_truyvet_doituong` (`LoaiDoiTuong`,`IDDoiTuong`,`ThoiGian`),
  CONSTRAINT `fk_truyvet_khachhang` FOREIGN KEY (`IDKhachHang`) REFERENCES `khachhang` (`IDKhachHang`),
  CONSTRAINT `fk_truyvet_user` FOREIGN KEY (`UserIDThucHien`) REFERENCES `users` (`UserID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `truyvet`
--

LOCK TABLES `truyvet` WRITE;
/*!40000 ALTER TABLE `truyvet` DISABLE KEYS */;
/*!40000 ALTER TABLE `truyvet` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `users` (
  `UserID` int NOT NULL AUTO_INCREMENT,
  `Email` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Username` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `PasswordHash` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Role` enum('khachhang','bacsi','letan','ktv','admin') COLLATE utf8mb4_unicode_ci NOT NULL,
  `IDKhachHang` varchar(10) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `IDBacSi` int DEFAULT NULL,
  `IDNhanVien` varchar(10) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `CreatedAt` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`UserID`),
  UNIQUE KEY `Email` (`Email`),
  UNIQUE KEY `Username` (`Username`),
  UNIQUE KEY `IDKhachHang` (`IDKhachHang`),
  UNIQUE KEY `IDBacSi` (`IDBacSi`),
  UNIQUE KEY `IDNhanVien` (`IDNhanVien`),
  CONSTRAINT `fk_users_bacsi` FOREIGN KEY (`IDBacSi`) REFERENCES `bacsi` (`IDBacSi`),
  CONSTRAINT `fk_users_khachhang` FOREIGN KEY (`IDKhachHang`) REFERENCES `khachhang` (`IDKhachHang`),
  CONSTRAINT `fk_users_nhanvien` FOREIGN KEY (`IDNhanVien`) REFERENCES `nhanvien` (`IDNhanVien`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `worklist`
--

DROP TABLE IF EXISTS `worklist`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `worklist` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `IDCTPhieu` bigint unsigned NOT NULL,
  `IDMau` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `IDKTV` varchar(10) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Instrument` varchar(120) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ReagentLot` varchar(64) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Status` enum('queue','running','to_result','rerun','finished','cancelled') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'queue',
  `ReceivedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `StartedAt` datetime DEFAULT NULL,
  `FinishedAt` datetime DEFAULT NULL,
  `UpdatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `fk_worklist_ctphieu` (`IDCTPhieu`),
  KEY `fk_worklist_mau` (`IDMau`),
  KEY `fk_worklist_ktv` (`IDKTV`),
  KEY `idx_worklist_status` (`Status`,`ReceivedAt`),
  CONSTRAINT `fk_worklist_ctphieu` FOREIGN KEY (`IDCTPhieu`) REFERENCES `ctphieuxetnghiem` (`IDCTPhieu`),
  CONSTRAINT `fk_worklist_ktv` FOREIGN KEY (`IDKTV`) REFERENCES `nhanvien` (`IDNhanVien`),
  CONSTRAINT `fk_worklist_mau` FOREIGN KEY (`IDMau`) REFERENCES `maubenhpham` (`IDMau`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `worklist`
--

LOCK TABLES `worklist` WRITE;
/*!40000 ALTER TABLE `worklist` DISABLE KEYS */;
/*!40000 ALTER TABLE `worklist` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-09-15 23:50:17
