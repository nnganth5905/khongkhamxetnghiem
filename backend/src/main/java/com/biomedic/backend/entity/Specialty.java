package com.biomedic.backend.entity;

/**
 * Domain model chuyên khoa. Dự kiến tương ứng bảng chuyenkhoa.
 *
 * IMPORTANT: Đây là domain model compile-safe.
 * Chưa gắn @Entity/@Table/@Column để tránh map sai schema MySQL hiện tại.
 */

import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import jakarta.persistence.Table;

@Entity
@Table(name = "chuyenkhoa")
public class Specialty {

    @Id
    @Column(name = "IDChuyenKhoa", length = 10)
    private String id;

    @Column(name = "TenChuyenKhoa", length = 128, nullable = false)
    private String name;

    @Column(name = "MoTa", length = 500)
    private String description;

    @Column(name = "Status", columnDefinition = "ENUM('yes','no') DEFAULT 'yes'")
    private String status;

    public Specialty() {}

    // Giữ nguyên các hàm Getter / Setter của bạn ở dưới...
    public String getId() { return id; }
    public void setId(String id) { this.id = id; }
    public String getName() { return name; }
    public void setName(String name) { this.name = name; }
    public String getDescription() { return description; }
    public void setDescription(String description) { this.description = description; }
    public String getStatus() { return status; }
    public void setStatus(String status) { this.status = status; }
}
