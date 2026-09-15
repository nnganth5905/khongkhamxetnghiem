package com.biomedic.backend.controller;

import com.biomedic.backend.service.NotificationService;

import org.springframework.http.ResponseEntity;
import org.springframework.security.core.Authentication;

import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PatchMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping("/api/notifications")
public class NotificationController {

    private final NotificationService notificationService;

    public NotificationController(
            NotificationService notificationService
    ) {
        this.notificationService =
                notificationService;
    }

    @GetMapping
    public ResponseEntity<?> getNotifications(
            Authentication authentication
    ) {
        return ResponseEntity.ok(
                notificationService.getNotifications(
                        authentication
                )
        );
    }

    @PatchMapping("/{id}/read")
    public ResponseEntity<?> markAsRead(
            @PathVariable
            String id,

            Authentication authentication
    ) {
        return ResponseEntity.ok(
                notificationService.markAsRead(
                        id,
                        authentication
                )
        );
    }

    @PatchMapping("/read-all")
    public ResponseEntity<?> markAllAsRead(
            Authentication authentication
    ) {
        return ResponseEntity.ok(
                notificationService.markAllAsRead(
                        authentication
                )
        );
    }
}