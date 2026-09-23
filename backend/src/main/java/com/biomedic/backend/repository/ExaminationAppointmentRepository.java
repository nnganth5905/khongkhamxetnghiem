package com.biomedic.backend.repository;

import com.biomedic.backend.entity.ExaminationAppointment;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.time.LocalDate;
import java.time.LocalTime;
import java.util.List;

@Repository
public interface ExaminationAppointmentRepository extends JpaRepository<ExaminationAppointment, Long> {

    List<ExaminationAppointment> findByCustomerId(String customerId);

    List<ExaminationAppointment> findByDoctorIdAndExaminationDate(Integer doctorId, LocalDate examinationDate);

    boolean existsByDoctorIdAndExaminationDateAndExaminationTime(Integer doctorId, LocalDate examinationDate, LocalTime examinationTime); 

    long countByCustomerIdAndStatusIn(String customerId, List<String> statuses);

    // BỔ SUNG: Hàm tìm danh sách lịch khám của một khách hàng trong 1 ngày cụ thể
    List<ExaminationAppointment> findByCustomerIdAndExaminationDate(String customerId, LocalDate examinationDate);
}