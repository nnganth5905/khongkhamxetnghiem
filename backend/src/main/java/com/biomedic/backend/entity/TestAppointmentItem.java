package com.biomedic.backend.entity;

import java.math.BigDecimal;

/**
 * Chi tiết xét nghiệm trong một lịch đặt. Dự kiến tương ứng bảng ctdatlichxetnghiem.
 *
 * IMPORTANT: Đây là domain model compile-safe.
 * Chưa gắn @Entity/@Table/@Column để tránh map sai schema MySQL hiện tại.
 */
public class TestAppointmentItem {

    private String id;
    private String testAppointmentId;
    private String testId;
    private BigDecimal price;
    private String status;
    private String note;

    public TestAppointmentItem() {
    }

    public String getId() {
        return id;
    }

    public void setId(String id) {
        this.id = id;
    }

    public String getTestAppointmentId() {
        return testAppointmentId;
    }

    public void setTestAppointmentId(String testAppointmentId) {
        this.testAppointmentId = testAppointmentId;
    }

    public String getTestId() {
        return testId;
    }

    public void setTestId(String testId) {
        this.testId = testId;
    }

    public BigDecimal getPrice() {
        return price;
    }

    public void setPrice(BigDecimal price) {
        this.price = price;
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