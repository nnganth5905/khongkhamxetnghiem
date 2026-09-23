package com.biomedic.backend.repository;

import com.biomedic.backend.entity.Specialty;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.List;

@Repository
public interface SpecialtyRepository extends JpaRepository<Specialty, String> {
    // Chỉ lấy ra các chuyên khoa đang hoạt động
    List<Specialty> findByStatus(String status);
}