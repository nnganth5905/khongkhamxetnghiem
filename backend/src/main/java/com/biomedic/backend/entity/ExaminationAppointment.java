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
@Table(name = "datlichkham")
public class ExaminationAppointment {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "IDDatLichKham")
    private Long id; 

    @Column(name = "MaDatLich")
    private String maDatLich; 

    // BỔ SUNG TRƯỜNG UserID
    @Column(name = "UserID")
    private Integer userId;

    @Column(name = "IDKhachHang")
    private String customerId;

    @Column(name = "IDChuyenKhoa")
    private String specialtyId;

    @Column(name = "IDBacSi")
    private Integer doctorId; 

    @Column(name = "CoSoID")
    private String roomId;

    @Column(name = "NgayKham")
    private LocalDate examinationDate;

    @Column(name = "GioKham")
    private LocalTime examinationTime;

    @Column(name = "TrangThai")
    private String status;

    @Column(name = "MaQR")
    private String qrCode;

    @Column(name = "CreatedAt", insertable = false, updatable = false)
    private LocalDateTime bookedAt;

    @Column(name = "GhiChu")
    private String note;

    @Transient
    private Integer queueNumber;
    
    @Transient
    private String reason;

    public ExaminationAppointment() {
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

    public String getSpecialtyId() { return specialtyId; }
    public void setSpecialtyId(String specialtyId) { this.specialtyId = specialtyId; }

    public Integer getDoctorId() { return doctorId; }
    public void setDoctorId(Integer doctorId) { this.doctorId = doctorId; }

    public String getRoomId() { return roomId; }
    public void setRoomId(String roomId) { this.roomId = roomId; }

    public LocalDate getExaminationDate() { return examinationDate; }
    public void setExaminationDate(LocalDate examinationDate) { this.examinationDate = examinationDate; }

    public LocalTime getExaminationTime() { return examinationTime; }
    public void setExaminationTime(LocalTime examinationTime) { this.examinationTime = examinationTime; }

    public Integer getQueueNumber() { return queueNumber; }
    public void setQueueNumber(Integer queueNumber) { this.queueNumber = queueNumber; }

    public String getStatus() { return status; }
    public void setStatus(String status) { this.status = status; }

    public String getQrCode() { return qrCode; }
    public void setQrCode(String qrCode) { this.qrCode = qrCode; }

    public LocalDateTime getBookedAt() { return bookedAt; }
    public void setBookedAt(LocalDateTime bookedAt) { this.bookedAt = bookedAt; }

    public String getReason() { return reason; }
    public void setReason(String reason) { this.reason = reason; }

    public String getNote() { return note; }
    public void setNote(String note) { this.note = note; }
}