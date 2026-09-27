package com.biomedic.backend.dto.request;

import jakarta.validation.constraints.NotBlank;

public record QuickAppointmentRequest(
        @NotBlank
        String hoten,

        String email,

        @NotBlank
        String ngay,

        @NotBlank
        String gio,

        String idbacsi,

        String ghichu
) {
}