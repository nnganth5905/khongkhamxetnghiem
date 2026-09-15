package com.biomedic.backend.service;

import com.biomedic.backend.controller.SpecimenController.CollectSpecimenRequest;

import java.util.Collections;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

import org.springframework.stereotype.Service;

@Service
public class SpecimenService {

    public Map<String, Object> collectSpecimen(
            CollectSpecimenRequest request
    ) {
        Map<String, Object> result = new LinkedHashMap<>();
        result.put("id", null);
        result.put("request", request);
        result.put("status", "COLLECTED");
        return result;
    }

    public Map<String, Object> handoverSpecimen(
            String id,
            String receiverId,
            String note
    ) {
        Map<String, Object> result = new LinkedHashMap<>();
        result.put("id", id);
        result.put("receiverId", receiverId);
        result.put("note", note);
        result.put("status", "HANDED_OVER");
        return result;
    }

    public List<Map<String, Object>> getSpecimens(
            String status
    ) {
        return Collections.emptyList();
    }

    public Map<String, Object> receiveSpecimen(
            String id
    ) {
        Map<String, Object> result = new LinkedHashMap<>();
        result.put("id", id);
        result.put("status", "RECEIVED");
        return result;
    }

    public List<Map<String, Object>> getWorklist(
            String status
    ) {
        return Collections.emptyList();
    }

    public Map<String, Object> startWorklist(
            String id
    ) {
        Map<String, Object> result = new LinkedHashMap<>();
        result.put("id", id);
        result.put("status", "IN_PROGRESS");
        return result;
    }
}