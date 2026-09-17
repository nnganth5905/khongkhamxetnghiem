package com.biomedic.backend.service;

import java.util.LinkedHashMap;
import java.util.Map;

import org.springframework.security.core.Authentication;
import org.springframework.stereotype.Service;

@Service
public class AuthService {

    public Map<String, Object> login(
            String identifier,
            String password
    ) {
        Map<String, Object> response =
                new LinkedHashMap<>();

        response.put(
                "message",
                "AuthService đã được kết nối. Chức năng đăng nhập sẽ được nối với bảng users ở bước repository/entity."
        );

        response.put(
                "identifier",
                identifier
        );

        return response;
    }

    public Map<String, Object> register(
            String name,
            String email,
            String phone,
            String gender,
            String password
    ) {
        Map<String, Object> response =
                new LinkedHashMap<>();

        response.put(
                "message",
                "Đăng ký đã nhận dữ liệu. Chưa ghi MySQL vì Entity/Repository chưa được map theo schema thật."
        );

        response.put(
                "name",
                name
        );

        response.put(
                "email",
                email
        );

        response.put(
                "phone",
                phone
        );

        response.put(
                "gender",
                gender
        );

        return response;
    }

    public Map<String, Object> getCurrentUser(
            Authentication authentication
    ) {
        Map<String, Object> response =
                new LinkedHashMap<>();

        if (
            authentication == null ||
            !authentication.isAuthenticated()
        ) {
            response.put(
                    "authenticated",
                    false
            );

            response.put(
                    "message",
                    "Chưa có người dùng đăng nhập."
            );

            return response;
        }

        response.put(
                "authenticated",
                true
        );

        response.put(
                "username",
                authentication.getName()
        );

        response.put(
                "authorities",
                authentication.getAuthorities()
        );

        return response;
    }

    public Map<String, Object> forgotPassword(
            String email
    ) {
        Map<String, Object> response =
                new LinkedHashMap<>();

        response.put(
                "message",
                "Nếu email tồn tại trong hệ thống, hướng dẫn đặt lại mật khẩu sẽ được gửi."
        );

        response.put(
                "email",
                email
        );

        return response;
    }

    public Map<String, Object> verifyResetToken(
            String token,
            String email
    ) {
        Map<String, Object> response =
                new LinkedHashMap<>();

        response.put(
                "valid",
                token != null &&
                !token.isBlank() &&
                email != null &&
                !email.isBlank()
        );

        response.put(
                "email",
                email
        );

        return response;
    }

    public Map<String, Object> resetPassword(
            String token,
            String email,
            String password
    ) {
        Map<String, Object> response =
                new LinkedHashMap<>();

        response.put(
                "message",
                "Yêu cầu đặt lại mật khẩu đã được tiếp nhận."
        );

        response.put(
                "email",
                email
        );

        return response;
    }

    public Map<String, Object> confirmEmail(
            String token
    ) {
        Map<String, Object> response =
                new LinkedHashMap<>();

        response.put(
                "success",
                token != null &&
                !token.isBlank()
        );

        response.put(
                "message",
                "Token xác nhận đã được tiếp nhận."
        );

        return response;
    }
}