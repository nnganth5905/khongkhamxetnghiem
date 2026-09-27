package com.biomedic.backend.service;

import com.biomedic.backend.controller.TestController.TestCatalogRequest;
import com.biomedic.backend.controller.TestController.TestOrderRequest;

import java.util.Collections;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

import org.springframework.stereotype.Service;

@Service
public class TestService {

    public List<Map<String, Object>> getTests(
            String q
    ) {
        return Collections.emptyList();
    }

    public Map<String, Object> getTestById(
            String id
    ) {
        Map<String, Object> result = new LinkedHashMap<>();
        result.put("id", id);
        result.put("message", "Chưa nối repository danh mục xét nghiệm.");
        return result;
    }

    public List<Map<String, Object>> getTestsByCategory(
            String slug
    ) {
        return Collections.emptyList();
    }

    public Map<String, Object> createTestOrder(
            TestOrderRequest request
    ) {
        Map<String, Object> result = new LinkedHashMap<>();
        result.put("id", null);
        result.put("request", request);
        result.put("status", "ORDERED");
        return result;
    }

    public Map<String, Object> createTest(
            TestCatalogRequest request
    ) {
        Map<String, Object> result = new LinkedHashMap<>();
        result.put("id", null);
        result.put("request", request);
        return result;
    }

    public Map<String, Object> updateTest(
            String id,
            TestCatalogRequest request
    ) {
        Map<String, Object> result = new LinkedHashMap<>();
        result.put("id", id);
        result.put("request", request);
        return result;
    }

    public void deleteTest(
            String id
    ) {
        // TODO: repository delete
    }
}