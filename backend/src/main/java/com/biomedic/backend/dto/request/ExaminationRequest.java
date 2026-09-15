package com.biomedic.backend.dto.request;

import jakarta.validation.constraints.NotBlank;

public record ExaminationRequest(
        @NotBlank
        String visitId,

        String symptoms,
        String history,
        String diagnosis,
        String conclusion,
        String advice
) {
}