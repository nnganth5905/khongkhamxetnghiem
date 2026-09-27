package com.biomedic.backend.service;

import org.springframework.jdbc.core.JdbcTemplate;
import org.springframework.stereotype.Service;

import java.time.LocalDate;
import java.time.LocalTime;
import java.util.*;

@Service
public class DashboardService {

    private final JdbcTemplate jdbcTemplate;

    public DashboardService(JdbcTemplate jdbcTemplate) {
        this.jdbcTemplate = jdbcTemplate;
    }

    // ==========================================
    // LOGIC DASHBOARD DÀNH CHO KHÁCH HÀNG
    // ==========================================
    public Map<String, Object> getCustomerDashboardData(String customerId, Integer userId) {
        Map<String, Object> data = new HashMap<>();
        
        List<Map<String, Object>> allAppointments = new ArrayList<>();
        
        // 1. LẤY TẤT CẢ LỊCH KHÁM
        String sqlExam = "SELECT d.MaDatLich, d.NgayKham as Ngay, d.GioKham as Gio, b.TenBacSi, " +
                         "COALESCE(lk.TrangThai, d.TrangThai) as RealStatus, " +
                         "ck.TenChuyenKhoa as ServiceName " +
                         "FROM datlichkham d " +
                         "LEFT JOIN bacsi b ON d.IDBacSi = b.IDBacSi " +
                         "LEFT JOIN chuyenkhoa ck ON d.IDChuyenKhoa = ck.IDChuyenKhoa " +
                         "LEFT JOIN luotkham lk ON d.IDDatLichKham = lk.IDDatLichKham " +
                         "WHERE d.UserID = ? OR (d.IDKhachHang = ? AND ? != '')";
                         
        List<Map<String, Object>> exams = jdbcTemplate.queryForList(sqlExam, userId, customerId, customerId);
        for (Map<String, Object> row : exams) {
            Map<String, Object> appt = new HashMap<>(row);
            appt.put("Type", "EXAMINATION");
            allAppointments.add(appt);
        }

        // 2. LẤY TẤT CẢ LỊCH XÉT NGHIỆM
        String sqlTest = "SELECT d.MaDatLich, d.NgayXetNghiem as Ngay, d.GioXetNghiem as Gio, b.TenBacSi, " +
                         "COALESCE(lxn.TrangThai, d.TrangThai) as RealStatus, " +
                         "'Xét nghiệm' as ServiceName " +
                         "FROM datlichxetnghiem d " +
                         "LEFT JOIN bacsi b ON d.IDBacSi = b.IDBacSi " +
                         "LEFT JOIN luotxetnghiem lxn ON d.IDDatLichXN = lxn.IDDatLichXN " +
                         "WHERE d.UserID = ? OR (d.IDKhachHang = ? AND ? != '')";
                         
        List<Map<String, Object>> tests = jdbcTemplate.queryForList(sqlTest, userId, customerId, customerId);
        for (Map<String, Object> row : tests) {
            Map<String, Object> appt = new HashMap<>(row);
            appt.put("Type", "TEST");
            allAppointments.add(appt);
        }

        long totalAppointments = allAppointments.size();
        
        LocalDate today = LocalDate.now();
        LocalTime now = LocalTime.now();
        
        List<Map<String, Object>> futureAppointments = new ArrayList<>();
        for (Map<String, Object> appt : allAppointments) {
            String status = (String) appt.get("RealStatus");
            if (status != null && (status.equalsIgnoreCase("cancelled") || status.equalsIgnoreCase("huy") || 
                                   status.equalsIgnoreCase("hoan_tat") || status.equalsIgnoreCase("completed") || 
                                   status.equalsIgnoreCase("no_show"))) {
                continue;
            }
            
            if (appt.get("Ngay") != null) {
                LocalDate date = LocalDate.parse(appt.get("Ngay").toString());
                LocalTime time = appt.get("Gio") != null ? LocalTime.parse(appt.get("Gio").toString()) : LocalTime.MIDNIGHT;
                
                if (date.isAfter(today) || (date.isEqual(today) && time.isAfter(now))) {
                    appt.put("ParsedDate", date);
                    appt.put("ParsedTime", time);
                    futureAppointments.add(appt);
                }
            }
        }
        
        futureAppointments.sort((a, b) -> {
            LocalDate dateA = (LocalDate) a.get("ParsedDate");
            LocalDate dateB = (LocalDate) b.get("ParsedDate");
            int dateCompare = dateA.compareTo(dateB);
            if (dateCompare != 0) return dateCompare;
            
            LocalTime timeA = (LocalTime) a.get("ParsedTime");
            LocalTime timeB = (LocalTime) b.get("ParsedTime");
            return timeA.compareTo(timeB);
        });
        
        Map<String, Object> upcomingAppointment = null;
        if (!futureAppointments.isEmpty()) {
            Map<String, Object> nearest = futureAppointments.get(0);
            upcomingAppointment = new HashMap<>();
            
            String type = (String) nearest.get("Type");
            String serviceName = nearest.get("ServiceName") != null ? nearest.get("ServiceName").toString() : (type.equals("EXAMINATION") ? "Khám bệnh" : "Xét nghiệm");
            
            upcomingAppointment.put("id", nearest.get("MaDatLich")); 
            upcomingAppointment.put("serviceName", serviceName);
            upcomingAppointment.put("date", nearest.get("Ngay").toString());
            upcomingAppointment.put("time", nearest.get("Gio") != null ? nearest.get("Gio").toString().substring(0, 5) : "");
            upcomingAppointment.put("doctorName", nearest.get("TenBacSi") != null ? nearest.get("TenBacSi") : "Theo phân công");
            upcomingAppointment.put("type", type);
        }

        List<Map<String, Object>> recentResults = new ArrayList<>();
        long availableResults = 0;
        
        if (customerId != null && !customerId.isEmpty()) {
            String sqlRecentResults = "SELECT p.IDPhieuXetNghiem as id, p.NgayTao as ngayXetNghiem, " +
                                      "CASE WHEN k.TrangThai = 'da_duyet' THEN 'Đã duyệt' " +
                                      "WHEN k.TrangThai = 'hoan_tat' THEN 'Hoàn tất' ELSE k.TrangThai END as status, " +
                                      "l.TenXetNghiem as tenXetNghiem " +
                                      "FROM ketquaxetnghiem k " +
                                      "JOIN ctphieuxetnghiem ct ON k.IDCTPhieu = ct.IDCTPhieu " +
                                      "JOIN phieuxetnghiem p ON ct.IDPhieuXetNghiem = p.IDPhieuXetNghiem " +
                                      "JOIN loaixetnghiem l ON ct.IDXetNghiem = l.IDXetNghiem " +
                                      "WHERE p.IDKhachHang = ? AND k.TrangThai IN ('da_duyet', 'hoan_tat') " +
                                      "ORDER BY k.ThoiGianDuyet DESC, p.NgayTao DESC LIMIT 5";
            
            recentResults = jdbcTemplate.queryForList(sqlRecentResults, customerId);
            
            String sqlCountResults = "SELECT COUNT(k.IDKetQua) FROM ketquaxetnghiem k " +
                                     "JOIN ctphieuxetnghiem ct ON k.IDCTPhieu = ct.IDCTPhieu " +
                                     "JOIN phieuxetnghiem p ON ct.IDPhieuXetNghiem = p.IDPhieuXetNghiem " +
                                     "WHERE p.IDKhachHang = ? AND k.TrangThai IN ('da_duyet', 'hoan_tat')";
            Long count = jdbcTemplate.queryForObject(sqlCountResults, Long.class, customerId);
            availableResults = count != null ? count : 0;
        }

        long unreadNotifications = 0;
        if (userId != null) {
            String sqlUnreadNoti = "SELECT COUNT(*) FROM thongbao WHERE UserIDNhan = ? AND DaDoc = 0";
            Long count = jdbcTemplate.queryForObject(sqlUnreadNoti, Long.class, userId);
            unreadNotifications = count != null ? count : 0;
        }

        data.put("totalAppointments", totalAppointments);
        data.put("availableResults", availableResults);
        data.put("unreadNotifications", unreadNotifications);
        data.put("upcomingAppointment", upcomingAppointment);
        data.put("recentResults", recentResults);

        return data;
    }

