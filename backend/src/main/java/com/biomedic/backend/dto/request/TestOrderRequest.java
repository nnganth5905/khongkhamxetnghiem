package com.biomedic.backend.dto.request;

import java.util.List;

public record TestOrderRequest(
        String visitId,
        String examinationId,
        String customerId,
        List<String> testIds,
        String priority,
        String note
) {
}