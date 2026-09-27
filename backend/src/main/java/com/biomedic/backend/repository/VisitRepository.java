package com.biomedic.backend.repository;

import com.biomedic.backend.entity.Visit;

import java.util.List;
import java.util.Optional;

/**
 * Visit là domain model chung.
 * Database hiện có luotkham và luotxetnghiem riêng.
 */
public interface VisitRepository {

    Optional<Visit> findById(
            String id
    );

    Optional<Visit> findByAppointmentId(
            String appointmentId
    );

    List<Visit> findByCustomerId(
            String customerId
    );

    List<Visit> findByDoctorId(
            String doctorId
    );

    List<Visit> findByStatus(
            String status
    );

    List<Visit> findWaiting(
            String type
    );

    Visit save(
            Visit visit
    );

    void deleteById(
            String id
    );
}