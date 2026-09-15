package com.biomedic.backend.dto.response;

import java.time.LocalDate;

public record CustomerResponse(
        String id,
        String accountId,
        String fullName,
        LocalDate birthDate,
        String gender,
        String phone,
        String citizenId,
        String address,
        String email,
        String status
) {
}