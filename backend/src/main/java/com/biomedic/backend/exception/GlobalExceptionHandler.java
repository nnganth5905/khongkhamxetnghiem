package com.biomedic.backend.exception;

import com.biomedic.backend.dto.response.ApiResponse;

import jakarta.validation.ConstraintViolationException;

import java.time.LocalDateTime;
import java.util.LinkedHashMap;
import java.util.Map;

import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.security.access.AccessDeniedException;
import org.springframework.validation.FieldError;
import org.springframework.web.HttpRequestMethodNotSupportedException;
import org.springframework.web.bind.MethodArgumentNotValidException;
import org.springframework.web.bind.MissingServletRequestParameterException;
import org.springframework.web.bind.annotation.ExceptionHandler;
import org.springframework.web.bind.annotation.RestControllerAdvice;
import org.springframework.web.servlet.resource.NoResourceFoundException;

@RestControllerAdvice
public class GlobalExceptionHandler {

    @ExceptionHandler(ResourceNotFoundException.class)
    public ResponseEntity<ApiResponse<Void>> handleResourceNotFound(
            ResourceNotFoundException exception
    ) {
        return ResponseEntity
                .status(HttpStatus.NOT_FOUND)
                .body(ApiResponse.error(exception.getMessage()));
    }

    @ExceptionHandler(BadRequestException.class)
    public ResponseEntity<ApiResponse<Void>> handleBadRequest(
            BadRequestException exception
    ) {
        return ResponseEntity
                .status(HttpStatus.BAD_REQUEST)
                .body(ApiResponse.error(exception.getMessage()));
    }

    @ExceptionHandler(MethodArgumentNotValidException.class)
    public ResponseEntity<ApiResponse<Map<String, String>>> handleValidation(
            MethodArgumentNotValidException exception
    ) {
        Map<String, String> errors = new LinkedHashMap<>();

        for (FieldError fieldError :
                exception.getBindingResult().getFieldErrors()) {
            errors.put(
                    fieldError.getField(),
                    fieldError.getDefaultMessage()
            );
        }

        ApiResponse<Map<String, String>> response =
                new ApiResponse<>(
                        false,
                        "Dữ liệu gửi lên không hợp lệ.",
                        errors,
                        LocalDateTime.now()
                );

        return ResponseEntity
                .badRequest()
                .body(response);
    }

    @ExceptionHandler(ConstraintViolationException.class)
    public ResponseEntity<ApiResponse<Void>> handleConstraintViolation(
            ConstraintViolationException exception
    ) {
        return ResponseEntity
                .badRequest()
                .body(ApiResponse.error(exception.getMessage()));
    }

    @ExceptionHandler(MissingServletRequestParameterException.class)
    public ResponseEntity<ApiResponse<Void>> handleMissingParameter(
            MissingServletRequestParameterException exception
    ) {
        return ResponseEntity
                .badRequest()
                .body(
                        ApiResponse.error(
                                "Thiếu tham số bắt buộc: "
                                        + exception.getParameterName()
                        )
                );
    }

    @ExceptionHandler(HttpRequestMethodNotSupportedException.class)
    public ResponseEntity<ApiResponse<Void>> handleMethodNotSupported(
            HttpRequestMethodNotSupportedException exception
    ) {
        return ResponseEntity
                .status(HttpStatus.METHOD_NOT_ALLOWED)
                .body(
                        ApiResponse.error(
                                "Phương thức HTTP không được hỗ trợ: "
                                        + exception.getMethod()
                        )
                );
    }

    @ExceptionHandler(NoResourceFoundException.class)
    public ResponseEntity<ApiResponse<Void>> handleNoResourceFound(
            NoResourceFoundException exception
    ) {
        return ResponseEntity
                .status(HttpStatus.NOT_FOUND)
                .body(
                        ApiResponse.error(
                                "Không tìm thấy endpoint hoặc tài nguyên: "
                                        + exception.getResourcePath()
                        )
                );
    }

    @ExceptionHandler(AccessDeniedException.class)
    public ResponseEntity<ApiResponse<Void>> handleAccessDenied(
            AccessDeniedException exception
    ) {
        return ResponseEntity
                .status(HttpStatus.FORBIDDEN)
                .body(
                        ApiResponse.error(
                                "Bạn không có quyền thực hiện thao tác này."
                        )
                );
    }

    @ExceptionHandler(IllegalArgumentException.class)
    public ResponseEntity<ApiResponse<Void>> handleIllegalArgument(
            IllegalArgumentException exception
    ) {
        return ResponseEntity
                .badRequest()
                .body(ApiResponse.error(exception.getMessage()));
    }

    @ExceptionHandler(Exception.class)
    public ResponseEntity<ApiResponse<Void>> handleUnexpectedException(
            Exception exception
    ) {
        exception.printStackTrace();

        String message =
                exception.getClass().getSimpleName()
                        + ": "
                        + (
                                exception.getMessage() != null
                                        ? exception.getMessage()
                                        : "Không có thông báo chi tiết."
                        );

        return ResponseEntity
                .status(HttpStatus.INTERNAL_SERVER_ERROR)
                .body(ApiResponse.error(message));
    }
}