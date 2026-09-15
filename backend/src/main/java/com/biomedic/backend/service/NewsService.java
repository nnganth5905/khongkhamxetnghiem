package com.biomedic.backend.service;

import java.util.Collections;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

import org.springframework.stereotype.Service;

@Service
public class NewsService {

    public List<Map<String, Object>> getMedicalArticles(
            String q
    ) {
        return Collections.emptyList();
    }

    public Map<String, Object> getMedicalArticle(
            String id
    ) {
        Map<String, Object> result = new LinkedHashMap<>();
        result.put("id", id);
        result.put("message", "Chưa có repository bài viết y khoa.");
        return result;
    }

    public List<Map<String, Object>> getHandbook() {
        return Collections.emptyList();
    }

    public List<Map<String, Object>> getSocialPosts(
            String q
    ) {
        return Collections.emptyList();
    }

    public Map<String, Object> getSocialPost(
            String id
    ) {
        Map<String, Object> result = new LinkedHashMap<>();
        result.put("id", id);
        result.put("message", "Chưa có repository hoạt động xã hội.");
        return result;
    }

    public List<Map<String, Object>> search(
            String q
    ) {
        return Collections.emptyList();
    }
}