package com.biomedic.backend.dto.request;

import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotNull;

import java.time.LocalDate;
import java.time.LocalTime;

public record ScheduleRequest(
        @NotBlank
        String loaiNhanSu,

        @NotBlank
        String idNhanSu,

        @NotNull
        LocalDate ngayLamViec,

        String caLamViec,

        @NotNull
        LocalTime gioBatDau,

        @NotNull
        LocalTime gioKetThuc,

        String idPhong,

        String trangThai,

        String ghiChu
) {
}