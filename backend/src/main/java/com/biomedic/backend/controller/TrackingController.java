package com.biomedic.backend.controller;

import com.biomedic.backend.service.TrackingService;

import org.springframework.http.ResponseEntity;
import org.springframework.security.access.prepost.PreAuthorize;

import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping("/api/tracking")
public class TrackingController {

    private final TrackingService trackingService;

    public TrackingController(
            TrackingService trackingService
    ) {
        this.trackingService = trackingService;
    }

    @GetMapping("/visits/{code}")
    public ResponseEntity<?> trackVisit(
            @PathVariable
            String code
    ) {
        return ResponseEntity.ok(
                trackingService.trackVisit(code)
        );
    }

    @GetMapping("/tests/{code}")
    public ResponseEntity<?> trackTest(
            @PathVariable
            String code
    ) {
        return ResponseEntity.ok(
                trackingService.trackTest(code)
        );
    }

    @GetMapping("/history")
    @PreAuthorize("hasRole('ADMIN')")
    public ResponseEntity<?> getHistory(
            @RequestParam(required = false)
            String type,

            @RequestParam(required = false)
            String objectId
    ) {
        return ResponseEntity.ok(
                trackingService.getHistory(
                        type,
                        objectId
                )
        );
    }
}