    // ==========================================
    // LOGIC DASHBOARD DÀNH CHO BÁC SĨ (ĐÃ TỐI ƯU HIỂN THỊ)
    // ==========================================
    public Map<String, Object> getDoctorDashboardData(Integer doctorId) {
        Map<String, Object> data = new HashMap<>();
        LocalDate today = LocalDate.now();

        // 1. Lấy phòng làm việc hiện tại
        String sqlRoom = "SELECT p.TenPhong FROM lichlamviec l " +
                         "JOIN phong p ON l.IDPhong = p.IDPhong " +
                         "WHERE l.IDBacSi = ? AND l.Ngay = ? AND l.TrangThai = 'duoc_duyet' LIMIT 1";
        String currentRoom = "—";
        try {
            List<String> rooms = jdbcTemplate.queryForList(sqlRoom, String.class, doctorId, today);
            if (!rooms.isEmpty()) {
                currentRoom = rooms.get(0);
            }
        } catch (Exception e) {
            // Không có phòng thì để mặc định
        }

        // TỔ HỢP TẤT CẢ TRẠNG THÁI "CHỜ" HOẶC "ĐANG XỬ LÝ" CỦA BỆNH NHÂN
        String waitingStatuses = "'da_tiep_nhan', 'cho_kham', 'da_den_luot', 'cho_goi_lai', 'moi_doc_kq'";

        // 2. Đang chờ khám
        String sqlWaiting = "SELECT COUNT(*) FROM luotkham lk " +
                            "JOIN datlichkham d ON lk.IDDatLichKham = d.IDDatLichKham " +
                            "WHERE lk.IDBacSi = ? AND lk.TrangThai IN (" + waitingStatuses + ") AND d.NgayKham = ?";
        Long waitingCount = jdbcTemplate.queryForObject(sqlWaiting, Long.class, doctorId, today);

        // 3. Lượt khám hôm nay (không tính hủy)
        String sqlTodayExams = "SELECT COUNT(*) FROM luotkham lk " +
                               "JOIN datlichkham d ON lk.IDDatLichKham = d.IDDatLichKham " +
                               "WHERE lk.IDBacSi = ? AND d.NgayKham = ? AND lk.TrangThai != 'huy'";
        Long todayExamsCount = jdbcTemplate.queryForObject(sqlTodayExams, Long.class, doctorId, today);

        // 4. Kết quả chờ duyệt
        String sqlPendingResults = "SELECT COUNT(*) FROM ketquaxetnghiem k " +
                                   "JOIN ctphieuxetnghiem ct ON k.IDCTPhieu = ct.IDCTPhieu " +
                                   "JOIN phieuxetnghiem p ON ct.IDPhieuXetNghiem = p.IDPhieuXetNghiem " +
                                   "WHERE k.TrangThai = 'cho_duyet' AND p.IDBacSiChiDinh = ?";
        Long pendingResultsCount = jdbcTemplate.queryForObject(sqlPendingResults, Long.class, doctorId);

        // 5. Hoàn tất hôm nay
        String sqlCompletedToday = "SELECT COUNT(*) FROM luotkham lk " +
                                   "JOIN datlichkham d ON lk.IDDatLichKham = d.IDDatLichKham " +
                                   "WHERE lk.IDBacSi = ? AND d.NgayKham = ? AND lk.TrangThai = 'hoan_tat'";
        Long completedTodayCount = jdbcTemplate.queryForObject(sqlCompletedToday, Long.class, doctorId, today);

        // 6. Lấy danh sách bệnh nhân tiếp theo (Dùng alias ngoặc kép "" để giữ nguyên camelCase)
        String sqlNextPatients = "SELECT lk.IDLuotKham AS \"id\", lk.SoThuTu AS \"queueNumber\", " +
                                 "kh.TenKhachHang AS \"patientName\", TIME_FORMAT(d.GioKham, '%H:%i') AS \"time\", " +
                                 "d.GhiChu AS \"reason\" " +
                                 "FROM luotkham lk " +
                                 "JOIN datlichkham d ON lk.IDDatLichKham = d.IDDatLichKham " +
                                 "JOIN khachhang kh ON lk.IDKhachHang = kh.IDKhachHang " +
                                 "WHERE lk.IDBacSi = ? AND lk.TrangThai IN (" + waitingStatuses + ") AND d.NgayKham = ? " +
                                 "ORDER BY lk.SoThuTu ASC LIMIT 5";
        List<Map<String, Object>> nextPatients = jdbcTemplate.queryForList(sqlNextPatients, doctorId, today);

        // 7. Lấy danh sách kết quả cần xử lý
        String sqlPendingResultsList = "SELECT kq.IDKetQua AS \"id\", kh.TenKhachHang AS \"patientName\", " +
                                       "lxn.TenXetNghiem AS \"testName\" " +
                                       "FROM ketquaxetnghiem kq " +
                                       "JOIN ctphieuxetnghiem ct ON kq.IDCTPhieu = ct.IDCTPhieu " +
                                       "JOIN loaixetnghiem lxn ON ct.IDXetNghiem = lxn.IDXetNghiem " +
                                       "JOIN phieuxetnghiem p ON ct.IDPhieuXetNghiem = p.IDPhieuXetNghiem " +
                                       "JOIN khachhang kh ON p.IDKhachHang = kh.IDKhachHang " +
                                       "WHERE kq.TrangThai = 'cho_duyet' AND p.IDBacSiChiDinh = ? " +
                                       "ORDER BY kq.ThoiGianNhap ASC LIMIT 5";
        List<Map<String, Object>> pendingResultsList = jdbcTemplate.queryForList(sqlPendingResultsList, doctorId);

        // Map kết quả trả về
        data.put("currentRoom", currentRoom);
        data.put("waitingCount", waitingCount != null ? waitingCount : 0);
        data.put("todayExamsCount", todayExamsCount != null ? todayExamsCount : 0);
        data.put("pendingResultsCount", pendingResultsCount != null ? pendingResultsCount : 0);
        data.put("completedTodayCount", completedTodayCount != null ? completedTodayCount : 0);
        
        // Mảng hiển thị bảng
        data.put("nextPatients", nextPatients);
        data.put("pendingResults", pendingResultsList);

        return data;
    }

