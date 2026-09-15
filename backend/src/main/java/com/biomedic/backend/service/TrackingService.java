package com.biomedic.backend.service;

import java.util.Collections;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

import org.springframework.stereotype.Service;

@Service
public class TrackingService {

    public Map<String, Object> trackVisit(
            String code
    ) {
        Map<String, Object> result = new LinkedHashMap<>();
        result.put("code", code);
        result.put("timeline", Collections.emptyList());
        return result;
    }

    public Map<String, Object> trackTest(
            String code
    ) {
        Map<String, Object> result = new LinkedHashMap<>();
        result.put("code", code);
        result.put("timeline", Collections.emptyList());
        return result;
    }

    public List<Map<String, Object>> getHistory(
            String type,
            String objectId
    ) {
        return Collections.emptyList();
    }
}