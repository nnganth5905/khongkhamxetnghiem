package com.biomedic.backend.dto.request;

import jakarta.validation.constraints.NotBlank;

public record CollectSpecimenRequest(
        @NotBlank
        String testOrderId,

        String customerId,

        @NotBlank
        String specimenType,

        String note
) {
}