package com.biomedic.backend.repository;

import com.biomedic.backend.entity.Customer;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;

import java.util.List;
import java.util.Optional;

@Repository
public interface CustomerRepository extends JpaRepository<Customer, String> {

    Optional<Customer> findByEmail(String email);

    Optional<Customer> findByPhone(String phone);

    @Query("SELECT c FROM Customer c WHERE " +
           "LOWER(c.id) LIKE LOWER(CONCAT('%', :q, '%')) OR " +
           "LOWER(c.fullName) LIKE LOWER(CONCAT('%', :q, '%')) OR " +
           "c.phone LIKE CONCAT('%', :q, '%') OR " +
           "c.citizenId LIKE CONCAT('%', :q, '%') OR " +
           "LOWER(c.email) LIKE LOWER(CONCAT('%', :q, '%'))")
    List<Customer> search(@Param("q") String q);

    @Query(value = "SELECT c.* FROM khachhang c INNER JOIN users u ON c.IDKhachHang = u.IDKhachHang WHERE u.UserID = :accountId", nativeQuery = true)
    Optional<Customer> findByAccountId(@Param("accountId") String accountId);
    
}