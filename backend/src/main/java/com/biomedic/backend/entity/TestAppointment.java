package com.biomedic.backend.entity;

import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.GenerationType;
import jakarta.persistence.Id;
import jakarta.persistence.Table;
import jakarta.persistence.Transient;

import java.time.LocalDate;
import java.time.LocalDateTime;
import java.time.LocalTime;

@Entity
@Table(name = "datlichxetnghiem") 
public class TestAppointment {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "IDDatLichXN")
    private Long id; 
    
    @Column(name = "MaDatLich")
    private String maDatLich; 

    @Column(name = "UserID")
    private Integer userId;

    @Column(name = "IDKhachHang")
    private String customerId;

    // ĐÃ SỬA: Đổi từ @Transient sang @Column để ánh xạ xuống Database
    @Column(name = "IDBacSi")
    private Integer doctorId; 

    @Column(name = "CoSoID")
    private String facilityId;

    @Column(name = "NgayXetNghiem")
    private LocalDate testDate;

    @Column(name = "GioXetNghiem")
    private LocalTime testTime;

    @Column(name = "TrangThai")
    private String status;

    @Column(name = "MaQR")
    private String qrCode;

    @Column(name = "CreatedAt", insertable = false, updatable = false)
    private LocalDateTime bookedAt;

    @Column(name = "GhiChu")
    private String note;

    @Transient
    private boolean forOther;
    @Transient
    private String patientName;
    @Transient
    private String patientPhone;
    @Transient
    private String patientGender;
    @Transient
    private LocalDate patientBirthDate;
    @Transient
    private String patientEmail;

    public TestAppointment() {
    }

    // Getters and Setters
    public Long getId() { return id; }
    public void setId(Long id) { this.id = id; }

    public String getMaDatLich() { return maDatLich; }
    public void setMaDatLich(String maDatLich) { this.maDatLich = maDatLich; }

    public Integer getUserId() { return userId; }
    public void setUserId(Integer userId) { this.userId = userId; }

    public String getCustomerId() { return customerId; }
    public void setCustomerId(String customerId) { this.customerId = customerId; }

    // ĐÃ SỬA: Giữ lại duy nhất 1 cặp Getter/Setter cho doctorId
    public Integer getDoctorId() { return doctorId; }
    public void setDoctorId(Integer doctorId) { this.doctorId = doctorId; }

    public String getFacilityId() { return facilityId; }
    public void setFacilityId(String facilityId) { this.facilityId = facilityId; }

    public LocalDate getTestDate() { return testDate; }
    public void setTestDate(LocalDate testDate) { this.testDate = testDate; }

    public LocalTime getTestTime() { return testTime; }
    public void setTestTime(LocalTime testTime) { this.testTime = testTime; }

    public String getStatus() { return status; }
    public void setStatus(String status) { this.status = status; }

    public String getQrCode() { return qrCode; }
    public void setQrCode(String qrCode) { this.qrCode = qrCode; }

    public LocalDateTime getBookedAt() { return bookedAt; }
    public void setBookedAt(LocalDateTime bookedAt) { this.bookedAt = bookedAt; }

    public boolean isForOther() { return forOther; }
    public void setForOther(boolean forOther) { this.forOther = forOther; }

    public String getPatientName() { return patientName; }
    public void setPatientName(String patientName) { this.patientName = patientName; }

    public String getPatientPhone() { return patientPhone; }
    public void setPatientPhone(String patientPhone) { this.patientPhone = patientPhone; }

    public String getPatientGender() { return patientGender; }
    public void setPatientGender(String patientGender) { this.patientGender = patientGender; }

    public LocalDate getPatientBirthDate() { return patientBirthDate; }
    public void setPatientBirthDate(LocalDate patientBirthDate) { this.patientBirthDate = patientBirthDate; }

    public String getPatientEmail() { return patientEmail; }
    public void setPatientEmail(String patientEmail) { this.patientEmail = patientEmail; }

    public String getNote() { return note; }
    public void setNote(String note) { this.note = note; }
}