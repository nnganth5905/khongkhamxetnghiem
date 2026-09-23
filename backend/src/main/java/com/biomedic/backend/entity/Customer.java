package com.biomedic.backend.entity;

import jakarta.persistence.*;
import java.time.LocalDate;
import java.time.LocalDateTime;

@Entity
@Table(name = "khachhang")
public class Customer {

    @Id
    @Column(name = "IDKhachHang")
    private String id;

    @Column(name = "TenKhachHang", nullable = false)
    private String fullName;

    @Column(name = "NgaySinh")
    private LocalDate birthDate;

    @Column(name = "SoDienThoai")
    private String phone;

    @Column(name = "GioiTinh")
    private String gender;

    @Column(name = "CCCD", unique = true)
    private String citizenId;

    @Column(name = "DiaChi")
    private String address;

    @Column(name = "Email")
    private String email;

    @Column(name = "Status", nullable = false)
    private String status = "yes";

    @Column(name = "CreatedAt", insertable = false, updatable = false)
    private LocalDateTime createdAt;

    @Column(name = "UpdatedAt", insertable = false, updatable = false)
    private LocalDateTime updatedAt;

    // Dùng @Transient để Hibernate bỏ qua cột này vì DB không có cột AccountId, 
    // giúp EntityMapper không bị lỗi khi gọi getAccountId().
    @Transient
    private String accountId;

    public Customer() {
    }

    // --- GETTERS AND SETTERS ---

    public String getId() { return id; }
    public void setId(String id) { this.id = id; }

    public String getFullName() { return fullName; }
    public void setFullName(String fullName) { this.fullName = fullName; }

    public LocalDate getBirthDate() { return birthDate; }
    public void setBirthDate(LocalDate birthDate) { this.birthDate = birthDate; }

    public String getPhone() { return phone; }
    public void setPhone(String phone) { this.phone = phone; }

    public String getGender() { return gender; }
    public void setGender(String gender) { this.gender = gender; }

    public String getCitizenId() { return citizenId; }
    public void setCitizenId(String citizenId) { this.citizenId = citizenId; }

    public String getAddress() { return address; }
    public void setAddress(String address) { this.address = address; }

    public String getEmail() { return email; }
    public void setEmail(String email) { this.email = email; }

    public String getStatus() { return status; }
    public void setStatus(String status) { this.status = status; }

    public LocalDateTime getCreatedAt() { return createdAt; }
    public void setCreatedAt(LocalDateTime createdAt) { this.createdAt = createdAt; }

    public LocalDateTime getUpdatedAt() { return updatedAt; }
    public void setUpdatedAt(LocalDateTime updatedAt) { this.updatedAt = updatedAt; }

    public String getAccountId() { return accountId; }
    public void setAccountId(String accountId) { this.accountId = accountId; }
}