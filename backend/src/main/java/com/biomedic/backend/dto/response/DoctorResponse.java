package com.biomedic.backend.dto.response;

public record DoctorResponse(
        String id,
        String employeeId,
        String fullName,
        String specialtyId,
        String specialtyName,
        String roomId,
        String degree,
        String title,
        String phone,
        String email,
        String imageUrl,
        String bio,
        String status
) {
}