package com.biomedic.backend.repository;

import com.biomedic.backend.entity.TestCatalog;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;

import java.util.List;

@Repository
public interface TestRepository extends JpaRepository<TestCatalog, String> {

    List<TestCatalog> findByCategoryId(String categoryId);

    List<TestCatalog> findByTestType(String testType);

    @Query("SELECT t FROM TestCatalog t WHERE LOWER(t.name) LIKE LOWER(CONCAT('%', :keyword, '%'))")
    List<TestCatalog> search(@Param("keyword") String keyword);
}