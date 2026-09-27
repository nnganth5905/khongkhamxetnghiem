package com.biomedic.backend.dto.request;

import jakarta.validation.constraints.NotBlank;

import java.time.LocalDate;

public record PromotionRequest(
        @NotBlank
        String title,

        String summary,

        String content,

        String image,

        LocalDate startDate,

        LocalDate endDate,

        String status
) {
}