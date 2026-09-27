package com.biomedic.backend.entity;

import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.GenerationType;
import jakarta.persistence.Id;
import jakarta.persistence.Table;
import jakarta.persistence.Transient;

import java.time.LocalDateTime;

@Entity
@Table(name = "thongbao")
public class Notification {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "IDThongBao")
    private Long id; // DB là bigint AUTO_INCREMENT nên đổi sang Long
    
    @Transient
    private String customerId; // Cột này không có trong DB nên phải dùng @Transient để Hibernate bỏ qua

    @Column(name = "UserIDNhan")
    private Integer accountId; // DB là int nên đổi sang Integer

    @Column(name = "LoaiThongBao")
    private String type;

    @Column(name = "TieuDe")
    private String title;

    @Column(name = "NoiDung")
    private String content;

    @Column(name = "IDDoiTuong")
    private String objectId;

    @Column(name = "LoaiDoiTuong")
    private String objectType;

    @Column(name = "ThoiGianTao", insertable = false, updatable = false)
    private LocalDateTime createdAt;
    
    @Column(name = "DaDoc")
    private boolean read;

    @Column(name = "ThoiGianDoc")
    private LocalDateTime readAt; // Bổ sung cột ThoiGianDoc có trong DB

    public Notification() {
    }

    public Long getId() {
        return id;
    }

    public void setId(Long id) {
        this.id = id;
    }

    public String getCustomerId() {
        return customerId;
    }

    public void setCustomerId(String customerId) {
        this.customerId = customerId;
    }

    public Integer getAccountId() {
        return accountId;
    }

    public void setAccountId(Integer accountId) {
        this.accountId = accountId;
    }

    public String getType() {
        return type;
    }

    public void setType(String type) {
        this.type = type;
    }

    public String getTitle() {
        return title;
    }

    public void setTitle(String title) {
        this.title = title;
    }

    public String getContent() {
        return content;
    }

    public void setContent(String content) {
        this.content = content;
    }

    public String getObjectId() {
        return objectId;
    }

    public void setObjectId(String objectId) {
        this.objectId = objectId;
    }

    public String getObjectType() {
        return objectType;
    }

    public void setObjectType(String objectType) {
        this.objectType = objectType;
    }

    public LocalDateTime getCreatedAt() {
        return createdAt;
    }

    public void setCreatedAt(LocalDateTime createdAt) {
        this.createdAt = createdAt;
    }

    public boolean isRead() {
        return read;
    }

    public void setRead(boolean read) {
        this.read = read;
    }

    public LocalDateTime getReadAt() {
        return readAt;
    }

    public void setReadAt(LocalDateTime readAt) {
        this.readAt = readAt;
    }
}