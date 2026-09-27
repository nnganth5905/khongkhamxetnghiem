package com.biomedic.backend.entity;

import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import jakarta.persistence.Table;
import java.math.BigDecimal;

@Entity
@Table(name = "loaixetnghiem")
public class TestCatalog {

    @Id
    @Column(name = "IDXetNghiem")
    private String id;

    @Column(name = "TenXetNghiem")
    private String name;

    @Column(name = "ChuyenKhoaID")
    private String categoryId;

    @Column(name = "Loai")
    private String testType;

    @Column(name = "MoTa")
    private String description;

    @Column(name = "Gia")
    private BigDecimal price;

    @Column(name = "LoaiMauMacDinh")
    private String defaultSampleType;

    @Column(name = "ThoiGianDuKienPhut")
    private Integer estimatedTimeMinutes;

    @Column(name = "Status")
    private String status = "yes";

    public TestCatalog() {
    }

    // --- GETTERS AND SETTERS ---

    public String getId() { return id; }
    public void setId(String id) { this.id = id; }

    public String getName() { return name; }
    public void setName(String name) { this.name = name; }

    public String getCategoryId() { return categoryId; }
    public void setCategoryId(String categoryId) { this.categoryId = categoryId; }

    public String getTestType() { return testType; }
    public void setTestType(String testType) { this.testType = testType; }

    public String getDescription() { return description; }
    public void setDescription(String description) { this.description = description; }

    public BigDecimal getPrice() { return price; }
    public void setPrice(BigDecimal price) { this.price = price; }

    public String getDefaultSampleType() { return defaultSampleType; }
    public void setDefaultSampleType(String defaultSampleType) { this.defaultSampleType = defaultSampleType; }

    public Integer getEstimatedTimeMinutes() { return estimatedTimeMinutes; }
    public void setEstimatedTimeMinutes(Integer estimatedTimeMinutes) { this.estimatedTimeMinutes = estimatedTimeMinutes; }

    public String getStatus() { return status; }
    public void setStatus(String status) { this.status = status; }
}