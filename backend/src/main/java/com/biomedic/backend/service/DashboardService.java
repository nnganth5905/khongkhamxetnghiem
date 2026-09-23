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
        
        // 1. LẤY TẤT CẢ LỊCH KHÁM (Bao gồm cả ID để link chi tiết)
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

        // 2. LẤY TẤT CẢ LỊCH XÉT NGHIỆM (Bao gồm cả ID)
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

        // TỔNG SỐ LỊCH HẸN
        long totalAppointments = allAppointments.size();
        
        // 3. TÌM LỊCH SẮP TỚI GẦN NHẤT
        LocalDate today = LocalDate.now();
        LocalTime now = LocalTime.now();
        
        List<Map<String, Object>> futureAppointments = new ArrayList<>();
        for (Map<String, Object> appt : allAppointments) {
            String status = (String) appt.get("RealStatus");
            // Bỏ qua các lịch đã hủy, vắng mặt hoặc đã hoàn tất
            if (status != null && (status.equalsIgnoreCase("cancelled") || status.equalsIgnoreCase("huy") || 
                                   status.equalsIgnoreCase("hoan_tat") || status.equalsIgnoreCase("completed") || 
                                   status.equalsIgnoreCase("no_show"))) {
                continue;
            }
            
            if (appt.get("Ngay") != null) {
                LocalDate date = LocalDate.parse(appt.get("Ngay").toString());
                LocalTime time = appt.get("Gio") != null ? LocalTime.parse(appt.get("Gio").toString()) : LocalTime.MIDNIGHT;
                
                // Lọc các lịch từ bây giờ trở đi
                if (date.isAfter(today) || (date.isEqual(today) && time.isAfter(now))) {
                    appt.put("ParsedDate", date);
                    appt.put("ParsedTime", time);
                    futureAppointments.add(appt);
                }
            }
        }
        
        // Sắp xếp tăng dần để lấy lịch hẹn sớm nhất sắp tới
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
            
            // Map dữ liệu để trả về Frontend
            upcomingAppointment.put("id", nearest.get("MaDatLich")); // ID dùng để route đến trang chi tiết
            upcomingAppointment.put("serviceName", serviceName);
            upcomingAppointment.put("date", nearest.get("Ngay").toString());
            upcomingAppointment.put("time", nearest.get("Gio") != null ? nearest.get("Gio").toString().substring(0, 5) : "");
            upcomingAppointment.put("doctorName", nearest.get("TenBacSi") != null ? nearest.get("TenBacSi") : "Theo phân công");
            upcomingAppointment.put("type", type);
        }

        // 4. LẤY KẾT QUẢ XÉT NGHIỆM GẦN ĐÂY
        List<Map<String, Object>> recentResults = new ArrayList<>();
        long availableResults = 0;
        
        if (customerId != null && !customerId.isEmpty()) {
            // Lấy 5 kết quả xét nghiệm mới nhất
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
            
            // Đếm tổng số kết quả đã có
            String sqlCountResults = "SELECT COUNT(k.IDKetQua) FROM ketquaxetnghiem k " +
                                     "JOIN ctphieuxetnghiem ct ON k.IDCTPhieu = ct.IDCTPhieu " +
                                     "JOIN phieuxetnghiem p ON ct.IDPhieuXetNghiem = p.IDPhieuXetNghiem " +
                                     "WHERE p.IDKhachHang = ? AND k.TrangThai IN ('da_duyet', 'hoan_tat')";
            Long count = jdbcTemplate.queryForObject(sqlCountResults, Long.class, customerId);
            availableResults = count != null ? count : 0;
        }

        // 5. ĐẾM SỐ THÔNG BÁO CHƯA ĐỌC
        long unreadNotifications = 0;
        if (userId != null) {
            String sqlUnreadNoti = "SELECT COUNT(*) FROM thongbao WHERE UserIDNhan = ? AND DaDoc = 0";
            Long count = jdbcTemplate.queryForObject(sqlUnreadNoti, Long.class, userId);
            unreadNotifications = count != null ? count : 0;
        }

        // 6. TRẢ VỀ DỮ LIỆU
        data.put("totalAppointments", totalAppointments);
        data.put("availableResults", availableResults);
        data.put("unreadNotifications", unreadNotifications);
        data.put("upcomingAppointment", upcomingAppointment);
        data.put("recentResults", recentResults);

        return data;
    }

    // ==========================================
    // LOGIC DASHBOARD DÀNH CHO BÁC SĨ (MỚI THÊM)
    // ==========================================
    public Map<String, Object> getDoctorDashboardData(Integer doctorId) {
        Map<String, Object> data = new HashMap<>();
        LocalDate today = LocalDate.now();

        // 1. Lấy phòng làm việc hiện tại trong ngày hôm nay
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
            // Không có phòng thì để mặc định "—"
        }

        // 2. Đang chờ khám: Trạng thái 'cho_kham' của bác sĩ trong ngày hôm nay
        String sqlWaiting = "SELECT COUNT(*) FROM luotkham lk " +
                            "JOIN datlichkham d ON lk.IDDatLichKham = d.IDDatLichKham " +
                            "WHERE lk.IDBacSi = ? AND lk.TrangThai = 'cho_kham' AND d.NgayKham = ?";
        Long waitingCount = jdbcTemplate.queryForObject(sqlWaiting, Long.class, doctorId, today);

        // 3. Lượt khám hôm nay: Tổng số lượt khám (không tính hủy)
        String sqlTodayExams = "SELECT COUNT(*) FROM luotkham lk " +
                               "JOIN datlichkham d ON lk.IDDatLichKham = d.IDDatLichKham " +
                               "WHERE lk.IDBacSi = ? AND d.NgayKham = ? AND lk.TrangThai != 'huy'";
        Long todayExamsCount = jdbcTemplate.queryForObject(sqlTodayExams, Long.class, doctorId, today);

        // 4. Kết quả chờ duyệt: Các kết quả xét nghiệm được chỉ định bởi bác sĩ này và đang chờ duyệt
        String sqlPendingResults = "SELECT COUNT(*) FROM ketquaxetnghiem k " +
                                   "JOIN ctphieuxetnghiem ct ON k.IDCTPhieu = ct.IDCTPhieu " +
                                   "JOIN phieuxetnghiem p ON ct.IDPhieuXetNghiem = p.IDPhieuXetNghiem " +
                                   "WHERE k.TrangThai = 'cho_duyet' AND p.IDBacSiChiDinh = ?";
        Long pendingResultsCount = jdbcTemplate.queryForObject(sqlPendingResults, Long.class, doctorId);

        // 5. Hoàn tất hôm nay: Trạng thái 'hoan_tat'
        String sqlCompletedToday = "SELECT COUNT(*) FROM luotkham lk " +
                                   "JOIN datlichkham d ON lk.IDDatLichKham = d.IDDatLichKham " +
                                   "WHERE lk.IDBacSi = ? AND d.NgayKham = ? AND lk.TrangThai = 'hoan_tat'";
        Long completedTodayCount = jdbcTemplate.queryForObject(sqlCompletedToday, Long.class, doctorId, today);

        // Map kết quả trả về
        data.put("currentRoom", currentRoom);
        
        // Trả về nhiều key khác nhau để Frontend bắt được (tuỳ thuộc vào Frontend gọi key nào)
        data.put("waitingCount", waitingCount != null ? waitingCount : 0);
        data.put("waitingExams", waitingCount != null ? waitingCount : 0);
        
        data.put("todayExamsCount", todayExamsCount != null ? todayExamsCount : 0);
        data.put("todayExams", todayExamsCount != null ? todayExamsCount : 0);
        
        data.put("pendingResultsCount", pendingResultsCount != null ? pendingResultsCount : 0);
        data.put("pendingResults", pendingResultsCount != null ? pendingResultsCount : 0);
        
        data.put("completedTodayCount", completedTodayCount != null ? completedTodayCount : 0);
        data.put("completedExams", completedTodayCount != null ? completedTodayCount : 0);

        return data;
    }
}