package com.biomedic.backend.controller;

import com.biomedic.backend.service.VisitService;

import jakarta.validation.Valid;
import jakarta.validation.constraints.NotBlank;

import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.security.access.prepost.PreAuthorize;

import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PatchMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping("/api")
public class VisitController {

    private final VisitService visitService;

    public VisitController(
            VisitService visitService
    ) {
        this.visitService = visitService;
    }

    @PostMapping("/reception/visits/check-in")
    @PreAuthorize("hasRole('RECEPTIONIST')")
    public ResponseEntity<?> checkIn(
            @Valid
            @RequestBody
            CheckInRequest request
    ) {
        Object created =
                visitService.checkIn(
                        request.appointmentId(),
                        request.customerId(),
                        request.visitType(),
                        request.note()
                );

        return ResponseEntity
                .status(HttpStatus.CREATED)
                .body(created);
    }

    @GetMapping("/reception/visits/waiting")
    @PreAuthorize("hasRole('RECEPTIONIST')")
    public ResponseEntity<?> getReceptionWaitingList(
            @RequestParam(required = false)
            String type
    ) {
        return ResponseEntity.ok(
                visitService.getWaitingList(type)
        );
    }

    @GetMapping("/doctor/visits/waiting")
    @PreAuthorize("hasRole('DOCTOR')")
    public ResponseEntity<?> getDoctorWaitingList() {
        return ResponseEntity.ok(
                visitService.getWaitingList("EXAMINATION")
        );
    }

    @GetMapping("/visits/{id}")
    public ResponseEntity<?> getVisitById(
            @PathVariable
            String id
    ) {
        return ResponseEntity.ok(
                visitService.getVisitById(id)
        );
    }

    @PatchMapping("/visits/{id}/status")
    public ResponseEntity<?> updateStatus(
            @PathVariable
            String id,

            @RequestBody
            StatusRequest request
    ) {
        return ResponseEntity.ok(
                visitService.updateStatus(
                        id,
                        request.status(),
                        request.note()
                )
        );
    }

    public record CheckInRequest(
            String appointmentId,
            String customerId,

            @NotBlank
            String visitType,

            String note
    ) {
    }

    public record StatusRequest(
            @NotBlank
            String status,

            String note
    ) {
    }
}