package com.biomedic.backend.dto.request;

import java.util.List;

public record ResultEntryRequest(
        String worklistId,
        String testOrderDetailId,
        List<ResultItemRequest> items,
        String note
) {
    public record ResultItemRequest(
            String indicatorId,
            String value,
            String unit,
            String referenceRange,
            Boolean abnormal
    ) {
    }
}