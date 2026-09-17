package com.biomedic.backend.controller;

import com.biomedic.backend.service.EmployeeService;

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
@RequestMapping("/api/admin/employees")
@PreAuthorize("hasRole('ADMIN')")
public class EmployeeController {

    private final EmployeeService employeeService;

    public EmployeeController(
            EmployeeService employeeService
    ) {
        this.employeeService = employeeService;
    }

    @GetMapping
    public ResponseEntity<?> getAllEmployees(
            @RequestParam(
                required = false
            )
            String q,

            @RequestParam(
                required = false
            )
            String role
    ) {

        return ResponseEntity.ok(
            employeeService.getAllEmployees(
                q,
                role
            )
        );
    }

    @GetMapping("/{id}")
    public ResponseEntity<?> getEmployeeById(
            @PathVariable
            String id
    ) {

        return ResponseEntity.ok(
            employeeService.getEmployeeById(
                id
            )
        );
    }

    @PostMapping
    public ResponseEntity<?> createEmployee(
            @Valid
            @RequestBody
            EmployeeRequest request
    ) {

        Object created =
            employeeService.createEmployee(
                request.hoTen(),
                request.ngaySinh(),
                request.gioiTinh(),
                request.soDienThoai(),
                request.email(),
                request.idChuyenKhoa(),
                request.idCoSo(),
                request.vaiTro(),
                request.ngayVaoLam(),
                request.trangThai(),
                request.ghiChu()
            );

        return ResponseEntity
            .status(HttpStatus.CREATED)
            .body(created);
    }

    @PutMapping("/{id}")
    public ResponseEntity<?> updateEmployee(
            @PathVariable
            String id,

            @Valid
            @RequestBody
            EmployeeRequest request
    ) {

        return ResponseEntity.ok(
            employeeService.updateEmployee(
                id,
                request.hoTen(),
                request.ngaySinh(),
                request.gioiTinh(),
                request.soDienThoai(),
                request.email(),
                request.idChuyenKhoa(),
                request.idCoSo(),
                request.vaiTro(),
                request.ngayVaoLam(),
                request.trangThai(),
                request.ghiChu()
            )
        );
    }

    @DeleteMapping("/{id}")
    public ResponseEntity<?> deleteEmployee(
            @PathVariable
            String id
    ) {

        employeeService.deleteEmployee(
            id
        );

        return ResponseEntity.noContent()
            .build();
    }

    public record EmployeeRequest(
        @NotBlank
        String hoTen,

        LocalDate ngaySinh,

        String gioiTinh,

        String soDienThoai,

        @Email
        String email,

        String idChuyenKhoa,

        String idCoSo,

        @NotBlank
        String vaiTro,

        LocalDate ngayVaoLam,

        String trangThai,

        String ghiChu
    ) {}
}