package com.biomedic.backend.repository;

import com.biomedic.backend.entity.Account;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.Optional;

@Repository
public interface AccountRepository extends JpaRepository<Account, Integer> {

    Optional<Account> findByUsername(String username);

    Optional<Account> findByEmail(String email);

    Optional<Account> findByIdBacSi(Integer idBacSi);

    boolean existsByUsername(String username);

    boolean existsByEmail(String email);
    
}