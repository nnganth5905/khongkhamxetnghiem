package com.biomedic.backend.service;

import java.util.Collections;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

import org.springframework.security.core.Authentication;
import org.springframework.stereotype.Service;

@Service
public class NotificationService {

    public List<Map<String, Object>> getNotifications(
            Authentication authentication
    ) {
        return Collections.emptyList();
    }

    public Map<String, Object> markAsRead(
            String id,
            Authentication authentication
    ) {
        Map<String, Object> result = new LinkedHashMap<>();
        result.put("id", id);
        result.put("read", true);
        return result;
    }

    public Map<String, Object> markAllAsRead(
            Authentication authentication
    ) {
        Map<String, Object> result = new LinkedHashMap<>();
        result.put("success", true);
        result.put("message", "Đã đánh dấu tất cả thông báo là đã đọc.");
        return result;
    }
}