    // ==========================================
    // LOGIC DASHBOARD DÀNH CHO LỄ TÂN
    // ==========================================
    public Map<String, Object> getReceptionistDashboardData() {
        Map<String, Object> data = new HashMap<>();
        LocalDate today = LocalDate.now();

        // 1. Tổng lịch hôm nay (Khám + Xét nghiệm, loại trừ lịch đã hủy)
        String sqlTotalAppointments = 
            "SELECT " +
            "(SELECT COUNT(*) FROM datlichkham WHERE NgayKham = ? AND TrangThai NOT IN ('cancelled', 'huy')) + " +
            "(SELECT COUNT(*) FROM datlichxetnghiem WHERE NgayXetNghiem = ? AND TrangThai NOT IN ('cancelled', 'huy'))";
        Long todayAppointments = jdbcTemplate.queryForObject(sqlTotalAppointments, Long.class, today, today);

        // 2. Khách đã check-in (Đã chuyển thành lượt khám/lượt xét nghiệm trong ngày)
        String sqlCheckedIn = 
            "SELECT " +
            "(SELECT COUNT(*) FROM luotkham lk JOIN datlichkham d ON lk.IDDatLichKham = d.IDDatLichKham WHERE d.NgayKham = ?) + " +
            "(SELECT COUNT(*) FROM luotxetnghiem lx JOIN datlichxetnghiem d ON lx.IDDatLichXN = d.IDDatLichXN WHERE d.NgayXetNghiem = ?)";
        Long checkedIn = jdbcTemplate.queryForObject(sqlCheckedIn, Long.class, today, today);

        // 3. Khách đang chờ (Tổng các trạng thái chờ ở phòng khám và phòng lấy mẫu/xét nghiệm)
        String sqlWaiting = 
            "SELECT " +
            "(SELECT COUNT(*) FROM luotkham lk JOIN datlichkham d ON lk.IDDatLichKham = d.IDDatLichKham WHERE d.NgayKham = ? AND lk.TrangThai IN ('da_tiep_nhan', 'cho_kham', 'da_den_luot')) + " +
            "(SELECT COUNT(*) FROM luotxetnghiem lx JOIN datlichxetnghiem d ON lx.IDDatLichXN = d.IDDatLichXN WHERE d.NgayXetNghiem = ? AND lx.TrangThai IN ('da_tiep_nhan', 'cho_xet_nghiem', 'da_den_luot'))";
        Long waiting = jdbcTemplate.queryForObject(sqlWaiting, Long.class, today, today);

        // 4. Khách tiếp nhận trực tiếp (Walk-ins: Các lịch được tạo trực tiếp tại quầy, thường không có UserID liên kết)
        String sqlWalkIns = 
            "SELECT " +
            "(SELECT COUNT(*) FROM datlichkham WHERE NgayKham = ? AND UserID IS NULL) + " +
            "(SELECT COUNT(*) FROM datlichxetnghiem WHERE NgayXetNghiem = ? AND UserID IS NULL)";
        Long walkIns = jdbcTemplate.queryForObject(sqlWalkIns, Long.class, today, today);

        // 5. Lịch hẹn sắp đến (Kết hợp cả lịch khám và lịch xét nghiệm đang ở trạng thái pending/confirmed)
        String sqlUpcoming = 
            "SELECT d.MaDatLich AS \"id\", TIME_FORMAT(d.GioKham, '%H:%i') AS \"time\", " +
            "kh.TenKhachHang AS \"patientName\", 'Khám bệnh' AS \"type\", " +
            "ck.TenChuyenKhoa AS \"serviceName\", d.TrangThai AS \"status\" " +
            "FROM datlichkham d " +
            "JOIN khachhang kh ON d.IDKhachHang = kh.IDKhachHang " +
            "JOIN chuyenkhoa ck ON d.IDChuyenKhoa = ck.IDChuyenKhoa " +
            "WHERE d.NgayKham = ? AND d.TrangThai IN ('pending', 'confirmed') " +
            "UNION ALL " +
            "SELECT dx.MaDatLich AS \"id\", TIME_FORMAT(dx.GioXetNghiem, '%H:%i') AS \"time\", " +
            "kh.TenKhachHang AS \"patientName\", 'Xét nghiệm' AS \"type\", " +
            "'Xét nghiệm' AS \"serviceName\", dx.TrangThai AS \"status\" " +
            "FROM datlichxetnghiem dx " +
            "JOIN khachhang kh ON dx.IDKhachHang = kh.IDKhachHang " +
            "WHERE dx.NgayXetNghiem = ? AND dx.TrangThai IN ('pending', 'confirmed') " +
            "ORDER BY \"time\" ASC LIMIT 10";
            
        List<Map<String, Object>> upcomingAppointments = jdbcTemplate.queryForList(sqlUpcoming, today, today);

        // Map kết quả cho Frontend
        data.put("todayAppointments", todayAppointments != null ? todayAppointments : 0);
        data.put("checkedIn", checkedIn != null ? checkedIn : 0);
        data.put("waiting", waiting != null ? waiting : 0);
        data.put("walkIns", walkIns != null ? walkIns : 0);
        data.put("upcomingAppointments", upcomingAppointments);

        return data;
    }

