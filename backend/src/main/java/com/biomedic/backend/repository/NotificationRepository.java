package com.biomedic.backend.repository;

import com.biomedic.backend.entity.Notification;

import java.util.List;
import java.util.Optional;

public interface NotificationRepository {

    Optional<Notification> findById(
            String id
    );

    List<Notification> findByCustomerId(
            String customerId
    );

    List<Notification> findByAccountId(
            String accountId
    );

    List<Notification> findUnreadByCustomerId(
            String customerId
    );

    List<Notification> findUnreadByAccountId(
            String accountId
    );

    long countUnreadByCustomerId(
            String customerId
    );

    Notification save(
            Notification notification
    );

    void deleteById(
            String id
    );
}