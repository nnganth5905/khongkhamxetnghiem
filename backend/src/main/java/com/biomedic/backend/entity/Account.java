package com.biomedic.backend.entity;

import jakarta.persistence.*;
import java.time.LocalDateTime;

@Entity
@Table(name = "users")
public class Account {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "UserID")
    private Integer userId;

    @Column(name = "Username", unique = true)
    private String username;

    @Column(name = "Email", unique = true, nullable = false)
    private String email;

    @Column(name = "PasswordHash", nullable = false)
    private String passwordHash;

    // Map trực tiếp chuỗi ENUM từ DB: 'khachhang','bacsi','letan','ktv','admin'
    @Column(name = "Role", nullable = false)
    private String role;

    @Column(name = "IDKhachHang")
    private String idKhachHang;

    @Column(name = "IDBacSi")
    private Integer idBacSi;

    @Column(name = "IDNhanVien")
    private String idNhanVien;

    @Column(name = "IsActive", nullable = false)
    private Boolean isActive = true;

    @Column(name = "CreatedAt", insertable = false, updatable = false)
    private LocalDateTime createdAt;

    @Column(name = "UpdatedAt", insertable = false, updatable = false)
    private LocalDateTime updatedAt;

    public Account() {
    }

    // --- GETTERS AND SETTERS ---

    public Integer getUserId() { return userId; }
    public void setUserId(Integer userId) { this.userId = userId; }

    public String getUsername() { return username; }
    public void setUsername(String username) { this.username = username; }

    public String getEmail() { return email; }
    public void setEmail(String email) { this.email = email; }

    public String getPasswordHash() { return passwordHash; }
    public void setPasswordHash(String passwordHash) { this.passwordHash = passwordHash; }

    public String getRole() { return role; }
    public void setRole(String role) { this.role = role; }

    public String getIdKhachHang() { return idKhachHang; }
    public void setIdKhachHang(String idKhachHang) { this.idKhachHang = idKhachHang; }

    public Integer getIdBacSi() { return idBacSi; }
    public void setIdBacSi(Integer idBacSi) { this.idBacSi = idBacSi; }

    public String getIdNhanVien() { return idNhanVien; }
    public void setIdNhanVien(String idNhanVien) { this.idNhanVien = idNhanVien; }

    public Boolean getIsActive() { return isActive; }
    public void setIsActive(Boolean isActive) { this.isActive = isActive; }

    public LocalDateTime getCreatedAt() { return createdAt; }
    public void setCreatedAt(LocalDateTime createdAt) { this.createdAt = createdAt; }

    public LocalDateTime getUpdatedAt() { return updatedAt; }
    public void setUpdatedAt(LocalDateTime updatedAt) { this.updatedAt = updatedAt; }
}