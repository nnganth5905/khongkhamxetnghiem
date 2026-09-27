-- MySQL dump 10.13  Distrib 26.7.0, for macos14.8 (arm64)
--
-- Host: 127.0.0.1    Database: phongkham_xetnghiem
-- ------------------------------------------------------
-- Server version	8.0.46

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
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
  `TenBacSi` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `HocVi` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ChucDanh` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KhoaID` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `CoSoID` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `MoTa` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
  `HinhAnh` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `SoSao` decimal(2,1) NOT NULL DEFAULT '5.0',
  `NamKinhNghiem` tinyint unsigned DEFAULT NULL,
  `TrangThai` enum('active','inactive') CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'active',
  `IDNhanVien` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`IDBacSi`),
  UNIQUE KEY `IDNhanVien` (`IDNhanVien`),
  KEY `fk_bacsi_chuyenkhoa` (`KhoaID`),
  KEY `fk_bacsi_coso` (`CoSoID`),
  CONSTRAINT `fk_bacsi_chuyenkhoa` FOREIGN KEY (`KhoaID`) REFERENCES `chuyenkhoa` (`IDChuyenKhoa`),
  CONSTRAINT `fk_bacsi_coso` FOREIGN KEY (`CoSoID`) REFERENCES `coso` (`CoSoID`),
  CONSTRAINT `fk_bacsi_nhanvien` FOREIGN KEY (`IDNhanVien`) REFERENCES `nhanvien` (`IDNhanVien`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bacsi`
--

LOCK TABLES `bacsi` WRITE;
/*!40000 ALTER TABLE `bacsi` DISABLE KEYS */;
INSERT INTO `bacsi` VALUES (1,'Hoàng Khắc Nam','Thạc sĩ','Bác sĩ chuyên khoa I','CK001','CS001',NULL,NULL,5.0,15,'active',NULL),(2,'Trần Mỹ Dung','Bác sĩ','Trưởng phòng Xét nghiệm','CK005','CS001',NULL,NULL,5.0,8,'active',NULL),(3,'Lê Văn Cường','Thạc sĩ','Bác sĩ chuyên khoa II','CK001','CS001',NULL,NULL,5.0,12,'active',NULL),(4,'Phạm Thị Mai','Bác sĩ','Trưởng khoa','CK005','CS001',NULL,NULL,4.8,8,'active',NULL),(5,'Nguyễn Minh Tuấn','Bác sĩ','Bác sĩ chuyên khoa I','CK002','CS001',NULL,NULL,5.0,10,'active',NULL),(6,'Đặng Thu Trang','Bác sĩ','Bác sĩ chuyên khoa I','CK002','CS001',NULL,NULL,4.9,7,'active',NULL),(7,'Vũ Hoàng Long','Bác sĩ','Bác sĩ chuyên khoa I','CK003','CS001',NULL,NULL,5.0,9,'active',NULL),(8,'Nguyễn Thị Hạnh','Bác sĩ','Bác sĩ chuyên khoa I','CK003','CS001',NULL,NULL,4.9,6,'active',NULL),(9,'Phạm Quốc Bảo','Bác sĩ','Bác sĩ chuyên khoa II','CK004','CS001',NULL,NULL,5.0,11,'active',NULL),(10,'Trần Ngọc Anh','Bác sĩ','Bác sĩ chuyên khoa I','CK004','CS001',NULL,NULL,4.9,8,'active',NULL);
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
  `IDMau` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `IDBacSiBanGiao` int NOT NULL,
  `IDKTVTiepNhan` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `ThoiGianBanGiao` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `ThoiGianTiepNhan` datetime DEFAULT NULL,
  `TrangThai` enum('da_ban_giao','da_tiep_nhan','tu_choi') CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'da_ban_giao',
  `LyDoTuChoi` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `GhiChu` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`IDBanGiao`),
  KEY `fk_bangiao_bacsi` (`IDBacSiBanGiao`),
  KEY `fk_bangiao_ktv` (`IDKTVTiepNhan`),
  KEY `idx_bangiao_mau` (`IDMau`,`ThoiGianBanGiao`),
  CONSTRAINT `fk_bangiao_bacsi` FOREIGN KEY (`IDBacSiBanGiao`) REFERENCES `bacsi` (`IDBacSi`),
  CONSTRAINT `fk_bangiao_ktv` FOREIGN KEY (`IDKTVTiepNhan`) REFERENCES `nhanvien` (`IDNhanVien`),
  CONSTRAINT `fk_bangiao_mau` FOREIGN KEY (`IDMau`) REFERENCES `maubenhpham` (`IDMau`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bangiaomau`
--

LOCK TABLES `bangiaomau` WRITE;
/*!40000 ALTER TABLE `bangiaomau` DISABLE KEYS */;
INSERT INTO `bangiaomau` VALUES (1,'M179021485939698',6,'NV011','2026-09-24 01:54:46','2026-09-24 07:52:48','da_tiep_nhan',NULL,''),(2,'M179025859845013',8,'NV011','2026-09-24 14:03:57','2026-09-24 14:05:35','da_tiep_nhan','mẫu hỏng',''),(3,'M179025906283337',2,'NV011','2026-09-24 14:11:11','2026-09-24 14:11:37','da_tiep_nhan',NULL,''),(4,'M179026222395394',2,'NV011','2026-09-24 15:03:52','2026-09-24 15:16:23','da_tiep_nhan',NULL,'');
/*!40000 ALTER TABLE `bangiaomau` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `chisoxetnghiem`
--

DROP TABLE IF EXISTS `chisoxetnghiem`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `chisoxetnghiem` (
  `IDChiSo` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `IDXetNghiem` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `TenChiSo` varchar(120) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `DonVi` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KieuDuLieu` enum('number','text') CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'number',
  `GhiChu` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Status` enum('yes','no') CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'yes',
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
INSERT INTO `chisoxetnghiem` VALUES ('CS001','XN001','WBC','10^9/L','number','Số lượng bạch cầu','yes'),('CS002','XN001','RBC','10^12/L','number','Số lượng hồng cầu','yes'),('CS003','XN001','HGB','g/dL','number','Nồng độ hemoglobin','yes'),('CS004','XN001','HCT','%','number','Hematocrit','yes'),('CS005','XN001','MCV','fL','number','Thể tích trung bình hồng cầu','yes'),('CS006','XN001','MCH','pg','number','Lượng hemoglobin trung bình hồng cầu','yes'),('CS007','XN001','MCHC','g/dL','number','Nồng độ hemoglobin trung bình hồng cầu','yes'),('CS008','XN001','RDW-CV','%','number','Độ phân bố kích thước hồng cầu','yes'),('CS009','XN001','PLT','10^9/L','number','Số lượng tiểu cầu','yes'),('CS010','XN001','MPV','fL','number','Thể tích tiểu cầu trung bình','yes'),('CS011','XN001','NEU%','%','number','Tỷ lệ bạch cầu trung tính','yes'),('CS012','XN001','LYM%','%','number','Tỷ lệ lympho bào','yes'),('CS013','XN001','MONO%','%','number','Tỷ lệ bạch cầu mono','yes'),('CS014','XN001','EOS%','%','number','Tỷ lệ bạch cầu ái toan','yes'),('CS015','XN001','BASO%','%','number','Tỷ lệ bạch cầu ái kiềm','yes'),('CS016','XN001','NEU#','10^9/L','number','Số lượng bạch cầu trung tính','yes'),('CS017','XN001','LYM#','10^9/L','number','Số lượng lympho bào','yes'),('CS018','XN001','MONO#','10^9/L','number','Số lượng bạch cầu mono','yes'),('CS019','XN001','EOS#','10^9/L','number','Số lượng bạch cầu ái toan','yes'),('CS020','XN001','BASO#','10^9/L','number','Số lượng bạch cầu ái kiềm','yes'),('CS021','XN002','Glucose','mmol/L','number','Đường huyết lúc đói','yes'),('CS022','XN003','AST','U/L','number','Aspartate aminotransferase','yes'),('CS023','XN003','ALT','U/L','number','Alanine aminotransferase','yes'),('CS024','XN004','Troponin I','ng/L','number','Chỉ số Troponin I','yes'),('CS025','XN005','CK-MB','U/L','number','Creatine kinase-MB','yes'),('CS026','XN006','NT-proBNP','pg/mL','number','N-terminal pro B-type natriuretic peptide','yes'),('CS027','XN007','Cholesterol toàn phần','mmol/L','number','Cholesterol toàn phần','yes'),('CS028','XN007','HDL-C','mmol/L','number','Cholesterol lipoprotein tỷ trọng cao','yes'),('CS029','XN007','LDL-C','mmol/L','number','Cholesterol lipoprotein tỷ trọng thấp','yes'),('CS030','XN007','Triglyceride','mmol/L','number','Triglyceride máu','yes'),('CS031','XN008','Kháng nguyên liên cầu khuẩn nhóm A','-','text','Kết quả âm tính hoặc dương tính','yes'),('CS032','XN009','Virus cúm A','-','text','Kết quả âm tính hoặc dương tính','yes'),('CS033','XN009','Virus cúm B','-','text','Kết quả âm tính hoặc dương tính','yes'),('CS034','XN010','IgE toàn phần','IU/mL','number','Định lượng IgE toàn phần','yes'),('CS035','XN011','Kết quả cấy dịch họng','-','text','Kết quả nuôi cấy','yes'),('CS036','XN011','Vi khuẩn định danh','-','text','Tên vi khuẩn được định danh','yes'),('CS037','XN011','Kháng sinh đồ','-','text','Kết quả kháng sinh đồ','yes'),('CS038','XN012','Bilirubin toàn phần','µmol/L','number','Bilirubin toàn phần','yes'),('CS039','XN013','CRP','mg/L','number','C-reactive protein','yes'),('CS040','XN014','WBC','10^9/L','number','Số lượng bạch cầu','yes'),('CS041','XN014','RBC','10^12/L','number','Số lượng hồng cầu','yes'),('CS042','XN014','HGB','g/dL','number','Nồng độ hemoglobin','yes'),('CS043','XN014','HCT','%','number','Hematocrit','yes'),('CS044','XN014','MCV','fL','number','Thể tích trung bình hồng cầu','yes'),('CS045','XN014','MCH','pg','number','Lượng hemoglobin trung bình hồng cầu','yes'),('CS046','XN014','MCHC','g/dL','number','Nồng độ hemoglobin trung bình hồng cầu','yes'),('CS047','XN014','RDW-CV','%','number','Độ phân bố kích thước hồng cầu','yes'),('CS048','XN014','PLT','10^9/L','number','Số lượng tiểu cầu','yes'),('CS049','XN014','MPV','fL','number','Thể tích tiểu cầu trung bình','yes'),('CS050','XN014','NEU%','%','number','Tỷ lệ bạch cầu trung tính','yes'),('CS051','XN014','LYM%','%','number','Tỷ lệ lympho bào','yes'),('CS052','XN014','MONO%','%','number','Tỷ lệ bạch cầu mono','yes'),('CS053','XN014','EOS%','%','number','Tỷ lệ bạch cầu ái toan','yes'),('CS054','XN014','BASO%','%','number','Tỷ lệ bạch cầu ái kiềm','yes'),('CS055','XN015','Glucose','mmol/L','number','Đường huyết','yes'),('CS056','XN016','HbA1c','%','number','Hemoglobin glycated','yes'),('CS057','XN017','Creatinine','µmol/L','number','Đánh giá chức năng thận','yes'),('CS058','XN018','Màu sắc','-','text','Màu sắc nước tiểu','yes'),('CS059','XN018','Độ trong','-','text','Độ trong của nước tiểu','yes'),('CS060','XN018','Tỷ trọng',NULL,'number','Tỷ trọng nước tiểu','yes'),('CS061','XN018','pH',NULL,'number','Độ pH nước tiểu','yes'),('CS062','XN018','Protein','-','text','Protein niệu','yes'),('CS063','XN018','Glucose','-','text','Glucose niệu','yes'),('CS064','XN018','Ketone','-','text','Thể ketone trong nước tiểu','yes'),('CS065','XN018','Blood','-','text','Máu/hemoglobin trong nước tiểu','yes'),('CS066','XN018','Bilirubin','-','text','Bilirubin niệu','yes'),('CS067','XN018','Urobilinogen','-','text','Urobilinogen niệu','yes'),('CS068','XN018','Nitrite','-','text','Nitrite niệu','yes'),('CS069','XN018','Leukocytes','-','text','Bạch cầu trên que thử','yes'),('CS070','XN018','Hồng cầu','tế bào/µL','number','Số lượng hồng cầu trong nước tiểu','yes'),('CS071','XN018','Bạch cầu','tế bào/µL','number','Số lượng bạch cầu trong nước tiểu','yes'),('CS072','XN018','Tế bào biểu mô','tế bào/µL','number','Tế bào biểu mô trong nước tiểu','yes');
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
  `MaHoaDon` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `LoaiDichVu` enum('kham','xet_nghiem','khac') CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `MaDichVu` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TenDichVu` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
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
  `IDChuyenKhoa` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `TenChuyenKhoa` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `MoTa` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Status` enum('yes','no') CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'yes',
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
  `CoSoID` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `TenCoSo` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `DiaChi` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `SoDienThoai` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Email` varchar(191) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TrangThai` enum('active','inactive') CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'active',
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
  `IDXetNghiem` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `DonGia` decimal(12,2) NOT NULL DEFAULT '0.00',
  `GhiChu` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`IDCTDatLichXN`),
  UNIQUE KEY `uq_ctdlxn` (`IDDatLichXN`,`IDXetNghiem`),
  KEY `fk_ctdlxn_xn` (`IDXetNghiem`),
  CONSTRAINT `fk_ctdlxn_datlich` FOREIGN KEY (`IDDatLichXN`) REFERENCES `datlichxetnghiem` (`IDDatLichXN`) ON DELETE CASCADE,
  CONSTRAINT `fk_ctdlxn_xn` FOREIGN KEY (`IDXetNghiem`) REFERENCES `loaixetnghiem` (`IDXetNghiem`)
) ENGINE=InnoDB AUTO_INCREMENT=24 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ctdatlichxetnghiem`
--

LOCK TABLES `ctdatlichxetnghiem` WRITE;
/*!40000 ALTER TABLE `ctdatlichxetnghiem` DISABLE KEYS */;
INSERT INTO `ctdatlichxetnghiem` VALUES (1,1,'XN017',0.00,''),(2,2,'XN013',0.00,''),(3,3,'XN015',0.00,''),(4,4,'XN001',0.00,''),(5,5,'XN003',0.00,''),(6,6,'XN002',0.00,''),(7,7,'XN016',0.00,''),(8,8,'XN001',0.00,''),(9,9,'XN009',0.00,''),(10,10,'XN010',0.00,''),(11,11,'XN007',0.00,''),(12,12,'XN004',0.00,''),(13,13,'XN009',0.00,''),(14,14,'XN001',0.00,''),(15,15,'XN016',0.00,''),(16,16,'XN017',0.00,''),(17,17,'XN001',0.00,''),(18,18,'XN005',0.00,''),(19,19,'XN005',0.00,''),(20,20,'XN004',0.00,''),(21,21,'XN009',0.00,''),(22,22,'XN017',0.00,''),(23,23,'XN016',0.00,'');
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
  `IDPhieuXetNghiem` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `IDXetNghiem` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `SoLuong` int NOT NULL DEFAULT '1',
  `DonGia` decimal(12,2) NOT NULL DEFAULT '0.00',
  `GhiChu` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TrangThai` enum('cho_lay_mau','da_lay_mau','ktv_tiep_nhan','dang_xet_nghiem','cho_duyet','da_duyet','can_lam_lai','hoan_tat','huy') CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'cho_lay_mau',
  `Status` enum('yes','no') CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'yes',
  PRIMARY KEY (`IDCTPhieu`),
  UNIQUE KEY `uq_ctphieu_xn` (`IDPhieuXetNghiem`,`IDXetNghiem`),
  KEY `fk_ctphieu_xn` (`IDXetNghiem`),
  CONSTRAINT `fk_ctphieu_phieu` FOREIGN KEY (`IDPhieuXetNghiem`) REFERENCES `phieuxetnghiem` (`IDPhieuXetNghiem`) ON DELETE CASCADE,
  CONSTRAINT `fk_ctphieu_xn` FOREIGN KEY (`IDXetNghiem`) REFERENCES `loaixetnghiem` (`IDXetNghiem`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ctphieuxetnghiem`
--

LOCK TABLES `ctphieuxetnghiem` WRITE;
/*!40000 ALTER TABLE `ctphieuxetnghiem` DISABLE KEYS */;
INSERT INTO `ctphieuxetnghiem` VALUES (1,'PXN8','XN004',1,0.00,NULL,'da_duyet','yes'),(2,'PXN9','XN009',1,0.00,NULL,'da_duyet','yes'),(3,'PXN10','XN017',1,0.00,NULL,'da_duyet','yes'),(4,'PXN11','XN016',1,0.00,NULL,'da_duyet','yes');
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
  `MaDatLich` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `UserID` int DEFAULT NULL,
  `IDKhachHang` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `IDChuyenKhoa` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `IDBacSi` int NOT NULL,
  `CoSoID` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `NgayKham` date NOT NULL,
  `GioKham` time NOT NULL,
  `GhiChu` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TrangThai` enum('pending','confirmed','checked_in','cancelled','no_show','completed') CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'pending',
  `StatusMail` enum('pending','sent','failed') CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'pending',
  `MaQR` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
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
) ENGINE=InnoDB AUTO_INCREMENT=31 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `datlichkham`
--

LOCK TABLES `datlichkham` WRITE;
/*!40000 ALTER TABLE `datlichkham` DISABLE KEYS */;
INSERT INTO `datlichkham` VALUES (7,'DLK-80BB01',NULL,'KH14B09585','CK001',1,'CS001','2026-09-24','08:00:00','','checked_in','pending',NULL,'2026-09-22 09:29:10','2026-09-24 00:31:15'),(8,'DLK-0F42C1',NULL,'KH14B09585','CK005',4,'CS001','2026-09-26','16:00:00','','pending','pending',NULL,'2026-09-22 09:30:16',NULL),(9,'DLK-066945',NULL,'KH14B09585','CK001',3,'CS001','2026-09-24','07:00:00','','checked_in','pending',NULL,'2026-09-22 09:32:39','2026-09-24 00:09:32'),(10,'DLK-0B40BA',NULL,'KHFD6C8512','CK001',1,'CS001','2026-10-10','10:00:00','','pending','pending',NULL,'2026-09-22 09:36:52','2026-09-23 20:53:59'),(11,'DLK-29DAC7',NULL,'KH14B09585','CK001',1,'CS001','2026-09-24','14:00:00','','checked_in','pending',NULL,'2026-09-22 09:41:51','2026-09-24 00:43:54'),(12,'DLK-921554',NULL,'KH14B09585','CK002',5,'CS001','2026-09-23','08:00:00','','no_show','pending',NULL,'2026-09-22 13:58:36','2026-09-24 17:00:59'),(13,'DLK-5DDAED',NULL,'KH14B09585','CK003',8,'CS001','2026-09-23','07:00:00','','checked_in','pending',NULL,'2026-09-22 14:01:12','2026-09-23 07:31:51'),(14,'DLK-9569AE',4,'KH14B09585','CK003',7,'CS001','2026-09-29','15:00:00','','pending','pending',NULL,'2026-09-22 14:11:44',NULL),(15,'DLK-922809',4,'KH14B09585','CK001',3,'CS001','2026-10-01','08:00:00','','cancelled','pending',NULL,'2026-09-22 14:12:28','2026-09-23 21:57:16'),(16,'DLK-DDC0F9',4,'KH14B09585','CK004',10,'CS001','2026-09-27','16:00:00','','pending','pending',NULL,'2026-09-22 14:21:44',NULL),(17,'DLK-407AF9',4,'KH1F7A16A3','CK001',3,'CS001','2026-10-09','16:00:00','','pending','pending',NULL,'2026-09-22 14:31:51',NULL),(18,'DLK-E4D290',4,'KH1F7A16A3','CK001',3,'CS001','2026-10-01','07:00:00','','pending','pending',NULL,'2026-09-22 15:14:55',NULL),(19,'DLK-197797',4,'KHB801BA83','CK001',1,'CS001','2026-10-09','12:00:00','','pending','pending',NULL,'2026-09-22 15:15:50','2026-09-23 21:20:36'),(20,'DLK-DC85C0',12,'KH07E205C5','CK001',1,'CS001','2026-09-23','13:00:00','','checked_in','pending',NULL,'2026-09-23 01:18:57','2026-09-23 15:13:34'),(21,'DLK-A49167',4,'KHB801BA83','CK001',1,'CS001','2026-09-23','11:00:00','','checked_in','pending',NULL,'2026-09-23 01:28:19','2026-09-23 14:52:01'),(22,'DLK-331B48',12,'KH07E205C5','CK001',1,'CS001','2026-09-23','15:00:00','','checked_in','pending',NULL,'2026-09-23 01:36:38','2026-09-23 23:10:03'),(23,'DLK-F7F24D',4,'KHB801BA83','CK004',10,'CS001','2026-09-27','10:00:00','','pending','pending',NULL,'2026-09-23 01:52:51',NULL),(24,'DLK-18A82E',4,'KH87045B4F','CK004',10,'CS001','2026-10-16','08:00:00','','pending','pending',NULL,'2026-09-23 16:38:09',NULL),(25,'DLK-A955E6',4,'KHB801BA83','CK003',8,'CS001','2026-09-24','15:00:00','','pending','pending',NULL,'2026-09-24 00:07:34',NULL),(26,'DLK-498994',5,'KHC5578C10','CK002',6,'CS001','2026-09-24','16:00:00','','checked_in','pending',NULL,'2026-09-24 13:58:14','2026-09-24 14:00:47'),(27,'DLK-44D6C3',21,'KHFED7A360','CK001',1,'CS001','2026-09-24','07:00:00','','cancelled','pending',NULL,'2026-09-24 14:46:21','2026-09-24 14:57:17'),(28,'DLK-F9C3CF',21,'KHFED7A360','CK001',1,'CS001','2026-09-24','07:00:00','','checked_in','pending',NULL,'2026-09-24 14:57:39','2026-09-24 14:59:33'),(29,'DLK-4E1C6D',4,'KHB801BA83','CK001',1,'CS001','2026-09-24','16:00:00','','checked_in','pending',NULL,'2026-09-24 16:28:16','2026-09-24 16:28:35'),(30,'DLK-01AFBC',NULL,'KH-52DC35','CK001',1,'CS001','2026-09-25','07:31:59','','checked_in','pending',NULL,'2026-09-25 00:31:59',NULL);
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
  `MaDatLich` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `UserID` int DEFAULT NULL,
  `IDKhachHang` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `IDBacSi` int DEFAULT NULL,
  `CoSoID` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `NgayXetNghiem` date NOT NULL,
  `GioXetNghiem` time NOT NULL,
  `GhiChu` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TrangThai` enum('pending','confirmed','checked_in','cancelled','no_show','completed') CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'pending',
  `StatusMail` enum('pending','sent','failed') CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'pending',
  `MaQR` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `CreatedAt` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`IDDatLichXN`),
  UNIQUE KEY `MaDatLich` (`MaDatLich`),
  UNIQUE KEY `MaQR` (`MaQR`),
  KEY `fk_dlxn_user` (`UserID`),
  KEY `idx_dlxn_time` (`CoSoID`,`NgayXetNghiem`,`GioXetNghiem`,`TrangThai`),
  KEY `idx_dlxn_khachhang` (`IDKhachHang`,`NgayXetNghiem`),
  KEY `fk_dlxn_bacsi` (`IDBacSi`),
  CONSTRAINT `fk_dlxn_bacsi` FOREIGN KEY (`IDBacSi`) REFERENCES `bacsi` (`IDBacSi`),
  CONSTRAINT `fk_dlxn_coso` FOREIGN KEY (`CoSoID`) REFERENCES `coso` (`CoSoID`),
  CONSTRAINT `fk_dlxn_khachhang` FOREIGN KEY (`IDKhachHang`) REFERENCES `khachhang` (`IDKhachHang`),
  CONSTRAINT `fk_dlxn_user` FOREIGN KEY (`UserID`) REFERENCES `users` (`UserID`)
) ENGINE=InnoDB AUTO_INCREMENT=24 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `datlichxetnghiem`
--

