package com.biomedic.backend.dto.response;

import java.time.LocalDateTime;

public record TestResultResponse(
        String id,
        String testOrderId,
        String testOrderItemId,
        String specimenId,
        String technicianId,
        String resultValue,
        String unit,
        String referenceRange,
        String comment,
        LocalDateTime performedAt,
        LocalDateTime enteredAt,
        String approvedByDoctorId,
        LocalDateTime approvedAt,
        String conclusion,
        String advice,
        String status
) {
}