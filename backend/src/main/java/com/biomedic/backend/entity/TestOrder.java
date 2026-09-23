package com.biomedic.backend.entity;

import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import jakarta.persistence.Table;
import java.math.BigDecimal;
import java.time.LocalDateTime;

@Entity
@Table(name = "phieuxetnghiem")
public class TestOrder {

    @Id
    @Column(name = "IDPhieuXetNghiem")
    private String id;

    @Column(name = "IDKhachHang")
    private String customerId;

    @Column(name = "IDKham")
    private Long examinationId;

    @Column(name = "IDLuotXetNghiem")
    private Long visitId;

    @Column(name = "NgayTao", insertable = false, updatable = false)
    private LocalDateTime orderedAt;

    @Column(name = "TongTien")
    private BigDecimal totalAmount;

    @Column(name = "TrangThai")
    private String status;

    @Column(name = "GhiChu")
    private String note;

    public TestOrder() {}

    // Getters and Setters
    public String getId() { return id; }
    public void setId(String id) { this.id = id; }
    public String getCustomerId() { return customerId; }
    public void setCustomerId(String customerId) { this.customerId = customerId; }
    public Long getExaminationId() { return examinationId; }
    public void setExaminationId(Long examinationId) { this.examinationId = examinationId; }
    public Long getVisitId() { return visitId; }
    public void setVisitId(Long visitId) { this.visitId = visitId; }
    public LocalDateTime getOrderedAt() { return orderedAt; }
    public void setOrderedAt(LocalDateTime orderedAt) { this.orderedAt = orderedAt; }
    public BigDecimal getTotalAmount() { return totalAmount; }
    public void setTotalAmount(BigDecimal totalAmount) { this.totalAmount = totalAmount; }
    public String getStatus() { return status; }
    public void setStatus(String status) { this.status = status; }
    public String getNote() { return note; }
    public void setNote(String note) { this.note = note; }
}