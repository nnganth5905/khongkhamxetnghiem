package com.biomedic.backend.dto.request;

public record TestAppointmentRequest(
        String hoten,
        String email,
        String sodienthoai,
        String ngay,
        String gio,
        String idbacsi,
        String idxetnghiem,
        String idcoso,
        String ghichu
) {
}