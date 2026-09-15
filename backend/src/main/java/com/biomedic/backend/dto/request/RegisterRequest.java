package com.biomedic.backend.dto.request;

import jakarta.validation.constraints.Email;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.Size;

public record RegisterRequest(
        @NotBlank(
                message = "Họ tên không được để trống."
        )
        String name,

        @NotBlank(
                message = "Email không được để trống."
        )
        @Email(
                message = "Email không hợp lệ."
        )
        String email,

        String phone,

        String gender,

        @NotBlank(
                message = "Mật khẩu không được để trống."
        )
        @Size(
                min = 6,
                message = "Mật khẩu phải có ít nhất 6 ký tự."
        )
        String password
) {
}