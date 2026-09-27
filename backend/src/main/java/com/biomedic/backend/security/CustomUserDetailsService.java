package com.biomedic.backend.security;

import org.springframework.security.core.userdetails.UserDetails;
import org.springframework.security.core.userdetails.UserDetailsService;
import org.springframework.security.core.userdetails.UsernameNotFoundException;

import org.springframework.stereotype.Service;

/**
 * Bản compile-safe trong giai đoạn entity/repository chưa map JPA thật.
 *
 * JwtAuthenticationFilter hiện xác thực trực tiếp từ JWT đã ký,
 * nên class này chưa được filter gọi.
 *
 * Khi AccountRepository được chuyển sang Spring Data JPA:
 * - inject AccountRepository ở đây
 * - tìm user theo username/email
 * - trả org.springframework.security.core.userdetails.User
 */
@Service
public class CustomUserDetailsService
        implements UserDetailsService {

    @Override
    public UserDetails loadUserByUsername(
            String username
    ) throws UsernameNotFoundException {

        throw new UsernameNotFoundException(
                "AccountRepository chưa được map JPA nên chưa thể tải user từ bảng users: "
                        + username
        );
    }
}