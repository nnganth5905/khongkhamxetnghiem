package com.biomedic.backend.service;

import java.util.ArrayList;
import java.util.Collections;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

import org.springframework.jdbc.core.JdbcTemplate;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

@Service
public class DoctorService {

    private final JdbcTemplate jdbcTemplate;

    // Inject JdbcTemplate
    public DoctorService(JdbcTemplate jdbcTemplate) {
        this.jdbcTemplate = jdbcTemplate;
    }

    // ==========================================
    // LOGIC DUYỆT KẾT QUẢ CỦA BÁC SĨ
    // ==========================================

    // 1. Lấy danh sách kết quả đang chờ duyệt (Xử lý an toàn với cả Email và Username)
    public List<Map<String, Object>> getPendingResults(String username) {
        // Bước 1: Tìm ID Bác Sĩ của tài khoản đang đăng nhập
        Integer idBacSi = null;
        try {
            idBacSi = jdbcTemplate.queryForObject(
                "SELECT IDBacSi FROM users WHERE (Username = ? OR Email = ?) AND IDBacSi IS NOT NULL LIMIT 1", 
                Integer.class, username, username
            );
        } catch (Exception e) {
            System.out.println("Cảnh báo: Tài khoản " + username + " không có mã IDBacSi hợp lệ.");
        }

        // Bước 2: Truy vấn danh sách
        String sql = 
            "SELECT " +
            "  kq.IDKetQua, " +
            "  m.MaBarcode, " +
            "  k.TenKhachHang, " +
            "  lxn.TenXetNghiem, " +
            "  nv.TenNhanVien as technicianName, " +
            "  kq.ThoiGianHoanThanh as submittedAt " +
            "FROM ketquaxetnghiem kq " +
            "LEFT JOIN ctphieuxetnghiem ct ON kq.IDCTPhieu = ct.IDCTPhieu " +
            "LEFT JOIN phieuxetnghiem p ON ct.IDPhieuXetNghiem = p.IDPhieuXetNghiem " +
            "LEFT JOIN luotxetnghiem luot ON p.IDLuotXetNghiem = luot.IDLuotXetNghiem " +
            "LEFT JOIN datlichxetnghiem dl ON luot.IDDatLichXN = dl.IDDatLichXN " +
            "LEFT JOIN khachhang k ON p.IDKhachHang = k.IDKhachHang " +
            "LEFT JOIN loaixetnghiem lxn ON ct.IDXetNghiem = lxn.IDXetNghiem " +
            "LEFT JOIN maubenhpham m ON kq.IDMau = m.IDMau " +
            "LEFT JOIN nhanvien nv ON kq.IDKTV = nv.IDNhanVien " +
            "WHERE kq.TrangThai = 'cho_duyet' ";

        List<Map<String, Object>> rawData;

        if (idBacSi != null) {
            // Chỉ lấy phiếu do mình phụ trách HOẶC phiếu không ai phụ trách
            sql += " AND (dl.IDBacSi = ? OR dl.IDBacSi IS NULL) ";
            sql += " ORDER BY kq.ThoiGianHoanThanh DESC";
            rawData = jdbcTemplate.queryForList(sql, idBacSi);
        } else {
            // Nếu không phải bác sĩ hợp lệ, chỉ lấy các phiếu mồ côi
            sql += " AND dl.IDBacSi IS NULL ORDER BY kq.ThoiGianHoanThanh DESC";
            rawData = jdbcTemplate.queryForList(sql);
        }

        List<Map<String, Object>> results = new ArrayList<>();

        for (Map<String, Object> row : rawData) {
            Map<String, Object> dto = new LinkedHashMap<>();
            dto.put("id", row.get("IDKetQua"));
            dto.put("specimenCode", row.get("MaBarcode") != null ? row.get("MaBarcode") : "Chưa rõ");
            dto.put("patientName", row.get("TenKhachHang") != null ? row.get("TenKhachHang") : "Khách hàng");
            dto.put("testName", row.get("TenXetNghiem") != null ? row.get("TenXetNghiem") : "Xét nghiệm");
            dto.put("technicianName", row.get("technicianName") != null ? row.get("technicianName") : "KTV Hệ Thống");
            dto.put("submittedAt", row.get("submittedAt"));
            dto.put("status", "PENDING_APPROVAL");
            results.add(dto);
        }
        return results;
    }

