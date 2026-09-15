package com.biomedic.backend.entity;

import java.time.LocalDateTime;

/**
 * Domain model mẫu bệnh phẩm. Dự kiến tương ứng bảng maubenhpham.
 *
 * IMPORTANT: Đây là domain model compile-safe.
 * Chưa gắn @Entity/@Table/@Column để tránh map sai schema MySQL hiện tại.
 */
public class Specimen {

    private String id;
    private String testOrderId;
    private String customerId;
    private String visitId;
    private String specimenType;
    private String specimenCode;
    private String barcode;
    private LocalDateTime collectedAt;
    private String collectorId;
    private LocalDateTime receivedAt;
    private String technicianId;
    private String status;
    private String note;

    public Specimen() {
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

    public String getCustomerId() {
        return customerId;
    }

    public void setCustomerId(String customerId) {
        this.customerId = customerId;
    }

    public String getVisitId() {
        return visitId;
    }

    public void setVisitId(String visitId) {
        this.visitId = visitId;
    }

    public String getSpecimenType() {
        return specimenType;
    }

    public void setSpecimenType(String specimenType) {
        this.specimenType = specimenType;
    }

    public String getSpecimenCode() {
        return specimenCode;
    }

    public void setSpecimenCode(String specimenCode) {
        this.specimenCode = specimenCode;
    }

    public String getBarcode() {
        return barcode;
    }

    public void setBarcode(String barcode) {
        this.barcode = barcode;
    }

    public LocalDateTime getCollectedAt() {
        return collectedAt;
    }

    public void setCollectedAt(LocalDateTime collectedAt) {
        this.collectedAt = collectedAt;
    }

    public String getCollectorId() {
        return collectorId;
    }

    public void setCollectorId(String collectorId) {
        this.collectorId = collectorId;
    }

    public LocalDateTime getReceivedAt() {
        return receivedAt;
    }

    public void setReceivedAt(LocalDateTime receivedAt) {
        this.receivedAt = receivedAt;
    }

    public String getTechnicianId() {
        return technicianId;
    }

    public void setTechnicianId(String technicianId) {
        this.technicianId = technicianId;
    }

    public String getStatus() {
        return status;
    }

    public void setStatus(String status) {
        this.status = status;
    }

    public String getNote() {
        return note;
    }

    public void setNote(String note) {
        this.note = note;
    }

}