LOCK TABLES `datlichxetnghiem` WRITE;
/*!40000 ALTER TABLE `datlichxetnghiem` DISABLE KEYS */;
INSERT INTO `datlichxetnghiem` VALUES (1,'DLXN-0936A4',4,'KH14B09585',NULL,'CS001','2026-09-30','09:00:00','','pending','pending',NULL,'2026-09-22 15:00:34',NULL),(2,'DLXN-56DF6E',4,'KH6F6F2D27',NULL,'CS001','2026-10-11','15:00:00','','pending','pending',NULL,'2026-09-22 15:01:57',NULL),(3,'DLXN-D94026',4,'KH53F88089',NULL,'CS001','2026-10-10','07:00:00','','cancelled','pending',NULL,'2026-09-22 15:03:47','2026-09-23 21:23:05'),(4,'DLXN-BE2273',4,'KH14B09585',NULL,'CS001','2026-09-26','08:00:00','','cancelled','pending',NULL,'2026-09-22 15:07:19','2026-09-23 21:44:48'),(5,'DLXN-C91482',4,'KHB801BA83',1,'CS001','2026-10-03','08:00:00','','pending','pending',NULL,'2026-09-22 19:40:26','2026-09-23 22:33:17'),(6,'DLXN-CE8602',6,'KH07E205C5',1,'CS001','2026-09-23','16:00:00','','cancelled','pending',NULL,'2026-09-23 01:20:27','2026-09-23 22:55:53'),(7,'DLXN-4945E0',12,'KH07E205C5',2,'CS001','2026-09-25','15:00:00','','pending','pending',NULL,'2026-09-23 01:22:23',NULL),(8,'DLXN-EB9785',12,'KH07E205C5',1,'CS001','2026-09-26','14:00:00','','pending','pending',NULL,'2026-09-23 01:23:21',NULL),(9,'DLXN-6925CF',12,'KH07E205C5',7,'CS001','2026-09-30','08:00:00','','pending','pending',NULL,'2026-09-23 01:25:55',NULL),(10,'DLXN-1E0066',12,'KH07E205C5',7,'CS001','2026-10-02','08:00:00','','pending','pending',NULL,'2026-09-23 01:30:58',NULL),(11,'DLXN-295058',12,'KH07E205C5',6,'CS001','2026-10-11','09:00:00','','pending','pending',NULL,'2026-09-23 01:31:34','2026-09-23 21:02:32'),(12,'DLXN-6C1DCA',12,'KH07E205C5',6,'CS001','2026-09-23','10:00:00','','checked_in','pending',NULL,'2026-09-23 01:38:37','2026-09-23 22:05:11'),(13,'DLXN-09D6D0',12,'KH07E205C5',7,'CS001','2026-09-27','08:00:00','','pending','pending',NULL,'2026-09-23 01:39:16',NULL),(14,'DLXN-6B154A',12,'KH07E205C5',1,'CS001','2026-09-24','09:00:00','','checked_in','pending',NULL,'2026-09-23 01:48:51','2026-09-24 00:37:38'),(15,'DLXN-535D4A',4,'KHB801BA83',4,'CS001','2026-10-09','08:00:00','','pending','pending',NULL,'2026-09-23 01:54:22',NULL),(16,'DLXN-5C118D',4,'KHB801BA83',4,'CS001','2026-09-24','08:00:00','','checked_in','pending',NULL,'2026-09-24 00:08:02','2026-09-24 01:25:50'),(17,'DLXN-BFAA50',4,'KHB801BA83',3,'CS001','2026-09-24','11:00:00','','checked_in','pending',NULL,'2026-09-24 00:36:41','2026-09-24 01:22:08'),(18,'DLXN-80A79C',5,'KHC5578C10',6,'CS001','2026-09-24','12:00:00','','checked_in','pending',NULL,'2026-09-24 01:39:22','2026-09-24 01:39:51'),(19,'DLXN-672DA8',5,'KHC5578C10',6,'CS001','2026-09-24','15:00:00','','checked_in','pending',NULL,'2026-09-24 01:46:40','2026-09-24 01:47:29'),(20,'DLXN-EDD703',5,'KHC5578C10',6,'CS001','2026-09-24','08:00:00','','checked_in','pending',NULL,'2026-09-24 01:52:57','2026-09-24 01:53:19'),(21,'DLXN-726EDC',5,'KHC5578C10',8,'CS001','2026-09-24','14:00:00','','completed','pending',NULL,'2026-09-24 13:58:44','2026-09-24 14:07:46'),(22,'DLXN-CEBD75',5,'KHC5578C10',2,'CS001','2026-09-24','09:00:00','','completed','pending',NULL,'2026-09-24 14:09:22','2026-09-24 14:12:47'),(23,'DLXN-1C7A20',21,'KHFED7A360',2,'CS001','2026-09-24','08:00:00','','completed','pending',NULL,'2026-09-24 14:58:01','2026-09-24 15:35:27');
/*!40000 ALTER TABLE `datlichxetnghiem` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `hoadon`
--

DROP TABLE IF EXISTS `hoadon`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `hoadon` (
  `MaHoaDon` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `IDKhachHang` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `IDPhieuXetNghiem` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `IDLuotKham` bigint unsigned DEFAULT NULL,
  `NgayTaoHoaDon` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `TongTien` decimal(12,2) NOT NULL DEFAULT '0.00',
  `TrangThaiThanhToan` enum('chua_thanh_toan','da_thanh_toan','hoan_tien','huy') CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'chua_thanh_toan',
  `PhuongThuc` enum('tien_mat','chuyen_khoan','the','khac') CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Status` enum('yes','no') CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'yes',
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
  `IDKetQua` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `IDChiSo` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `GiaTriSo` decimal(15,4) DEFAULT NULL,
  `GiaTriText` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `NguongMinApDung` decimal(15,4) DEFAULT NULL,
  `NguongMaxApDung` decimal(15,4) DEFAULT NULL,
  `GiaTriThamChieu` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `DanhGia` enum('binh_thuong','thap','cao','bat_thuong','chua_danh_gia') CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'chua_danh_gia',
  `GhiChu` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Status` enum('yes','no') CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'yes',
  `CreatedAt` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`IDKetQuaChiSo`),
  UNIQUE KEY `uq_kq_chiso` (`IDKetQua`,`IDChiSo`),
  KEY `fk_kqchiso_chiso` (`IDChiSo`),
  CONSTRAINT `fk_kqchiso_chiso` FOREIGN KEY (`IDChiSo`) REFERENCES `chisoxetnghiem` (`IDChiSo`),
  CONSTRAINT `fk_kqchiso_kq` FOREIGN KEY (`IDKetQua`) REFERENCES `ketquaxetnghiem` (`IDKetQua`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ketquachiso`
--

LOCK TABLES `ketquachiso` WRITE;
/*!40000 ALTER TABLE `ketquachiso` DISABLE KEYS */;
INSERT INTO `ketquachiso` VALUES (3,'KQ1','CS024',NULL,'12,6',NULL,NULL,NULL,'binh_thuong',NULL,'yes','2026-09-24 08:19:04',NULL),(4,'KQ2','CS032',NULL,'123',NULL,NULL,NULL,'binh_thuong',NULL,'yes','2026-09-24 14:06:26',NULL),(5,'KQ2','CS033',NULL,'123 bình thường',NULL,NULL,NULL,'binh_thuong',NULL,'yes','2026-09-24 14:06:26',NULL),(6,'KQ3','CS057',NULL,'0.9',NULL,NULL,NULL,'binh_thuong',NULL,'yes','2026-09-24 14:11:56',NULL),(7,'KQ4','CS056',NULL,'5',NULL,NULL,NULL,'bat_thuong',NULL,'yes','2026-09-24 15:05:56',NULL);
/*!40000 ALTER TABLE `ketquachiso` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ketquaxetnghiem`
--

DROP TABLE IF EXISTS `ketquaxetnghiem`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ketquaxetnghiem` (
  `IDKetQua` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `IDCTPhieu` bigint unsigned NOT NULL,
  `IDMau` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `IDKTV` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `ThoiGianBatDau` datetime DEFAULT NULL,
  `ThoiGianHoanThanh` datetime DEFAULT NULL,
  `ThoiGianNhap` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `KetQuaTongQuat` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TrangThai` enum('dang_thuc_hien','cho_duyet','da_duyet','can_lam_lai','hoan_tat') CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'cho_duyet',
  `IDBacSiDuyet` int DEFAULT NULL,
  `ThoiGianDuyet` datetime DEFAULT NULL,
  `KetLuanBacSi` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
  `GhiChu` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
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
INSERT INTO `ketquaxetnghiem` VALUES ('KQ1',1,'M179021485939698','KTV01','2026-09-24 08:19:04','2026-09-24 08:19:04','2026-09-24 08:19:04',NULL,'da_duyet',1,'2026-09-24 08:50:32','êrb\nLời khuyên: fewfw',''),('KQ2',2,'M179025859845013','KTV01','2026-09-24 14:06:26','2026-09-24 14:06:26','2026-09-24 14:06:26',NULL,'da_duyet',1,'2026-09-24 14:07:46','Bình thường\nLời khuyên: Giữ ấm cơ thể',''),('KQ3',3,'M179025906283337','KTV01','2026-09-24 14:11:56','2026-09-24 14:11:56','2026-09-24 14:11:56',NULL,'da_duyet',1,'2026-09-24 14:12:47','Bình thường',''),('KQ4',4,'M179026222395394','KTV01','2026-09-24 15:05:56','2026-09-24 15:16:33','2026-09-24 15:05:56',NULL,'da_duyet',2,'2026-09-24 15:35:27','Khám lại','');
/*!40000 ALTER TABLE `ketquaxetnghiem` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `khachhang`
--

DROP TABLE IF EXISTS `khachhang`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `khachhang` (
  `IDKhachHang` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `TenKhachHang` varchar(120) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `NgaySinh` date DEFAULT NULL,
  `SoDienThoai` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `GioiTinh` enum('nam','nu','khac') CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `CCCD` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `DiaChi` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Email` varchar(191) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Status` enum('yes','no') CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'yes',
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
INSERT INTO `khachhang` VALUES ('KH-52DC35','Hà Văn Dũng','1980-04-05','0923423456','nam',NULL,NULL,'dung@gmail.com','yes','2026-09-25 00:31:59',NULL),('KH001','Bùi Tuấn Anh','1985-10-12','0909123456','nam','001085001234','Quận Cầu Giấy, Hà Nội','buituananh85@gmail.com','yes','2026-09-19 08:07:07',NULL),('KH002','Nguyễn Thu Hà','1992-03-25','0938765432','nu','001192005678','Quận Ba Đình, Hà Nội','nguyenthuha.92@gmail.com','yes','2026-09-19 08:07:07',NULL),('KH07E205C5','Haha',NULL,'0999876542','khac',NULL,NULL,'haha@gmail.com','yes','2026-09-23 01:18:57',NULL),('KH14B09585','Hello',NULL,'0334586792','nam',NULL,NULL,'hello@gmail.com','yes','2026-09-19 13:03:04',NULL),('KH1F7A16A3','Nguyễn Văn Hùng','2019-02-10','0334586792','nam',NULL,NULL,'hello@gmail.com','yes','2026-09-22 14:31:51',NULL),('KH53F88089','Nguyễn Văn Hùng','2019-02-10','0334586792','nam',NULL,NULL,'hello@gmail.com','yes','2026-09-22 15:03:47',NULL),('KH6F6F2D27','Nguyễn Văn Hưng','2020-04-12','0334586792','nam',NULL,NULL,'hello@gmail.com','yes','2026-09-22 15:01:57',NULL),('KH84D04A','Hihi','1999-02-11','0123456785','nu',NULL,'123, Ngõ 5','123@gmail.com','yes','2026-09-23 02:40:38',NULL),('KH87045B4F','Nguyễn Thu Minh','1995-01-25','064536233','nu',NULL,NULL,'hello@gmail.com','yes','2026-09-23 16:38:08',NULL),('KHAB94FB','Nguyễn Thị Hà','2003-04-23','0978345623','nu',NULL,'Abc, Đường 65','ha@gmail.com','yes','2026-09-23 02:09:45',NULL),('KHB801BA83','Hello',NULL,'0334586792','khac',NULL,NULL,'hello@gmail.com','yes','2026-09-22 15:15:50',NULL),('KHC5578C10','Nguyễn Nhật Minh',NULL,'0947456273','khac',NULL,NULL,'nnm@gmail.com','yes','2026-09-24 01:39:22',NULL),('KHC5BC4D5E','Nguyễn Nhật Minh',NULL,'0947456273','nam',NULL,NULL,'nnm@gmail.com','yes','2026-09-20 00:49:55',NULL),('KHD8529D','Nguyễn Thị Giang','1996-09-12','0837832322','nu',NULL,'Hà Nội','','yes','2026-09-24 23:27:23',NULL),('KHF7E08285','Vũ Minh Phương',NULL,'0745723452','nu',NULL,NULL,'phuong@gmail.com','yes','2026-09-24 14:45:48',NULL),('KHFD6C8512','Haha',NULL,'0999876542','nu',NULL,NULL,'haha@gmail.com','yes','2026-09-22 08:20:53',NULL),('KHFED7A360','Vũ Minh Phương',NULL,'0745723452','khac',NULL,NULL,'phuong@gmail.com','yes','2026-09-24 14:46:21',NULL);
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
  `TrieuChung` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
  `TienSuBenh` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
  `ChanDoan` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
  `KetLuan` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
  `HuongDieuTri` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
  `ThoiGianKham` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`IDKham`),
  UNIQUE KEY `IDLuotKham` (`IDLuotKham`),
  KEY `fk_kham_bacsi` (`IDBacSi`),
  CONSTRAINT `fk_kham_bacsi` FOREIGN KEY (`IDBacSi`) REFERENCES `bacsi` (`IDBacSi`),
  CONSTRAINT `fk_kham_luot` FOREIGN KEY (`IDLuotKham`) REFERENCES `luotkham` (`IDLuotKham`)
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `kham`
--

LOCK TABLES `kham` WRITE;
/*!40000 ALTER TABLE `kham` DISABLE KEYS */;
INSERT INTO `kham` VALUES (1,1,8,NULL,NULL,NULL,NULL,NULL,'2026-09-23 14:56:57',NULL),(2,2,1,NULL,NULL,NULL,NULL,NULL,'2026-09-23 15:07:48',NULL),(3,3,1,'Đau đầu ','Không có ','Abcd','abcd','Ngủ đủ giấc','2026-09-23 15:15:11','2026-09-23 15:15:39'),(4,6,1,'êw','fef ','vẻv','vẻvrcd','ẻveve','2026-09-24 00:32:25',NULL),(7,7,1,'cưce','ewv','evwv','vewvw','Ăn uống lành mạnh','2026-09-24 00:45:32',NULL),(9,8,6,'Đau bụng','không có','Đau bụng','Đau bụng','Ăn chín uống sôi','2026-09-24 14:01:38',NULL),(11,9,1,'asd','','asd','asd','','2026-09-24 15:00:32',NULL),(13,11,1,'Đau bụng','','','','','2026-09-25 00:32:58',NULL);
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
  `IDPhong` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Ngay` date NOT NULL,
  `Ca` enum('Sang','Chieu','Toi','TuyChinh') CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'TuyChinh',
  `GioBatDau` time NOT NULL,
  `GioKetThuc` time NOT NULL,
  `NguonTao` enum('bacsi','admin') CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'bacsi',
  `NguoiTaoUserID` int DEFAULT NULL,
  `TrangThai` enum('duoc_duyet','huy') CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'duoc_duyet',
  `GhiChu` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
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
  `IDXetNghiem` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `TenXetNghiem` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `ChuyenKhoaID` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Loai` varchar(80) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `MoTa` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Gia` decimal(12,2) NOT NULL DEFAULT '0.00',
  `LoaiMauMacDinh` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ThoiGianDuKienPhut` int DEFAULT NULL,
  `Status` enum('yes','no') CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'yes',
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
INSERT INTO `loaixetnghiem` VALUES ('XN001','Tổng phân tích tế bào máu','CK001','Huyết học','Đánh giá các chỉ số tế bào máu ngoại vi',120000.00,'Máu toàn phần',30,'yes'),('XN002','Đường huyết lúc đói','CK001','Sinh hóa','Định lượng glucose máu lúc đói',60000.00,'Máu',15,'yes'),('XN003','Chức năng gan (AST, ALT)','CK001','Sinh hóa','Đánh giá men gan AST và ALT',150000.00,'Máu',30,'yes'),('XN004','Troponin I','CK002','Tim mạch','Định lượng Troponin I hỗ trợ đánh giá tổn thương cơ tim',180000.00,'Máu',30,'yes'),('XN005','CK-MB','CK002','Tim mạch','Định lượng CK-MB hỗ trợ đánh giá tổn thương cơ tim',150000.00,'Máu',30,'yes'),('XN006','NT-proBNP','CK002','Tim mạch','Định lượng NT-proBNP hỗ trợ đánh giá suy tim',250000.00,'Máu',45,'yes'),('XN007','Mỡ máu toàn phần','CK002','Tim mạch','Định lượng Cholesterol, HDL-C, LDL-C và Triglyceride',200000.00,'Máu',45,'yes'),('XN008','Test nhanh liên cầu khuẩn nhóm A','CK003','Tai mũi họng','Phát hiện nhanh kháng nguyên liên cầu khuẩn nhóm A từ dịch họng',120000.00,'Dịch họng',20,'yes'),('XN009','PCR cúm A/B','CK003','Tai mũi họng','Phát hiện virus cúm A và cúm B bằng kỹ thuật PCR',280000.00,'Dịch tỵ hầu',120,'yes'),('XN010','IgE toàn phần','CK003','Miễn dịch','Định lượng IgE toàn phần hỗ trợ đánh giá cơ địa dị ứng',180000.00,'Máu',60,'yes'),('XN011','Cấy dịch họng','CK003','Vi sinh','Nuôi cấy và định danh vi khuẩn từ mẫu dịch họng',220000.00,'Dịch họng',1440,'yes'),('XN012','Bilirubin toàn phần','CK004','Sinh hóa nhi','Định lượng bilirubin toàn phần, thường dùng trong đánh giá vàng da ở trẻ',80000.00,'Máu',30,'yes'),('XN013','CRP định lượng','CK004','Viêm nhiễm','Đánh giá tình trạng viêm và nhiễm trùng',100000.00,'Máu',30,'yes'),('XN014','Tổng phân tích tế bào máu trẻ em','CK004','Huyết học nhi','Đánh giá các chỉ số huyết học cơ bản ở trẻ em',120000.00,'Máu toàn phần',30,'yes'),('XN015','Đường huyết','CK004','Sinh hóa nhi','Định lượng glucose máu hỗ trợ đánh giá rối loạn đường huyết ở trẻ',60000.00,'Máu',15,'yes'),('XN016','HbA1c','CK005','Hóa sinh','Đánh giá đường huyết trung bình trong khoảng 2-3 tháng',150000.00,'Máu toàn phần',60,'yes'),('XN017','Creatinine','CK005','Hóa sinh','Đánh giá chức năng thận thông qua nồng độ creatinine',80000.00,'Máu',30,'yes'),('XN018','Tổng phân tích nước tiểu','CK005','Nước tiểu','Phân tích các chỉ số hóa học và tế bào trong nước tiểu',90000.00,'Nước tiểu',30,'yes');
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
  `IDKhachHang` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `IDBacSi` int NOT NULL,
  `IDPhong` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `SoThuTu` int DEFAULT NULL,
  `ThoiGianTiepNhan` datetime DEFAULT NULL,
  `ThoiGianBatDau` datetime DEFAULT NULL,
  `ThoiGianKetThuc` datetime DEFAULT NULL,
  `TrangThai` enum('da_tiep_nhan','cho_kham','da_den_luot','cho_goi_lai','dang_kham','da_chi_dinh_xn','moi_doc_kq','dang_tu_van_kq','bo_luot','hoan_tat') CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'da_tiep_nhan',
  PRIMARY KEY (`IDLuotKham`),
  UNIQUE KEY `IDDatLichKham` (`IDDatLichKham`),
  KEY `fk_luotkham_khachhang` (`IDKhachHang`),
  KEY `fk_luotkham_phong` (`IDPhong`),
  KEY `idx_luotkham_queue` (`IDBacSi`,`TrangThai`,`SoThuTu`),
  CONSTRAINT `fk_luotkham_bacsi` FOREIGN KEY (`IDBacSi`) REFERENCES `bacsi` (`IDBacSi`),
  CONSTRAINT `fk_luotkham_datlich` FOREIGN KEY (`IDDatLichKham`) REFERENCES `datlichkham` (`IDDatLichKham`),
  CONSTRAINT `fk_luotkham_khachhang` FOREIGN KEY (`IDKhachHang`) REFERENCES `khachhang` (`IDKhachHang`),
  CONSTRAINT `fk_luotkham_phong` FOREIGN KEY (`IDPhong`) REFERENCES `phong` (`IDPhong`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `luotkham`
--

LOCK TABLES `luotkham` WRITE;
/*!40000 ALTER TABLE `luotkham` DISABLE KEYS */;
INSERT INTO `luotkham` VALUES (1,13,'KH14B09585',8,NULL,1,'2026-09-23 07:31:51','2026-09-23 14:56:57',NULL,'dang_kham'),(2,21,'KHB801BA83',1,NULL,1,'2026-09-23 14:52:01','2026-09-23 15:07:48',NULL,'cho_goi_lai'),(3,20,'KH07E205C5',1,NULL,2,'2026-09-23 15:13:34','2026-09-23 15:15:11','2026-09-23 15:16:16','hoan_tat'),(4,22,'KH07E205C5',1,NULL,3,'2026-09-23 23:10:19',NULL,NULL,'da_den_luot'),(5,9,'KH14B09585',3,NULL,1,'2026-09-24 00:23:53','2026-09-24 00:24:41',NULL,'dang_kham'),(6,7,'KH14B09585',1,NULL,1,'2026-09-24 00:31:15','2026-09-24 00:32:13','2026-09-24 00:34:41','hoan_tat'),(7,11,'KH14B09585',1,NULL,2,'2026-09-24 00:43:54','2026-09-24 00:45:13','2026-09-24 00:46:35','hoan_tat'),(8,26,'KHC5578C10',6,NULL,1,'2026-09-24 14:00:47','2026-09-24 14:01:05','2026-09-24 14:01:44','hoan_tat'),(9,28,'KHFED7A360',1,NULL,3,'2026-09-24 14:59:33','2026-09-24 14:59:52','2026-09-24 15:00:38','hoan_tat'),(10,29,'KHB801BA83',1,NULL,4,'2026-09-24 16:28:35',NULL,NULL,'da_tiep_nhan'),(11,30,'KH-52DC35',1,NULL,1,'2026-09-25 00:31:59','2026-09-25 00:32:39','2026-09-25 00:32:58','hoan_tat');
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
  `IDKhachHang` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `IDPhong` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `SoThuTu` int DEFAULT NULL,
  `ThoiGianTiepNhan` datetime DEFAULT NULL,
  `ThoiGianBatDau` datetime DEFAULT NULL,
  `ThoiGianKetThuc` datetime DEFAULT NULL,
  `TrangThai` enum('da_tiep_nhan','cho_xet_nghiem','da_den_luot','dang_lay_mau','da_lay_mau','ktv_tiep_nhan','dang_xet_nghiem','cho_duyet_kq','da_co_kq','moi_doc_kq','dang_tu_van_kq','hoan_tat','huy') CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'da_tiep_nhan',
  PRIMARY KEY (`IDLuotXetNghiem`),
  UNIQUE KEY `IDDatLichXN` (`IDDatLichXN`),
  KEY `fk_luotxn_khachhang` (`IDKhachHang`),
  KEY `fk_luotxn_phong` (`IDPhong`),
  KEY `idx_luotxn_queue` (`TrangThai`,`SoThuTu`),
  CONSTRAINT `fk_luotxn_datlich` FOREIGN KEY (`IDDatLichXN`) REFERENCES `datlichxetnghiem` (`IDDatLichXN`),
  CONSTRAINT `fk_luotxn_khachhang` FOREIGN KEY (`IDKhachHang`) REFERENCES `khachhang` (`IDKhachHang`),
  CONSTRAINT `fk_luotxn_phong` FOREIGN KEY (`IDPhong`) REFERENCES `phong` (`IDPhong`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `luotxetnghiem`
--

LOCK TABLES `luotxetnghiem` WRITE;
/*!40000 ALTER TABLE `luotxetnghiem` DISABLE KEYS */;
INSERT INTO `luotxetnghiem` VALUES (1,6,'KH07E205C5',NULL,1,'2026-09-23 21:58:06',NULL,NULL,'huy'),(2,12,'KH07E205C5',NULL,2,'2026-09-23 23:06:37',NULL,NULL,'da_den_luot'),(3,14,'KH07E205C5',NULL,1,'2026-09-24 00:50:43','2026-09-24 01:05:09',NULL,'dang_lay_mau'),(4,17,'KHB801BA83',NULL,2,'2026-09-24 01:22:08','2026-09-24 01:22:29',NULL,'dang_lay_mau'),(5,16,'KHB801BA83',NULL,3,'2026-09-24 01:25:50','2026-09-24 01:26:27',NULL,'dang_lay_mau'),(6,18,'KHC5578C10',NULL,4,'2026-09-24 01:39:51','2026-09-24 01:40:10',NULL,'ktv_tiep_nhan'),(7,19,'KHC5578C10',NULL,5,'2026-09-24 01:47:29','2026-09-24 01:47:53',NULL,'ktv_tiep_nhan'),(8,20,'KHC5578C10',NULL,6,'2026-09-24 01:53:19','2026-09-24 01:54:05',NULL,'da_co_kq'),(9,21,'KHC5578C10',NULL,7,'2026-09-24 14:02:42','2026-09-24 14:03:14',NULL,'hoan_tat'),(10,22,'KHC5578C10',NULL,8,'2026-09-24 14:09:59','2026-09-24 14:11:00',NULL,'hoan_tat'),(11,23,'KHFED7A360',NULL,9,'2026-09-24 15:03:35','2026-09-24 15:03:41',NULL,'hoan_tat');
/*!40000 ALTER TABLE `luotxetnghiem` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `maubenhpham`
--

DROP TABLE IF EXISTS `maubenhpham`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `maubenhpham` (
  `IDMau` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `IDCTPhieu` bigint unsigned NOT NULL,
  `MaBarcode` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `LoaiMau` varchar(80) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `IDBacSiLayMau` int DEFAULT NULL,
  `ThoiGianLayMau` datetime DEFAULT NULL,
  `TrangThai` enum('moi_tao','da_lay_mau','da_ban_giao','ktv_tiep_nhan','dang_xu_ly','hoan_tat','tu_choi_mau','huy') CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'moi_tao',
  `GhiChu` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
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
INSERT INTO `maubenhpham` VALUES ('M179021485939698',1,'179021485939698','Máu toàn phần',6,'2026-09-24 01:54:19','hoan_tat',''),('M179025859845013',2,'179025859845013','Máu toàn phần',8,'2026-09-24 14:03:18','hoan_tat',''),('M179025906283337',3,'179025906283337','Máu toàn phần',2,'2026-09-24 14:11:02','hoan_tat',''),('M179026222395394',4,'179026222395394','Máu toàn phần',2,'2026-09-24 15:03:43','hoan_tat','');
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
  `IDChiSo` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `GioiTinhApDung` enum('tatca','nam','nu') CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'tatca',
  `TuoiMin` smallint unsigned DEFAULT NULL,
  `TuoiMax` smallint unsigned DEFAULT NULL,
  `GiaTriMin` decimal(15,4) DEFAULT NULL,
  `GiaTriMax` decimal(15,4) DEFAULT NULL,
  `GiaTriTextBinhThuong` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `GhiChu` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Status` enum('yes','no') CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'yes',
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
  `IDNhanVien` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `TenNhanVien` varchar(120) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `ViTri` enum('bacsi','letan','ktv','admin','dieu_duong','khac') CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `SoDienThoai` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Email` varchar(191) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `CoSoID` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Status` enum('yes','no') CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'yes',
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
INSERT INTO `nhanvien` VALUES ('NV001','Hoàng Khắc Nam','bacsi',NULL,'hkn@biomedic.vn','CS001','yes','2026-09-24 01:15:44'),('NV002','Trần Mỹ Dung','bacsi',NULL,'tmd@biomedic.vn','CS001','yes','2026-09-24 01:15:44'),('NV003','Lê Văn Cường','bacsi',NULL,'lvc@biomedic.vn','CS001','yes','2026-09-24 01:15:44'),('NV004','Phạm Thị Mai','bacsi',NULL,'ptm@biomedic.vn','CS001','yes','2026-09-24 01:15:44'),('NV005','Nguyễn Minh Tuấn','bacsi',NULL,'nmt@biomedic.vn','CS001','yes','2026-09-24 01:15:44'),('NV006','Đặng Thu Trang','bacsi',NULL,'dtt@biomedic.vn','CS001','yes','2026-09-24 01:15:44'),('NV007','Vũ Hoàng Long','bacsi',NULL,'vhl@biomedic.vn','CS001','yes','2026-09-24 01:15:44'),('NV008','Nguyễn Thị Hạnh','bacsi',NULL,'nth@biomedic.vn','CS001','yes','2026-09-24 01:15:44'),('NV009','Phạm Quốc Bảo','bacsi',NULL,'pqb@biomedic.vn','CS001','yes','2026-09-24 01:15:44'),('NV010','Trần Ngọc Anh','bacsi',NULL,'tna@biomedic.vn','CS001','yes','2026-09-24 01:15:44'),('NV011','Kỹ thuật viên xét nghiệm','ktv',NULL,'ktv@biomedic.vn','CS001','yes','2026-09-24 01:15:44'),('NV012','Lễ tân','letan',NULL,'letan@biomedic.vn','CS001','yes','2026-09-24 01:15:44');
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
  `TokenHash` char(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
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
  `IDPhieuXetNghiem` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `IDKhachHang` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `IDKham` bigint unsigned DEFAULT NULL,
  `IDLuotXetNghiem` bigint unsigned DEFAULT NULL,
  `IDBacSiChiDinh` int DEFAULT NULL,
  `IDBacSiPhuTrach` int DEFAULT NULL,
  `NgayTao` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `TongTien` decimal(12,2) NOT NULL DEFAULT '0.00',
  `TrangThaiThanhToan` enum('chua_thanh_toan','da_thanh_toan','mien_phi') CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'chua_thanh_toan',
  `TrangThai` enum('moi_tao','cho_lay_mau','da_lay_mau','ktv_tiep_nhan','dang_xet_nghiem','cho_duyet','da_co_kq','hoan_tat','huy') CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'moi_tao',
  `GhiChu` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
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
INSERT INTO `phieuxetnghiem` VALUES ('PXN10','KHC5578C10',NULL,10,2,NULL,'2026-09-24 14:11:02',0.00,'chua_thanh_toan','da_co_kq',NULL),('PXN11','KHFED7A360',NULL,11,2,NULL,'2026-09-24 15:03:43',0.00,'chua_thanh_toan','da_co_kq',NULL),('PXN8','KHC5578C10',NULL,8,6,NULL,'2026-09-24 01:54:19',0.00,'chua_thanh_toan','da_co_kq',NULL),('PXN9','KHC5578C10',NULL,9,8,NULL,'2026-09-24 14:03:18',0.00,'chua_thanh_toan','da_co_kq',NULL);
/*!40000 ALTER TABLE `phieuxetnghiem` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `phong`
--

DROP TABLE IF EXISTS `phong`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `phong` (
  `IDPhong` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `CoSoID` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `IDChuyenKhoa` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TenPhong` varchar(120) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `LoaiPhong` enum('kham','lay_mau','xet_nghiem','tu_van','khac') CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Tang` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TrangThai` enum('active','inactive','maintenance') CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'active',
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
INSERT INTO `phong` VALUES ('P001','CS001','CK001','BS Hoàng Khắc Nam','kham','Tầng 1','active'),('P002','CS001','CK005','BS Trần Mỹ Dung','xet_nghiem','Tầng 1','active'),('P003','CS001','CK001','BS Lê Văn Cường','kham','Tầng 1','active'),('P004','CS001','CK005','BS Phạm Thị Mai','xet_nghiem','Tầng 1','active'),('P005','CS001','CK002','BS Nguyễn Minh Tuấn','kham','Tầng 2','active'),('P006','CS001','CK002','BS Đặng Thu Trang','kham','Tầng 2','active'),('P007','CS001','CK003','BS Vũ Hoàng Long','kham','Tầng 2','active'),('P008','CS001','CK003','BS Nguyễn Thị Hạnh','kham','Tầng 2','active'),('P009','CS001','CK004','BS Phạm Quốc Bảo','kham','Tầng 3','active'),('P010','CS001','CK004','BS Trần Ngọc Anh','kham','Tầng 3','active');
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
  `LoaiThongBao` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `TieuDe` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `NoiDung` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `LoaiDoiTuong` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `IDDoiTuong` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `DaDoc` tinyint(1) NOT NULL DEFAULT '0',
  `ThoiGianTao` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `ThoiGianDoc` datetime DEFAULT NULL,
  PRIMARY KEY (`IDThongBao`),
  KEY `idx_thongbao_user` (`UserIDNhan`,`DaDoc`,`ThoiGianTao`),
  CONSTRAINT `fk_thongbao_user` FOREIGN KEY (`UserIDNhan`) REFERENCES `users` (`UserID`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=17 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `thongbao`
--

LOCK TABLES `thongbao` WRITE;
/*!40000 ALTER TABLE `thongbao` DISABLE KEYS */;
INSERT INTO `thongbao` VALUES (1,4,'kham_benh','Đã đến lượt khám','Vui lòng di chuyển vào phòng khám để gặp bác sĩ.',NULL,NULL,1,'2026-09-23 14:56:45','2026-09-23 16:33:14'),(2,4,'kham_benh','Đã đến lượt khám','Vui lòng di chuyển vào phòng khám để gặp bác sĩ.',NULL,NULL,1,'2026-09-23 14:56:56','2026-09-23 16:33:11'),(3,4,'kham_benh','Đã đến lượt khám!','Mời bạn di chuyển vào Phòng khám để bác sĩ thăm khám.','luotkham','6',1,'2026-09-24 00:31:29','2026-09-24 00:31:48'),(4,4,'kham_benh','Đã đến lượt khám!','Mời bạn di chuyển vào Phòng khám để bác sĩ thăm khám.','luotkham','7',1,'2026-09-24 00:44:15','2026-09-24 00:44:28'),(5,12,'xet_nghiem','Đã đến lượt lấy mẫu!','Mời bạn di chuyển vào Phòng lấy mẫu để thực hiện lấy mẫu xét nghiệm.','luotxetnghiem','3',1,'2026-09-24 00:50:45','2026-09-24 00:51:19'),(6,4,'xet_nghiem','Đã đến lượt lấy mẫu!','Mời bạn di chuyển vào Phòng lấy mẫu để thực hiện lấy mẫu xét nghiệm.','luotxetnghiem','4',1,'2026-09-24 01:22:23','2026-09-24 08:58:07'),(7,4,'xet_nghiem','Đã đến lượt lấy mẫu!','Mời bạn di chuyển vào Phòng lấy mẫu để thực hiện lấy mẫu xét nghiệm.','luotxetnghiem','5',1,'2026-09-24 01:26:25','2026-09-24 08:58:07'),(8,5,'xet_nghiem','Đã đến lượt lấy mẫu!','Mời bạn di chuyển vào Phòng lấy mẫu để thực hiện lấy mẫu xét nghiệm.','luotxetnghiem','6',1,'2026-09-24 01:40:08','2026-09-24 01:42:07'),(9,5,'xet_nghiem','Đã đến lượt lấy mẫu!','Mời bạn di chuyển vào Phòng lấy mẫu để thực hiện lấy mẫu xét nghiệm.','luotxetnghiem','7',1,'2026-09-24 01:47:50','2026-09-24 07:54:07'),(10,5,'xet_nghiem','Đã đến lượt lấy mẫu!','Mời bạn di chuyển vào Phòng lấy mẫu để thực hiện lấy mẫu xét nghiệm.','luotxetnghiem','8',1,'2026-09-24 01:54:02','2026-09-24 07:54:07'),(11,5,'kham_benh','Đã đến lượt khám!','Mời bạn di chuyển vào Phòng khám để bác sĩ thăm khám.','luotkham','8',1,'2026-09-24 14:01:00','2026-09-24 14:02:02'),(12,5,'xet_nghiem','Đã đến lượt lấy mẫu!','Mời bạn di chuyển vào Phòng lấy mẫu để thực hiện lấy mẫu xét nghiệm.','luotxetnghiem','9',1,'2026-09-24 14:03:12','2026-09-24 14:06:53'),(13,5,'xet_nghiem','Đã đến lượt lấy mẫu!','Mời bạn di chuyển vào Phòng lấy mẫu để thực hiện lấy mẫu xét nghiệm.','luotxetnghiem','10',0,'2026-09-24 14:10:56',NULL),(14,21,'kham_benh','Đã đến lượt khám!','Mời bạn di chuyển vào Phòng khám để bác sĩ thăm khám.','luotkham','9',1,'2026-09-24 14:59:49','2026-09-24 15:00:56'),(15,21,'xet_nghiem','Đã đến lượt lấy mẫu!','Mời bạn di chuyển vào Phòng lấy mẫu để thực hiện lấy mẫu xét nghiệm.','luotxetnghiem','11',1,'2026-09-24 15:03:31','2026-09-24 15:09:46'),(16,21,'xet_nghiem','Đã đến lượt lấy mẫu!','Mời bạn di chuyển vào Phòng lấy mẫu để thực hiện lấy mẫu xét nghiệm.','luotxetnghiem','11',1,'2026-09-24 15:03:38','2026-09-24 15:09:46');
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
  `IDKhachHang` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `LoaiDoiTuong` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `IDDoiTuong` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `HanhDong` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `TrangThaiCu` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TrangThaiMoi` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `UserIDThucHien` int DEFAULT NULL,
  `NguonThucHien` enum('user','system') CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'user',
  `ThoiGian` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `MoTa` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `IPAddress` varchar(45) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`IDTruyVet`),
  KEY `fk_truyvet_user` (`UserIDThucHien`),
  KEY `idx_truyvet_khachhang` (`IDKhachHang`,`ThoiGian`),
  KEY `idx_truyvet_doituong` (`LoaiDoiTuong`,`IDDoiTuong`,`ThoiGian`),
  CONSTRAINT `fk_truyvet_khachhang` FOREIGN KEY (`IDKhachHang`) REFERENCES `khachhang` (`IDKhachHang`),
  CONSTRAINT `fk_truyvet_user` FOREIGN KEY (`UserIDThucHien`) REFERENCES `users` (`UserID`)
) ENGINE=InnoDB AUTO_INCREMENT=80 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `truyvet`
--

LOCK TABLES `truyvet` WRITE;
/*!40000 ALTER TABLE `truyvet` DISABLE KEYS */;
INSERT INTO `truyvet` VALUES (1,'KHB801BA83','luotkham','2','Bác sĩ gọi vào phòng',NULL,NULL,NULL,'user','2026-09-23 14:54:34',NULL,NULL),(2,'KH14B09585','luotkham','1','Bác sĩ gọi vào phòng',NULL,NULL,NULL,'user','2026-09-23 14:56:45',NULL,NULL),(3,'KH14B09585','luotkham','1','Bệnh nhân vắng mặt, chuyển trạng thái chờ gọi lại',NULL,NULL,NULL,'user','2026-09-23 14:56:54',NULL,NULL),(4,'KH14B09585','luotkham','1','Bác sĩ gọi vào phòng',NULL,NULL,NULL,'user','2026-09-23 14:56:56',NULL,NULL),(5,'KH14B09585','luotkham','1','Bác sĩ bắt đầu khám',NULL,NULL,NULL,'user','2026-09-23 14:56:57',NULL,NULL),(6,'KHB801BA83','luotkham','2','Bác sĩ bắt đầu khám',NULL,NULL,NULL,'user','2026-09-23 15:07:48',NULL,NULL),(7,'KH07E205C5','luotkham','3','Bác sĩ gọi vào phòng',NULL,NULL,NULL,'user','2026-09-23 15:14:11',NULL,NULL),(8,'KH07E205C5','luotkham','3','Bác sĩ bắt đầu khám',NULL,NULL,NULL,'user','2026-09-23 15:15:11',NULL,NULL),(9,'KH07E205C5','luotkham','3','Bác sĩ hoàn tất khám',NULL,NULL,NULL,'user','2026-09-23 15:16:16',NULL,NULL),(10,'KHB801BA83','datlichkham','19','Khách hàng thay đổi lịch hẹn',NULL,NULL,NULL,'user','2026-09-23 21:20:36',NULL,NULL),(11,'KHB801BA83','datlichxetnghiem','5','Khách hàng thay đổi lịch hẹn',NULL,NULL,NULL,'user','2026-09-23 21:21:16',NULL,NULL),(12,'KH53F88089','datlichxetnghiem','3','Khách hàng hủy lịch hẹn',NULL,NULL,NULL,'user','2026-09-23 21:23:05',NULL,NULL),(13,'KH14B09585','datlichxetnghiem','4','Khách hàng hủy lịch hẹn',NULL,NULL,NULL,'user','2026-09-23 21:44:48',NULL,NULL),(14,'KH14B09585','datlichkham','15','Khách hàng hủy lịch hẹn',NULL,NULL,NULL,'user','2026-09-23 21:57:16',NULL,NULL),(15,'KHB801BA83','datlichxetnghiem','5','Khách hàng thay đổi lịch hẹn',NULL,NULL,NULL,'user','2026-09-23 22:33:17',NULL,NULL),(16,'KH14B09585','luotkham','5','Bác sĩ gọi bệnh nhân vào phòng',NULL,NULL,NULL,'user','2026-09-24 00:23:57',NULL,NULL),(17,'KH14B09585','luotkham','5','Bác sĩ bắt đầu khám bệnh',NULL,NULL,NULL,'user','2026-09-24 00:24:41',NULL,NULL),(18,'KH14B09585','luotkham','6','Bác sĩ gọi bệnh nhân vào phòng',NULL,NULL,NULL,'system','2026-09-24 00:31:29',NULL,NULL),(19,'KH14B09585','luotkham','6','Bác sĩ bắt đầu khám bệnh',NULL,NULL,NULL,'user','2026-09-24 00:32:13',NULL,NULL),(20,'KH14B09585','luotkham','6','Bác sĩ hoàn tất khám',NULL,NULL,NULL,'user','2026-09-24 00:34:41',NULL,NULL),(21,'KH07E205C5','luotxetnghiem','3','Bác sĩ/KTV gọi bệnh nhân vào lấy mẫu',NULL,NULL,NULL,'system','2026-09-24 00:38:14',NULL,NULL),(22,'KH07E205C5','luotxetnghiem','3','Nhân viên gọi bệnh nhân vào lấy mẫu',NULL,NULL,NULL,'system','2026-09-24 00:41:56',NULL,NULL),(23,'KH14B09585','luotkham','7','Bác sĩ gọi bệnh nhân vào phòng',NULL,NULL,NULL,'system','2026-09-24 00:44:15',NULL,NULL),(24,'KH14B09585','luotkham','7','Bác sĩ bắt đầu khám bệnh',NULL,NULL,NULL,'user','2026-09-24 00:45:13',NULL,NULL),(25,'KH14B09585','luotkham','7','Bác sĩ hoàn tất khám',NULL,NULL,NULL,'user','2026-09-24 00:46:35',NULL,NULL),(26,'KH07E205C5','luotxetnghiem','3','Nhân viên gọi bệnh nhân vào lấy mẫu',NULL,NULL,NULL,'system','2026-09-24 00:46:42',NULL,NULL),(27,'KH07E205C5','luotxetnghiem','3','Nhân viên gọi bệnh nhân vào lấy mẫu',NULL,NULL,NULL,'system','2026-09-24 00:48:08',NULL,NULL),(28,'KH07E205C5','luotxetnghiem','3','Bác sĩ/KTV gọi bệnh nhân vào lấy mẫu',NULL,NULL,NULL,'system','2026-09-24 00:50:45',NULL,NULL),(29,'KH07E205C5','luotxetnghiem','3','Bác sĩ/KTV bắt đầu lấy mẫu',NULL,NULL,NULL,'user','2026-09-24 01:05:09',NULL,NULL),(30,'KHB801BA83','luotxetnghiem','4','Bác sĩ/KTV gọi bệnh nhân vào lấy mẫu',NULL,NULL,NULL,'system','2026-09-24 01:22:23',NULL,NULL),(31,'KHB801BA83','luotxetnghiem','4','Bác sĩ/KTV bắt đầu lấy mẫu',NULL,NULL,NULL,'user','2026-09-24 01:22:29',NULL,NULL),(32,'KHB801BA83','luotxetnghiem','5','Bác sĩ/KTV gọi bệnh nhân vào lấy mẫu',NULL,NULL,NULL,'system','2026-09-24 01:26:25',NULL,NULL),(33,'KHB801BA83','luotxetnghiem','5','Bác sĩ/KTV bắt đầu lấy mẫu',NULL,NULL,NULL,'user','2026-09-24 01:26:27',NULL,NULL),(34,'KHC5578C10','luotxetnghiem','6','Bác sĩ/KTV gọi bệnh nhân vào lấy mẫu',NULL,NULL,NULL,'system','2026-09-24 01:40:08',NULL,NULL),(35,'KHC5578C10','luotxetnghiem','6','Bác sĩ/KTV bắt đầu lấy mẫu',NULL,NULL,NULL,'user','2026-09-24 01:40:10',NULL,NULL),(36,'KHC5578C10','luotxetnghiem','6','Bác sĩ xác nhận đã lấy mẫu',NULL,NULL,NULL,'user','2026-09-24 01:40:14','Mã mẫu/Barcode: 179021401457778',NULL),(37,'KHC5578C10','luotxetnghiem','6','Mẫu bệnh phẩm đã bàn giao cho KTV - Chờ kết quả',NULL,NULL,NULL,'user','2026-09-24 01:41:06','Giao cho KTV: NV011. Ghi chú: ',NULL),(38,'KHC5578C10','luotxetnghiem','7','Bác sĩ/KTV gọi bệnh nhân vào lấy mẫu',NULL,NULL,NULL,'system','2026-09-24 01:47:50',NULL,NULL),(39,'KHC5578C10','luotxetnghiem','7','Bác sĩ/KTV bắt đầu lấy mẫu',NULL,NULL,NULL,'user','2026-09-24 01:47:53',NULL,NULL),(40,'KHC5578C10','luotxetnghiem','7','Bác sĩ xác nhận đã lấy mẫu',NULL,NULL,NULL,'user','2026-09-24 01:48:02','Mã mẫu/Barcode: 179021448203210',NULL),(41,'KHC5578C10','luotxetnghiem','7','Mẫu bệnh phẩm đã bàn giao cho KTV - Chờ kết quả',NULL,NULL,NULL,'user','2026-09-24 01:48:43','Giao cho KTV: NV011. Ghi chú: ',NULL),(42,'KHC5578C10','luotxetnghiem','8','Bác sĩ/KTV gọi bệnh nhân vào lấy mẫu',NULL,NULL,NULL,'system','2026-09-24 01:54:02',NULL,NULL),(43,'KHC5578C10','luotxetnghiem','8','Bác sĩ/KTV bắt đầu lấy mẫu',NULL,NULL,NULL,'user','2026-09-24 01:54:05',NULL,NULL),(44,'KHC5578C10','luotxetnghiem','8','Bác sĩ xác nhận đã lấy mẫu',NULL,NULL,NULL,'user','2026-09-24 01:54:19','Mã mẫu/Barcode: 179021485939698',NULL),(45,'KHC5578C10','luotxetnghiem','8','Mẫu bệnh phẩm đã bàn giao cho KTV (Chờ kết quả)',NULL,NULL,NULL,'user','2026-09-24 01:54:46','Mã mẫu: 179021485939698 - KTV tiếp nhận: NV011',NULL),(46,'KHC5578C10','luotxetnghiem','8','Bác sĩ đã phê duyệt và trả kết quả xét nghiệm',NULL,NULL,NULL,'system','2026-09-24 08:50:32','Bác sĩ duyệt: dtt@biomedic.vn',NULL),(47,'KHC5578C10','datlichkham','26','Khách hàng thay đổi lịch hẹn',NULL,NULL,NULL,'user','2026-09-24 13:59:05',NULL,NULL),(48,'KHC5578C10','datlichkham','26','Khách hàng thay đổi lịch hẹn',NULL,NULL,NULL,'user','2026-09-24 13:59:17',NULL,NULL),(49,'KHC5578C10','datlichkham','26','Khách hàng thay đổi lịch hẹn',NULL,NULL,NULL,'user','2026-09-24 13:59:34',NULL,NULL),(50,'KHC5578C10','datlichkham','26','Khách hàng thay đổi lịch hẹn',NULL,NULL,NULL,'user','2026-09-24 13:59:54',NULL,NULL),(51,'KHC5578C10','luotkham','8','Bác sĩ gọi bệnh nhân vào phòng',NULL,NULL,NULL,'system','2026-09-24 14:01:00',NULL,NULL),(52,'KHC5578C10','luotkham','8','Bác sĩ bắt đầu khám bệnh',NULL,NULL,NULL,'user','2026-09-24 14:01:05',NULL,NULL),(53,'KHC5578C10','luotkham','8','Bác sĩ hoàn tất khám',NULL,NULL,NULL,'user','2026-09-24 14:01:44',NULL,NULL),(54,'KHC5578C10','luotxetnghiem','9','Bác sĩ/KTV gọi bệnh nhân vào lấy mẫu',NULL,NULL,NULL,'system','2026-09-24 14:03:12',NULL,NULL),(55,'KHC5578C10','luotxetnghiem','9','Bác sĩ/KTV bắt đầu lấy mẫu',NULL,NULL,NULL,'user','2026-09-24 14:03:14',NULL,NULL),(56,'KHC5578C10','luotxetnghiem','9','Bác sĩ xác nhận đã lấy mẫu',NULL,NULL,NULL,'user','2026-09-24 14:03:18','Mã mẫu/Barcode: 179025859845013',NULL),(57,'KHC5578C10','luotxetnghiem','9','Mẫu bệnh phẩm đã bàn giao cho KTV (Chờ kết quả)',NULL,NULL,NULL,'user','2026-09-24 14:03:57','Mã mẫu: 179025859845013 - KTV tiếp nhận: NV011',NULL),(58,'KHC5578C10','datlichxetnghiem','21','Đã có kết quả xét nghiệm',NULL,NULL,NULL,'system','2026-09-24 14:07:46','Bác sĩ đã phê duyệt và trả kết quả.',NULL),(59,'KHC5578C10','luotxetnghiem','10','Bác sĩ/KTV gọi bệnh nhân vào lấy mẫu',NULL,NULL,NULL,'system','2026-09-24 14:10:56',NULL,NULL),(60,'KHC5578C10','luotxetnghiem','10','Bác sĩ/KTV bắt đầu lấy mẫu',NULL,NULL,NULL,'user','2026-09-24 14:11:00',NULL,NULL),(61,'KHC5578C10','luotxetnghiem','10','Bác sĩ xác nhận đã lấy mẫu',NULL,NULL,NULL,'user','2026-09-24 14:11:02','Mã mẫu/Barcode: 179025906283337',NULL),(62,'KHC5578C10','luotxetnghiem','10','Mẫu bệnh phẩm đã bàn giao cho KTV (Chờ kết quả)',NULL,NULL,NULL,'user','2026-09-24 14:11:11','Mã mẫu: 179025906283337 - KTV tiếp nhận: NV011',NULL),(63,'KHC5578C10','datlichxetnghiem','22','Đã có kết quả xét nghiệm',NULL,NULL,NULL,'system','2026-09-24 14:12:47','Bác sĩ đã phê duyệt và trả kết quả.',NULL),(64,'KHFED7A360','datlichkham','27','Khách hàng thay đổi lịch hẹn',NULL,NULL,NULL,'user','2026-09-24 14:46:30',NULL,NULL),(65,'KHFED7A360','datlichkham','27','Khách hàng thay đổi lịch hẹn',NULL,NULL,NULL,'user','2026-09-24 14:56:40','Khách hàng dời lịch sang 08:00 ngày 2026-09-25',NULL),(66,'KHFED7A360','datlichkham','27','Khách hàng thay đổi lịch hẹn',NULL,NULL,NULL,'user','2026-09-24 14:56:55','Khách hàng dời lịch sang 07:00 ngày 2026-09-24',NULL),(67,'KHFED7A360','datlichkham','27','Khách hàng hủy lịch hẹn',NULL,NULL,NULL,'user','2026-09-24 14:57:17',NULL,NULL),(68,'KHFED7A360','luotkham','9','Bác sĩ gọi bệnh nhân vào phòng',NULL,NULL,NULL,'system','2026-09-24 14:59:49',NULL,NULL),(69,'KHFED7A360','luotkham','9','Bác sĩ bắt đầu khám bệnh',NULL,NULL,NULL,'user','2026-09-24 14:59:52',NULL,NULL),(70,'KHFED7A360','luotkham','9','Bác sĩ hoàn tất khám',NULL,NULL,NULL,'user','2026-09-24 15:00:38',NULL,NULL),(71,'KHFED7A360','luotxetnghiem','11','Bác sĩ/KTV gọi bệnh nhân vào lấy mẫu',NULL,NULL,NULL,'system','2026-09-24 15:03:31',NULL,NULL),(72,'KHFED7A360','luotxetnghiem','11','Bác sĩ/KTV gọi bệnh nhân vào lấy mẫu',NULL,NULL,NULL,'system','2026-09-24 15:03:38',NULL,NULL),(73,'KHFED7A360','luotxetnghiem','11','Bác sĩ/KTV bắt đầu lấy mẫu',NULL,NULL,NULL,'user','2026-09-24 15:03:41',NULL,NULL),(74,'KHFED7A360','luotxetnghiem','11','Bác sĩ xác nhận đã lấy mẫu',NULL,NULL,NULL,'user','2026-09-24 15:03:43','Mã mẫu/Barcode: 179026222395394',NULL),(75,'KHFED7A360','luotxetnghiem','11','Mẫu bệnh phẩm đã bàn giao cho KTV (Chờ kết quả)',NULL,NULL,NULL,'user','2026-09-24 15:03:52','Mã mẫu: 179026222395394 - KTV tiếp nhận: NV011',NULL),(76,'KHFED7A360','datlichxetnghiem','23','Đã có kết quả xét nghiệm',NULL,NULL,NULL,'system','2026-09-24 15:35:27','Bác sĩ đã phê duyệt và trả kết quả.',NULL),(77,'KH-52DC35','luotkham','11','Bác sĩ gọi bệnh nhân vào phòng',NULL,NULL,NULL,'system','2026-09-25 00:32:37',NULL,NULL),(78,'KH-52DC35','luotkham','11','Bác sĩ bắt đầu khám bệnh',NULL,NULL,NULL,'user','2026-09-25 00:32:39',NULL,NULL),(79,'KH-52DC35','luotkham','11','Bác sĩ hoàn tất khám',NULL,NULL,NULL,'user','2026-09-25 00:32:58',NULL,NULL);
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
  `Email` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Username` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `PasswordHash` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Role` enum('khachhang','bacsi','letan','ktv','admin') CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `IDKhachHang` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `IDBacSi` int DEFAULT NULL,
  `IDNhanVien` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
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
) ENGINE=InnoDB AUTO_INCREMENT=22 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (1,'admin@example.com','admin01','123456','admin',NULL,NULL,NULL,1,'2026-09-19 04:14:11',NULL),(4,'hello@gmail.com','hello@gmail.com','$2a$10$fp9fYZvIa6B9nVKWVyCVzujwFavmaodKyKGBEsFWAzaxS2UTzM6SW','khachhang','KH14B09585',NULL,NULL,1,'2026-09-19 13:03:04',NULL),(5,'nnm@gmail.com','nnm@gmail.com','$2a$10$Cbxb/B1fwJ.SvuVIkbTckeZujYNEPXsgjIh684Fc/pkVpbpiSZXMO','khachhang','KHC5BC4D5E',NULL,NULL,1,'2026-09-20 00:49:55',NULL),(6,'hkn@biomedic.vn','hkn','$2a$10$9EZLH4SAhx.Kt3rYPIKuCukMTYVtF9/Mh69QKdJ.Y6GzdxcpkIHY6','bacsi',NULL,1,NULL,1,'2026-09-20 01:08:07','2026-09-22 21:53:52'),(7,'tmd@biomedic.vn','tmd','$2a$10$Z0yHRDl/65MkewKIfA5BNOCTry9z1NOcagbFcQH8vqI2y3NIY5Agi','bacsi',NULL,2,NULL,1,'2026-09-20 01:08:07','2026-09-22 21:53:52'),(8,'admin@biomedic.vn','admin','$2a$10$OfbFqh66Z4v.ODJBm/Aft.sJIWR4hMGc/CmYSltCIAvlL3EWJOEby','admin',NULL,NULL,NULL,1,'2026-09-20 01:16:12',NULL),(9,'bacsi@biomedic.vn','bacsi','$2a$10$KyxJP1cQQMw0PTBlBPwMGenWPHLC/P47Yzo9VdSsNBKU8hHXHZHva','bacsi',NULL,NULL,NULL,1,'2026-09-20 01:16:12',NULL),(10,'ktv@biomedic.vn','ktv','$2a$10$zbDRyOLEfWIHoofoYUWGruMDpj9siAynBTbz4lYhFQ5MlfhAea/YW','ktv',NULL,NULL,'NV011',1,'2026-09-20 01:16:13','2026-09-24 22:51:51'),(11,'letan@biomedic.vn','letan','$2a$10$XESNf8FM4icfIzQnbCgNzeg01OHWyJlLfRfu0o1/nqhb3WyFgIGnS','letan',NULL,NULL,NULL,1,'2026-09-20 01:16:13',NULL),(12,'haha@gmail.com','haha@gmail.com','$2a$10$/Ig0r.MSzxOKSnJW9WTNCu67fCMHAqfJFUFjibHtx.upFwvvZgph6','khachhang','KHFD6C8512',NULL,NULL,1,'2026-09-22 08:20:53',NULL),(13,'lvc@biomedic.vn','lvc','$2a$10$nwVnxF6YUQSpuePbwSvbz.bomcDdt9NdAP3NTV0EZMjfxHQ9FVUZG','bacsi',NULL,3,NULL,1,'2026-09-22 21:53:52',NULL),(14,'ptm@biomedic.vn','ptm','$2a$10$VDlDBaUBNP0S4BDqDRPHXeQcKXY8vU9p07rOrZmuF09tvo428PEeC','bacsi',NULL,4,NULL,1,'2026-09-22 21:53:52',NULL),(15,'nmt@biomedic.vn','nmt','$2a$10$s0TwMReB60RPh/vGd.H/vOfqoUFO4jLyJc/rMgofhK0doGNoTn0PK','bacsi',NULL,5,NULL,1,'2026-09-22 21:53:52',NULL),(16,'dtt@biomedic.vn','dtt','$2a$10$SXeWm7N4zfSbpWjWAwu3WuYdC6nSiK.1fY.7cDxNBnfffr0aQU2OW','bacsi',NULL,6,NULL,1,'2026-09-22 21:53:52',NULL),(17,'vhl@biomedic.vn','vhl','$2a$10$kNe5Edg2vF6bRO6DzU4Wfu1coDPlpOnO64i1gsu79fZ0UdVGWhY2q','bacsi',NULL,7,NULL,1,'2026-09-22 21:53:52',NULL),(18,'nth@biomedic.vn','nth','$2a$10$IT0lbEaqRFe.W1pdfEFmTOb1fH4EyPznZk7npXNr/.91iXnT3wlb2','bacsi',NULL,8,NULL,1,'2026-09-22 21:53:52',NULL),(19,'pqb@biomedic.vn','pqb','$2a$10$e1XfLB99MjWIsVZeXv4gKeF5EZrZhX4KtE/anfCo2yjlVEzlt4c3G','bacsi',NULL,9,NULL,1,'2026-09-22 21:53:52',NULL),(20,'tna@biomedic.vn','tna','$2a$10$vzwDZlpBlIoWDHQpaCxKDOVQr6hGDz6UYvQI88HNMtYghvljEtTFq','bacsi',NULL,10,NULL,1,'2026-09-22 21:53:53',NULL),(21,'phuong@gmail.com','phuong@gmail.com','$2a$10$74DA9Gf.jvLD1fdHz/rVZu3NrLu9uOdVDrlW2UObUFqPTemZRDHKG','khachhang','KHF7E08285',NULL,NULL,1,'2026-09-24 14:45:48',NULL);
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
  `IDMau` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `IDKTV` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Instrument` varchar(120) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ReagentLot` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Status` enum('queue','running','to_result','rerun','finished','cancelled') CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'queue',
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
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `worklist`
--

LOCK TABLES `worklist` WRITE;
/*!40000 ALTER TABLE `worklist` DISABLE KEYS */;
INSERT INTO `worklist` VALUES (1,1,'M179021485939698',NULL,NULL,NULL,'finished','2026-09-24 07:27:32','2026-09-24 07:27:52','2026-09-24 08:19:04','2026-09-24 08:19:04'),(2,2,'M179025859845013',NULL,NULL,NULL,'finished','2026-09-24 14:05:35','2026-09-24 14:05:43','2026-09-24 14:06:26','2026-09-24 14:06:26'),(3,3,'M179025906283337',NULL,NULL,NULL,'finished','2026-09-24 14:11:37','2026-09-24 14:11:40','2026-09-24 14:11:56','2026-09-24 14:11:56'),(4,4,'M179026222395394',NULL,NULL,NULL,'finished','2026-09-24 15:04:21','2026-09-24 15:04:29','2026-09-24 15:16:33','2026-09-24 15:16:33');
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

-- Dump completed on 2026-09-25  9:08:46
