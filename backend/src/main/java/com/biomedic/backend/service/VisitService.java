package com.biomedic.backend.service;

import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;
import java.util.ArrayList;
import java.util.Collections;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

import org.springframework.jdbc.core.JdbcTemplate;
import org.springframework.stereotype.Service;

@Service
public class VisitService {

    private final JdbcTemplate jdbcTemplate;

    public VisitService(JdbcTemplate jdbcTemplate) {
        this.jdbcTemplate = jdbcTemplate;
    }

    public Map<String, Object> checkIn(
            String appointmentId,
            String customerId,
            String visitType,
            String note
    ) {
        Map<String, Object> result = new LinkedHashMap<>();
        result.put("id", null);
        result.put("appointmentId", appointmentId);
        result.put("customerId", customerId);
        result.put("visitType", visitType);
        result.put("status", "WAITING");
        result.put("note", note);

        return result;
    }

    // HÀM ĐÃ ĐƯỢC FIX LỖI ÉP KIỂU
    public List<Map<String, Object>> getWaitingList(String type) {
        List<Map<String, Object>> waitingList = new ArrayList<>();
        DateTimeFormatter formatter = DateTimeFormatter.ofPattern("HH:mm");

        // 1. Lấy danh sách chờ Khám bệnh
        if (type == null || "EXAMINATION".equalsIgnoreCase(type) || "ALL".equalsIgnoreCase(type)) {
            String sqlExam = "SELECT lk.SoThuTu, d.MaDatLich, k.TenKhachHang, 'Khám bệnh' AS DichVu, " +
                             "p.TenPhong, lk.ThoiGianTiepNhan, lk.TrangThai " +
                             "FROM luotkham lk " +
                             "JOIN datlichkham d ON lk.IDDatLichKham = d.IDDatLichKham " +
                             "JOIN khachhang k ON lk.IDKhachHang = k.IDKhachHang " +
                             "LEFT JOIN phong p ON lk.IDPhong = p.IDPhong " +
                             "WHERE DATE(lk.ThoiGianTiepNhan) = CURRENT_DATE " +
                             "AND lk.TrangThai IN ('da_tiep_nhan', 'cho_kham')";
            
            List<Map<String, Object>> exams = jdbcTemplate.queryForList(sqlExam);
            for (Map<String, Object> row : exams) {
                Map<String, Object> item = new LinkedHashMap<>();
                item.put("stt", row.get("SoThuTu"));
                item.put("maLuot", row.get("MaDatLich"));
                item.put("tenNguoiBenh", row.get("TenKhachHang"));
                item.put("dichVu", row.get("DichVu"));
                item.put("phong", row.get("TenPhong") != null ? row.get("TenPhong") : "Chưa xếp phòng");
                
                // Xử lý an toàn cho ThoiGianTiepNhan
                Object thoiGianObj = row.get("ThoiGianTiepNhan");
                LocalDateTime thoiGian = null;
                if (thoiGianObj instanceof java.sql.Timestamp) {
                    thoiGian = ((java.sql.Timestamp) thoiGianObj).toLocalDateTime();
                } else if (thoiGianObj instanceof LocalDateTime) {
                    thoiGian = (LocalDateTime) thoiGianObj;
                }

                item.put("checkInTime", thoiGian != null ? thoiGian.format(formatter) : "");
                item.put("trangThai", row.get("TrangThai"));
                item.put("thoiGianRaw", thoiGian); // Để sort
                
                waitingList.add(item);
            }
        }

        // 2. Lấy danh sách chờ Xét nghiệm
        if (type == null || "TEST".equalsIgnoreCase(type) || "ALL".equalsIgnoreCase(type)) {
            String sqlTest = "SELECT lx.SoThuTu, d.MaDatLich, k.TenKhachHang, 'Xét nghiệm' AS DichVu, " +
                             "p.TenPhong, lx.ThoiGianTiepNhan, lx.TrangThai " +
                             "FROM luotxetnghiem lx " +
                             "JOIN datlichxetnghiem d ON lx.IDDatLichXN = d.IDDatLichXN " +
                             "JOIN khachhang k ON lx.IDKhachHang = k.IDKhachHang " +
                             "LEFT JOIN phong p ON lx.IDPhong = p.IDPhong " +
                             "WHERE DATE(lx.ThoiGianTiepNhan) = CURRENT_DATE " +
                             "AND lx.TrangThai IN ('da_tiep_nhan', 'cho_xet_nghiem')";
            
            List<Map<String, Object>> tests = jdbcTemplate.queryForList(sqlTest);
            for (Map<String, Object> row : tests) {
                Map<String, Object> item = new LinkedHashMap<>();
                item.put("stt", row.get("SoThuTu"));
                item.put("maLuot", row.get("MaDatLich"));
                item.put("tenNguoiBenh", row.get("TenKhachHang"));
                item.put("dichVu", row.get("DichVu"));
                item.put("phong", row.get("TenPhong") != null ? row.get("TenPhong") : "Chưa xếp phòng");
                
                // Xử lý an toàn cho ThoiGianTiepNhan
                Object thoiGianObj = row.get("ThoiGianTiepNhan");
                LocalDateTime thoiGian = null;
                if (thoiGianObj instanceof java.sql.Timestamp) {
                    thoiGian = ((java.sql.Timestamp) thoiGianObj).toLocalDateTime();
                } else if (thoiGianObj instanceof LocalDateTime) {
                    thoiGian = (LocalDateTime) thoiGianObj;
                }

                item.put("checkInTime", thoiGian != null ? thoiGian.format(formatter) : "");
                item.put("trangThai", row.get("TrangThai"));
                item.put("thoiGianRaw", thoiGian); // Để sort
                
                waitingList.add(item);
            }
        }

        // Sắp xếp danh sách theo thời gian check-in (ai đến trước lên trước)
        waitingList.sort((a, b) -> {
            LocalDateTime timeA = (LocalDateTime) a.get("thoiGianRaw");
            LocalDateTime timeB = (LocalDateTime) b.get("thoiGianRaw");
            if (timeA == null && timeB == null) return 0;
            if (timeA == null) return 1; // Đẩy null xuống cuối
            if (timeB == null) return -1;
            return timeA.compareTo(timeB);
        });

        // Loại bỏ trường thoiGianRaw trước khi trả về
        waitingList.forEach(item -> item.remove("thoiGianRaw"));

        return waitingList;
    }

