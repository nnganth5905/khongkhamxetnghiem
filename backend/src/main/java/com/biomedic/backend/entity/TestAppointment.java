package com.biomedic.backend.entity;

import java.time.LocalDate;
import java.time.LocalDateTime;
import java.time.LocalTime;

/**
 * Domain model lịch xét nghiệm. Dự kiến tương ứng bảng datlichxetnghiem.
 *
 * IMPORTANT: Đây là domain model compile-safe.
 * Chưa gắn @Entity/@Table/@Column để tránh map sai schema MySQL hiện tại.
 */
public class TestAppointment {

    private String id;
    private String customerId;
    private String doctorId;
    private String facilityId;
    private LocalDate testDate;
    private LocalTime testTime;
    private String status;
    private String qrCode;
    private LocalDateTime bookedAt;
    private boolean forOther;
    private String patientName;
    private String patientPhone;
    private String patientGender;
    private LocalDate patientBirthDate;
    private String patientEmail;
    private String note;

    public TestAppointment() {
    }

    public String getId() {
        return id;
    }

    public void setId(String id) {
        this.id = id;
    }

    public String getCustomerId() {
        return customerId;
    }

    public void setCustomerId(String customerId) {
        this.customerId = customerId;
    }

    public String getDoctorId() {
        return doctorId;
    }

    public void setDoctorId(String doctorId) {
        this.doctorId = doctorId;
    }

    public String getFacilityId() {
        return facilityId;
    }

    public void setFacilityId(String facilityId) {
        this.facilityId = facilityId;
    }

    public LocalDate getTestDate() {
        return testDate;
    }

    public void setTestDate(LocalDate testDate) {
        this.testDate = testDate;
    }

    public LocalTime getTestTime() {
        return testTime;
    }

    public void setTestTime(LocalTime testTime) {
        this.testTime = testTime;
    }

    public String getStatus() {
        return status;
    }

    public void setStatus(String status) {
        this.status = status;
    }

    public String getQrCode() {
        return qrCode;
    }

    public void setQrCode(String qrCode) {
        this.qrCode = qrCode;
    }

    public LocalDateTime getBookedAt() {
        return bookedAt;
    }

    public void setBookedAt(LocalDateTime bookedAt) {
        this.bookedAt = bookedAt;
    }

    public boolean isForOther() {
        return forOther;
    }

    public void setForOther(boolean forOther) {
        this.forOther = forOther;
    }

    public String getPatientName() {
        return patientName;
    }

    public void setPatientName(String patientName) {
        this.patientName = patientName;
    }

    public String getPatientPhone() {
        return patientPhone;
    }

    public void setPatientPhone(String patientPhone) {
        this.patientPhone = patientPhone;
    }

    public String getPatientGender() {
        return patientGender;
    }

    public void setPatientGender(String patientGender) {
        this.patientGender = patientGender;
    }

    public LocalDate getPatientBirthDate() {
        return patientBirthDate;
    }

    public void setPatientBirthDate(LocalDate patientBirthDate) {
        this.patientBirthDate = patientBirthDate;
    }

    public String getPatientEmail() {
        return patientEmail;
    }

    public void setPatientEmail(String patientEmail) {
        this.patientEmail = patientEmail;
    }

    public String getNote() {
        return note;
    }

    public void setNote(String note) {
        this.note = note;
    }

}