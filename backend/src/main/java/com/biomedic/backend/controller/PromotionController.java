package com.biomedic.backend.controller;

import com.biomedic.backend.service.PromotionService;

import jakarta.validation.Valid;
import jakarta.validation.constraints.NotBlank;

import java.time.LocalDate;

import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.security.access.prepost.PreAuthorize;

import org.springframework.web.bind.annotation.DeleteMapping;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.PutMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping("/api")
public class PromotionController {

    private final PromotionService promotionService;

    public PromotionController(
            PromotionService promotionService
    ) {
        this.promotionService = promotionService;
    }

    @GetMapping("/promotions")
    public ResponseEntity<?> getPromotions(
            @RequestParam(required = false)
            String q
    ) {
        return ResponseEntity.ok(
                promotionService.getPromotions(q)
        );
    }

    @GetMapping("/promotions/{id}")
    public ResponseEntity<?> getPromotionById(
            @PathVariable
            String id
    ) {
        return ResponseEntity.ok(
                promotionService.getPromotionById(id)
        );
    }

    @PostMapping("/admin/promotions")
    @PreAuthorize("hasRole('ADMIN')")
    public ResponseEntity<?> createPromotion(
            @Valid
            @RequestBody
            PromotionRequest request
    ) {
        Object created =
                promotionService.createPromotion(request);

        return ResponseEntity
                .status(HttpStatus.CREATED)
                .body(created);
    }

    @PutMapping("/admin/promotions/{id}")
    @PreAuthorize("hasRole('ADMIN')")
    public ResponseEntity<?> updatePromotion(
            @PathVariable
            String id,

            @Valid
            @RequestBody
            PromotionRequest request
    ) {
        return ResponseEntity.ok(
                promotionService.updatePromotion(
                        id,
                        request
                )
        );
    }

    @DeleteMapping("/admin/promotions/{id}")
    @PreAuthorize("hasRole('ADMIN')")
    public ResponseEntity<Void> deletePromotion(
            @PathVariable
            String id
    ) {
        promotionService.deletePromotion(id);

        return ResponseEntity
                .noContent()
                .build();
    }

    public record PromotionRequest(
            @NotBlank
            String title,

            String summary,
            String content,
            String image,
            LocalDate startDate,
            LocalDate endDate,
            String status
    ) {
    }
}