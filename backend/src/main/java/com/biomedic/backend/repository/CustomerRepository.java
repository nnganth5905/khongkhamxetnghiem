package com.biomedic.backend.repository;

import com.biomedic.backend.entity.Customer;

import java.util.List;
import java.util.Optional;

public interface CustomerRepository {

    Optional<Customer> findById(
            String id
    );

    Optional<Customer> findByAccountId(
            String accountId
    );

    Optional<Customer> findByEmail(
            String email
    );

    Optional<Customer> findByPhone(
            String phone
    );

    List<Customer> findAll();

    List<Customer> search(
            String keyword
    );

    Customer save(
            Customer customer
    );

    void deleteById(
            String id
    );
}