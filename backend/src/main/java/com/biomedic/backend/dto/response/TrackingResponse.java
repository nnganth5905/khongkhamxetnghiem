package com.biomedic.backend.dto.response;

import java.time.LocalDateTime;
import java.util.List;

public record TrackingResponse(
        String code,
        String objectType,
        String objectId,
        String currentStatus,
        List<TimelineItem> timeline
) {
    public record TimelineItem(
            String action,
            String status,
            String description,
            LocalDateTime occurredAt,
            String performedBy
    ) {
    }
}