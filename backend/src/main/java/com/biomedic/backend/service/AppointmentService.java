package com.biomedic.backend.service;

import com.biomedic.backend.controller.AppointmentController.ExaminationAppointmentRequest;
import com.biomedic.backend.controller.AppointmentController.TestAppointmentRequest;
import com.biomedic.backend.controller.AppointmentController.UpdateAppointmentRequest;

import java.time.LocalDate;
import java.util.Collections;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

import org.springframework.security.core.Authentication;
import org.springframework.stereotype.Service;

@Service
public class AppointmentService {

    public Map<String, Object> getAppointmentOptions() {
        Map<String, Object> result = new LinkedHashMap<>();

        result.put("specialties", Collections.emptyList());
        result.put("doctors", Collections.emptyList());
        result.put("tests", Collections.emptyList());
        result.put("facilities", Collections.emptyList());

        return result;
    }

    public List<String> getTakenTimes(
            String type,
            String doctorId,
            LocalDate date
    ) {
        return Collections.emptyList();
    }

    public Map<String, Object> createQuickAppointment(
            String hoten,
            String email,
            String ngay,
            String gio,
            String idbacsi,
            String ghichu
    ) {
        Map<String, Object> result = new LinkedHashMap<>();

        result.put("id", null);
        result.put("hoten", hoten);
        result.put("email", email);
        result.put("ngay", ngay);
        result.put("gio", gio);
        result.put("idbacsi", idbacsi);
        result.put("ghichu", ghichu);
        result.put("message", "Đã nhận yêu cầu đặt lịch nhanh.");

        return result;
    }

    public Map<String, Object> createTestAppointment(
            TestAppointmentRequest request
    ) {
        Map<String, Object> result = new LinkedHashMap<>();
        result.put("id", null);
        result.put("type", "TEST");
        result.put("request", request);
        result.put("message", "Đã nhận yêu cầu đặt lịch xét nghiệm.");
        return result;
    }

    public Map<String, Object> createExaminationAppointment(
            ExaminationAppointmentRequest request
    ) {
        Map<String, Object> result = new LinkedHashMap<>();
        result.put("id", null);
        result.put("type", "EXAMINATION");
        result.put("request", request);
        result.put("message", "Đã nhận yêu cầu đặt lịch khám.");
        return result;
    }

    public List<Map<String, Object>> getMyAppointments(
            Authentication authentication
    ) {
        return Collections.emptyList();
    }

    public Map<String, Object> getAppointmentDetail(
            String id
    ) {
        Map<String, Object> result = new LinkedHashMap<>();
        result.put("id", id);
        result.put("message", "Chưa nối repository lịch hẹn.");
        return result;
    }

    public Map<String, Object> updateAppointment(
            String id,
            UpdateAppointmentRequest request
    ) {
        Map<String, Object> result = new LinkedHashMap<>();
        result.put("id", id);
        result.put("request", request);
        result.put("message", "Đã nhận yêu cầu cập nhật lịch hẹn.");
        return result;
    }

    public Map<String, Object> cancelAppointment(
            String id
    ) {
        Map<String, Object> result = new LinkedHashMap<>();
        result.put("id", id);
        result.put("status", "CANCELLED");
        result.put("message", "Đã nhận yêu cầu hủy lịch.");
        return result;
    }
}