package com.biomedic.backend.entity;

import jakarta.persistence.*;
import java.math.BigDecimal;

@Entity
@Table(name = "ctdatlichxetnghiem") // Map chuẩn với bảng trong cơ sở dữ liệu
public class TestAppointmentItem {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "IDCTDatLichXN")
    private Long id;

    @Column(name = "IDDatLichXN", nullable = false)
    private Long testAppointmentId;

    @Column(name = "IDXetNghiem", nullable = false)
    private String testId;

    @Column(name = "DonGia", nullable = false)
    private BigDecimal price = BigDecimal.ZERO;

    @Column(name = "GhiChu")
    private String note;

    // Cột status không có trong bảng ctdatlichxetnghiem, dùng Transient để tránh lỗi
    @Transient
    private String status;

    public TestAppointmentItem() {
    }

    // --- GETTERS AND SETTERS ---
    public Long getId() { return id; }
    public void setId(Long id) { this.id = id; }

    public Long getTestAppointmentId() { return testAppointmentId; }
    public void setTestAppointmentId(Long testAppointmentId) { this.testAppointmentId = testAppointmentId; }

    public String getTestId() { return testId; }
    public void setTestId(String testId) { this.testId = testId; }

    public BigDecimal getPrice() { return price; }
    public void setPrice(BigDecimal price) { this.price = price; }

    public String getNote() { return note; }
    public void setNote(String note) { this.note = note; }

    public String getStatus() { return status; }
    public void setStatus(String status) { this.status = status; }
}