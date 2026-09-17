package com.biomedic.backend.repository;

import com.biomedic.backend.entity.Doctor;

import java.util.List;
import java.util.Optional;

public interface DoctorRepository {

    Optional<Doctor> findById(
            String id
    );

    Optional<Doctor> findByEmployeeId(
            String employeeId
    );

    List<Doctor> findAll();

    List<Doctor> findBySpecialtyId(
            String specialtyId
    );

    List<Doctor> search(
            String specialtyId,
            String keyword
    );

    Doctor save(
            Doctor doctor
    );

    void deleteById(
            String id
    );
}