package com.biomedic.backend.dto.response;

import java.time.LocalDate;
import java.time.LocalTime;

public record AppointmentResponse(
        String id,
        String type,
        String customerId,
        String customerName,
        String customerPhone, // Bổ sung trường số điện thoại
        String doctorId,
        String doctorName,
        LocalDate appointmentDate,
        LocalTime appointmentTime,
        String status,
        String qrCode,
        String note
) {
}