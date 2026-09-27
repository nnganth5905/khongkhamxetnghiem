package com.biomedic.backend.service;

import org.springframework.jdbc.core.JdbcTemplate;
import org.springframework.scheduling.annotation.Scheduled;
import org.springframework.stereotype.Component;
import org.springframework.transaction.annotation.Transactional;

@Component
public class AppointmentScheduler {

    private final JdbcTemplate jdbcTemplate;

    public AppointmentScheduler(JdbcTemplate jdbcTemplate) {
        this.jdbcTemplate = jdbcTemplate;
    }

    // Chạy vào lúc 00:01 mỗi ngày
    @Scheduled(cron = "0 1 0 * * ?")
    @Transactional
    public void autoCancelExpiredAppointments() {
        // Cập nhật lịch khám bệnh quá hạn thành no_show
        String sqlExam = "UPDATE datlichkham SET TrangThai = 'no_show' " +
                         "WHERE NgayKham < CURRENT_DATE AND TrangThai IN ('pending', 'confirmed')";
        int examUpdated = jdbcTemplate.update(sqlExam);

        // Cập nhật lịch xét nghiệm quá hạn thành no_show
        String sqlTest = "UPDATE datlichxetnghiem SET TrangThai = 'no_show' " +
                         "WHERE NgayXetNghiem < CURRENT_DATE AND TrangThai IN ('pending', 'confirmed')";
        int testUpdated = jdbcTemplate.update(sqlTest);

        System.out.println("Auto-Scheduler: Đã chuyển " + examUpdated + " lịch khám và " + testUpdated + " lịch XN sang trạng thái Không đến (No show).");
    }
}