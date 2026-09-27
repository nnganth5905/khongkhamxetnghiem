package com.biomedic.backend.dto.request;

import jakarta.validation.constraints.NotBlank;

public record LoginRequest(
        String login,
        String email,
        String username,

        @NotBlank(
                message = "Mật khẩu không được để trống."
        )
        String password
) {
    public String identifier() {
        if (login != null && !login.isBlank()) {
            return login.trim();
        }

        if (email != null && !email.isBlank()) {
            return email.trim();
        }

        if (username != null && !username.isBlank()) {
            return username.trim();
        }

        return "";
    }
}