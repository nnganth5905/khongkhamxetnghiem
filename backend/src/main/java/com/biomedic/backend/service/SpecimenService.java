package com.biomedic.backend.service;

import com.biomedic.backend.controller.SpecimenController;
import org.springframework.jdbc.core.JdbcTemplate;
import org.springframework.jdbc.support.GeneratedKeyHolder;
import org.springframework.jdbc.support.KeyHolder;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.sql.PreparedStatement;
import java.sql.Statement;
import java.util.ArrayList;
import java.util.Collections;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

@Service
public class SpecimenService {

    private final JdbcTemplate jdbcTemplate;

    public SpecimenService(JdbcTemplate jdbcTemplate) {
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
    // CÁC HÀM CỦA BÁC SĨ (GIỮ NGUYÊN)
    // ==========================================

    public List<Map<String, Object>> getTechnicians() {
        String sql = "SELECT IDNhanVien as id, TenNhanVien as name FROM nhanvien WHERE ViTri = 'ktv' AND Status = 'yes'";
        return jdbcTemplate.queryForList(sql);
    }

    @Transactional
    public Map<String, Object> collectSpecimen(SpecimenController.CollectSpecimenRequest request) {
        Map<String, Object> result = new LinkedHashMap<>();
        
        try {
            Map<String, Object> info = jdbcTemplate.queryForMap(
                "SELECT lxn.IDLuotXetNghiem, lxn.IDKhachHang, d.IDDatLichXN, d.IDBacSi " +
                "FROM luotxetnghiem lxn " +
                "JOIN datlichxetnghiem d ON lxn.IDDatLichXN = d.IDDatLichXN WHERE d.MaDatLich = ?", 
                request.appointmentId()
            );
            Long idLuotXN = ((Number) info.get("IDLuotXetNghiem")).longValue();
            String idKhachHang = (String) info.get("IDKhachHang");
            Long idDatLichXN = ((Number) info.get("IDDatLichXN")).longValue();
            Integer idBacSi = info.get("IDBacSi") != null ? ((Number) info.get("IDBacSi")).intValue() : 1;

            jdbcTemplate.update("UPDATE luotxetnghiem SET TrangThai = 'da_lay_mau' WHERE IDLuotXetNghiem = ?", idLuotXN);

            String maPhieu = "PXN" + idLuotXN;
            List<String> phieuExists = jdbcTemplate.queryForList("SELECT IDPhieuXetNghiem FROM phieuxetnghiem WHERE IDPhieuXetNghiem = ?", String.class, maPhieu);
            if (phieuExists.isEmpty()) {
                jdbcTemplate.update("INSERT INTO phieuxetnghiem (IDPhieuXetNghiem, IDKhachHang, IDLuotXetNghiem, TrangThai, IDBacSiChiDinh) VALUES (?, ?, ?, 'da_lay_mau', ?)", maPhieu, idKhachHang, idLuotXN, idBacSi);
            } else {
                jdbcTemplate.update("UPDATE phieuxetnghiem SET TrangThai = 'da_lay_mau' WHERE IDPhieuXetNghiem = ?", maPhieu);
            }

            List<Long> ctPhieuIds = jdbcTemplate.queryForList("SELECT IDCTPhieu FROM ctphieuxetnghiem WHERE IDPhieuXetNghiem = ?", Long.class, maPhieu);
            Long idCTPhieu;
            if (ctPhieuIds.isEmpty()) {
                List<String> tests = jdbcTemplate.queryForList("SELECT IDXetNghiem FROM ctdatlichxetnghiem WHERE IDDatLichXN = ? LIMIT 1", String.class, idDatLichXN);
                String idXetNghiem = tests.isEmpty() ? "XN001" : tests.get(0);
                
                KeyHolder keyHolder = new GeneratedKeyHolder();
                jdbcTemplate.update(connection -> {
                    PreparedStatement ps = connection.prepareStatement(
                        "INSERT INTO ctphieuxetnghiem (IDPhieuXetNghiem, IDXetNghiem, TrangThai) VALUES (?, ?, 'da_lay_mau')",
                        Statement.RETURN_GENERATED_KEYS
                    );
                    ps.setString(1, maPhieu);
                    ps.setString(2, idXetNghiem);
                    return ps;
                }, keyHolder);
                idCTPhieu = keyHolder.getKey().longValue();
            } else {
                idCTPhieu = ctPhieuIds.get(0);
                jdbcTemplate.update("UPDATE ctphieuxetnghiem SET TrangThai = 'da_lay_mau' WHERE IDCTPhieu = ?", idCTPhieu);
            }

            String idMau = "M" + request.barcode();
            jdbcTemplate.update(
                "INSERT INTO maubenhpham (IDMau, IDCTPhieu, MaBarcode, LoaiMau, IDBacSiLayMau, ThoiGianLayMau, TrangThai, GhiChu) " +
                "VALUES (?, ?, ?, ?, ?, NOW(), 'da_lay_mau', ?) " +
                "ON DUPLICATE KEY UPDATE LoaiMau = VALUES(LoaiMau), GhiChu = VALUES(GhiChu)",
                idMau, idCTPhieu, request.barcode(), request.sampleType(), idBacSi, request.notes()
            );

            jdbcTemplate.update(
                "INSERT INTO truyvet (IDKhachHang, LoaiDoiTuong, IDDoiTuong, HanhDong, NguonThucHien, ThoiGian, MoTa) VALUES (?, 'luotxetnghiem', ?, 'Bác sĩ xác nhận đã lấy mẫu', 'user', NOW(), ?)", 
                idKhachHang, String.valueOf(idLuotXN), "Mã mẫu/Barcode: " + request.barcode()
            );

            result.put("status", "COLLECTED");
            result.put("message", "Đã lấy mẫu thành công và lưu vào CSDL");
        } catch (Exception e) {
            org.springframework.transaction.interceptor.TransactionAspectSupport.currentTransactionStatus().setRollbackOnly();
            e.printStackTrace();
            result.put("error", "Lỗi CSDL khi lấy mẫu: " + e.getMessage());
        }
        return result;
    }

    @Transactional
    public Map<String, Object> handoverSpecimen(String barcode, SpecimenController.HandoverRequest request) {
        Map<String, Object> result = new LinkedHashMap<>();
        
        try {
            Map<String, Object> info = jdbcTemplate.queryForMap(
                "SELECT lxn.IDLuotXetNghiem, d.IDBacSi " +
                "FROM luotxetnghiem lxn JOIN datlichxetnghiem d ON lxn.IDDatLichXN = d.IDDatLichXN WHERE d.MaDatLich = ?", 
                request.appointmentId()
            );
            Long idLuotXN = ((Number) info.get("IDLuotXetNghiem")).longValue();
            Integer idBacSi = info.get("IDBacSi") != null ? ((Number) info.get("IDBacSi")).intValue() : 1;

            List<String> mauIds = jdbcTemplate.queryForList("SELECT IDMau FROM maubenhpham WHERE MaBarcode = ?", String.class, barcode);
            if (mauIds.isEmpty()) throw new RuntimeException("Không tìm thấy mẫu bệnh phẩm với Barcode này. Hãy thử lấy mẫu lại!");
            String idMau = mauIds.get(0);

            jdbcTemplate.update("UPDATE luotxetnghiem SET TrangThai = 'ktv_tiep_nhan' WHERE IDLuotXetNghiem = ?", idLuotXN);
            jdbcTemplate.update("UPDATE phieuxetnghiem SET TrangThai = 'ktv_tiep_nhan' WHERE IDLuotXetNghiem = ?", idLuotXN);
            jdbcTemplate.update("UPDATE ctphieuxetnghiem SET TrangThai = 'ktv_tiep_nhan' WHERE IDPhieuXetNghiem = (SELECT IDPhieuXetNghiem FROM phieuxetnghiem WHERE IDLuotXetNghiem = ?)", idLuotXN);
            jdbcTemplate.update("UPDATE maubenhpham SET TrangThai = 'da_ban_giao' WHERE IDMau = ?", idMau);
            
            jdbcTemplate.update(
                "INSERT INTO bangiaomau (IDMau, IDBacSiBanGiao, IDKTVTiepNhan, ThoiGianBanGiao, TrangThai, GhiChu) " +
                "VALUES (?, ?, ?, NOW(), 'da_ban_giao', ?)",
                idMau, idBacSi, request.receiverId(), request.note()
            );

            jdbcTemplate.update(
                "INSERT INTO truyvet (IDKhachHang, LoaiDoiTuong, IDDoiTuong, HanhDong, NguonThucHien, ThoiGian, MoTa) VALUES (?, 'luotxetnghiem', ?, 'Mẫu bệnh phẩm đã bàn giao cho KTV (Chờ kết quả)', 'user', NOW(), ?)", 
                request.patientCode(), String.valueOf(idLuotXN), "Mã mẫu: " + barcode + " - KTV tiếp nhận: " + request.receiverId()
            );

            result.put("id", barcode);
            result.put("receiverId", request.receiverId());
            result.put("status", "HANDED_OVER");
        } catch (Exception e) {
            org.springframework.transaction.interceptor.TransactionAspectSupport.currentTransactionStatus().setRollbackOnly();
            e.printStackTrace();
            result.put("error", "Lỗi CSDL khi bàn giao: " + e.getMessage());
        }
        return result;
    }

    // ==========================================
    // CÁC HÀM CỦA KỸ THUẬT VIÊN (ĐÃ CẬP NHẬT)
    // ==========================================

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
            dto.put("trangThai", mapSpecimenStatusToApi((String) row.get("TrangThai"))); 
            
            results.add(dto);
        }
        