    public Map<String, Object> getVisitById(String id) {
        Map<String, Object> result = new LinkedHashMap<>();
        result.put("id", id);
        result.put("message", "Chưa nối repository lượt khám/lượt xét nghiệm.");
        return result;
    }

    public Map<String, Object> updateStatus(
            String id,
            String status,
            String note
    ) {
        Map<String, Object> result = new LinkedHashMap<>();
        result.put("id", id);
        result.put("status", status);
        result.put("note", note);
        return result;
    }
    // Lấy danh sách hàng chờ riêng cho Bác sĩ đang đăng nhập
    public List<Map<String, Object>> getDoctorWaitingQueue(org.springframework.security.core.Authentication auth) {
        // Lấy IDBacSi từ user đang đăng nhập
        String email = auth.getName();
        Integer idBacSi = jdbcTemplate.queryForObject("SELECT IDBacSi FROM users WHERE Email = ?", Integer.class, email);

        String sql = "SELECT lk.IDLuotKham, lk.SoThuTu, k.IDKhachHang, k.TenKhachHang, d.GioKham, lk.TrangThai, lk.ThoiGianTiepNhan " +
                     "FROM luotkham lk " +
                     "JOIN datlichkham d ON lk.IDDatLichKham = d.IDDatLichKham " +
                     "JOIN khachhang k ON lk.IDKhachHang = k.IDKhachHang " +
                     "WHERE lk.IDBacSi = ? AND DATE(lk.ThoiGianTiepNhan) = CURRENT_DATE " +
                     "AND lk.TrangThai IN ('da_tiep_nhan', 'cho_kham', 'da_den_luot', 'cho_goi_lai') " +
                     "ORDER BY CASE lk.TrangThai WHEN 'da_den_luot' THEN 1 ELSE 2 END, lk.SoThuTu ASC";

        List<Map<String, Object>> rows = jdbcTemplate.queryForList(sql, idBacSi);
        List<Map<String, Object>> result = new java.util.ArrayList<>();
        
        java.time.format.DateTimeFormatter formatter = java.time.format.DateTimeFormatter.ofPattern("HH:mm");
        for (Map<String, Object> row : rows) {
            Map<String, Object> item = new LinkedHashMap<>();
            item.put("id", row.get("IDLuotKham"));
            item.put("stt", row.get("SoThuTu"));
            item.put("maKhachHang", row.get("IDKhachHang"));
            item.put("tenKhachHang", row.get("TenKhachHang"));
            item.put("gioKham", row.get("GioKham") != null ? row.get("GioKham").toString() : "---");
            item.put("trangThai", row.get("TrangThai"));
            result.add(item);
        }
        return result;
    }

