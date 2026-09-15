package com.biomedic.backend.enums;

public enum RoleName {
    ADMIN,
    DOCTOR,
    CUSTOMER,
    RECEPTIONIST,
    TECHNICIAN;

    public static RoleName fromValue(
            String value
    ) {
        if (
            value == null ||
            value.isBlank()
        ) {
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

            case "KHACHHANG",
                 "CUSTOMER",
                 "PATIENT" -> CUSTOMER;

            case "LETAN",
                 "TIEPTAN",
                 "RECEPTIONIST",
                 "RECEPTION" -> RECEPTIONIST;

            case "KTV",
                 "KYTHUATVIEN",
                 "TECHNICIAN" -> TECHNICIAN;

            default -> CUSTOMER;
        };
    }

    public String authority() {
        return "ROLE_" + name();
    }
}