package com.biomedic.backend.repository;

import com.biomedic.backend.entity.Employee;
import com.biomedic.backend.entity.Role;

import java.util.List;
import java.util.Optional;

public interface EmployeeRepository {

    Optional<Employee> findById(
            String id
    );

    Optional<Employee> findByAccountId(
            String accountId
    );

    Optional<Employee> findByEmail(
            String email
    );

    List<Employee> findAll();

    List<Employee> findByRole(
            Role role
    );

    List<Employee> search(
            String keyword,
            Role role
    );

    Employee save(
            Employee employee
    );

    void deleteById(
            String id
    );
}