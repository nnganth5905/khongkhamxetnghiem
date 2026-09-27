package com.biomedic.backend.dto.request;

import jakarta.validation.constraints.Email;
import jakarta.validation.constraints.NotBlank;

import java.time.LocalDate;

public record CustomerRequest(
        @NotBlank(
                message = "Họ tên không được để trống."
        )
        String hoTen,

        LocalDate ngaySinh,

        String gioiTinh,

        String soDienThoai,

        String cccd,

        String diaChi,

        @Email(
                message = "Email không hợp lệ."
        )
        String email,

        String trangThai,

        String ghiChu
) {
}