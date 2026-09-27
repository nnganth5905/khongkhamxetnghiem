package com.biomedic.backend.service;

import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

import org.springframework.jdbc.core.JdbcTemplate;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

@Service
public class ExaminationService {

    private final JdbcTemplate jdbcTemplate;

    public ExaminationService(JdbcTemplate jdbcTemplate) {
        this.jdbcTemplate = jdbcTemplate;
    }

    @Transactional
    public Map<String, Object> getByVisit(String visitId) {
        Map<String, Object> result = new LinkedHashMap<>();
        try {
            Long numericId = -1L;
            try { numericId = Long.parseLong(visitId); } catch(Exception e) {}

            // Dùng LEFT JOIN với bảng `kham` để luôn lấy được thông tin Khách hàng dù bệnh án chưa tạo
            String sql = "SELECT lk.IDLuotKham, lk.TrangThai, kh.IDKhachHang, kh.TenKhachHang, kh.GioiTinh, kh.NgaySinh, kh.SoDienThoai, " +
                         "k.IDKham, k.TrieuChung, k.TienSuBenh, k.ChanDoan, k.KetLuan, k.HuongDieuTri " +
                         "FROM luotkham lk " +
                         "JOIN datlichkham d ON lk.IDDatLichKham = d.IDDatLichKham " +
                         "JOIN khachhang kh ON lk.IDKhachHang = kh.IDKhachHang " +
                         "LEFT JOIN kham k ON k.IDLuotKham = lk.IDLuotKham " +
                         "WHERE d.MaDatLich = ? OR lk.IDLuotKham = ?";
                         
            List<Map<String, Object>> rows = jdbcTemplate.queryForList(sql, visitId, numericId);
            
            if (!rows.isEmpty()) {
                Map<String, Object> row = rows.get(0);
                String idLuotKham = String.valueOf(row.get("IDLuotKham"));
                String trangThai = (String) row.get("TrangThai");
                String idKhachHang = (String) row.get("IDKhachHang");
                
                // TỰ ĐỘNG CHUYỂN TRẠNG THÁI "ĐANG KHÁM" & GHI TRUY VẾT KHI BÁC SĨ MỞ TRANG
                if ("da_tiep_nhan".equals(trangThai) || "da_den_luot".equals(trangThai) || "cho_goi_lai".equals(trangThai)) {
                    jdbcTemplate.update("UPDATE luotkham SET TrangThai = 'dang_kham', ThoiGianBatDau = NOW() WHERE IDLuotKham = ?", idLuotKham);
                    jdbcTemplate.update(
                        "INSERT INTO truyvet (IDKhachHang, LoaiDoiTuong, IDDoiTuong, HanhDong, NguonThucHien, ThoiGian) VALUES (?, 'luotkham', ?, 'Bác sĩ bắt đầu khám bệnh', 'user', NOW())", 
                        idKhachHang, idLuotKham
                    );
                }

                result.put("visitId", visitId); 
                result.put("IDKham", row.get("IDKham"));
                result.put("TrieuChung", row.get("TrieuChung"));
                result.put("TienSuBenh", row.get("TienSuBenh"));
                result.put("ChanDoan", row.get("ChanDoan"));
                result.put("KetLuan", row.get("KetLuan"));
                result.put("HuongDieuTri", row.get("HuongDieuTri"));
                
                // Trả về thông tin khách hàng cho Frontend hiển thị
                result.put("TenKhachHang", row.get("TenKhachHang"));
                result.put("GioiTinh", row.get("GioiTinh"));
                result.put("NgaySinh", row.get("NgaySinh"));
                result.put("SoDienThoai", row.get("SoDienThoai"));
            } else {
                result.put("visitId", visitId);
                result.put("message", "Chưa có dữ liệu bệnh án cho lượt khám này.");
            }
        } catch (Exception e) {
            result.put("error", "Lỗi truy vấn cơ sở dữ liệu: " + e.getMessage());
        }
        return result;
    }

    @Transactional
    public Map<String, Object> createExamination(
            String visitId,
            String symptoms,
            String history,
            String diagnosis,
            String conclusion,
            String advice
    ) {
        Long numericId = -1L;
        try { numericId = Long.parseLong(visitId); } catch(Exception e) {}

        // 1. Lấy IDLuotKham thật và IDBacSi từ bảng luotkham
        Map<String, Object> info = jdbcTemplate.queryForMap(
            "SELECT lk.IDLuotKham, lk.IDBacSi " +
            "FROM luotkham lk " +
            "JOIN datlichkham d ON lk.IDDatLichKham = d.IDDatLichKham " +
            "WHERE d.MaDatLich = ? OR lk.IDLuotKham = ?", 
            visitId, numericId
        );
        
        Long realIdLuotKham = ((Number) info.get("IDLuotKham")).longValue();
        Integer idBacSi = ((Number) info.get("IDBacSi")).intValue();

        // 2. Thêm IDBacSi vào câu lệnh INSERT
        String sql = "INSERT INTO kham (IDLuotKham, IDBacSi, TrieuChung, TienSuBenh, ChanDoan, KetLuan, HuongDieuTri, ThoiGianKham) " +
                     "VALUES (?, ?, ?, ?, ?, ?, ?, NOW())";
        jdbcTemplate.update(sql, realIdLuotKham, idBacSi, symptoms, history, diagnosis, conclusion, advice);

        return build(null, visitId, symptoms, history, diagnosis, conclusion, advice);
    }

