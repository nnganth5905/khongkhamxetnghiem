package com.biomedic.backend.service;

import org.springframework.jdbc.core.JdbcTemplate;
import org.springframework.security.core.Authentication;
import org.springframework.stereotype.Service;

import java.sql.Timestamp;
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

/**
 * ============================================================
 * LẤY TOÀN BỘ KẾT QUẢ CỦA KHÁCH HÀNG ĐANG ĐĂNG NHẬP
 * ============================================================
 */
public Map<String, Object> getMyResults(Authentication authentication) {

    Map<String, Object> response = new LinkedHashMap<>();
    List<Map<String, Object>> allResults = new ArrayList<>();

    try {

        if (authentication == null) {
            response.put("status", "ERROR");
            response.put("error", "Bạn chưa đăng nhập.");
            response.put("items", allResults);
            return response;
        }

        String identifier = authentication.getName();

        if (identifier == null || identifier.trim().isEmpty()) {
            response.put("status", "ERROR");
            response.put("error", "Không xác định được tài khoản đăng nhập.");
            response.put("items", allResults);
            return response;
        }

        identifier = identifier.trim();

        /*
         * ========================================================
         * 1. TÌM ID KHÁCH HÀNG TỪ TÀI KHOẢN ĐĂNG NHẬP
         * ========================================================
         */
        String idKhachHang = null;

        try {
            idKhachHang = jdbcTemplate.queryForObject(
                    """
                    SELECT IDKhachHang
                    FROM users
                    WHERE (Email = ? OR Username = ?)
                      AND IDKhachHang IS NOT NULL
                    LIMIT 1
                    """,
                    String.class,
                    identifier,
                    identifier
            );
        } catch (Exception ignored) {
            // Xử lý bên dưới
        }

        if (idKhachHang == null || idKhachHang.trim().isEmpty()) {
            response.put("status", "ERROR");
            response.put(
                    "error",
                    "Tài khoản của bạn chưa được liên kết với hồ sơ khách hàng."
            );
            response.put("items", allResults);
            return response;
        }

        /*
         * ========================================================
         * 2. LẤY KẾT QUẢ XÉT NGHIỆM
         *
         * Quan hệ:
         *
         * ketquaxetnghiem
         *      ↓ IDCTPhieu
         * ctphieuxetnghiem
         *      ↓ IDPhieuXetNghiem
         * phieuxetnghiem
         *      ↓ IDLuotXetNghiem
         * luotxetnghiem
         *      ↓ IDDatLichXN
         * datlichxetnghiem
         *      ↓
         * khách hàng
         * ========================================================
         */
        String sqlXetNghiem =
                """
                SELECT
                    kq.IDKetQua AS id,
                    kq.IDKetQua AS code,
                    'XÉT NGHIỆM' AS type,

                    COALESCE(
                        lxn.TenXetNghiem,
                        'Kết quả xét nghiệm'
                    ) AS title,

                    kq.ThoiGianHoanThanh AS date,

                    kq.KetLuanBacSi AS summary,

                    b.TenBacSi AS doctorName,

                    kq.TrangThai AS status,

                    kq.KetQuaTongQuat AS resultValue,

                    kq.GhiChu AS comment,

                    kq.IDCTPhieu AS idCtPhieu,

                    kq.IDBacSiDuyet AS approvedByDoctorId,

                    kq.ThoiGianDuyet AS approvedAt

                FROM ketquaxetnghiem kq

                LEFT JOIN ctphieuxetnghiem ct
                    ON kq.IDCTPhieu = ct.IDCTPhieu

                LEFT JOIN phieuxetnghiem p
                    ON ct.IDPhieuXetNghiem = p.IDPhieuXetNghiem

                LEFT JOIN luotxetnghiem luot
                    ON p.IDLuotXetNghiem = luot.IDLuotXetNghiem

                LEFT JOIN datlichxetnghiem dl
                    ON luot.IDDatLichXN = dl.IDDatLichXN

                LEFT JOIN loaixetnghiem lxn
                    ON ct.IDXetNghiem = lxn.IDXetNghiem

                LEFT JOIN bacsi b
                    ON kq.IDBacSiDuyet = b.IDBacSi

                WHERE kq.TrangThai = 'da_duyet'

                  AND (
                      p.IDKhachHang = ?
                      OR luot.IDKhachHang = ?
                      OR dl.IDKhachHang = ?
                  )

                ORDER BY kq.ThoiGianHoanThanh DESC
                """;

        List<Map<String, Object>> listXetNghiem =
                jdbcTemplate.queryForList(
                        sqlXetNghiem,
                        idKhachHang,
                        idKhachHang,
                        idKhachHang
                );

        allResults.addAll(listXetNghiem);

        /*
         * ========================================================
         * 3. LẤY KẾT QUẢ KHÁM BỆNH
         * ========================================================
         */
        String sqlKhamBenh =
                """
                SELECT
                    k.IDKham AS id,
                    k.IDKham AS code,
                    'KHÁM BỆNH' AS type,

                    COALESCE(
                        ck.TenChuyenKhoa,
                        'Khám bệnh'
                    ) AS title,

                    k.ThoiGianKham AS date,

                    k.KetLuan AS summary,

                    b.TenBacSi AS doctorName,

                    lk.TrangThai AS status

                FROM kham k

                LEFT JOIN luotkham lk
                    ON k.IDLuotKham = lk.IDLuotKham

                LEFT JOIN datlichkham dlk
                    ON lk.IDDatLichKham = dlk.IDDatLichKham

                LEFT JOIN chuyenkhoa ck
                    ON dlk.IDChuyenKhoa = ck.IDChuyenKhoa

                LEFT JOIN bacsi b
                    ON k.IDBacSi = b.IDBacSi

                WHERE lk.IDKhachHang = ?
                  AND lk.TrangThai = 'hoan_tat'

                ORDER BY k.ThoiGianKham DESC
                """;

        List<Map<String, Object>> listKhamBenh =
                jdbcTemplate.queryForList(
                        sqlKhamBenh,
                        idKhachHang
                );

        allResults.addAll(listKhamBenh);

        /*
         * ========================================================
         * 4. SẮP XẾP TẤT CẢ THEO THỜI GIAN
         * ========================================================
         */
        allResults.sort((a, b) -> {

            Object dateA = a.get("date");
            Object dateB = b.get("date");

            if (dateA == null && dateB == null) {
                return 0;
            }

            if (dateA == null) {
                return 1;
            }

            if (dateB == null) {
                return -1;
            }

            if (dateA instanceof Timestamp &&
                    dateB instanceof Timestamp) {

                return ((Timestamp) dateB)
                        .compareTo((Timestamp) dateA);
            }

            return dateB
                    .toString()
                    .compareTo(dateA.toString());
        });

        /*
         * ========================================================
         * 5. TRẢ RESPONSE
         * ========================================================
         */
        response.put("status", "SUCCESS");
        response.put("customerId", idKhachHang);
        response.put("count", allResults.size());
        response.put("items", allResults);

        return response;

    } catch (Exception e) {

        e.printStackTrace();

        response.clear();

        response.put("status", "ERROR");
        response.put(
                "error",
                "Lỗi Server khi tải kết quả: " + e.getMessage()
        );
        response.put("items", new ArrayList<>());

        return response;
    }
}

