package com.biomedic.backend.service;

import java.time.LocalDate;
import java.util.ArrayList;
import java.util.Collections;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

import org.springframework.jdbc.core.JdbcTemplate;
import org.springframework.security.core.Authentication;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.stereotype.Service;

@Service
public class TechnicianService {

    private final JdbcTemplate jdbcTemplate;

    // Inject JdbcTemplate để thực thi SQL Query
    public TechnicianService(JdbcTemplate jdbcTemplate) {
        this.jdbcTemplate = jdbcTemplate;
    }

    // ==========================================
    // MAPPER TRẠNG THÁI (DB <-> API FRONTEND)
    // ==========================================
    private String mapSpecimenStatusToApi(String dbStatus) {
        if (dbStatus == null) return "UNKNOWN";
        return switch (dbStatus) {
            case "da_ban_giao" -> "HANDED_OVER";
            case "ktv_tiep_nhan" -> "RECEIVED";
            case "tu_choi_mau" -> "REJECTED";
            case "dang_xu_ly" -> "IN_PROGRESS";
            case "hoan_tat" -> "COMPLETED";
            default -> dbStatus.toUpperCase();
        };
    }

    private String mapApiStatusToDb(String apiStatus) {
        if (apiStatus == null || apiStatus.isEmpty()) return null;
        return switch (apiStatus.toUpperCase()) {
            case "HANDED_OVER" -> "da_ban_giao";
            case "RECEIVED" -> "ktv_tiep_nhan";
            case "REJECTED" -> "tu_choi_mau";
            case "IN_PROGRESS" -> "dang_xu_ly";
            case "COMPLETED" -> "hoan_tat";
            default -> apiStatus.toLowerCase();
        };
    }

    // ==========================================
    // QUẢN LÝ MẪU BỆNH PHẨM (DÀNH CHO KTV)
    // ==========================================
    
    // API: Lấy danh sách mẫu bệnh phẩm (Sử dụng cho DanhSachMau.jsx)
    public List<Map<String, Object>> getSpecimens(String apiStatus) {
        String dbStatus = mapApiStatusToDb(apiStatus);
        
        StringBuilder sql = new StringBuilder(
            "SELECT " +
            "  m.IDMau, " +
            "  m.MaBarcode, " +
            "  k.TenKhachHang, " +
            "  m.LoaiMau, " +
            "  m.ThoiGianLayMau, " +
            "  m.TrangThai " +
            "FROM maubenhpham m " +
            "LEFT JOIN ctphieuxetnghiem ct ON m.IDCTPhieu = ct.IDCTPhieu " +
            "LEFT JOIN phieuxetnghiem p ON ct.IDPhieuXetNghiem = p.IDPhieuXetNghiem " +
            "LEFT JOIN khachhang k ON p.IDKhachHang = k.IDKhachHang "
        );
        
        List<Object> params = new ArrayList<>();
        if (dbStatus != null) {
            sql.append(" WHERE m.TrangThai = ? ");
            params.add(dbStatus);
        } else {
            // Nếu không truyền status, chỉ lấy các mẫu liên quan đến KTV (đã bàn giao trở lên)
            sql.append(" WHERE m.TrangThai NOT IN ('moi_tao', 'da_lay_mau', 'huy') ");
        }
        
        sql.append(" ORDER BY m.ThoiGianLayMau DESC");

        List<Map<String, Object>> rawData = jdbcTemplate.queryForList(sql.toString(), params.toArray());
        List<Map<String, Object>> results = new ArrayList<>();

        for (Map<String, Object> row : rawData) {
            Map<String, Object> dto = new LinkedHashMap<>();
            dto.put("idMauBenhPham", row.get("IDMau"));
            dto.put("maMau", row.get("IDMau"));
            dto.put("maVach", row.get("MaBarcode"));
            dto.put("tenKhachHang", row.get("TenKhachHang"));
            dto.put("loaiMau", row.get("LoaiMau"));
            dto.put("thoiGianLay", row.get("ThoiGianLayMau"));
            
            String trangThai = (String) row.get("TrangThai");
            dto.put("trangThai", mapSpecimenStatusToApi(trangThai)); 
            
            results.add(dto);
        }
        
        return results;
    }

