package com.biomedic.backend.entity;

/**
 * Vai trò đăng nhập dùng trong backend.
 *
 * Chưa map thành bảng riêng vì database hiện tại đã có bảng users
 * nhưng chưa có SQL schema chính xác cho role.
 */
public enum Role {
    CUSTOMER,
    DOCTOR,
    RECEPTIONIST,
    TECHNICIAN,
    ADMIN;

    public static Role fromDatabaseValue(String value) {
        if (value == null || value.isBlank()) {
            return CUSTOMER;
        }

        String normalized = value
                .trim()
                .toUpperCase()
                .replace("-", "")
                .replace("_", "")
                .replace(" ", "");

        return switch (normalized) {
            case "ADMIN" -> ADMIN;

            case "BACSI",
                 "DOCTOR",
                 "BS" -> DOCTOR;

            case "LETAN",
                 "TIEPTAN",
                 "RECEPTIONIST",
                 "RECEPTION" -> RECEPTIONIST;

            case "KTV",
                 "KYTHUATVIEN",
                 "TECHNICIAN" -> TECHNICIAN;

            case "KHACHHANG",
                 "CUSTOMER",
                 "PATIENT" -> CUSTOMER;

            default -> CUSTOMER;
        };
    }
}