package com.biomedic.backend.dto.response;

import java.math.BigDecimal;

public record TestResponse(
        String id,
        String name,
        String description,
        String categoryId,
        String testType,
        String defaultSampleType, // Đổi tên cho khớp Entity
        BigDecimal price,
        Integer estimatedTimeMinutes, // Đổi tên cho khớp Entity
        String status
) {
}