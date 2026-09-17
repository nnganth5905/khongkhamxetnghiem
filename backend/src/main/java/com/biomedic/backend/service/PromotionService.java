package com.biomedic.backend.service;

import com.biomedic.backend.controller.PromotionController.PromotionRequest;

import java.util.Collections;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

import org.springframework.stereotype.Service;

@Service
public class PromotionService {

    public List<Map<String, Object>> getPromotions(
            String q
    ) {
        return Collections.emptyList();
    }

    public Map<String, Object> getPromotionById(
            String id
    ) {
        Map<String, Object> result = new LinkedHashMap<>();
        result.put("id", id);
        result.put("message", "Chưa có repository khuyến mãi.");
        return result;
    }

    public Map<String, Object> createPromotion(
            PromotionRequest request
    ) {
        Map<String, Object> result = new LinkedHashMap<>();
        result.put("id", null);
        result.put("request", request);
        return result;
    }

    public Map<String, Object> updatePromotion(
            String id,
            PromotionRequest request
    ) {
        Map<String, Object> result = new LinkedHashMap<>();
        result.put("id", id);
        result.put("request", request);
        return result;
    }

    public void deletePromotion(
            String id
    ) {
        // TODO repository delete
    }
}