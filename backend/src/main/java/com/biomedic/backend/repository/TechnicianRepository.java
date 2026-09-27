package com.biomedic.backend.repository;

import com.biomedic.backend.entity.Technician;

import java.util.List;
import java.util.Optional;

/**
 * Database hiện chưa xác nhận có bảng technician riêng.
 * Contract này được giữ để khớp kiến trúc service/controller hiện tại.
 */
public interface TechnicianRepository {

    Optional<Technician> findById(
            String id
    );

    Optional<Technician> findByEmployeeId(
            String employeeId
    );

    List<Technician> findAll();

    List<Technician> search(
            String keyword
    );

    Technician save(
            Technician technician
    );

    void deleteById(
            String id
    );
}