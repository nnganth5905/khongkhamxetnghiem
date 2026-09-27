package com.biomedic.backend.repository;

import com.biomedic.backend.entity.Doctor;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.List;

@Repository
public interface DoctorRepository extends JpaRepository<Doctor, Integer> {
    // Tìm các bác sĩ đang hoạt động
    List<Doctor> findByStatus(String status);
}