    @Transactional
    public Map<String, Object> updateExamination(
            String id, // IDKham
            String visitId,
            String symptoms,
            String history,
            String diagnosis,
            String conclusion,
            String advice
    ) {
        Long numericId = -1L;
        try { numericId = Long.parseLong(visitId); } catch(Exception e) {}

        // 1. Lấy IDLuotKham thật và IDBacSi
        Map<String, Object> info = jdbcTemplate.queryForMap(
            "SELECT lk.IDLuotKham, lk.IDBacSi " +
            "FROM luotkham lk " +
            "JOIN datlichkham d ON lk.IDDatLichKham = d.IDDatLichKham " +
            "WHERE d.MaDatLich = ? OR lk.IDLuotKham = ?", 
            visitId, numericId
        );
        
        Long realIdLuotKham = ((Number) info.get("IDLuotKham")).longValue();
        Integer idBacSi = ((Number) info.get("IDBacSi")).intValue();

        // 2. Sử dụng UPSERT: Nếu chưa có thì INSERT, nếu có rồi (trùng IDLuotKham) thì tự động UPDATE
        String sql = "INSERT INTO kham (IDLuotKham, IDBacSi, TrieuChung, TienSuBenh, ChanDoan, KetLuan, HuongDieuTri, ThoiGianKham) " +
                     "VALUES (?, ?, ?, ?, ?, ?, ?, NOW()) " +
                     "ON DUPLICATE KEY UPDATE " +
                     "TrieuChung = VALUES(TrieuChung), " +
                     "TienSuBenh = VALUES(TienSuBenh), " +
                     "ChanDoan = VALUES(ChanDoan), " +
                     "KetLuan = VALUES(KetLuan), " +
                     "HuongDieuTri = VALUES(HuongDieuTri)";

        jdbcTemplate.update(sql, realIdLuotKham, idBacSi, symptoms, history, diagnosis, conclusion, advice);

        return build(id, visitId, symptoms, history, diagnosis, conclusion, advice);
    }

    @Transactional
    public Map<String, Object> completeExamination(String visitId) { 
        Map<String, Object> result = new LinkedHashMap<>();
        try {
            Long numericId = -1L;
            try { numericId = Long.parseLong(visitId); } catch(Exception e) {}

            // Lấy IDLuotKham và IDKhachHang
            Map<String, Object> lk = jdbcTemplate.queryForMap(
                "SELECT lk.IDLuotKham, kh.IDKhachHang FROM luotkham lk JOIN datlichkham d ON lk.IDDatLichKham = d.IDDatLichKham JOIN khachhang kh ON lk.IDKhachHang = kh.IDKhachHang WHERE d.MaDatLich = ? OR lk.IDLuotKham = ?", 
                visitId, numericId);
            
            Long realIdLuotKham = ((Number) lk.get("IDLuotKham")).longValue();
            String idKhachHang = (String) lk.get("IDKhachHang");

            // Chuyển trạng thái lượt khám thành hoan_tat
            jdbcTemplate.update("UPDATE luotkham SET TrangThai = 'hoan_tat', ThoiGianKetThuc = NOW() WHERE IDLuotKham = ?", realIdLuotKham);
            
            // Ghi lịch sử truy vết
            jdbcTemplate.update("INSERT INTO truyvet (IDKhachHang, LoaiDoiTuong, IDDoiTuong, HanhDong, NguonThucHien, ThoiGian) VALUES (?, 'luotkham', ?, 'Bác sĩ hoàn tất khám', 'user', NOW())", idKhachHang, String.valueOf(realIdLuotKham));
            
            result.put("id", visitId);
            result.put("status", "COMPLETED");
            result.put("message", "Đã hoàn tất phiên khám.");
        } catch (Exception e) {
            result.put("error", e.getMessage());
        }
        return result;
    }

    private Map<String, Object> build(
            String id,
            String visitId,
            String symptoms,
            String history,
            String diagnosis,
            String conclusion,
            String advice
    ) {
        Map<String, Object> result = new LinkedHashMap<>();

        result.put("id", id);
        result.put("visitId", visitId);
        result.put("symptoms", symptoms);
        result.put("history", history);
        result.put("diagnosis", diagnosis);
        result.put("conclusion", conclusion);
        result.put("advice", advice);

        return result;
    }
}