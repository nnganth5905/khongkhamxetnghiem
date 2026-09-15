package com.biomedic.backend.service;

import java.time.LocalDate;
import java.util.Collections;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

import org.springframework.security.core.Authentication;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.stereotype.Service;

@Service
public class TechnicianService {

    /*
     * FIX:
     * Controller gọi getCurrentTechnician() KHÔNG truyền tham số.
     * Service tự lấy Authentication từ SecurityContextHolder.
     */
    public Map<String, Object> getCurrentTechnician() {
        Authentication authentication =
                SecurityContextHolder
                        .getContext()
                        .getAuthentication();

        return getCurrentTechnician(authentication);
    }

    /*
     * Giữ overload này để các code cũ nếu đang truyền Authentication
     * vẫn có thể dùng được.
     */
    public Map<String, Object> getCurrentTechnician(
            Authentication authentication
    ) {
        Map<String, Object> result =
                new LinkedHashMap<>();

        result.put(
                "username",
                authentication != null
                    ? authentication.getName()
                    : null
        );

        result.put(
                "authenticated",
                authentication != null
                    && authentication.isAuthenticated()
        );

        result.put(
                "message",
                "TechnicianService đã hoạt động. Sẽ nối repository bảng nhanvien khi map schema MySQL."
        );

        return result;
    }

    public List<Map<String, Object>> getTechnicians(
            String q
    ) {
        return Collections.emptyList();
    }

    public Map<String, Object> getTechnicianById(
            String id
    ) {
        Map<String, Object> result =
                new LinkedHashMap<>();

        result.put("id", id);
        result.put(
                "message",
                "Chưa nối repository kỹ thuật viên."
        );

        return result;
    }

    public Map<String, Object> createTechnician(
            String hoTen,
            String idCoSo,
            String trinhDo,
            String soDienThoai,
            String email,
            LocalDate ngayVaoLam,
            String trangThai,
            String ghiChu
    ) {
        return buildTechnician(
                null,
                hoTen,
                idCoSo,
                trinhDo,
                soDienThoai,
                email,
                ngayVaoLam,
                trangThai,
                ghiChu
        );
    }

    public Map<String, Object> updateTechnician(
            String id,
            String hoTen,
            String idCoSo,
            String trinhDo,
            String soDienThoai,
            String email,
            LocalDate ngayVaoLam,
            String trangThai,
            String ghiChu
    ) {
        return buildTechnician(
                id,
                hoTen,
                idCoSo,
                trinhDo,
                soDienThoai,
                email,
                ngayVaoLam,
                trangThai,
                ghiChu
        );
    }

    public void deleteTechnician(
            String id
    ) {
        // TODO: technicianRepository.deleteById(...)
    }

    private Map<String, Object> buildTechnician(
            String id,
            String hoTen,
            String idCoSo,
            String trinhDo,
            String soDienThoai,
            String email,
            LocalDate ngayVaoLam,
            String trangThai,
            String ghiChu
    ) {
        Map<String, Object> result =
                new LinkedHashMap<>();

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