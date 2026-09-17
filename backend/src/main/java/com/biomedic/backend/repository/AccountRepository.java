package com.biomedic.backend.repository;

import com.biomedic.backend.entity.Account;

import java.util.List;
import java.util.Optional;

/**
 * Compile-safe repository contract cho Account.
 *
 * Chưa extends JpaRepository vì Account hiện chưa được map @Entity
 * theo schema thật của bảng users.
 */
public interface AccountRepository {

    Optional<Account> findById(
            String id
    );

    Optional<Account> findByUsername(
            String username
    );

    Optional<Account> findByEmail(
            String email
    );

    List<Account> findAll();

    boolean existsByUsername(
            String username
    );

    boolean existsByEmail(
            String email
    );

    Account save(
            Account account
    );

    void deleteById(
            String id
    );
}