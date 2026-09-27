package com.biomedic.backend.controller;

import com.biomedic.backend.service.NotificationService;
import org.springframework.http.ResponseEntity;
import org.springframework.security.core.Authentication;
import org.springframework.web.bind.annotation.*;
import java.util.Map;

@RestController
@RequestMapping("/api/notifications")
public class NotificationController {
    private final NotificationService notificationService;

    public NotificationController(NotificationService notificationService) {
        this.notificationService = notificationService;
    }

    @GetMapping
    public ResponseEntity<?> getMyNotifications(Authentication auth) {
        return ResponseEntity.ok(notificationService.getMyNotifications(auth));
    }

    @PatchMapping("/{id}/read")
    public ResponseEntity<?> markAsRead(@PathVariable Long id) {
        notificationService.markAsRead(id);
        return ResponseEntity.ok(Map.of("message", "Đã đánh dấu đọc"));
    }

    @PostMapping("/read-all")
    public ResponseEntity<?> markAllAsRead(Authentication auth) {
        notificationService.markAllAsRead(auth);
        return ResponseEntity.ok(Map.of("message", "Đã đánh dấu đọc tất cả"));
    }
}