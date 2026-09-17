package com.biomedic.backend.dto.response;

public record UserResponse(
        String id,
        String username,
        String email,
        String fullName,
        String role,
        String status
) {
}