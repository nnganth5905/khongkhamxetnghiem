package com.biomedic.backend.entity;

import java.time.LocalDateTime;

/**
 * Domain model kết quả xét nghiệm. Dự kiến tương ứng bảng ketquaxetnghiem.
 *
 * IMPORTANT: Đây là domain model compile-safe.
 * Chưa gắn @Entity/@Table/@Column để tránh map sai schema MySQL hiện tại.
 */
public class TestResult {

    private String id;
    private String testOrderId;
    private String testOrderItemId;
    private String specimenId;
    private String technicianId;
    private String resultValue;
    private String unit;
    private String referenceRange;
    private String comment;
    private LocalDateTime performedAt;
    private LocalDateTime enteredAt;
    private String approvedByDoctorId;
    private LocalDateTime approvedAt;
    private String conclusion;
    private String advice;
    private String status;

    public TestResult() {
    }

    public String getId() {
        return id;
    }

    public void setId(String id) {
        this.id = id;
    }

    public String getTestOrderId() {
        return testOrderId;
    }

    public void setTestOrderId(String testOrderId) {
        this.testOrderId = testOrderId;
    }

    public String getTestOrderItemId() {
        return testOrderItemId;
    }

    public void setTestOrderItemId(String testOrderItemId) {
        this.testOrderItemId = testOrderItemId;
    }

    public String getSpecimenId() {
        return specimenId;
    }

    public void setSpecimenId(String specimenId) {
        this.specimenId = specimenId;
    }

    public String getTechnicianId() {
        return technicianId;
    }

    public void setTechnicianId(String technicianId) {
        this.technicianId = technicianId;
    }

    public String getResultValue() {
        return resultValue;
    }

    public void setResultValue(String resultValue) {
        this.resultValue = resultValue;
    }

    public String getUnit() {
        return unit;
    }

    public void setUnit(String unit) {
        this.unit = unit;
    }

    public String getReferenceRange() {
        return referenceRange;
    }

    public void setReferenceRange(String referenceRange) {
        this.referenceRange = referenceRange;
    }

    public String getComment() {
        return comment;
    }

    public void setComment(String comment) {
        this.comment = comment;
    }

    public LocalDateTime getPerformedAt() {
        return performedAt;
    }

    public void setPerformedAt(LocalDateTime performedAt) {
        this.performedAt = performedAt;
    }

    public LocalDateTime getEnteredAt() {
        return enteredAt;
    }

    public void setEnteredAt(LocalDateTime enteredAt) {
        this.enteredAt = enteredAt;
    }

    public String getApprovedByDoctorId() {
        return approvedByDoctorId;
    }

    public void setApprovedByDoctorId(String approvedByDoctorId) {
        this.approvedByDoctorId = approvedByDoctorId;
    }

    public LocalDateTime getApprovedAt() {
        return approvedAt;
    }

    public void setApprovedAt(LocalDateTime approvedAt) {
        this.approvedAt = approvedAt;
    }

    public String getConclusion() {
        return conclusion;
    }

    public void setConclusion(String conclusion) {
        this.conclusion = conclusion;
    }

    public String getAdvice() {
        return advice;
    }

    public void setAdvice(String advice) {
        this.advice = advice;
    }

    public String getStatus() {
        return status;
    }

    public void setStatus(String status) {
        this.status = status;
    }

}