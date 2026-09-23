package com.biomedic.backend.mapper;

import com.biomedic.backend.dto.response.CustomerResponse;
import com.biomedic.backend.dto.response.DoctorResponse;
import com.biomedic.backend.dto.response.NotificationResponse;
import com.biomedic.backend.dto.response.SpecimenResponse;
import com.biomedic.backend.dto.response.TestResponse;
import com.biomedic.backend.dto.response.TestResultResponse;
import com.biomedic.backend.dto.response.UserResponse;
import com.biomedic.backend.dto.response.VisitResponse;

import com.biomedic.backend.entity.Account;
import com.biomedic.backend.entity.Customer;
import com.biomedic.backend.entity.Doctor;
import com.biomedic.backend.entity.Notification;
import com.biomedic.backend.entity.Specimen;
import com.biomedic.backend.entity.TestCatalog;
import com.biomedic.backend.entity.TestResult;
import com.biomedic.backend.entity.Visit;

public final class EntityMapper {

    private EntityMapper() {
    }

    public static UserResponse toUserResponse(
            Account account
    ) {
        if (account == null) {
            return null;
        }

        return new UserResponse(
                // Ép kiểu Integer userId sang String để không lỗi DTO cũ
                account.getUserId() != null ? String.valueOf(account.getUserId()) : null,
                account.getUsername(),
                account.getEmail(),
                null,
                // Role bây giờ là String nên không cần gọi .name()
                account.getRole(),
                // Chuyển đổi IsActive (Boolean) sang Status (String)
                account.getIsActive() != null && account.getIsActive() ? "active" : "inactive"
        );
    }

    public static CustomerResponse toCustomerResponse(
            Customer customer
    ) {
        if (customer == null) {
            return null;
        }

        return new CustomerResponse(
                customer.getId(),
                customer.getAccountId(),
                customer.getFullName(),
                customer.getBirthDate(),
                customer.getGender(),
                customer.getPhone(),
                customer.getCitizenId(),
                customer.getAddress(),
                customer.getEmail(),
                customer.getStatus()
        );
    }

    public static DoctorResponse toDoctorResponse(
            Doctor doctor
    ) {
        if (doctor == null) {
            return null;
        }

        return new DoctorResponse(
                // Chuyển Integer id sang String
                doctor.getId() != null ? String.valueOf(doctor.getId()) : null,
                doctor.getEmployeeId(),
                doctor.getName(), // Đã sửa từ null thành doctor.getName() ở bước trước
                doctor.getSpecialtyId(),
                null,
                doctor.getRoomId(),
                doctor.getDegree(),
                doctor.getTitle(),
                doctor.getPhone(),
                doctor.getEmail(),
                doctor.getImageUrl(),
                doctor.getBio(),
                doctor.getStatus()
        );
    }

    public static VisitResponse toVisitResponse(
            Visit visit
    ) {
        if (visit == null) {
            return null;
        }

        return new VisitResponse(
                visit.getId(),
                visit.getAppointmentId(),
                visit.getCustomerId(),
                visit.getDoctorId(),
                visit.getRoomId(),
                visit.getType(),
                visit.getQueueNumber(),
                visit.getStatus(),
                visit.getReceivedAt(),
                visit.getStartedAt(),
                visit.getEndedAt(),
                visit.getNote()
        );
    }

    public static TestResponse toTestResponse(
            TestCatalog test
    ) {
        if (test == null) {
            return null;
        }

        return new TestResponse(
                test.getId(),
                test.getName(),
                test.getDescription(),
                test.getCategoryId(),
                test.getTestType(),
                test.getDefaultSampleType(),
                test.getPrice(),
                test.getEstimatedTimeMinutes(),
                test.getStatus()
        );
    }

    public static SpecimenResponse toSpecimenResponse(
            Specimen specimen
    ) {
        if (specimen == null) {
            return null;
        }

        return new SpecimenResponse(
                specimen.getId(),
                specimen.getTestOrderId(),
                specimen.getCustomerId(),
                specimen.getSpecimenType(),
                specimen.getSpecimenCode(),
                specimen.getBarcode(),
                specimen.getCollectedAt(),
                specimen.getCollectorId(),
                specimen.getReceivedAt(),
                specimen.getTechnicianId(),
                specimen.getStatus(),
                specimen.getNote()
        );
    }

    public static TestResultResponse toTestResultResponse(
            TestResult result
    ) {
        if (result == null) {
            return null;
        }

        return new TestResultResponse(
                result.getId(),
                result.getIdCtPhieu(),
                result.getSpecimenId(),
                result.getTechnicianId(),
                result.getResultValue(),
                result.getComment(),
                result.getPerformedAt(),
                result.getCompletedAt(),
                result.getEnteredAt(),
                result.getApprovedByDoctorId(),
                result.getApprovedAt(),
                result.getConclusion(),
                result.getStatus()
        );
    }

    public static NotificationResponse toNotificationResponse(
            Notification notification
    ) {
        if (notification == null) {
            return null;
        }

        return new NotificationResponse(
                String.valueOf(notification.getId()), // ĐÃ SỬA: Ép kiểu Long sang String
                notification.getType(),
                notification.getTitle(),
                notification.getContent(),
                notification.getObjectId(),
                notification.getObjectType(),
                notification.getCreatedAt(),
                notification.isRead()
        );
    }
}