    // API: Lấy chi tiết 1 mẫu bệnh phẩm (Sử dụng cho TiepNhanMau.jsx)
    public Map<String, Object> getSpecimen(String specimenId) {
        String sql = 
            "SELECT " +
            "  m.IDMau, " +
            "  m.MaBarcode, " +
            "  k.TenKhachHang, " +
            "  m.LoaiMau, " +
            "  m.ThoiGianLayMau, " +
            "  m.TrangThai, " +
            "  m.GhiChu " +
            "FROM maubenhpham m " +
            "LEFT JOIN ctphieuxetnghiem ct ON m.IDCTPhieu = ct.IDCTPhieu " +
            "LEFT JOIN phieuxetnghiem p ON ct.IDPhieuXetNghiem = p.IDPhieuXetNghiem " +
            "LEFT JOIN khachhang k ON p.IDKhachHang = k.IDKhachHang " +
            "WHERE m.IDMau = ?";
            
        List<Map<String, Object>> rawData = jdbcTemplate.queryForList(sql, specimenId);
        if (rawData.isEmpty()) {
            throw new RuntimeException("Không tìm thấy mẫu bệnh phẩm với ID: " + specimenId);
        }
        
        Map<String, Object> row = rawData.get(0);
        Map<String, Object> dto = new LinkedHashMap<>();
        
        dto.put("code", row.get("IDMau"));
        dto.put("barcode", row.get("MaBarcode"));
        dto.put("patientName", row.get("TenKhachHang"));
        dto.put("specimenType", row.get("LoaiMau"));
        dto.put("collectedAt", row.get("ThoiGianLayMau"));
        dto.put("status", mapSpecimenStatusToApi((String) row.get("TrangThai")));
        dto.put("notes", row.get("GhiChu"));
        
        // Mock dữ liệu người bàn giao (Nếu cần chính xác, bạn cần JOIN với bảng bangiaomau và nhanvien)
        dto.put("handoverBy", "Bác sĩ / Điều dưỡng (Chưa map DB)");
        
        return dto;
    }


    // ==========================================
    // CÁC HÀM CŨ (GIỮ NGUYÊN)
    // ==========================================
    public Map<String, Object> getCurrentTechnician() {
        Authentication authentication =
                SecurityContextHolder
                        .getContext()
                        .getAuthentication();
        return getCurrentTechnician(authentication);
    }

    public Map<String, Object> getCurrentTechnician(
            Authentication authentication
    ) {
        Map<String, Object> result = new LinkedHashMap<>();
        result.put("username", authentication != null ? authentication.getName() : null);
        result.put("authenticated", authentication != null && authentication.isAuthenticated());
        result.put("message", "TechnicianService đã hoạt động. Sẽ nối repository bảng nhanvien khi map schema MySQL.");
        return result;
    }

    public List<Map<String, Object>> getTechnicians(String q) {
        return Collections.emptyList();
    }

    public Map<String, Object> getTechnicianById(String id) {
        Map<String, Object> result = new LinkedHashMap<>();
        result.put("id", id);
        result.put("message", "Chưa nối repository kỹ thuật viên.");
        return result;
    }

    public Map<String, Object> createTechnician(
            String hoTen, String idCoSo, String trinhDo, String soDienThoai,
            String email, LocalDate ngayVaoLam, String trangThai, String ghiChu
    ) {
        return buildTechnician(null, hoTen, idCoSo, trinhDo, soDienThoai, email, ngayVaoLam, trangThai, ghiChu);
    }

    public Map<String, Object> updateTechnician(
            String id, String hoTen, String idCoSo, String trinhDo, String soDienThoai,
            String email, LocalDate ngayVaoLam, String trangThai, String ghiChu
    ) {
        return buildTechnician(id, hoTen, idCoSo, trinhDo, soDienThoai, email, ngayVaoLam, trangThai, ghiChu);
    }

    public void deleteTechnician(String id) {
        // TODO: technicianRepository.deleteById(...)
    }

    private Map<String, Object> buildTechnician(
            String id, String hoTen, String idCoSo, String trinhDo, String soDienThoai,
            String email, LocalDate ngayVaoLam, String trangThai, String ghiChu
    ) {
        Map<String, Object> result = new LinkedHashMap<>();
        result.put("id", id);
        result.put("hoTen", hoTen);
        result.put("idCoSo", idCoSo);
        result.put("trinhDo", trinhDo);
        result.put("soDienThoai", soDienThoai);
        result.put("email", email);
        result.put("ngayVaoLam", ngayVaoLam);
        result.put("trangThai", trangThai);
        result.put("ghiChu", ghiChu);
        return result;
    }
}