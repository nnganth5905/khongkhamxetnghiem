package com.biomedic.backend.service;

import java.util.Collections;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

import org.springframework.stereotype.Service;

@Service
public class VisitService {

    public Map<String, Object> checkIn(
            String appointmentId,
            String customerId,
            String visitType,
            String note
    ) {
        Map<String, Object> result = new LinkedHashMap<>();

        result.put("id", null);
        result.put("appointmentId", appointmentId);
        result.put("customerId", customerId);
        result.put("visitType", visitType);
        result.put("status", "WAITING");
        result.put("note", note);

        return result;
    }

    public List<Map<String, Object>> getWaitingList(
            String type
    ) {
        return Collections.emptyList();
    }

    public Map<String, Object> getVisitById(
            String id
    ) {
        Map<String, Object> result = new LinkedHashMap<>();
        result.put("id", id);
        result.put("message", "Chưa nối repository lượt khám/lượt xét nghiệm.");
        return result;
    }

    public Map<String, Object> updateStatus(
            String id,
            String status,
            String note
    ) {
        Map<String, Object> result = new LinkedHashMap<>();
        result.put("id", id);
        result.put("status", status);
        result.put("note", note);
        return result;
    }
}