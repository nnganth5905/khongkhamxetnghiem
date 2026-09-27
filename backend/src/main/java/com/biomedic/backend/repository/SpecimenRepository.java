package com.biomedic.backend.repository;

import com.biomedic.backend.entity.Specimen;

import java.util.List;
import java.util.Optional;

public interface SpecimenRepository {

    Optional<Specimen> findById(
            String id
    );

    Optional<Specimen> findBySpecimenCode(
            String specimenCode
    );

    Optional<Specimen> findByBarcode(
            String barcode
    );

    List<Specimen> findByTestOrderId(
            String testOrderId
    );

    List<Specimen> findByCustomerId(
            String customerId
    );

    List<Specimen> findByStatus(
            String status
    );

    List<Specimen> findByTechnicianId(
            String technicianId
    );

    Specimen save(
            Specimen specimen
    );

    void deleteById(
            String id
    );
}