    // ==========================================
    // LOGIC DASHBOARD DÀNH CHO KỸ THUẬT VIÊN
    // ==========================================
    public Map<String, Object> getTechnicianDashboardData(String technicianId) {
        Map<String, Object> data = new HashMap<>();

        // 1. Mẫu chờ tiếp nhận (Trạng thái 'da_ban_giao' từ bác sĩ/người lấy mẫu)
        String sqlWaitingSpecimens = "SELECT COUNT(*) FROM maubenhpham WHERE TrangThai = 'da_ban_giao'";
        Long waitingSpecimens = jdbcTemplate.queryForObject(sqlWaitingSpecimens, Long.class);

        // 2. Mẫu đã tiếp nhận (Trạng thái 'ktv_tiep_nhan')
        String sqlReceivedSpecimens = "SELECT COUNT(*) FROM maubenhpham WHERE TrangThai = 'ktv_tiep_nhan'";
        Long receivedSpecimens = jdbcTemplate.queryForObject(sqlReceivedSpecimens, Long.class);

        // 3. Đang thực hiện (Trạng thái worklist đang chạy máy: queue, running, rerun)
        String sqlInProgress = "SELECT COUNT(*) FROM worklist WHERE Status IN ('queue', 'running', 'rerun')";
        Long inProgress = jdbcTemplate.queryForObject(sqlInProgress, Long.class);

        // 4. Chờ nhập kết quả (Worklist đã ra kết quả từ máy, chờ KTV nhập/xác nhận)
        String sqlPendingResultEntries = "SELECT COUNT(*) FROM worklist WHERE Status = 'to_result'";
        Long pendingResultEntries = jdbcTemplate.queryForObject(sqlPendingResultEntries, Long.class);

        // 5. Lấy danh sách Worklist cần xử lý (Hiển thị cho bảng)
        String sqlWorklist = 
            "SELECT w.id AS \"id\", " +
            "m.MaBarcode AS \"specimenCode\", " +
            "kh.TenKhachHang AS \"patientName\", " +
            "lxn.TenXetNghiem AS \"testName\", " +
            "'NORMAL' AS \"priority\", " + // Database hiện tại không có cột ưu tiên, mặc định là NORMAL
            "w.Status AS \"status\" " +
            "FROM worklist w " +
            "JOIN maubenhpham m ON w.IDMau = m.IDMau " +
            "JOIN ctphieuxetnghiem ct ON w.IDCTPhieu = ct.IDCTPhieu " +
            "JOIN loaixetnghiem lxn ON ct.IDXetNghiem = lxn.IDXetNghiem " +
            "JOIN phieuxetnghiem p ON ct.IDPhieuXetNghiem = p.IDPhieuXetNghiem " +
            "JOIN khachhang kh ON p.IDKhachHang = kh.IDKhachHang " +
            "WHERE w.Status NOT IN ('finished', 'cancelled') " +
            "ORDER BY w.ReceivedAt ASC LIMIT 10";
            
        List<Map<String, Object>> worklist = jdbcTemplate.queryForList(sqlWorklist);

        // Map kết quả trả về
        data.put("waitingSpecimens", waitingSpecimens != null ? waitingSpecimens : 0);
        data.put("receivedSpecimens", receivedSpecimens != null ? receivedSpecimens : 0);
        data.put("inProgress", inProgress != null ? inProgress : 0);
        data.put("pendingResultEntries", pendingResultEntries != null ? pendingResultEntries : 0);
        data.put("worklist", worklist);

        return data;
    }

