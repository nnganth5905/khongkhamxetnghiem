package com.biomedic.backend.service;

import com.biomedic.backend.controller.ScheduleController.ScheduleRequest;

import java.time.LocalDate;
import java.util.Collections;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

import org.springframework.stereotype.Service;

@Service
public class ScheduleService {

    public List<Map<String, Object>> getDoctorSchedule(
            String doctorId,
            LocalDate date
    ) {
        return Collections.emptyList();
    }

    public List<Map<String, Object>> getSchedules(
            String q
    ) {
        return Collections.emptyList();
    }

    public Map<String, Object> createSchedule(
            ScheduleRequest request
    ) {
        Map<String, Object> result = new LinkedHashMap<>();
        result.put("id", null);
        result.put("request", request);
        return result;
    }

    public Map<String, Object> updateSchedule(
            String id,
            ScheduleRequest request
    ) {
        Map<String, Object> result = new LinkedHashMap<>();
        result.put("id", id);
        result.put("request", request);
        return result;
    }

    public void deleteSchedule(
            String id
    ) {
        // TODO repository delete
    }
}