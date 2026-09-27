package com.biomedic.backend.controller;

import com.biomedic.backend.service.AppointmentService;

import jakarta.validation.Valid;
import jakarta.validation.constraints.NotBlank;

import java.time.LocalDate;

import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.security.core.Authentication;

import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.PutMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping("/api/appointments")
public class AppointmentController {

    private final AppointmentService appointmentService;

    public AppointmentController(
            AppointmentService appointmentService
    ) {
        this.appointmentService = appointmentService;
    }

    @GetMapping("/options")
    public ResponseEntity<?> getAppointmentOptions() {
        return ResponseEntity.ok(
                appointmentService.getAppointmentOptions()
        );
    }

    @GetMapping("/taken-times")
    public ResponseEntity<?> getTakenTimes(
            @RequestParam
            String type,

            @RequestParam
            String doctorId,

            @RequestParam
            LocalDate date
    ) {
        return ResponseEntity.ok(
                appointmentService.getTakenTimes(
                        type,
                        doctorId,
                        date
                )
        );
    }

    @PostMapping("/quick")
    public ResponseEntity<?> createQuickAppointment(
            @Valid
            @RequestBody
            QuickAppointmentRequest request
    ) {
        Object created =
                appointmentService.createQuickAppointment(
                        request.hoten(),
                        request.email(),
                        request.ngay(),
                        request.gio(),
                        request.idbacsi(),
                        request.ghichu()
                );

        return ResponseEntity
                .status(HttpStatus.CREATED)
                .body(created);
    }

    @PostMapping("/tests")
    public ResponseEntity<?> createTestAppointment(
            @RequestBody
            TestAppointmentRequest request,
            Authentication authentication
    ) {
        Object created =
                appointmentService.createTestAppointment(
                        request,
                        authentication
                );

        return ResponseEntity
                .status(HttpStatus.CREATED)
                .body(created);
    }

    @PostMapping("/examinations")
    public ResponseEntity<?> createExaminationAppointment(
            @RequestBody
            ExaminationAppointmentRequest request,
            Authentication authentication
    ) {
        Object created =
                appointmentService.createExaminationAppointment(
                        request,
                        authentication
                );

        return ResponseEntity
                .status(HttpStatus.CREATED)
                .body(created);
    }

    // ĐÃ CHỈNH SỬA: Giữ lại duy nhất 1 endpoint POST /walk-in
    @PostMapping("/walk-in")
    public ResponseEntity<?> createWalkInVisit(
            @Valid
            @RequestBody
            WalkInAppointmentRequest request,
            Authentication authentication
    ) {
        Object created = appointmentService.createWalkInVisit(request, authentication);

        return ResponseEntity
                .status(HttpStatus.CREATED)
                .body(created);
    }

    @GetMapping("/my")
    public ResponseEntity<?> getMyAppointments(
            Authentication authentication
    ) {
        return ResponseEntity.ok(
                appointmentService.getMyAppointments(
                        authentication
                )
        );
    }

    @GetMapping
    public ResponseEntity<?> getAllAppointments(@RequestParam(required = false) LocalDate date) {
        if (date == null) date = LocalDate.now();
        return ResponseEntity.ok(appointmentService.getAllAppointments(date));
    }

    @PostMapping("/{id}/check-in")
    public ResponseEntity<?> checkInAppointment(@PathVariable String id, @RequestBody java.util.Map<String, String> payload) {
        String type = payload.get("type"); 
        return ResponseEntity.ok(appointmentService.checkInAppointmentAdmin(id, type));
    }

    @GetMapping("/waiting-queue")
    public ResponseEntity<?> getWaitingQueue(
            @RequestParam(required = false) String type,
            Authentication authentication
    ) {
        return ResponseEntity.ok(appointmentService.getWaitingQueue(type, authentication));
    }

    @PostMapping("/queue/{id}/call")
    public ResponseEntity<?> callPatientQueue(@PathVariable String id) {
        appointmentService.callPatientQueue(id);
        return ResponseEntity.ok(java.util.Map.of("success", true, "message", "Đã gọi bệnh nhân"));
    }

    @PostMapping("/queue/{id}/hold")
    public ResponseEntity<?> holdPatientQueue(@PathVariable String id) {
        appointmentService.holdPatientQueue(id);
        return ResponseEntity.ok(java.util.Map.of("success", true, "message", "Đã chuyển vào danh sách gọi lại sau"));
    }

    @PostMapping("/queue/{id}/skip")
    public ResponseEntity<?> skipPatientQueue(@PathVariable String id) {
        appointmentService.skipPatientQueue(id);
        return ResponseEntity.ok(java.util.Map.of("success", true, "message", "Bệnh nhân bỏ lượt"));
    }


    @GetMapping("/{id}")
    public ResponseEntity<?> getAppointmentDetail(@PathVariable String id) {
        return ResponseEntity.ok(appointmentService.getAppointmentDetail(id));
    }

    @PutMapping("/{id}")
    public ResponseEntity<?> updateAppointment(
            @PathVariable
            String id,

            @RequestBody
            UpdateAppointmentRequest request
    ) {
        return ResponseEntity.ok(
                appointmentService.updateAppointment(
                        id,
                        request
                )
        );
    }

    @PostMapping("/{id}/cancel")
    public ResponseEntity<?> cancelAppointment(
            @PathVariable
            String id
    ) {
        return ResponseEntity.ok(
                appointmentService.cancelAppointment(
                        id
                )
        );
    }

    public record QuickAppointmentRequest(
            @NotBlank
            String hoten,

            String email,

            @NotBlank
            String ngay,

            @NotBlank
            String gio,

            String idbacsi,

            String ghichu
    ) {
    }

    public record TestAppointmentRequest(
            String hoten,
            String email,
            String sodienthoai,
            String gioitinh, 
            String ngaysinh, 
            String ngay,
            String gio,
            String idbacsi,
            String idxetnghiem,
            String idcoso,
            String ghichu
    ) {
    }

    public record ExaminationAppointmentRequest(
            String hoten,
            String email,
            String sodienthoai,
            String gioitinh, 
            String ngaysinh, 
            String ngay,
            String gio,
            String idbacsi,
            String idchuyenkhoa,
            String idcoso,
            String lydokham,
            String ghichu
    ) {
    }

    public record UpdateAppointmentRequest(
            String new_date,
            String new_time,
            String new_bs,
            String ghichu
    ) {
    }

    // ĐÃ CHỈNH SỬA: Giữ lại duy nhất 1 Record cấu trúc WalkIn
    public record WalkInAppointmentRequest(
            @NotBlank(message = "Họ và tên không được để trống")
            String fullName,

            @NotBlank(message = "Số điện thoại không được để trống")
            String phone,

            String email,
            String dateOfBirth,
            String gender,
            String address,
            String serviceType,
            String specialtyId,
            String reason,
            String notes
    ) {
    }
}