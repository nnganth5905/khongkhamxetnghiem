package com.biomedic.backend.dto.response;

import java.time.LocalDateTime;

public record VisitResponse(
        String id,
        String appointmentId,
        String customerId,
        String doctorId,
        String roomId,
        String type,
        Integer queueNumber,
        String status,
        LocalDateTime receivedAt,
        LocalDateTime startedAt,
        LocalDateTime endedAt,
        String note
) {
}