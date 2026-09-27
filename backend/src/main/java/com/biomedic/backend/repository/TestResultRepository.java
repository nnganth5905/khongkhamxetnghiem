package com.biomedic.backend.repository;

import com.biomedic.backend.entity.TestResult;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;

import java.util.List;

@Repository
public interface TestResultRepository extends JpaRepository<TestResult, String> {

    List<TestResult> findBySpecimenId(String specimenId);

    List<TestResult> findByTechnicianId(String technicianId);

    List<TestResult> findByStatus(String status);

    @Query("SELECT t FROM TestResult t WHERE t.status = 'chua_duyet'")
    List<TestResult> findPendingDoctorApproval();

    // SỬ DỤNG NATIVE QUERY ĐỂ ĐẾM THEO CUSTOMER_ID QUA BẢNG TRUNG GIAN
    @Query(value = "SELECT COUNT(k.IDKetQua) FROM ketquaxetnghiem k " +
                   "JOIN ctphieuxetnghiem c ON k.IDCTPhieu = c.IDCTPhieu " +
                   "JOIN phieuxetnghiem p ON c.IDPhieuXetNghiem = p.IDPhieuXetNghiem " +
                   "WHERE p.IDKhachHang = :customerId AND k.TrangThai = :status", 
           nativeQuery = true)
    long countByCustomerIdAndStatusNative(@Param("customerId") String customerId, @Param("status") String status);
}