    // ==========================================
    // LOGIC DASHBOARD DÀNH CHO ADMIN
    // ==========================================
    public Map<String, Object> getAdminDashboardData() {
        Map<String, Object> data = new HashMap<>();
        LocalDate today = LocalDate.now();

        // 1. Tổng quan Khách hàng, Nhân viên, Bác sĩ, Kỹ thuật viên
        data.put("totalCustomers", jdbcTemplate.queryForObject("SELECT COUNT(*) FROM khachhang", Long.class));
        data.put("totalEmployees", jdbcTemplate.queryForObject("SELECT COUNT(*) FROM nhanvien", Long.class));
        data.put("totalDoctors", jdbcTemplate.queryForObject("SELECT COUNT(*) FROM bacsi", Long.class));
        data.put("totalTechnicians", jdbcTemplate.queryForObject("SELECT COUNT(*) FROM nhanvien WHERE ViTri = 'ktv'", Long.class));

        // 2. Tổng số phiếu xét nghiệm và Kết quả
        data.put("totalTestOrders", jdbcTemplate.queryForObject("SELECT COUNT(*) FROM phieuxetnghiem", Long.class));
        data.put("totalResults", jdbcTemplate.queryForObject("SELECT COUNT(*) FROM ketquaxetnghiem", Long.class));

        // 3. Lịch hôm nay (Khám + Xét nghiệm, loại bỏ các lịch đã hủy)
        String sqlTodayAppointments = 
            "SELECT " +
            "(SELECT COUNT(*) FROM datlichkham WHERE NgayKham = ? AND TrangThai NOT IN ('cancelled', 'huy')) + " +
            "(SELECT COUNT(*) FROM datlichxetnghiem WHERE NgayXetNghiem = ? AND TrangThai NOT IN ('cancelled', 'huy'))";
        Long todayAppointments = jdbcTemplate.queryForObject(sqlTodayAppointments, Long.class, today, today);
        data.put("todayAppointments", todayAppointments != null ? todayAppointments : 0);

        // 4. Chờ duyệt kết quả
        data.put("pendingResults", jdbcTemplate.queryForObject("SELECT COUNT(*) FROM ketquaxetnghiem WHERE TrangThai = 'cho_duyet'", Long.class));

        // 5. Doanh thu hôm nay (Chỉ tính các hóa đơn đã thanh toán)
        String sqlRevenue = "SELECT SUM(TongTien) FROM hoadon WHERE DATE(NgayTaoHoaDon) = ? AND TrangThaiThanhToan = 'da_thanh_toan'";
        Double revenueToday = jdbcTemplate.queryForObject(sqlRevenue, Double.class, today);
        data.put("revenueToday", revenueToday != null ? revenueToday : 0.0);

        // 6. Mẫu đang chờ (Chưa đến tay KTV hoặc đang trung chuyển)
        String sqlWaitingSpecimens = "SELECT COUNT(*) FROM maubenhpham WHERE TrangThai IN ('moi_tao', 'da_lay_mau', 'da_ban_giao')";
        data.put("waitingSpecimens", jdbcTemplate.queryForObject(sqlWaitingSpecimens, Long.class));

        // 7. Hoạt động gần đây (Lấy 6 dòng mới nhất từ bảng truy vết)
        String sqlRecentActivities = 
            "SELECT IDTruyVet AS \"id\", " +
            "HanhDong AS \"action\", " +
            "MoTa AS \"description\", " +
            "DATE_FORMAT(ThoiGian, '%H:%i %d/%m/%Y') AS \"time\" " +
            "FROM truyvet " +
            "ORDER BY ThoiGian DESC LIMIT 6";
        List<Map<String, Object>> recentActivities = jdbcTemplate.queryForList(sqlRecentActivities);
        data.put("recentActivities", recentActivities);

        return data;
    }
}