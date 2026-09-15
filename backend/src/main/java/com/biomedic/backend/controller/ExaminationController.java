package com.biomedic.backend.controller;

import com.biomedic.backend.service.ExaminationService;

import jakarta.validation.Valid;
import jakarta.validation.constraints.NotBlank;

import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.security.access.prepost.PreAuthorize;

import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.PutMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping("/api/doctor/examinations")
@PreAuthorize("hasRole('DOCTOR')")
public class ExaminationController {

    private final ExaminationService examinationService;

    public ExaminationController(
            ExaminationService examinationService
    ) {
        this.examinationService = examinationService;
    }

    @GetMapping("/visit/{visitId}")
    public ResponseEntity<?> getByVisit(
            @PathVariable
            String visitId
    ) {
        return ResponseEntity.ok(
                examinationService.getByVisit(visitId)
        );
    }

    @PostMapping
    public ResponseEntity<?> createExamination(
            @Valid
            @RequestBody
            ExaminationRequest request
    ) {
        Object created =
                examinationService.createExamination(
                        request.visitId(),
                        request.symptoms(),
                        request.history(),
                        request.diagnosis(),
                        request.conclusion(),
                        request.advice()
                );

        return ResponseEntity
                .status(HttpStatus.CREATED)
                .body(created);
    }

    @PutMapping("/{id}")
    public ResponseEntity<?> updateExamination(
            @PathVariable
            String id,

            @Valid
            @RequestBody
            ExaminationRequest request
    ) {
        return ResponseEntity.ok(
                examinationService.updateExamination(
                        id,
                        request.visitId(),
                        request.symptoms(),
                        request.history(),
                        request.diagnosis(),
                        request.conclusion(),
                        request.advice()
                )
        );
    }

    @PostMapping("/{id}/complete")
    public ResponseEntity<?> completeExamination(
            @PathVariable
            String id
    ) {
        return ResponseEntity.ok(
                examinationService.completeExamination(id)
        );
    }

    public record ExaminationRequest(
            @NotBlank
            String visitId,

            String symptoms,
            String history,
            String diagnosis,
            String conclusion,
            String advice
    ) {
    }
}