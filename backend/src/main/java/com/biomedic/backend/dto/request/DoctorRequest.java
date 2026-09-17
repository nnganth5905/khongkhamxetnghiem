package com.biomedic.backend.dto.request;

import jakarta.validation.constraints.Email;
import jakarta.validation.constraints.NotBlank;

public record DoctorRequest(
        @NotBlank(
                message = "Họ tên bác sĩ không được để trống."
        )
        String hoTen,

        @NotBlank(
                message = "Chuyên khoa không được để trống."
        )
        String idChuyenKhoa,

        String hocVi,

        String chucDanh,

        String soDienThoai,

        @Email(
                message = "Email không hợp lệ."
        )
        String email,

        String idPhong,

        String hinhAnh,

        String gioiThieu,

        String trangThai
) {
}