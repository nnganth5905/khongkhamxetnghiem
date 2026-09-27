package com.biomedic.backend.dto.response;

import java.time.LocalDateTime;

public record NotificationResponse(
        String id,
        String type,
        String title,
        String content,
        String objectId,
        String objectType,
        LocalDateTime createdAt,
        boolean read
) {
}