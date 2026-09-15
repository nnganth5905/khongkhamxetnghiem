package com.biomedic.backend.dto.request;

import jakarta.validation.constraints.Email;
import jakarta.validation.constraints.NotBlank;

import java.time.LocalDate;

public record TechnicianRequest(
        @NotBlank(
                message = "Họ tên kỹ thuật viên không được để trống."
        )
        String hoTen,

        String idCoSo,

        String trinhDo,

        String soDienThoai,

        @Email(
                message = "Email không hợp lệ."
        )
        String email,

        LocalDate ngayVaoLam,

        String trangThai,

        String ghiChu
) {
}