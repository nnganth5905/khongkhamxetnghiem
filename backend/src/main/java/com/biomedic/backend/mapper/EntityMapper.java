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
                account.getId(),
                account.getUsername(),
                account.getEmail(),
                null,
                account.getRole() != null
                        ? account.getRole().name()
                        : null,
                account.getStatus()
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
                doctor.getId(),
                doctor.getEmployeeId(),
                null,
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
                test.getSpecimenType(),
                test.getUnit(),
                test.getReferenceValue(),
                test.getPrice(),
                test.getTurnaroundTime(),
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
                result.getTestOrderId(),
                result.getTestOrderItemId(),
                result.getSpecimenId(),
                result.getTechnicianId(),
                result.getResultValue(),
                result.getUnit(),
                result.getReferenceRange(),
                result.getComment(),
                result.getPerformedAt(),
                result.getEnteredAt(),
                result.getApprovedByDoctorId(),
                result.getApprovedAt(),
                result.getConclusion(),
                result.getAdvice(),
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
                notification.getId(),
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