        return results;
    }

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
            throw new RuntimeException("Không tìm thấy mẫu bệnh phẩm.");
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
        dto.put("handoverBy", "Bác sĩ / Điều dưỡng");
        
        return dto;
    }

    @Transactional
    public Map<String, Object> receiveSpecimen(String id, Map<String, Object> payload) {
        Map<String, Object> result = new LinkedHashMap<>();
        try {
            // 1. Cập nhật trạng thái mẫu bệnh phẩm và ghi chú KTV
            String notes = payload.containsKey("notes") ? (String) payload.get("notes") : "";
            jdbcTemplate.update("UPDATE maubenhpham SET TrangThai = 'ktv_tiep_nhan', GhiChu = ? WHERE IDMau = ?", notes, id);

            // 2. Đánh dấu đã tiếp nhận trong bảng giao nhận
            jdbcTemplate.update("UPDATE bangiaomau SET TrangThai = 'da_tiep_nhan', ThoiGianTiepNhan = NOW() WHERE IDMau = ?", id);

            // 3. Đưa mẫu vào Worklist để tiến hành xét nghiệm
            Long idCTPhieu = jdbcTemplate.queryForObject("SELECT IDCTPhieu FROM maubenhpham WHERE IDMau = ?", Long.class, id);
            
            List<Long> worklistExists = jdbcTemplate.queryForList("SELECT id FROM worklist WHERE IDCTPhieu = ?", Long.class, idCTPhieu);
            if (worklistExists.isEmpty()) {
                jdbcTemplate.update("INSERT INTO worklist (IDCTPhieu, IDMau, Status, ReceivedAt) VALUES (?, ?, 'queue', NOW())", idCTPhieu, id);
            }

            result.put("status", "SUCCESS");
            result.put("message", "Đã tiếp nhận mẫu thành công");
        } catch (Exception e) {
            org.springframework.transaction.interceptor.TransactionAspectSupport.currentTransactionStatus().setRollbackOnly();
            result.put("error", "Lỗi CSDL: " + e.getMessage());
        }
        return result;
    }

    @Transactional
    public Map<String, Object> rejectSpecimen(String id, String reason) {
        Map<String, Object> result = new LinkedHashMap<>();
        try {
            jdbcTemplate.update("UPDATE maubenhpham SET TrangThai = 'tu_choi_mau' WHERE IDMau = ?", id);
            jdbcTemplate.update("UPDATE bangiaomau SET TrangThai = 'tu_choi', LyDoTuChoi = ?, ThoiGianTiepNhan = NOW() WHERE IDMau = ?", reason, id);

            result.put("status", "REJECTED");
            result.put("message", "Đã từ chối mẫu");
        } catch (Exception e) {
            org.springframework.transaction.interceptor.TransactionAspectSupport.currentTransactionStatus().setRollbackOnly();
            result.put("error", "Lỗi CSDL: " + e.getMessage());
        }
        return result;
    }
    
    // ==========================================
    // LOGIC TRANG WORKLIST
    // ==========================================
    public List<Map<String, Object>> getWorklist(String apiStatus) {
        String dbStatus = null;
        if (apiStatus != null && !apiStatus.isEmpty()) {
            dbStatus = switch (apiStatus.toUpperCase()) {
                case "PENDING" -> "queue";
                case "IN_PROGRESS" -> "running";
                case "COMPLETED" -> "to_result";
                case "RESULT_ENTERED" -> "finished";
                default -> apiStatus.toLowerCase();
            };
        }

        StringBuilder sql = new StringBuilder(
            "SELECT w.id, m.MaBarcode, lxn.TenXetNghiem, k.TenKhachHang, w.ReceivedAt, w.Status " +
            "FROM worklist w " +
            "JOIN maubenhpham m ON w.IDMau = m.IDMau " +
            "JOIN ctphieuxetnghiem ct ON w.IDCTPhieu = ct.IDCTPhieu " +
            "JOIN loaixetnghiem lxn ON ct.IDXetNghiem = lxn.IDXetNghiem " +
            "JOIN phieuxetnghiem p ON ct.IDPhieuXetNghiem = p.IDPhieuXetNghiem " +
            "JOIN khachhang k ON p.IDKhachHang = k.IDKhachHang "
        );

        List<Object> params = new ArrayList<>();
        if (dbStatus != null) {
            sql.append(" WHERE w.Status = ? ");
            params.add(dbStatus);
        }

        sql.append(" ORDER BY w.ReceivedAt ASC");

        List<Map<String, Object>> rawData = jdbcTemplate.queryForList(sql.toString(), params.toArray());
        List<Map<String, Object>> results = new ArrayList<>();

        for (Map<String, Object> row : rawData) {
            Map<String, Object> dto = new LinkedHashMap<>();
            dto.put("id", row.get("id"));
            dto.put("specimenCode", row.get("MaBarcode"));
            dto.put("testName", row.get("TenXetNghiem"));
            dto.put("patientName", row.get("TenKhachHang"));
            dto.put("priority", "NORMAL"); 
            dto.put("assignedAt", row.get("ReceivedAt"));

            String st = (String) row.get("Status");
            String mappedStatus = switch(st) {
                case "queue" -> "PENDING";
                case "running" -> "IN_PROGRESS";
                case "to_result" -> "COMPLETED";
                case "finished" -> "RESULT_ENTERED";
                default -> st.toUpperCase();
            };
            dto.put("status", mappedStatus);

            results.add(dto);
        }
        return results;
    }

    @Transactional
    public Map<String, Object> startWorklist(String id) {
        Map<String, Object> result = new LinkedHashMap<>();
        try {
            jdbcTemplate.update("UPDATE worklist SET Status = 'running', StartedAt = NOW() WHERE id = ?", id);
            
            // Cập nhật trạng thái mẫu đang xử lý
            jdbcTemplate.update("UPDATE maubenhpham SET TrangThai = 'dang_xu_ly' WHERE IDMau = (SELECT IDMau FROM worklist WHERE id = ?)", id);

            result.put("status", "SUCCESS");
            result.put("message", "Đã bắt đầu xét nghiệm");
        } catch (Exception e) {
            org.springframework.transaction.interceptor.TransactionAspectSupport.currentTransactionStatus().setRollbackOnly();
            result.put("error", "Lỗi CSDL: " + e.getMessage());
        }
        return result;
    }

    @Transactional
    public Map<String, Object> completeWorklist(String id) {
        Map<String, Object> result = new LinkedHashMap<>();
        try {
            jdbcTemplate.update("UPDATE worklist SET Status = 'to_result', FinishedAt = NOW() WHERE id = ?", id);
            
            result.put("status", "SUCCESS");
            result.put("message", "Hoàn tất chạy máy, chờ nhập kết quả");
        } catch (Exception e) {
            org.springframework.transaction.interceptor.TransactionAspectSupport.currentTransactionStatus().setRollbackOnly();
            result.put("error", "Lỗi CSDL: " + e.getMessage());
        }
        return result;
    }

    // ==========================================
    // LOGIC NHẬP KẾT QUẢ VÀ GỬI DUYỆT
    // ==========================================
    public Map<String, Object> getResultEntry(String worklistId) {
        String infoSql = 
            "SELECT " +
            "  w.IDCTPhieu, w.IDMau, " +
            "  m.MaBarcode as specimenCode, " +
            "  k.TenKhachHang as patientName, " +
            "  lxn.TenXetNghiem as testName, " +
            "  lxn.IDXetNghiem " +
            "FROM worklist w " +
            "JOIN maubenhpham m ON w.IDMau = m.IDMau " +
            "JOIN ctphieuxetnghiem ct ON w.IDCTPhieu = ct.IDCTPhieu " +
            "JOIN phieuxetnghiem p ON ct.IDPhieuXetNghiem = p.IDPhieuXetNghiem " +
            "JOIN khachhang k ON p.IDKhachHang = k.IDKhachHang " +
            "JOIN loaixetnghiem lxn ON ct.IDXetNghiem = lxn.IDXetNghiem " +
            "WHERE w.id = ?";

        Map<String, Object> workInfo = jdbcTemplate.queryForMap(infoSql, worklistId);
        Long idCTPhieu = ((Number) workInfo.get("IDCTPhieu")).longValue();
        String idXetNghiem = (String) workInfo.get("IDXetNghiem");

        Map<String, Object> response = new LinkedHashMap<>();
        response.put("patientName", workInfo.get("patientName"));
        response.put("specimenCode", workInfo.get("specimenCode"));
        response.put("testName", workInfo.get("testName"));

        // Lấy bản nháp nếu có
        String kqSql = "SELECT IDKetQua, GhiChu FROM ketquaxetnghiem WHERE IDCTPhieu = ?";
        List<Map<String, Object>> savedResults = jdbcTemplate.queryForList(kqSql, idCTPhieu);
        String idKetQua = null;
        if (!savedResults.isEmpty()) {
            idKetQua = (String) savedResults.get(0).get("IDKetQua");
            response.put("notes", savedResults.get(0).get("GhiChu"));
        } else {
            response.put("notes", "");
        }

        // Lấy danh sách các chỉ số thật từ Database
        String chisoSql = "SELECT IDChiSo as indicatorId, TenChiSo as name, DonVi as unit FROM chisoxetnghiem WHERE IDXetNghiem = ?";
        List<Map<String, Object>> indicators = jdbcTemplate.queryForList(chisoSql, idXetNghiem);

        // Map giá trị cũ nếu đã từng lưu nháp
        if (idKetQua != null && !indicators.isEmpty()) {
            String valSql = "SELECT IDChiSo, GiaTriText, DanhGia FROM ketquachiso WHERE IDKetQua = ?";
            List<Map<String, Object>> savedVals = jdbcTemplate.queryForList(valSql, idKetQua);
            for (Map<String, Object> ind : indicators) {
                String indId = (String) ind.get("indicatorId");
                ind.put("value", "");
                ind.put("abnormal", false);
                for (Map<String, Object> sv : savedVals) {
                    if (indId.equals(sv.get("IDChiSo"))) {
                        ind.put("value", sv.get("GiaTriText"));
                        ind.put("abnormal", "bat_thuong".equals(sv.get("DanhGia")));
                        break;
                    }
                }
            }
        } else {
            for (Map<String, Object> ind : indicators) {
                ind.put("value", "");
                ind.put("abnormal", false);
                ind.put("reference", "Bình thường");
            }
        }

        response.put("indicators", indicators);
        return response;
    }

    @Transactional
    public Map<String, Object> saveResultEntry(String worklistId, Map<String, Object> payload, boolean isSubmit) {
        Map<String, Object> response = new LinkedHashMap<>();
        try {
            Map<String, Object> workInfo = jdbcTemplate.queryForMap("SELECT IDCTPhieu, IDMau, IDKTV FROM worklist WHERE id = ?", worklistId);
            Long idCTPhieu = ((Number) workInfo.get("IDCTPhieu")).longValue();
            String idMau = (String) workInfo.get("IDMau");
            
            // Xử lý an toàn khóa ngoại IDKTV (Phòng trường hợp KTV chưa có trong bảng nhanvien)
            String idKTV = (String) workInfo.get("IDKTV"); 
            if (idKTV == null) {
                idKTV = "KTV01"; 
                jdbcTemplate.update("INSERT IGNORE INTO nhanvien (IDNhanVien, TenNhanVien, ViTri, Status) VALUES ('KTV01', 'KTV Hệ Thống', 'ktv', 'yes')");
            }

            String idKetQua = "KQ" + idCTPhieu;
            String notes = (String) payload.getOrDefault("notes", "");
            
            // 1. Lưu thông tin tổng quát vào ketquaxetnghiem
            List<String> exists = jdbcTemplate.queryForList("SELECT IDKetQua FROM ketquaxetnghiem WHERE IDCTPhieu = ?", String.class, idCTPhieu);
            if (exists.isEmpty()) {
                jdbcTemplate.update("INSERT INTO ketquaxetnghiem (IDKetQua, IDCTPhieu, IDMau, IDKTV, ThoiGianBatDau, TrangThai, GhiChu) VALUES (?, ?, ?, ?, NOW(), 'dang_thuc_hien', ?)", 
                    idKetQua, idCTPhieu, idMau, idKTV, notes);
            } else {
                jdbcTemplate.update("UPDATE ketquaxetnghiem SET GhiChu = ? WHERE IDKetQua = ?", notes, idKetQua);
            }

            // 2. Lưu chi tiết từng chỉ số vào ketquachiso
            @SuppressWarnings("unchecked")
            List<Map<String, Object>> indicators = (List<Map<String, Object>>) payload.get("indicators");
            if (indicators != null) {
                for (Map<String, Object> ind : indicators) {
                    String indId = (String) ind.get("indicatorId");
                    String value = (String) ind.get("value");
                    boolean abnormal = ind.get("abnormal") != null && (Boolean) ind.get("abnormal");
                    String danhGia = abnormal ? "bat_thuong" : "binh_thuong";

                    List<Long> valExists = jdbcTemplate.queryForList("SELECT IDKetQuaChiSo FROM ketquachiso WHERE IDKetQua = ? AND IDChiSo = ?", Long.class, idKetQua, indId);
                    if (valExists.isEmpty()) {
                        jdbcTemplate.update("INSERT INTO ketquachiso (IDKetQua, IDChiSo, GiaTriText, DanhGia) VALUES (?, ?, ?, ?)", idKetQua, indId, value, danhGia);
                    } else {
                        jdbcTemplate.update("UPDATE ketquachiso SET GiaTriText = ?, DanhGia = ? WHERE IDKetQuaChiSo = ?", value, danhGia, valExists.get(0));
                    }
                }
            }

            // 3. Nếu là Gửi Duyệt -> Đổi trạng thái để Bác sĩ thấy
            if (isSubmit) {
                jdbcTemplate.update("UPDATE ketquaxetnghiem SET TrangThai = 'cho_duyet', ThoiGianHoanThanh = NOW() WHERE IDKetQua = ?", idKetQua);
                jdbcTemplate.update("UPDATE worklist SET Status = 'finished', FinishedAt = NOW() WHERE id = ?", worklistId);
                jdbcTemplate.update("UPDATE ctphieuxetnghiem SET TrangThai = 'cho_duyet' WHERE IDCTPhieu = ?", idCTPhieu);
                response.put("message", "Đã gửi kết quả sang bác sĩ duyệt.");
            } else {
                response.put("message", "Đã lưu nháp kết quả.");
            }

            response.put("status", "SUCCESS");
        } catch (Exception e) {
            org.springframework.transaction.interceptor.TransactionAspectSupport.currentTransactionStatus().setRollbackOnly();
            e.printStackTrace();
            response.put("error", "Lỗi CSDL khi lưu kết quả: " + e.getMessage());
        }
        return response;
    }
}