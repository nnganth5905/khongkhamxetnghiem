package com.biomedic.backend.service;

import com.biomedic.backend.controller.TestResultController.ResultRequest;

import java.util.Collections;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

import org.springframework.security.core.Authentication;
import org.springframework.stereotype.Service;

@Service
public class TestResultService {

    public List<Map<String, Object>> getMyResults(
            Authentication authentication
    ) {
        return Collections.emptyList();
    }

    public Map<String, Object> getResultDetail(
            String id
    ) {
        Map<String, Object> result = new LinkedHashMap<>();
        result.put("id", id);
        result.put("message", "Chưa nối repository kết quả xét nghiệm.");
        return result;
    }

    public Map<String, Object> lookupResult(
            String code,
            String phone,
            String email
    ) {
        Map<String, Object> result = new LinkedHashMap<>();
        result.put("code", code);
        result.put("phone", phone);
        result.put("email", email);
        result.put("found", false);
        return result;
    }

    public Map<String, Object> createResult(
            ResultRequest request
    ) {
        Map<String, Object> result = new LinkedHashMap<>();
        result.put("id", null);
        result.put("request", request);
        result.put("status", "ENTERED");
        return result;
    }

    public Map<String, Object> updateResult(
            String id,
            ResultRequest request
    ) {
        Map<String, Object> result = new LinkedHashMap<>();
        result.put("id", id);
        result.put("request", request);
        result.put("status", "ENTERED");
        return result;
    }

    public Map<String, Object> approveResult(
            String id
    ) {
        Map<String, Object> result = new LinkedHashMap<>();
        result.put("id", id);
        result.put("status", "APPROVED");
        return result;
    }

    public Map<String, Object> concludeResult(
            String id,
            String conclusion,
            String advice
    ) {
        Map<String, Object> result = new LinkedHashMap<>();
        result.put("id", id);
        result.put("conclusion", conclusion);
        result.put("advice", advice);
        result.put("status", "COMPLETED");
        return result;
    }
}