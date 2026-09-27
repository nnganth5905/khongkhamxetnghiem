package com.biomedic.backend.entity;

import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.GenerationType;
import jakarta.persistence.Id;
import jakarta.persistence.Table;
import jakarta.persistence.Transient;

@Entity
@Table(name = "bacsi")
public class Doctor {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "IDBacSi")
    private Integer id; // Khớp DB (AUTO_INCREMENT)

    @Column(name = "IDNhanVien")
    private String employeeId;

    @Column(name = "KhoaID")
    private String specialtyId;

    @Column(name = "CoSoID")
    private String roomId; 

    @Column(name = "HocVi")
    private String degree;

    @Column(name = "ChucDanh")
    private String title;

    @Column(name = "TenBacSi")
    private String name; 

    @Column(name = "HinhAnh")
    private String imageUrl;

    @Column(name = "MoTa")
    private String bio;

    @Column(name = "TrangThai", columnDefinition = "ENUM('active','inactive') DEFAULT 'active'")
    private String status;

    // Sử dụng @Transient để EntityMapper lấy được dữ liệu 
    // nhưng Hibernate sẽ không cố mapping chúng vào database bảng "bacsi"
    @Transient
    private String phone;

    @Transient
    private String email;

    public Doctor() {
    }

    public Integer getId() {
        return id;
    }

    public void setId(Integer id) {
        this.id = id;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public String getEmployeeId() {
        return employeeId;
    }

    public void setEmployeeId(String employeeId) {
        this.employeeId = employeeId;
    }

    public String getSpecialtyId() {
        return specialtyId;
    }

    public void setSpecialtyId(String specialtyId) {
        this.specialtyId = specialtyId;
    }

    public String getRoomId() {
        return roomId;
    }

    public void setRoomId(String roomId) {
        this.roomId = roomId;
    }

    public String getDegree() {
        return degree;
    }

    public void setDegree(String degree) {
        this.degree = degree;
    }

    public String getTitle() {
        return title;
    }

    public void setTitle(String title) {
        this.title = title;
    }

    public String getImageUrl() {
        return imageUrl;
    }

    public void setImageUrl(String imageUrl) {
        this.imageUrl = imageUrl;
    }

    public String getBio() {
        return bio;
    }

    public void setBio(String bio) {
        this.bio = bio;
    }

    public String getStatus() {
        return status;
    }

    public void setStatus(String status) {
        this.status = status;
    }

    public String getPhone() {
        return phone;
    }

    public void setPhone(String phone) {
        this.phone = phone;
    }

    public String getEmail() {
        return email;
    }

    public void setEmail(String email) {
        this.email = email;
    }
}