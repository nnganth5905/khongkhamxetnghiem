package com.biomedic.backend.repository;

import com.biomedic.backend.entity.TestCatalog;

import java.util.List;
import java.util.Optional;

public interface TestRepository {

    Optional<TestCatalog> findById(
            String id
    );

    List<TestCatalog> findAll();

    List<TestCatalog> findByCategoryId(
            String categoryId
    );

    List<TestCatalog> findByTestType(
            String testType
    );

    List<TestCatalog> search(
            String keyword
    );

    TestCatalog save(
            TestCatalog test
    );

    void deleteById(
            String id
    );
}