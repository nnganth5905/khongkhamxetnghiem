package com.biomedic.backend.controller;

import com.biomedic.backend.service.DoctorService;
import org.springframework.http.ResponseEntity;
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.security.core.Authentication;
import org.springframework.web.bind.annotation.*;

import java.util.Map;

@RestController
@RequestMapping("/api/doctor")
public class DoctorController {

    private final DoctorService doctorService;

    public DoctorController(DoctorService doctorService) {
        this.doctorService = doctorService;
    }

    // 1. Lấy danh sách kết quả chờ duyệt
    @GetMapping("/results/pending")
    @PreAuthorize("hasRole('DOCTOR')")
    public ResponseEntity<?> getPendingResults(Authentication authentication) {
        String username = authentication.getName();
        return ResponseEntity.ok(doctorService.getPendingResults(username));
    }

    // 2. Lấy chi tiết một kết quả để đọc
    @GetMapping("/results/{id}")
    @PreAuthorize("hasRole('DOCTOR')")
    public ResponseEntity<?> getResultDetail(@PathVariable String id) {
        return ResponseEntity.ok(doctorService.getResultDetail(id));
    }

    // 3. Ký duyệt và lưu kết luận (API bị thiếu gây ra lỗi)
    @PostMapping("/results/{id}/approve")
    @PreAuthorize("hasRole('DOCTOR')")
    public ResponseEntity<?> approveResult(@PathVariable String id, @RequestBody Map<String, String> payload, Authentication authentication) {
        String username = authentication.getName();
        String conclusion = payload.getOrDefault("conclusion", "");
        return ResponseEntity.ok(doctorService.approveResult(id, conclusion, username));
    }
}