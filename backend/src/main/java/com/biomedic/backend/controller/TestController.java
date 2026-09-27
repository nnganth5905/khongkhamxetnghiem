package com.biomedic.backend.controller;

import com.biomedic.backend.service.TestService;

import jakarta.validation.Valid;
import jakarta.validation.constraints.NotBlank;

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
public class TestController {

    private final TestService testService;

    public TestController(
            TestService testService
    ) {
        this.testService = testService;
    }

    // =========================
    // PUBLIC CATALOG
    // =========================
    @GetMapping("/tests")
    public ResponseEntity<?> getTests(
            @RequestParam(required = false)
            String q
    ) {
        return ResponseEntity.ok(
                testService.getTests(q)
        );
    }

    @GetMapping("/tests/{id}")
    public ResponseEntity<?> getTestById(
            @PathVariable
            String id
    ) {
        return ResponseEntity.ok(
                testService.getTestById(id)
        );
    }

    @GetMapping("/tests/category/{slug}")
    public ResponseEntity<?> getTestsByCategory(
            @PathVariable
            String slug
    ) {
        return ResponseEntity.ok(
                testService.getTestsByCategory(slug)
        );
    }

    // =========================
    // DOCTOR - CREATE TEST ORDER
    // =========================
    @PostMapping("/doctor/test-orders")
    @PreAuthorize("hasRole('DOCTOR')")
    public ResponseEntity<?> createTestOrder(
            @RequestBody
            TestOrderRequest request
    ) {
        Object created =
                testService.createTestOrder(request);

        return ResponseEntity
                .status(HttpStatus.CREATED)
                .body(created);
    }

    // =========================
    // ADMIN CRUD
    // =========================
    @GetMapping("/admin/tests")
    @PreAuthorize("hasRole('ADMIN')")
    public ResponseEntity<?> adminGetTests(
            @RequestParam(required = false)
            String q
    ) {
        return ResponseEntity.ok(
                testService.getTests(q)
        );
    }

    @PostMapping("/admin/tests")
    @PreAuthorize("hasRole('ADMIN')")
    public ResponseEntity<?> createTest(
            @Valid
            @RequestBody
            TestCatalogRequest request
    ) {
        Object created =
                testService.createTest(request);

        return ResponseEntity
                .status(HttpStatus.CREATED)
                .body(created);
    }

    @PutMapping("/admin/tests/{id}")
    @PreAuthorize("hasRole('ADMIN')")
    public ResponseEntity<?> updateTest(
            @PathVariable
            String id,

            @Valid
            @RequestBody
            TestCatalogRequest request
    ) {
        return ResponseEntity.ok(
                testService.updateTest(
                        id,
                        request
                )
        );
    }

    @DeleteMapping("/admin/tests/{id}")
    @PreAuthorize("hasRole('ADMIN')")
    public ResponseEntity<Void> deleteTest(
            @PathVariable
            String id
    ) {
        testService.deleteTest(id);

        return ResponseEntity
                .noContent()
                .build();
    }

    public record TestOrderRequest(
            String visitId,
            String examinationId,
            String customerId,
            java.util.List<String> testIds,
            String priority,
            String note
    ) {
    }

    public record TestCatalogRequest(
            @NotBlank
            String tenXetNghiem,

            String idLoaiXetNghiem,
            String loaiMau,
            Double gia,
            String thoiGianTraKetQua,
            String trangThai,
            String moTa,
            String chuanBi
    ) {
    }
}