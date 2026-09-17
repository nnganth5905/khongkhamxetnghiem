package com.biomedic.backend.security;

import com.biomedic.backend.enums.RoleName;
import com.fasterxml.jackson.core.type.TypeReference;
import com.fasterxml.jackson.databind.ObjectMapper;

import java.nio.charset.StandardCharsets;

import java.time.Instant;

import java.util.Base64;
import java.util.LinkedHashMap;
import java.util.Map;

import javax.crypto.Mac;
import javax.crypto.spec.SecretKeySpec;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Service;

@Service
public class JwtService {

    private static final String HMAC_ALGORITHM =
            "HmacSHA256";

    private final ObjectMapper objectMapper;

    private final byte[] secretBytes;

    private final long expirationSeconds;

    public JwtService(
            ObjectMapper objectMapper,

            @Value(
                "${app.jwt.secret:change-this-development-secret-key-please}"
            )
            String secret,

            @Value(
                "${app.jwt.expiration-seconds:86400}"
            )
            long expirationSeconds
    ) {
        this.objectMapper =
                objectMapper;

        this.secretBytes =
                secret.getBytes(
                        StandardCharsets.UTF_8
                );

        this.expirationSeconds =
                expirationSeconds;
    }

    public String generateToken(
            String subject,
            RoleName role
    ) {
        Map<String, Object> claims =
                new LinkedHashMap<>();

        claims.put(
                "sub",
                subject
        );

        claims.put(
                "role",
                role != null
                        ? role.name()
                        : RoleName.CUSTOMER.name()
        );

        return generateToken(claims);
    }

    public String generateToken(
            Map<String, Object> claims
    ) {
        try {
            Instant now =
                    Instant.now();

            Map<String, Object> header =
                    Map.of(
                            "alg",
                            "HS256",
                            "typ",
                            "JWT"
                    );

            Map<String, Object> payload =
                    new LinkedHashMap<>(
                            claims
                    );

            payload.putIfAbsent(
                    "iat",
                    now.getEpochSecond()
            );

            payload.putIfAbsent(
                    "exp",
                    now
                            .plusSeconds(
                                    expirationSeconds
                            )
                            .getEpochSecond()
            );

            String headerPart =
                    base64UrlEncode(
                            objectMapper
                                    .writeValueAsBytes(
                                            header
                                    )
                    );

            String payloadPart =
                    base64UrlEncode(
                            objectMapper
                                    .writeValueAsBytes(
                                            payload
                                    )
                    );

            String unsignedToken =
                    headerPart
                            + "."
                            + payloadPart;

            String signature =
                    sign(unsignedToken);

            return unsignedToken
                    + "."
                    + signature;

        } catch (Exception exception) {
            throw new IllegalStateException(
                    "Không thể tạo JWT.",
                    exception
            );
        }
    }

    public String extractUsername(
            String token
    ) {
        Object subject =
                extractClaims(token)
                        .get("sub");

        return subject != null
                ? subject.toString()
                : null;
    }

    public RoleName extractRole(
            String token
    ) {
        Object role =
                extractClaims(token)
                        .get("role");

        return RoleName.fromValue(
                role != null
                        ? role.toString()
                        : null
        );
    }

    public long getExpirationSeconds() {
        return expirationSeconds;
    }

    public boolean isTokenValid(
            String token
    ) {
        try {
            Map<String, Object> claims =
                    extractClaims(token);

            Object expiration =
                    claims.get("exp");

            if (
                    expiration
                    == null
            ) {
                return false;
            }

            long exp =
                    Long.parseLong(
                            expiration.toString()
                    );

            return Instant.now()
                    .getEpochSecond()
                    < exp;

        } catch (Exception exception) {
            return false;
        }
    }

    public boolean isTokenValid(
            String token,
            String expectedUsername
    ) {
        if (
                !isTokenValid(token)
        ) {
            return false;
        }

        String username =
                extractUsername(token);

        return username != null
                && username.equals(
                        expectedUsername
                );
    }

    public Map<String, Object> extractClaims(
            String token
    ) {
        try {
            String[] parts =
                    token.split("\\.");

            if (parts.length != 3) {
                throw new IllegalArgumentException(
                        "JWT không hợp lệ."
                );
            }

            String unsignedToken =
                    parts[0]
                            + "."
                            + parts[1];

            String expectedSignature =
                    sign(unsignedToken);

            if (
                    !constantTimeEquals(
                            expectedSignature,
                            parts[2]
                    )
            ) {
                throw new IllegalArgumentException(
                        "Chữ ký JWT không hợp lệ."
                );
            }

            byte[] payloadBytes =
                    Base64
                            .getUrlDecoder()
                            .decode(
                                    parts[1]
                            );

            return objectMapper
                    .readValue(
                            payloadBytes,
                            new TypeReference<
                                    Map<String, Object>
                            >() {
                            }
                    );

        } catch (IllegalArgumentException exception) {
            throw exception;

        } catch (Exception exception) {
            throw new IllegalArgumentException(
                    "Không thể đọc JWT.",
                    exception
            );
        }
    }

    private String sign(
            String value
    ) throws Exception {

        Mac mac =
                Mac.getInstance(
                        HMAC_ALGORITHM
                );

        mac.init(
                new SecretKeySpec(
                        secretBytes,
                        HMAC_ALGORITHM
                )
        );

        byte[] signature =
                mac.doFinal(
                        value.getBytes(
                                StandardCharsets.UTF_8
                        )
                );

        return base64UrlEncode(
                signature
        );
    }

    private String base64UrlEncode(
            byte[] data
    ) {
        return Base64
                .getUrlEncoder()
                .withoutPadding()
                .encodeToString(data);
    }

    private boolean constantTimeEquals(
            String left,
            String right
    ) {
        if (
                left == null
                || right == null
        ) {
            return false;
        }

        byte[] leftBytes =
                left.getBytes(
                        StandardCharsets.UTF_8
                );

        byte[] rightBytes =
                right.getBytes(
                        StandardCharsets.UTF_8
                );

        return java.security.MessageDigest
                .isEqual(
                        leftBytes,
                        rightBytes
                );
    }
}