package com.biomedic.backend.repository;

import com.biomedic.backend.entity.TestAppointmentItem;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

@Repository
public interface TestAppointmentItemRepository extends JpaRepository<TestAppointmentItem, Long> {
}