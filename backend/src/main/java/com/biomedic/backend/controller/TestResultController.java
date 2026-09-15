package com.biomedic.backend.controller;

import com.biomedic.backend.service.TestResultService;

import jakarta.validation.Valid;
import jakarta.validation.constraints.NotBlank;

import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.security.core.Authentication;
import org.springframework.security.access.prepost.PreAuthorize;

import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.PutMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping("/api")
public class TestResultController {

    private final TestResultService testResultService;

    public TestResultController(
            TestResultService testResultService
    ) {
        this.testResultService = testResultService;
    }

    // =========================
    // CUSTOMER
    // =========================
    @GetMapping("/results/mine")
    public ResponseEntity<?> getMyResults(
            Authentication authentication
    ) {
        return ResponseEntity.ok(
                testResultService.getMyResults(
                        authentication
                )
        );
    }

    @GetMapping("/results/{id}")
    public ResponseEntity<?> getResultDetail(
            @PathVariable
            String id
    ) {
        return ResponseEntity.ok(
                testResultService.getResultDetail(id)
        );
    }

    // =========================
    // PUBLIC LOOKUP
    // =========================
    @PostMapping("/results/lookup")
    public ResponseEntity<?> lookupResult(
            @Valid
            @RequestBody
            LookupRequest request
    ) {
        return ResponseEntity.ok(
                testResultService.lookupResult(
                        request.code(),
                        request.phone(),
                        request.email()
                )
        );
    }

    // =========================
    // TECHNICIAN
    // =========================
    @PostMapping("/technician/results")
    @PreAuthorize("hasRole('TECHNICIAN')")
    public ResponseEntity<?> createResult(
            @RequestBody
            ResultRequest request
    ) {
        Object created =
                testResultService.createResult(request);

        return ResponseEntity
                .status(HttpStatus.CREATED)
                .body(created);
    }

    @PutMapping("/technician/results/{id}")
    @PreAuthorize("hasRole('TECHNICIAN')")
    public ResponseEntity<?> updateResult(
            @PathVariable
            String id,

            @RequestBody
            ResultRequest request
    ) {
        return ResponseEntity.ok(
                testResultService.updateResult(
                        id,
                        request
                )
        );
    }

    // =========================
    // DOCTOR
    // =========================
    @PostMapping("/doctor/results/{id}/approve")
    @PreAuthorize("hasRole('DOCTOR')")
    public ResponseEntity<?> approveResult(
            @PathVariable
            String id
    ) {
        return ResponseEntity.ok(
                testResultService.approveResult(id)
        );
    }

    @PostMapping("/doctor/results/{id}/conclude")
    @PreAuthorize("hasRole('DOCTOR')")
    public ResponseEntity<?> concludeResult(
            @PathVariable
            String id,

            @RequestBody
            ConclusionRequest request
    ) {
        return ResponseEntity.ok(
                testResultService.concludeResult(
                        id,
                        request.conclusion(),
                        request.advice()
                )
        );
    }

    public record LookupRequest(
            @NotBlank
            String code,

            String phone,
            String email
    ) {
    }

    public record ResultRequest(
            String worklistId,
            String testOrderDetailId,
            java.util.List<ResultItem> items,
            String note
    ) {
    }

    public record ResultItem(
            String indicatorId,
            String value,
            String unit,
            String referenceRange,
            Boolean abnormal
    ) {
    }

    public record ConclusionRequest(
            String conclusion,
            String advice
    ) {
    }
}