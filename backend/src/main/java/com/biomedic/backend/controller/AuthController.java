package com.biomedic.backend.controller;

import com.biomedic.backend.service.AuthService;

import jakarta.validation.Valid;
import jakarta.validation.constraints.Email;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.Size;

import java.util.Map;

import org.springframework.http.ResponseEntity;
import org.springframework.security.core.Authentication;

import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping("/api")
public class AuthController {

    private final AuthService authService;

    public AuthController(AuthService authService) {
        this.authService = authService;
    }

    // =========================
    // LOGIN
    // =========================
    @PostMapping({
            "/auth/login",
            "/login"
    })
    public ResponseEntity<?> login(
            @Valid @RequestBody LoginRequest request
    ) {
        String identifier = firstNotBlank(
                request.email(),
                request.username(),
                request.login()
        );

        return ResponseEntity.ok(
                authService.login(
                        identifier,
                        request.password()
                )
        );
    }

    // =========================
    // REGISTER
    // =========================
    @PostMapping({
            "/auth/register",
            "/register"
    })
    public ResponseEntity<?> register(
            @Valid @RequestBody RegisterRequest request
    ) {
        return ResponseEntity.ok(
                authService.register(
                        request.name(),
                        request.email(),
                        request.phone(),
                        request.gender(),
                        request.password()
                )
        );
    }

    // =========================
    // CURRENT USER
    // =========================
    @GetMapping("/auth/me")
    public ResponseEntity<?> getCurrentUser(
            Authentication authentication
    ) {
        return ResponseEntity.ok(
                authService.getCurrentUser(authentication)
        );
    }

    // =========================
    // LOGOUT
    // =========================
    @PostMapping("/auth/logout")
    public ResponseEntity<?> logout() {
        return ResponseEntity.ok(
                Map.of(
                        "message",
                        "Đăng xuất thành công."
                )
        );
    }

    // =========================
    // FORGOT PASSWORD
    // =========================
    @PostMapping({
            "/auth/forgot-password",
            "/forgot-password"
    })
    public ResponseEntity<?> forgotPassword(
            @Valid @RequestBody ForgotPasswordRequest request
    ) {
        return ResponseEntity.ok(
                authService.forgotPassword(
                        request.email()
                )
        );
    }

    // =========================
    // VERIFY RESET TOKEN
    // =========================
    @GetMapping({
            "/auth/reset-password/verify",
            "/auth/verify-reset-token",
            "/verify-reset-token"
    })
    public ResponseEntity<?> verifyResetToken(
            @RequestParam String token,
            @RequestParam String email
    ) {
        return ResponseEntity.ok(
                authService.verifyResetToken(
                        token,
                        email
                )
        );
    }

    // =========================
    // RESET PASSWORD
    // =========================
    @PostMapping({
            "/auth/reset-password",
            "/reset-password"
    })
    public ResponseEntity<?> resetPassword(
            @Valid @RequestBody ResetPasswordRequest request
    ) {
        return ResponseEntity.ok(
                authService.resetPassword(
                        request.token(),
                        request.email(),
                        request.password()
                )
        );
    }

    // =========================
    // CONFIRM EMAIL
    // =========================
    @GetMapping({
            "/auth/confirm-email",
            "/confirm-email"
    })
    public ResponseEntity<?> confirmEmail(
            @RequestParam String token
    ) {
        return ResponseEntity.ok(
                authService.confirmEmail(token)
        );
    }

    private String firstNotBlank(
            String... values
    ) {
        if (values == null) {
            return "";
        }

        for (String value : values) {
            if (value != null && !value.isBlank()) {
                return value.trim();
            }
        }

        return "";
    }

    public record LoginRequest(
            String login,
            String email,
            String username,

            @NotBlank(
                    message = "Mật khẩu không được để trống."
            )
            String password
    ) {
    }

    public record RegisterRequest(
            @NotBlank(
                    message = "Họ tên không được để trống."
            )
            String name,

            @NotBlank(
                    message = "Email không được để trống."
            )
            @Email(
                    message = "Email không hợp lệ."
            )
            String email,

            String phone,
            String gender,

            @NotBlank(
                    message = "Mật khẩu không được để trống."
            )
            @Size(
                    min = 6,
                    message = "Mật khẩu phải có ít nhất 6 ký tự."
            )
            String password
    ) {
    }

    public record ForgotPasswordRequest(
            @NotBlank(
                    message = "Email không được để trống."
            )
            @Email(
                    message = "Email không hợp lệ."
            )
            String email
    ) {
    }

    public record ResetPasswordRequest(
            @NotBlank(
                    message = "Token không được để trống."
            )
            String token,

            @NotBlank(
                    message = "Email không được để trống."
            )
            @Email(
                    message = "Email không hợp lệ."
            )
            String email,

            @NotBlank(
                    message = "Mật khẩu mới không được để trống."
            )
            @Size(
                    min = 6,
                    message = "Mật khẩu mới phải có ít nhất 6 ký tự."
            )
            String password
    ) {
    }
}