package com.biomedic.backend.service;

import java.util.LinkedHashMap;
import java.util.Map;

import org.springframework.stereotype.Service;

@Service
public class ExaminationService {

    public Map<String, Object> getByVisit(
            String visitId
    ) {
        Map<String, Object> result = new LinkedHashMap<>();
        result.put("visitId", visitId);
        result.put("message", "Chưa nối repository bảng kham.");
        return result;
    }

    public Map<String, Object> createExamination(
            String visitId,
            String symptoms,
            String history,
            String diagnosis,
            String conclusion,
            String advice
    ) {
        return build(
                null,
                visitId,
                symptoms,
                history,
                diagnosis,
                conclusion,
                advice
        );
    }

    public Map<String, Object> updateExamination(
            String id,
            String visitId,
            String symptoms,
            String history,
            String diagnosis,
            String conclusion,
            String advice
    ) {
        return build(
                id,
                visitId,
                symptoms,
                history,
                diagnosis,
                conclusion,
                advice
        );
    }

    public Map<String, Object> completeExamination(
            String id
    ) {
        Map<String, Object> result = new LinkedHashMap<>();
        result.put("id", id);
        result.put("status", "COMPLETED");
        return result;
    }

    private Map<String, Object> build(
            String id,
            String visitId,
            String symptoms,
            String history,
            String diagnosis,
            String conclusion,
            String advice
    ) {
        Map<String, Object> result = new LinkedHashMap<>();

        result.put("id", id);
        result.put("visitId", visitId);
        result.put("symptoms", symptoms);
        result.put("history", history);
        result.put("diagnosis", diagnosis);
        result.put("conclusion", conclusion);
        result.put("advice", advice);

        return result;
    }
}