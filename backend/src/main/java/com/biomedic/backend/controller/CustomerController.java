package com.biomedic.backend.controller;

import com.biomedic.backend.service.CustomerService;
import jakarta.validation.Valid;
import jakarta.validation.constraints.Email;
import jakarta.validation.constraints.NotBlank;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.security.core.Authentication;
import org.springframework.web.bind.annotation.*;

import java.time.LocalDate;

@RestController
@RequestMapping("/api")
public class CustomerController {

    private final CustomerService customerService;

    public CustomerController(CustomerService customerService) {
        this.customerService = customerService;
    }

    // =========================
    // CURRENT CUSTOMER
    // =========================
    @GetMapping("/customers/me")
    public ResponseEntity<?> getMyProfile(Authentication authentication) {
        return ResponseEntity.ok(customerService.getCurrentCustomer(authentication));
    }

    @PutMapping("/customers/me")
    public ResponseEntity<?> updateMyProfile(
            Authentication authentication,
            @Valid @RequestBody CustomerRequest request
    ) {
        return ResponseEntity.ok(customerService.updateCurrentCustomer(
                authentication,
                request.tenKhachHang(),
                request.ngaySinh(),
                request.gioiTinh(),
                request.soDienThoai(),
                request.cccd(),
                request.diaChi(),
                request.email(),
                null
        ));
    }

    // =========================
    // ADMIN - LIST
    // =========================
    @GetMapping("/admin/customers")
    @PreAuthorize("hasRole('ADMIN')")
    public ResponseEntity<?> getAllCustomers(@RequestParam(required = false) String q) {
        return ResponseEntity.ok(customerService.getAllCustomers(q));
    }

    // =========================
    // ADMIN - DETAIL
    // =========================
    @GetMapping("/admin/customers/{id}")
    @PreAuthorize("hasRole('ADMIN')")
    public ResponseEntity<?> getCustomerById(@PathVariable String id) {
        return ResponseEntity.ok(customerService.getCustomerById(id));
    }

    // =========================
    // ADMIN - CREATE
    // =========================
    @PostMapping("/admin/customers")
    @PreAuthorize("hasRole('ADMIN')")
    public ResponseEntity<?> createCustomer(@Valid @RequestBody CustomerRequest request) {
        Object created = customerService.createCustomer(
                request.tenKhachHang(),
                request.ngaySinh(),
                request.gioiTinh(),
                request.soDienThoai(),
                request.cccd(),
                request.diaChi(),
                request.email(),
                request.status()
        );

        return ResponseEntity.status(HttpStatus.CREATED).body(created);
    }

    // =========================
    // ADMIN - UPDATE
    // =========================
    @PutMapping("/admin/customers/{id}")
    @PreAuthorize("hasRole('ADMIN')")
    public ResponseEntity<?> updateCustomer(
            @PathVariable String id,
            @Valid @RequestBody CustomerRequest request
    ) {
        return ResponseEntity.ok(customerService.updateCustomer(
                id,
                request.tenKhachHang(),
                request.ngaySinh(),
                request.gioiTinh(),
                request.soDienThoai(),
                request.cccd(),
                request.diaChi(),
                request.email(),
                request.status()
        ));
    }

    // =========================
    // ADMIN - DELETE
    // =========================
    @DeleteMapping("/admin/customers/{id}")
    @PreAuthorize("hasRole('ADMIN')")
    public ResponseEntity<Void> deleteCustomer(@PathVariable String id) {
        customerService.deleteCustomer(id);
        return ResponseEntity.noContent().build();
    }

    // DTO hứng dữ liệu trực tiếp từ Frontend
    public record CustomerRequest(
            @NotBlank(message = "Họ tên không được để trống.")
            String tenKhachHang,
            LocalDate ngaySinh,
            String gioiTinh,
            String soDienThoai,
            String cccd,
            String diaChi,
            @Email(message = "Email không hợp lệ.")
            String email,
            String status
    ) {}
}