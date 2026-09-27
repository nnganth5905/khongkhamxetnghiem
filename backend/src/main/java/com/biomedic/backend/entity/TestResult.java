package com.biomedic.backend.entity;

import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import jakarta.persistence.Table;

import java.time.LocalDateTime;

@Entity
@Table(name = "ketquaxetnghiem")
public class TestResult {

    @Id
    @Column(name = "IDKetQua")
    private String id;
    
    @Column(name = "IDCTPhieu")
    private Long idCtPhieu;

    @Column(name = "IDMau")
    private String specimenId;

    @Column(name = "IDKTV")
    private String technicianId;

    @Column(name = "ThoiGianBatDau")
    private LocalDateTime performedAt;

    @Column(name = "ThoiGianHoanThanh")
    private LocalDateTime completedAt;

    @Column(name = "ThoiGianNhap")
    private LocalDateTime enteredAt;

    @Column(name = "KetQuaTongQuat")
    private String resultValue;

    @Column(name = "TrangThai")
    private String status;

    @Column(name = "IDBacSiDuyet")
    private Integer approvedByDoctorId;

    @Column(name = "ThoiGianDuyet")
    private LocalDateTime approvedAt;

    @Column(name = "KetLuanBacSi")
    private String conclusion;

    @Column(name = "GhiChu")
    private String comment;

    public TestResult() {}

    public String getId() { return id; }
    public void setId(String id) { this.id = id; }

    public Long getIdCtPhieu() { return idCtPhieu; }
    public void setIdCtPhieu(Long idCtPhieu) { this.idCtPhieu = idCtPhieu; }

    public String getSpecimenId() { return specimenId; }
    public void setSpecimenId(String specimenId) { this.specimenId = specimenId; }

    public String getTechnicianId() { return technicianId; }
    public void setTechnicianId(String technicianId) { this.technicianId = technicianId; }

    public LocalDateTime getPerformedAt() { return performedAt; }
    public void setPerformedAt(LocalDateTime performedAt) { this.performedAt = performedAt; }

    public LocalDateTime getCompletedAt() { return completedAt; }
    public void setCompletedAt(LocalDateTime completedAt) { this.completedAt = completedAt; }

    public LocalDateTime getEnteredAt() { return enteredAt; }
    public void setEnteredAt(LocalDateTime enteredAt) { this.enteredAt = enteredAt; }

    public String getResultValue() { return resultValue; }
    public void setResultValue(String resultValue) { this.resultValue = resultValue; }

    public String getStatus() { return status; }
    public void setStatus(String status) { this.status = status; }

    public Integer getApprovedByDoctorId() { return approvedByDoctorId; }
    public void setApprovedByDoctorId(Integer approvedByDoctorId) { this.approvedByDoctorId = approvedByDoctorId; }

    public LocalDateTime getApprovedAt() { return approvedAt; }
    public void setApprovedAt(LocalDateTime approvedAt) { this.approvedAt = approvedAt; }

    public String getConclusion() { return conclusion; }
    public void setConclusion(String conclusion) { this.conclusion = conclusion; }

    public String getComment() { return comment; }
    public void setComment(String comment) { this.comment = comment; }
}