    @org.springframework.transaction.annotation.Transactional
    public void callPatient(Long idLuotKham) {
        // Đổi trạng thái thành Đã đến lượt
        jdbcTemplate.update("UPDATE luotkham SET TrangThai = 'da_den_luot' WHERE IDLuotKham = ?", idLuotKham);
        
        String idKhachHang = jdbcTemplate.queryForObject("SELECT IDKhachHang FROM luotkham WHERE IDLuotKham = ?", String.class, idLuotKham);
        
        // Ghi lịch sử truy vết
        jdbcTemplate.update("INSERT INTO truyvet (IDKhachHang, LoaiDoiTuong, IDDoiTuong, HanhDong, NguonThucHien, ThoiGian) VALUES (?, 'luotkham', ?, 'Bác sĩ gọi vào phòng', 'user', NOW())", idKhachHang, idLuotKham.toString());
        
        // Gửi thông báo cho bệnh nhân
        try {
            Integer userId = jdbcTemplate.queryForObject("SELECT UserID FROM users WHERE IDKhachHang = ?", Integer.class, idKhachHang);
            if(userId != null) {
                jdbcTemplate.update("INSERT INTO thongbao (UserIDNhan, LoaiThongBao, TieuDe, NoiDung, ThoiGianTao) VALUES (?, 'kham_benh', 'Đã đến lượt khám', 'Vui lòng di chuyển vào phòng khám để gặp bác sĩ.', NOW())", userId);
            }
        } catch (org.springframework.dao.EmptyResultDataAccessException e) {
            // Khách hàng chưa có tài khoản app, bỏ qua việc gửi thông báo
        }
    }

    @org.springframework.transaction.annotation.Transactional
    public void skipPatient(Long idLuotKham) {
        jdbcTemplate.update("UPDATE luotkham SET TrangThai = 'cho_goi_lai' WHERE IDLuotKham = ?", idLuotKham);
        String idKhachHang = jdbcTemplate.queryForObject("SELECT IDKhachHang FROM luotkham WHERE IDLuotKham = ?", String.class, idLuotKham);
        jdbcTemplate.update("INSERT INTO truyvet (IDKhachHang, LoaiDoiTuong, IDDoiTuong, HanhDong, NguonThucHien, ThoiGian) VALUES (?, 'luotkham', ?, 'Bệnh nhân vắng mặt, chuyển trạng thái chờ gọi lại', 'user', NOW())", idKhachHang, idLuotKham.toString());
    }

    @org.springframework.transaction.annotation.Transactional
    public void startExam(Long idLuotKham, org.springframework.security.core.Authentication auth) {
        String email = auth.getName();
        Integer idBacSi = jdbcTemplate.queryForObject("SELECT IDBacSi FROM users WHERE Email = ?", Integer.class, email);

        // Đổi trạng thái và cập nhật thời gian bắt đầu
        jdbcTemplate.update("UPDATE luotkham SET TrangThai = 'dang_kham', ThoiGianBatDau = NOW() WHERE IDLuotKham = ?", idLuotKham);
        
        // Tạo bản ghi trong bảng `kham`
        jdbcTemplate.update("INSERT INTO kham (IDLuotKham, IDBacSi, ThoiGianKham) VALUES (?, ?, NOW())", idLuotKham, idBacSi);
        
        String idKhachHang = jdbcTemplate.queryForObject("SELECT IDKhachHang FROM luotkham WHERE IDLuotKham = ?", String.class, idLuotKham);
        jdbcTemplate.update("INSERT INTO truyvet (IDKhachHang, LoaiDoiTuong, IDDoiTuong, HanhDong, NguonThucHien, ThoiGian) VALUES (?, 'luotkham', ?, 'Bác sĩ bắt đầu khám', 'user', NOW())", idKhachHang, idLuotKham.toString());
    }
}