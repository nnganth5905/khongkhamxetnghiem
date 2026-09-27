package com.biomedic.backend.dto.request;

import jakarta.validation.constraints.Email;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.Size;

public record ResetPasswordRequest(
        @NotBlank(
                message = "Token không được để trống."
        )
        String token,

        @NotBlank(
                message = "Email không được để trống."
        )
        @Email(
                message = "Email không hợp lệ."
        )
        String email,

        @NotBlank(
                message = "Mật khẩu mới không được để trống."
        )
        @Size(
                min = 6,
                message = "Mật khẩu mới phải có ít nhất 6 ký tự."
        )
        String password
) {
}