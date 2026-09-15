package com.biomedic.backend.controller;

import com.biomedic.backend.service.TechnicianService;

import jakarta.validation.Valid;
import jakarta.validation.constraints.Email;
import jakarta.validation.constraints.NotBlank;

import java.time.LocalDate;

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
public class TechnicianController {

    private final TechnicianService technicianService;

    public TechnicianController(
            TechnicianService technicianService
    ) {
        this.technicianService = technicianService;
    }

    // =========================
    // TECHNICIAN - CURRENT USER
    // =========================
    @GetMapping("/technician/me")
    @PreAuthorize("hasRole('TECHNICIAN')")
    public ResponseEntity<?> getCurrentTechnician() {
        return ResponseEntity.ok(
                technicianService.getCurrentTechnician()
        );
    }

    // =========================
    // ADMIN - LIST
    // =========================
    @GetMapping("/admin/technicians")
    @PreAuthorize("hasRole('ADMIN')")
    public ResponseEntity<?> getTechnicians(
            @RequestParam(required = false)
            String q
    ) {
        return ResponseEntity.ok(
                technicianService.getTechnicians(q)
        );
    }

    // =========================
    // ADMIN - DETAIL
    // =========================
    @GetMapping("/admin/technicians/{id}")
    @PreAuthorize("hasRole('ADMIN')")
    public ResponseEntity<?> getTechnicianById(
            @PathVariable
            String id
    ) {
        return ResponseEntity.ok(
                technicianService.getTechnicianById(id)
        );
    }

    // =========================
    // ADMIN - CREATE
    // =========================
    @PostMapping("/admin/technicians")
    @PreAuthorize("hasRole('ADMIN')")
    public ResponseEntity<?> createTechnician(
            @Valid
            @RequestBody
            TechnicianRequest request
    ) {
        Object created =
                technicianService.createTechnician(
                        request.hoTen(),
                        request.idCoSo(),
                        request.trinhDo(),
                        request.soDienThoai(),
                        request.email(),
                        request.ngayVaoLam(),
                        request.trangThai(),
                        request.ghiChu()
                );

        return ResponseEntity
                .status(HttpStatus.CREATED)
                .body(created);
    }

    // =========================
    // ADMIN - UPDATE
    // =========================
    @PutMapping("/admin/technicians/{id}")
    @PreAuthorize("hasRole('ADMIN')")
    public ResponseEntity<?> updateTechnician(
            @PathVariable
            String id,

            @Valid
            @RequestBody
            TechnicianRequest request
    ) {
        return ResponseEntity.ok(
                technicianService.updateTechnician(
                        id,
                        request.hoTen(),
                        request.idCoSo(),
                        request.trinhDo(),
                        request.soDienThoai(),
                        request.email(),
                        request.ngayVaoLam(),
                        request.trangThai(),
                        request.ghiChu()
                )
        );
    }

    // =========================
    // ADMIN - DELETE
    // =========================
    @DeleteMapping("/admin/technicians/{id}")
    @PreAuthorize("hasRole('ADMIN')")
    public ResponseEntity<Void> deleteTechnician(
            @PathVariable
            String id
    ) {
        technicianService.deleteTechnician(id);

        return ResponseEntity
                .noContent()
                .build();
    }

    public record TechnicianRequest(
            @NotBlank(
                    message = "Họ tên kỹ thuật viên không được để trống."
            )
            String hoTen,

            String idCoSo,

            String trinhDo,

            String soDienThoai,

            @Email(
                    message = "Email không hợp lệ."
            )
            String email,

            LocalDate ngayVaoLam,

            String trangThai,

            String ghiChu
    ) {
    }
}