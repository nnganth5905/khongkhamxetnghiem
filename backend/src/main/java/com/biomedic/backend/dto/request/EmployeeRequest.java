package com.biomedic.backend.dto.request;

import jakarta.validation.constraints.Email;
import jakarta.validation.constraints.NotBlank;

import java.time.LocalDate;

public record EmployeeRequest(
        @NotBlank(
                message = "Họ tên không được để trống."
        )
        String hoTen,

        LocalDate ngaySinh,

        String gioiTinh,

        String soDienThoai,

        @Email(
                message = "Email không hợp lệ."
        )
        String email,

        String idChuyenKhoa,

        String idCoSo,

        @NotBlank(
                message = "Vai trò không được để trống."
        )
        String vaiTro,

        LocalDate ngayVaoLam,

        String trangThai,

        String ghiChu
) {
}