/**
 * ============================================================
 * CHI TIẾT KẾT QUẢ
 * ============================================================
 */
public Map<String, Object> getResultDetail(String resultId) {

    Map<String, Object> result = new LinkedHashMap<>();

    if (resultId == null || resultId.trim().isEmpty()) {
        result.put("status", "ERROR");
        result.put("error", "Thiếu mã kết quả.");
        return result;
    }

    try {

        /*
         * ========================================================
         * 1. LẤY THÔNG TIN KẾT QUẢ
         * ========================================================
         */
        String sql =
                """
                SELECT
                    kq.IDKetQua AS id,
                    kq.IDKetQua AS code,

                    'XÉT NGHIỆM' AS type,

                    kq.IDCTPhieu AS idCtPhieu,

                    kq.IDMau AS specimenId,

                    kq.IDKTV AS technicianId,

                    kq.ThoiGianBatDau AS performedAt,

                    kq.ThoiGianHoanThanh AS completedAt,

                    kq.ThoiGianNhap AS enteredAt,

                    kq.KetQuaTongQuat AS resultValue,

                    kq.TrangThai AS status,

                    kq.IDBacSiDuyet AS approvedByDoctorId,

                    kq.ThoiGianDuyet AS approvedAt,

                    kq.KetLuanBacSi AS conclusion,

                    kq.GhiChu AS comment,

                    lxn.TenXetNghiem AS testName,

                    b.TenBacSi AS doctorName

                FROM ketquaxetnghiem kq

                LEFT JOIN ctphieuxetnghiem ct
                    ON kq.IDCTPhieu = ct.IDCTPhieu

                LEFT JOIN loaixetnghiem lxn
                    ON ct.IDXetNghiem = lxn.IDXetNghiem

                LEFT JOIN bacsi b
                    ON kq.IDBacSiDuyet = b.IDBacSi

                WHERE kq.IDKetQua = ?

                LIMIT 1
                """;

        List<Map<String, Object>> rows =
                jdbcTemplate.queryForList(
                        sql,
                        resultId
                );

        if (rows.isEmpty()) {
            result.put("status", "ERROR");
            result.put("error", "Không tìm thấy kết quả xét nghiệm.");
            return result;
        }

        Map<String, Object> detail = rows.get(0);

        result.put("status", "SUCCESS");
        result.putAll(detail);

        /*
         * ========================================================
         * 2. LẤY CÁC CHỈ SỐ CHI TIẾT NẾU DATABASE CÓ BẢNG
         * ========================================================
         *
         * Hiện tại cấu trúc bảng chi tiết chỉ được suy ra từ
         * code ResultItem của frontend/backend.
         *
         * Không tự động truy vấn một bảng chưa được cung cấp
         * schema để tránh làm API lỗi.
         *
         * Vì vậy luôn trả indicators = [].
         */
        result.put("indicators", new ArrayList<>());

        return result;

    } catch (Exception e) {

        e.printStackTrace();

        result.clear();

        result.put("status", "ERROR");
        result.put(
                "error",
                "Lỗi Server khi tải chi tiết kết quả: "
                        + e.getMessage()
        );
        result.put("indicators", new ArrayList<>());

        return result;
    }
}

/**
 * ============================================================
 * TRA CỨU KẾT QUẢ CÔNG KHAI
 * ============================================================
 */
public Map<String, Object> lookupResult(
        String code,
        String phone,
        String email
) {

    Map<String, Object> response =
            new LinkedHashMap<>();

    response.put("status", "ERROR");
    response.put(
            "error",
            "Chức năng tra cứu công khai chưa được triển khai."
    );

    return response;
}

/**
 * ============================================================
 * TẠO KẾT QUẢ
 * ============================================================
 *
 * Endpoint cũ đang được giữ lại để không phá API hiện tại.
 * Phần nhập kết quả thực tế của hệ thống có thể đang nằm ở
 * TechnicianService.
 */
public Object createResult(Object req) {
    return null;
}

/**
 * ============================================================
 * CẬP NHẬT KẾT QUẢ
 * ============================================================
 */
public Object updateResult(
        String id,
        Object req
) {
    return null;
}


}
