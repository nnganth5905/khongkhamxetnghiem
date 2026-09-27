package com.biomedic.backend.service;

import org.springframework.jdbc.core.JdbcTemplate;
import org.springframework.security.core.Authentication;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.List;
import java.util.Map;

@Service
public class NotificationService {
    private final JdbcTemplate jdbcTemplate;

    public NotificationService(JdbcTemplate jdbcTemplate) {
        this.jdbcTemplate = jdbcTemplate;
    }

    public List<Map<String, Object>> getMyNotifications(Authentication auth) {
        String email = auth.getName();
        Integer userId = jdbcTemplate.queryForObject("SELECT UserID FROM users WHERE Email = ?", Integer.class, email);
        
        // Truy vấn lấy các trường khớp với cơ sở dữ liệu
        String sql = "SELECT IDThongBao, LoaiThongBao, TieuDe, NoiDung, DaDoc, ThoiGianTao " +
                     "FROM thongbao WHERE UserIDNhan = ? ORDER BY ThoiGianTao DESC";
        return jdbcTemplate.queryForList(sql, userId);
    }

    @Transactional
    public void markAsRead(Long id) {
        jdbcTemplate.update("UPDATE thongbao SET DaDoc = 1, ThoiGianDoc = NOW() WHERE IDThongBao = ?", id);
    }

    @Transactional
    public void markAllAsRead(Authentication auth) {
        String email = auth.getName();
        Integer userId = jdbcTemplate.queryForObject("SELECT UserID FROM users WHERE Email = ?", Integer.class, email);
        jdbcTemplate.update("UPDATE thongbao SET DaDoc = 1, ThoiGianDoc = NOW() WHERE UserIDNhan = ? AND DaDoc = 0", userId);
    }
}