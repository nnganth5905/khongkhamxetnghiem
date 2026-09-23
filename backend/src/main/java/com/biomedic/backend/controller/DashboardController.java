package com.biomedic.backend.controller;

import com.biomedic.backend.entity.Account;
import com.biomedic.backend.repository.AccountRepository;
import com.biomedic.backend.service.DashboardService;
import org.springframework.http.ResponseEntity;
import org.springframework.security.core.Authentication;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import java.util.Optional;

@RestController
@RequestMapping("/api/dashboard")
public class DashboardController {

    private final DashboardService dashboardService;
    private final AccountRepository accountRepository;

    public DashboardController(DashboardService dashboardService, AccountRepository accountRepository) {
        this.dashboardService = dashboardService;
        this.accountRepository = accountRepository;
    }

    // ==========================================
    // 1. DASHBOARD KHÁCH HÀNG
    // ==========================================
    @GetMapping("/customer")
    public ResponseEntity<?> getCustomerDashboard(Authentication authentication) {
        if (authentication == null || !authentication.isAuthenticated() || "anonymousUser".equals(authentication.getPrincipal())) {
            return ResponseEntity.status(401).body("Unauthorized");
        }

        String email = authentication.getName();
        Optional<Account> accountOpt = accountRepository.findByEmail(email);
        
        if (accountOpt.isEmpty()) {
            return ResponseEntity.status(404).body("Không tìm thấy thông tin tài khoản.");
        }

        Account account = accountOpt.get();
        String customerId = account.getIdKhachHang();
        Integer userId = account.getUserId();

        if (customerId == null) {
            customerId = "";
        }

        return ResponseEntity.ok(dashboardService.getCustomerDashboardData(customerId, userId));
    }

    // ==========================================
    // 2. DASHBOARD BÁC SĨ (VỪA BỔ SUNG)
    // ==========================================
    @GetMapping("/doctor")
    public ResponseEntity<?> getDoctorDashboard(Authentication authentication) {
        if (authentication == null || !authentication.isAuthenticated() || "anonymousUser".equals(authentication.getPrincipal())) {
            return ResponseEntity.status(401).body("Unauthorized");
        }

        String email = authentication.getName();
        Optional<Account> accountOpt = accountRepository.findByEmail(email);
        
        if (accountOpt.isEmpty()) {
            return ResponseEntity.status(404).body("Không tìm thấy thông tin tài khoản.");
        }

        Account account = accountOpt.get();
        Integer doctorId = account.getIdBacSi(); // Lấy ID Bác sĩ từ Account

        if (doctorId == null) {
            return ResponseEntity.status(400).body("Tài khoản đang đăng nhập không phải là bác sĩ.");
        }

        // Gọi service lấy dữ liệu cho bảng điều khiển của bác sĩ
        return ResponseEntity.ok(dashboardService.getDoctorDashboardData(doctorId));
    }
    
}