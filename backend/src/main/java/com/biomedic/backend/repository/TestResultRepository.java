package com.biomedic.backend.repository;

import com.biomedic.backend.entity.TestResult;

import java.util.List;
import java.util.Optional;

public interface TestResultRepository {

    Optional<TestResult> findById(
            String id
    );

    List<TestResult> findByTestOrderId(
            String testOrderId
    );

    List<TestResult> findBySpecimenId(
            String specimenId
    );

    List<TestResult> findByTechnicianId(
            String technicianId
    );

    List<TestResult> findByStatus(
            String status
    );

    List<TestResult> findPendingDoctorApproval();

    TestResult save(
            TestResult result
    );

    void deleteById(
            String id
    );
}