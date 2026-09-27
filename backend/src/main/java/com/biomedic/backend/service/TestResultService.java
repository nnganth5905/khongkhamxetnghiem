package com.biomedic.backend.service;

import org.springframework.jdbc.core.JdbcTemplate;
import org.springframework.security.core.Authentication;
import org.springframework.stereotype.Service;

import java.util.ArrayList;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

@Service
public class TestResultService {

    private final JdbcTemplate jdbcTemplate;

    public TestResultService(JdbcTemplate jdbcTemplate) {
        this.jdbcTemplate = jdbcTemplate;
    }

    // API LẤY DANH SÁCH KẾT QUẢ KHÁM & XÉT NGHIỆM CHO KHÁCH HÀNG ĐANG ĐĂNG NHẬP
    public Map<String, Object> getMyResults(Authentication authentication) {
        Map<String, Object> response = new LinkedHashMap<>();
        try {
            // Spring Security thường lưu Email hoặc Username vào đây
            String identifier = authentication.getName(); 
            
            // 1. Truy xuất chính xác IDKhachHang từ bảng users
            String idKhachHang = null;
            try {
                idKhachHang = jdbcTemplate.queryForObject(
                    "SELECT IDKhachHang FROM users WHERE (Email = ? OR Username = ?) AND IDKhachHang IS NOT NULL LIMIT 1", 
                    String.class, identifier, identifier
                );
            } catch (Exception e) {
                // Không tìm thấy thì bỏ qua, sẽ xử lý báo lỗi ở dưới
            }
            
            if (idKhachHang == null || idKhachHang.trim().isEmpty()) {
                response.put("error", "Tài khoản của bạn chưa được liên kết với hồ sơ y tế.");
                response.put("items", new ArrayList<>());
                return response;
            }

            // 2. Lấy Kết Quả Xét Nghiệm (Quét toàn bộ chuỗi liên kết để không sót phiếu)
            String sqlXN = 
                "SELECT kq.IDKetQua as id, 'XÉT NGHIỆM' as type, lxn.TenXetNghiem as title, " +
                "kq.ThoiGianHoanThanh as date, kq.KetLuanBacSi as summary, b.TenBacSi as doctorName " +
                "FROM ketquaxetnghiem kq " +
                "LEFT JOIN ctphieuxetnghiem ct ON kq.IDCTPhieu = ct.IDCTPhieu " +
                "LEFT JOIN phieuxetnghiem p ON ct.IDPhieuXetNghiem = p.IDPhieuXetNghiem " +
                "LEFT JOIN luotxetnghiem luot ON p.IDLuotXetNghiem = luot.IDLuotXetNghiem " +
                "LEFT JOIN datlichxetnghiem dl ON luot.IDDatLichXN = dl.IDDatLichXN " +
                "LEFT JOIN loaixetnghiem lxn ON ct.IDXetNghiem = lxn.IDXetNghiem " +
                "LEFT JOIN bacsi b ON kq.IDBacSiDuyet = b.IDBacSi " +
                "WHERE kq.TrangThai = 'da_duyet' " +
                "AND (p.IDKhachHang = ? OR luot.IDKhachHang = ? OR dl.IDKhachHang = ?) " +
                "ORDER BY kq.ThoiGianHoanThanh DESC";
            
            List<Map<String, Object>> listXN = jdbcTemplate.queryForList(sqlXN, idKhachHang, idKhachHang, idKhachHang);

            // 3. Lấy Kết Quả Khám Bệnh
            String sqlKham = 
                "SELECT k.IDKham as id, 'KHÁM BỆNH' as type, ck.TenChuyenKhoa as title, " +
                "k.ThoiGianKham as date, k.KetLuan as summary, b.TenBacSi as doctorName " +
                "FROM kham k " +
                "LEFT JOIN luotkham lk ON k.IDLuotKham = lk.IDLuotKham " +
                "LEFT JOIN datlichkham dlk ON lk.IDDatLichKham = dlk.IDDatLichKham " +
                "LEFT JOIN chuyenkhoa ck ON dlk.IDChuyenKhoa = ck.IDChuyenKhoa " +
                "LEFT JOIN bacsi b ON k.IDBacSi = b.IDBacSi " +
                "WHERE lk.IDKhachHang = ? AND lk.TrangThai = 'hoan_tat' " +
                "ORDER BY k.ThoiGianKham DESC";
            
            List<Map<String, Object>> listKham = jdbcTemplate.queryForList(sqlKham, idKhachHang);

            // 4. Gộp danh sách và Sắp xếp thời gian giảm dần
            List<Map<String, Object>> allResults = new ArrayList<>();
            allResults.addAll(listXN);
            allResults.addAll(listKham);

            allResults.sort((a, b) -> {
                Object d1 = a.get("date");
                Object d2 = b.get("date");
                if (d1 == null && d2 == null) return 0;
                if (d1 == null) return 1;
                if (d2 == null) return -1;
                if (d1 instanceof Comparable && d2 instanceof Comparable) {
                    @SuppressWarnings("unchecked")
                    Comparable<Object> comp2 = (Comparable<Object>) d2;
                    return comp2.compareTo(d1);
                }
                return d2.toString().compareTo(d1.toString());
            });

            response.put("items", allResults);
            response.put("status", "SUCCESS");
        } catch (Exception e) {
            e.printStackTrace();
            response.put("error", "Lỗi Server: " + e.getMessage());
            response.put("items", new ArrayList<>());
        }
        return response;
    }

    // 2. API XEM CHI TIẾT KẾT QUẢ CHO KHÁCH HÀNG
    public Map<String, Object> getResultDetail(String resultId) {
        // Có thể tái sử dụng logic giống như DoctorService.getResultDetail()
        return new LinkedHashMap<>();
    }

    // --- CÁC HÀM CŨ ĐỂ KHÔNG BÁO LỖI (CHƯA DÙNG TỚI) ---
    public Map<String, Object> lookupResult(String code, String phone, String email) { return new LinkedHashMap<>(); }
    public Object createResult(Object req) { return null; }
    public Object updateResult(String id, Object req) { return null; }
}