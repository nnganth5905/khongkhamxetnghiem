package com.biomedic.backend.controller;

import com.biomedic.backend.service.SpecimenService;
import jakarta.validation.Valid;
import jakarta.validation.constraints.NotBlank;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.web.bind.annotation.*;
import java.util.Map;

@RestController
@RequestMapping("/api")
public class SpecimenController {

    private final SpecimenService specimenService;

    public SpecimenController(SpecimenService specimenService) {
        this.specimenService = specimenService;
    }

    @PostMapping("/doctor/specimens")
    @PreAuthorize("hasRole('DOCTOR')")
    public ResponseEntity<?> collectSpecimen(@Valid @RequestBody CollectSpecimenRequest request) {
        return ResponseEntity.status(HttpStatus.CREATED).body(specimenService.collectSpecimen(request));
    }

    @PostMapping("/doctor/specimens/{id}/handover")
    @PreAuthorize("hasRole('DOCTOR')")
    public ResponseEntity<?> handoverSpecimen(@PathVariable String id, @RequestBody HandoverRequest request) {
        return ResponseEntity.ok(specimenService.handoverSpecimen(id, request));
    }

    @GetMapping("/technicians")
    public ResponseEntity<?> getTechniciansList() {
        return ResponseEntity.ok(specimenService.getTechnicians());
    }

    @GetMapping("/technician/specimens")
    @PreAuthorize("hasRole('TECHNICIAN')")
    public ResponseEntity<?> getSpecimens(@RequestParam(required = false) String status) {
        return ResponseEntity.ok(specimenService.getSpecimens(status));
    }

    @PostMapping("/technician/specimens/{id}/receive")
    @PreAuthorize("hasRole('TECHNICIAN')")
    public ResponseEntity<?> receiveSpecimen(@PathVariable String id, @RequestBody Map<String, Object> payload) {
        return ResponseEntity.ok(specimenService.receiveSpecimen(id, payload));
    }

    @PostMapping("/technician/specimens/{id}/reject")
    @PreAuthorize("hasRole('TECHNICIAN')")
    public ResponseEntity<?> rejectSpecimen(@PathVariable String id, @RequestBody Map<String, String> payload) {
        return ResponseEntity.ok(specimenService.rejectSpecimen(id, payload.get("reason")));
    }

    @GetMapping("/technician/worklist")
    @PreAuthorize("hasRole('TECHNICIAN')")
    public ResponseEntity<?> getWorklist(@RequestParam(required = false) String status) {
        return ResponseEntity.ok(specimenService.getWorklist(status));
    }

    @GetMapping("/technician/specimens/{id}")
    @PreAuthorize("hasRole('TECHNICIAN')")
    public ResponseEntity<?> getSpecimenById(@PathVariable String id) {
        return ResponseEntity.ok(specimenService.getSpecimen(id));
    }

    @PostMapping("/technician/worklist/{id}/start")
    @PreAuthorize("hasRole('TECHNICIAN')")
    public ResponseEntity<?> startWorklist(@PathVariable String id) {
        return ResponseEntity.ok(specimenService.startWorklist(id));
    }

    @PostMapping("/technician/worklist/{id}/complete")
    @PreAuthorize("hasRole('TECHNICIAN')")
    public ResponseEntity<?> completeWorklist(@PathVariable String id) {
        return ResponseEntity.ok(specimenService.completeWorklist(id));
    }
// ==========================================
    // NHẬP KẾT QUẢ XÉT NGHIỆM
    // ==========================================
    @GetMapping("/technician/results/{worklistId}")
    @PreAuthorize("hasRole('TECHNICIAN')")
    public ResponseEntity<?> getResultEntry(@PathVariable String worklistId) {
        return ResponseEntity.ok(specimenService.getResultEntry(worklistId));
    }

    @PutMapping("/technician/results/{worklistId}")
    @PreAuthorize("hasRole('TECHNICIAN')")
    public ResponseEntity<?> saveResultEntry(@PathVariable String worklistId, @RequestBody Map<String, Object> payload) {
        return ResponseEntity.ok(specimenService.saveResultEntry(worklistId, payload, false));
    }

    @PostMapping("/technician/results/{worklistId}/submit")
    @PreAuthorize("hasRole('TECHNICIAN')")
    public ResponseEntity<?> submitResultEntry(@PathVariable String worklistId, @RequestBody Map<String, Object> payload) {
        return ResponseEntity.ok(specimenService.saveResultEntry(worklistId, payload, true));
    }

    public record CollectSpecimenRequest(
            @NotBlank String patientCode,
            String patientName,
            String sampleType,
            @NotBlank String barcode,
            String samplingTime,
            String notes,
            @NotBlank String appointmentId
    ) {}

    public record HandoverRequest(
            String receiverId,
            String note,
            String appointmentId,
            String patientCode
    ) {}

    
}