package com.biomedic.backend.repository;

import com.biomedic.backend.entity.TestAppointment;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.time.LocalDate;
import java.time.LocalTime;
import java.util.List;

@Repository
public interface TestAppointmentRepository extends JpaRepository<TestAppointment, Long> {

    List<TestAppointment> findByCustomerId(String customerId);

    List<TestAppointment> findByTestDate(LocalDate testDate);

    boolean existsByTestDateAndTestTime(LocalDate testDate, LocalTime testTime);

    long countByCustomerIdAndStatusIn(String customerId, List<String> statuses);

    List<TestAppointment> findByCustomerIdAndTestDate(String customerId, LocalDate testDate);

    // 👇 BỔ SUNG THÊM HÀM NÀY ĐỂ ĐỒNG BỘ VỚI DASHBOARD SERVICE
    List<TestAppointment> findByCustomerIdAndStatusIn(String customerId, List<String> statuses);
}