package com.biomedic.backend.repository;

import com.biomedic.backend.entity.Notification;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;

import java.util.List;

@Repository
public interface NotificationRepository extends JpaRepository<Notification, Long> {

    List<Notification> findByAccountId(Integer accountId);

    // Đã thay thế n.customerId bằng n.accountId (hoặc bạn có thể dùng native query nếu cần)
    @Query("SELECT n FROM Notification n WHERE n.accountId = :accountId AND n.read = false")
    List<Notification> findUnreadByAccountId(@Param("accountId") Integer accountId);

    // Hàm gọi từ DashboardService lấy số lượng thông báo chưa đọc
    @Query(value = "SELECT COUNT(IDThongBao) FROM thongbao WHERE UserIDNhan = :userId AND DaDoc = 0", nativeQuery = true)
    long countByUserIdNhanAndDaDocFalse(@Param("userId") Integer userId);

    // SỬ DỤNG NATIVE QUERY ĐỂ BỎ QUA LỖI MAP ENTITY
    @Query(value = "SELECT COUNT(IDThongBao) FROM thongbao WHERE UserIDNhan = :userId AND DaDoc = 0", nativeQuery = true)
    long countUnreadNotifications(@Param("userId") Integer userId);
}