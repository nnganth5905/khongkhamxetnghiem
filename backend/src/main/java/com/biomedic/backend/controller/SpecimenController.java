package com.biomedic.backend.controller;

import com.biomedic.backend.service.SpecimenService;

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
public class SpecimenController {

    private final SpecimenService specimenService;

    public SpecimenController(
            SpecimenService specimenService
    ) {
        this.specimenService = specimenService;
    }

    @PostMapping("/doctor/specimens")
    @PreAuthorize("hasRole('DOCTOR')")
    public ResponseEntity<?> collectSpecimen(
            @Valid
            @RequestBody
            CollectSpecimenRequest request
    ) {
        Object created =
                specimenService.collectSpecimen(request);

        return ResponseEntity
                .status(HttpStatus.CREATED)
                .body(created);
    }

    @PostMapping("/doctor/specimens/{id}/handover")
    @PreAuthorize("hasRole('DOCTOR')")
    public ResponseEntity<?> handoverSpecimen(
            @PathVariable
            String id,

            @RequestBody
            HandoverRequest request
    ) {
        return ResponseEntity.ok(
                specimenService.handoverSpecimen(
                        id,
                        request.receiverId(),
                        request.note()
                )
        );
    }

    @GetMapping("/technician/specimens")
    @PreAuthorize("hasRole('TECHNICIAN')")
    public ResponseEntity<?> getSpecimens(
            @RequestParam(required = false)
            String status
    ) {
        return ResponseEntity.ok(
                specimenService.getSpecimens(status)
        );
    }

    @PostMapping("/technician/specimens/{id}/receive")
    @PreAuthorize("hasRole('TECHNICIAN')")
    public ResponseEntity<?> receiveSpecimen(
            @PathVariable
            String id
    ) {
        return ResponseEntity.ok(
                specimenService.receiveSpecimen(id)
        );
    }

    @GetMapping("/technician/worklist")
    @PreAuthorize("hasRole('TECHNICIAN')")
    public ResponseEntity<?> getWorklist(
            @RequestParam(required = false)
            String status
    ) {
        return ResponseEntity.ok(
                specimenService.getWorklist(status)
        );
    }

    @PatchMapping("/technician/worklist/{id}/start")
    @PreAuthorize("hasRole('TECHNICIAN')")
    public ResponseEntity<?> startWorklist(
            @PathVariable
            String id
    ) {
        return ResponseEntity.ok(
                specimenService.startWorklist(id)
        );
    }

    public record CollectSpecimenRequest(
            @NotBlank
            String testOrderId,

            String customerId,

            @NotBlank
            String specimenType,

            String note
    ) {
    }

    public record HandoverRequest(
            String receiverId,
            String note
    ) {
    }
}