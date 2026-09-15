package com.biomedic.backend.dto.response;

import java.math.BigDecimal;

public record TestResponse(
        String id,
        String name,
        String description,
        String categoryId,
        String testType,
        String specimenType,
        String unit,
        String referenceValue,
        BigDecimal price,
        String turnaroundTime,
        String status
) {
}