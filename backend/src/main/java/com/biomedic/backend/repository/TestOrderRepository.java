package com.biomedic.backend.repository;

import com.biomedic.backend.entity.TestOrder;

import java.util.List;
import java.util.Optional;

public interface TestOrderRepository {

    Optional<TestOrder> findById(
            String id
    );

    List<TestOrder> findByVisitId(
            String visitId
    );

    List<TestOrder> findByCustomerId(
            String customerId
    );

    List<TestOrder> findByDoctorId(
            String doctorId
    );

    List<TestOrder> findByStatus(
            String status
    );

    TestOrder save(
            TestOrder testOrder
    );

    void deleteById(
            String id
    );
}