    // 2. Xem chi tiết kết quả và các chỉ số
    public Map<String, Object> getResultDetail(String resultId) {
        String infoSql = 
            "SELECT " +
            "  kq.IDKetQua, m.MaBarcode, k.TenKhachHang, lxn.TenXetNghiem, " +
            "  kq.GhiChu as technicianNotes " +
            "FROM ketquaxetnghiem kq " +
            "JOIN maubenhpham m ON kq.IDMau = m.IDMau " +
            "JOIN ctphieuxetnghiem ct ON kq.IDCTPhieu = ct.IDCTPhieu " +
            "JOIN phieuxetnghiem p ON ct.IDPhieuXetNghiem = p.IDPhieuXetNghiem " +
            "JOIN khachhang k ON p.IDKhachHang = k.IDKhachHang " +
            "JOIN loaixetnghiem lxn ON ct.IDXetNghiem = lxn.IDXetNghiem " +
            "WHERE kq.IDKetQua = ?";

        Map<String, Object> response = jdbcTemplate.queryForMap(infoSql, resultId);
        response.put("specimenCode", response.get("MaBarcode"));
        response.put("patientName", response.get("TenKhachHang"));
        response.put("testName", response.get("TenXetNghiem"));

        String chisoSql = 
            "SELECT " +
            "  cs.TenChiSo as name, " +
            "  kqc.GiaTriText as value, " +
            "  cs.DonVi as unit, " +
            "  kqc.DanhGia as evaluation " +
            "FROM ketquachiso kqc " +
            "JOIN chisoxetnghiem cs ON kqc.IDChiSo = cs.IDChiSo " +
            "WHERE kqc.IDKetQua = ?";

        List<Map<String, Object>> rawIndicators = jdbcTemplate.queryForList(chisoSql, resultId);
        List<Map<String, Object>> indicators = new ArrayList<>();
        
        for (Map<String, Object> row : rawIndicators) {
            Map<String, Object> ind = new LinkedHashMap<>();
            ind.put("name", row.get("name"));
            ind.put("value", row.get("value"));
            ind.put("unit", row.get("unit"));
            ind.put("abnormal", "bat_thuong".equals(row.get("evaluation"))); 
            indicators.add(ind);
        }

        response.put("indicators", indicators);
        return response;
    }

