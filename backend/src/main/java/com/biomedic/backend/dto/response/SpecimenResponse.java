package com.biomedic.backend.dto.response;

import java.time.LocalDateTime;

public record SpecimenResponse(
        String id,
        String testOrderId,
        String customerId,
        String specimenType,
        String specimenCode,
        String barcode,
        LocalDateTime collectedAt,
        String collectorId,
        LocalDateTime receivedAt,
        String technicianId,
        String status,
        String note
) {
}