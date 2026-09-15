package com.biomedic.backend.repository;

import com.biomedic.backend.entity.ExaminationAppointment;
import com.biomedic.backend.entity.TestAppointment;

import java.time.LocalDate;
import java.time.LocalTime;
import java.util.List;
import java.util.Optional;

/**
 * Database thật có datlichkham và datlichxetnghiem riêng,
 * nên repository contract này xử lý cả hai loại lịch hẹn.
 */
public interface AppointmentRepository {

    Optional<ExaminationAppointment> findExaminationById(
            String id
    );

    Optional<TestAppointment> findTestById(
            String id
    );

    List<ExaminationAppointment> findExaminationsByCustomerId(
            String customerId
    );

    List<TestAppointment> findTestsByCustomerId(
            String customerId
    );

    List<ExaminationAppointment> findExaminationsByDoctorAndDate(
            String doctorId,
            LocalDate date
    );

    List<TestAppointment> findTestsByDoctorAndDate(
            String doctorId,
            LocalDate date
    );

    boolean existsExaminationSlot(
            String doctorId,
            LocalDate date,
            LocalTime time
    );

    boolean existsTestSlot(
            String doctorId,
            LocalDate date,
            LocalTime time
    );

    ExaminationAppointment saveExamination(
            ExaminationAppointment appointment
    );

    TestAppointment saveTest(
            TestAppointment appointment
    );

    void deleteExaminationById(
            String id
    );

    void deleteTestById(
            String id
    );
}