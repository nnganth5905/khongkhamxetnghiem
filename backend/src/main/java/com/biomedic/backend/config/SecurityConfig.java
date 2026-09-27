package com.biomedic.backend.config;

import com.biomedic.backend.security.JwtAuthenticationFilter;

import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.security.authentication.AuthenticationManager;
import org.springframework.security.config.Customizer;
import org.springframework.security.config.annotation.authentication.configuration.AuthenticationConfiguration;
import org.springframework.security.config.annotation.method.configuration.EnableMethodSecurity;
import org.springframework.security.config.annotation.web.builders.HttpSecurity;
import org.springframework.security.config.http.SessionCreationPolicy;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.security.web.SecurityFilterChain;
import org.springframework.security.web.authentication.UsernamePasswordAuthenticationFilter;

@Configuration
@EnableMethodSecurity
public class SecurityConfig {

    private final JwtAuthenticationFilter jwtAuthenticationFilter;

    public SecurityConfig(
            JwtAuthenticationFilter jwtAuthenticationFilter
    ) {
        this.jwtAuthenticationFilter = jwtAuthenticationFilter;
    }

    @Bean
    public PasswordEncoder passwordEncoder() {
        return new BCryptPasswordEncoder();
    }

    @Bean
    public AuthenticationManager authenticationManager(
            AuthenticationConfiguration configuration
    ) throws Exception {
        return configuration.getAuthenticationManager();
    }

    @Bean
    public SecurityFilterChain securityFilterChain(
            HttpSecurity http
    ) throws Exception {

        http
                .csrf(csrf -> csrf.disable())
                .cors(Customizer.withDefaults())
                .sessionManagement(session ->
                        session.sessionCreationPolicy(
                                SessionCreationPolicy.STATELESS
                        )
                )
                .authorizeHttpRequests(auth -> auth
                        .requestMatchers(
                                "/api/health",
                                "/api/auth/**",
                                "/api/login",
                                "/api/register",
                                "/api/forgot-password",
                                "/api/reset-password",
                                "/api/verify-reset-token",
                                "/api/confirm-email",
                                "/api/doctors",
                                "/api/doctors/**",
                                "/api/tests",
                                "/api/tests/**",
                                "/api/appointments/options",
                                "/api/appointments/taken-times",
                                "/api/appointments/quick",
                                "/api/appointments/tests",
                                "/api/appointments/examinations",
                                "/api/results/lookup",
                                "/api/tracking/visits/**",
                                "/api/tracking/tests/**",
                                "/api/promotions",
                                "/api/promotions/**",
                                "/api/medical-knowledge",
                                "/api/medical-knowledge/**",
                                "/api/handbook",
                                "/api/social-posts",
                                "/api/social-posts/**",
                                "/api/search",
                                "/ws",
                                "/ws/**",
                                "/error"
                        )
                        .permitAll()

                        .requestMatchers("/api/admin/**")
                        .hasRole("ADMIN")

                        .requestMatchers("/api/doctor/**")
                        .hasRole("DOCTOR")

                        .requestMatchers("/api/reception/**")
                        .hasRole("RECEPTIONIST")

                        .requestMatchers("/api/technician/**")
                        .hasRole("TECHNICIAN")

                        .requestMatchers(
                                "/api/notifications/**",
                                "/api/appointments/my",
                                "/api/results/mine"
                        )
                        .authenticated()

                        .anyRequest()
                        .permitAll()
                )
                .addFilterBefore(
                        jwtAuthenticationFilter,
                        UsernamePasswordAuthenticationFilter.class
                );

        return http.build();
    }
}