    // 3. Phê duyệt kết quả (Đồng bộ toàn bộ các bảng)
    @Transactional
    public Map<String, Object> approveResult(String resultId, String conclusion, String username) {
        Map<String, Object> response = new LinkedHashMap<>();
        try {
            Integer idBacSi = 1;
            try {
                // Sửa câu lệnh tìm ID trong hàm approveResult
                Integer queryId = jdbcTemplate.queryForObject(
                    "SELECT IDBacSi FROM users WHERE (Username = ? OR Email = ?) AND IDBacSi IS NOT NULL LIMIT 1", 
                    Integer.class, username, username
                );
                if (queryId != null) idBacSi = queryId;
            } catch (Exception ignored) {}

            Map<String, Object> kqInfo = jdbcTemplate.queryForMap(
                "SELECT IDCTPhieu, IDMau FROM ketquaxetnghiem WHERE IDKetQua = ?", resultId
            );
            Long idCTPhieu = ((Number) kqInfo.get("IDCTPhieu")).longValue();
            String idMau = (String) kqInfo.get("IDMau");

            String idPhieu = jdbcTemplate.queryForObject(
                "SELECT IDPhieuXetNghiem FROM ctphieuxetnghiem WHERE IDCTPhieu = ?", String.class, idCTPhieu
            );

            Map<String, Object> phieuInfo = jdbcTemplate.queryForMap(
                "SELECT IDLuotXetNghiem, IDKhachHang FROM phieuxetnghiem WHERE IDPhieuXetNghiem = ?", idPhieu
            );
            Long idLuot = phieuInfo.get("IDLuotXetNghiem") != null ? ((Number) phieuInfo.get("IDLuotXetNghiem")).longValue() : null;
            String idKhachHang = (String) phieuInfo.get("IDKhachHang");

            // Cập nhật các bảng nội bộ
            jdbcTemplate.update("UPDATE ketquaxetnghiem SET TrangThai = 'da_duyet', KetLuanBacSi = ?, IDBacSiDuyet = ?, ThoiGianDuyet = NOW() WHERE IDKetQua = ?", conclusion, idBacSi, resultId);
            jdbcTemplate.update("UPDATE ctphieuxetnghiem SET TrangThai = 'da_duyet' WHERE IDCTPhieu = ?", idCTPhieu);
            jdbcTemplate.update("UPDATE phieuxetnghiem SET TrangThai = 'da_co_kq' WHERE IDPhieuXetNghiem = ?", idPhieu);
            if (idMau != null) jdbcTemplate.update("UPDATE maubenhpham SET TrangThai = 'hoan_tat' WHERE IDMau = ?", idMau);

            // BỔ SUNG: CẬP NHẬT TRẠNG THÁI TIMELINE VÀ LỊCH HẸN
            if (idLuot != null) {
                jdbcTemplate.update("UPDATE luotxetnghiem SET TrangThai = 'hoan_tat' WHERE IDLuotXetNghiem = ?", idLuot);
                
                // Lấy thông tin lịch hẹn để cập nhật
                List<Map<String, Object>> dlInfoList = jdbcTemplate.queryForList(
                    "SELECT IDDatLichXN FROM luotxetnghiem WHERE IDLuotXetNghiem = ?", idLuot
                );
                
                if (!dlInfoList.isEmpty()) {
                    Long idDatLichXN = ((Number) dlInfoList.get(0).get("IDDatLichXN")).longValue();
                    
                    // Chốt lịch hẹn thành completed
                    jdbcTemplate.update("UPDATE datlichxetnghiem SET TrangThai = 'completed' WHERE IDDatLichXN = ?", idDatLichXN);
                    
                    // Ghi log vào Timeline Khách hàng
                    jdbcTemplate.update(
                        "INSERT INTO truyvet (IDKhachHang, LoaiDoiTuong, IDDoiTuong, HanhDong, NguonThucHien, ThoiGian, MoTa) VALUES (?, 'datlichxetnghiem', ?, 'Đã có kết quả xét nghiệm', 'system', NOW(), ?)", 
                        idKhachHang, String.valueOf(idDatLichXN), "Bác sĩ đã phê duyệt và trả kết quả."
                    );
                }
            }

            response.put("status", "SUCCESS");
            response.put("message", "Đã phê duyệt kết quả xét nghiệm thành công.");
        } catch (Exception e) {
            org.springframework.transaction.interceptor.TransactionAspectSupport.currentTransactionStatus().setRollbackOnly();
            response.put("error", "Lỗi CSDL khi duyệt kết quả: " + e.getMessage());
        }
        return response;
    }
    // ==========================================
    // CÁC HÀM CŨ (GIỮ NGUYÊN)
    // ==========================================
    public List<Map<String, Object>> getDoctors(String specialtyId, String q) {
        return Collections.emptyList();
    }

    public Map<String, Object> getDoctorById(String id) {
        Map<String, Object> result = new LinkedHashMap<>();
        result.put("id", id);
        result.put("message", "Chưa nối repository bảng bacsi.");
        return result;
    }

    public Map<String, Object> createDoctor(
            String hoTen, String idChuyenKhoa, String hocVi, String chucDanh,
            String soDienThoai, String email, String idPhong, String hinhAnh,
            String gioiThieu, String trangThai
    ) {
        return buildDoctor(null, hoTen, idChuyenKhoa, hocVi, chucDanh, soDienThoai, email, idPhong, hinhAnh, gioiThieu, trangThai);
    }

    public Map<String, Object> updateDoctor(
            String id, String hoTen, String idChuyenKhoa, String hocVi, String chucDanh,
            String soDienThoai, String email, String idPhong, String hinhAnh,
            String gioiThieu, String trangThai
    ) {
        return buildDoctor(id, hoTen, idChuyenKhoa, hocVi, chucDanh, soDienThoai, email, idPhong, hinhAnh, gioiThieu, trangThai);
    }

    public void deleteDoctor(String id) {
        // TODO: doctorRepository.deleteById(...)
    }

    private Map<String, Object> buildDoctor(
            String id, String hoTen, String idChuyenKhoa, String hocVi, String chucDanh,
            String soDienThoai, String email, String idPhong, String hinhAnh,
            String gioiThieu, String trangThai
    ) {
        Map<String, Object> result = new LinkedHashMap<>();
        result.put("id", id);
        result.put("hoTen", hoTen);
        result.put("idChuyenKhoa", idChuyenKhoa);
        result.put("hocVi", hocVi);
        result.put("chucDanh", chucDanh);
        result.put("soDienThoai", soDienThoai);
        result.put("email", email);
        result.put("idPhong", idPhong);
        result.put("hinhAnh", hinhAnh);
        result.put("gioiThieu", gioiThieu);
        result.put("trangThai", trangThai);
        return result;
    }
}