package com.biomedic.backend.controller;

import com.biomedic.backend.service.NewsService;

import org.springframework.http.ResponseEntity;

import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping("/api")
public class NewsController {

    private final NewsService newsService;

    public NewsController(
            NewsService newsService
    ) {
        this.newsService = newsService;
    }

    @GetMapping("/medical-knowledge")
    public ResponseEntity<?> getMedicalArticles(
            @RequestParam(required = false)
            String q
    ) {
        return ResponseEntity.ok(
                newsService.getMedicalArticles(q)
        );
    }

    @GetMapping("/medical-knowledge/{id}")
    public ResponseEntity<?> getMedicalArticle(
            @PathVariable
            String id
    ) {
        return ResponseEntity.ok(
                newsService.getMedicalArticle(id)
        );
    }

    @GetMapping("/handbook")
    public ResponseEntity<?> getHandbook() {
        return ResponseEntity.ok(
                newsService.getHandbook()
        );
    }

    @GetMapping("/social-posts")
    public ResponseEntity<?> getSocialPosts(
            @RequestParam(required = false)
            String q
    ) {
        return ResponseEntity.ok(
                newsService.getSocialPosts(q)
        );
    }

    @GetMapping("/social-posts/{id}")
    public ResponseEntity<?> getSocialPost(
            @PathVariable
            String id
    ) {
        return ResponseEntity.ok(
                newsService.getSocialPost(id)
        );
    }

    @GetMapping("/search")
    public ResponseEntity<?> searchSite(
            @RequestParam
            String q
    ) {
        return ResponseEntity.ok(
                newsService.search(q)
        );
    }
}