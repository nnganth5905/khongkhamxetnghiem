package com.biomedic.backend.dto.response;

import java.time.LocalDateTime;

public record TestResultResponse(
        String id,
        Long idCtPhieu,
        String specimenId,
        String technicianId,
        String resultValue,
        String comment,
        LocalDateTime performedAt,
        LocalDateTime completedAt,
        LocalDateTime enteredAt,
        Integer approvedByDoctorId,
        LocalDateTime approvedAt,
        String conclusion,
        String status
) {
}