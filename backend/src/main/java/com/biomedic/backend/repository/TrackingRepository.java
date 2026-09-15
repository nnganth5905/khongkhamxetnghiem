package com.biomedic.backend.repository;

import com.biomedic.backend.entity.TrackingEvent;

import java.util.List;
import java.util.Optional;

public interface TrackingRepository {

    Optional<TrackingEvent> findById(
            String id
    );

    List<TrackingEvent> findByCustomerId(
            String customerId
    );

    List<TrackingEvent> findByObject(
            String objectType,
            String objectId
    );

    List<TrackingEvent> findByPerformedById(
            String performedById
    );

    TrackingEvent save(
            TrackingEvent event
    );
}