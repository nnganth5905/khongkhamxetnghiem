package com.biomedic.backend.dto.request;

public record ExaminationAppointmentRequest(
        String hoten,
        String email,
        String sodienthoai,
        String ngay,
        String gio,
        String idbacsi,
        String idchuyenkhoa,
        String idcoso,
        String lydokham,
        String ghichu
) {
}