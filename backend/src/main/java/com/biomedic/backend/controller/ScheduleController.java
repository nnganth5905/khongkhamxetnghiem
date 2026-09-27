package com.biomedic.backend.controller;

import com.biomedic.backend.service.ScheduleService;

import jakarta.validation.Valid;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotNull;

import java.time.LocalDate;
import java.time.LocalTime;

import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.security.access.prepost.PreAuthorize;

import org.springframework.web.bind.annotation.DeleteMapping;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.PutMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping("/api")
public class ScheduleController {

    private final ScheduleService scheduleService;

    public ScheduleController(
            ScheduleService scheduleService
    ) {
        this.scheduleService = scheduleService;
    }

    @GetMapping("/schedules/doctors/{doctorId}")
    public ResponseEntity<?> getDoctorSchedule(
            @PathVariable
            String doctorId,

            @RequestParam(required = false)
            LocalDate date
    ) {
        return ResponseEntity.ok(
                scheduleService.getDoctorSchedule(
                        doctorId,
                        date
                )
        );
    }

    @GetMapping("/admin/work-schedules")
    @PreAuthorize("hasRole('ADMIN')")
    public ResponseEntity<?> getSchedules(
            @RequestParam(required = false)
            String q
    ) {
        return ResponseEntity.ok(
                scheduleService.getSchedules(q)
        );
    }

    @PostMapping("/admin/work-schedules")
    @PreAuthorize("hasRole('ADMIN')")
    public ResponseEntity<?> createSchedule(
            @Valid
            @RequestBody
            ScheduleRequest request
    ) {
        Object created =
                scheduleService.createSchedule(request);

        return ResponseEntity
                .status(HttpStatus.CREATED)
                .body(created);
    }

    @PutMapping("/admin/work-schedules/{id}")
    @PreAuthorize("hasRole('ADMIN')")
    public ResponseEntity<?> updateSchedule(
            @PathVariable
            String id,

            @Valid
            @RequestBody
            ScheduleRequest request
    ) {
        return ResponseEntity.ok(
                scheduleService.updateSchedule(
                        id,
                        request
                )
        );
    }

    @DeleteMapping("/admin/work-schedules/{id}")
    @PreAuthorize("hasRole('ADMIN')")
    public ResponseEntity<Void> deleteSchedule(
            @PathVariable
            String id
    ) {
        scheduleService.deleteSchedule(id);

        return ResponseEntity
                .noContent()
                .build();
    }

    public record ScheduleRequest(
            @NotBlank
            String loaiNhanSu,

            @NotBlank
            String idNhanSu,

            @NotNull
            LocalDate ngayLamViec,

            String caLamViec,

            @NotNull
            LocalTime gioBatDau,

            @NotNull
            LocalTime gioKetThuc,

            String idPhong,
            String trangThai,
            String ghiChu
    ) {
    }
}