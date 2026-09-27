package com.biomedic.backend.config;

import java.util.Properties;

import org.springframework.beans.factory.annotation.Value;

import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

import org.springframework.mail.javamail.JavaMailSender;
import org.springframework.mail.javamail.JavaMailSenderImpl;

@Configuration
public class MailConfig {

    @Value("${spring.mail.host:smtp.gmail.com}")
    private String host;

    @Value("${spring.mail.port:587}")
    private int port;

    @Value("${spring.mail.username:}")
    private String username;

    @Value("${spring.mail.password:}")
    private String password;

    @Value("${spring.mail.properties.mail.smtp.auth:true}")
    private boolean auth;

    @Value("${spring.mail.properties.mail.smtp.starttls.enable:true}")
    private boolean startTls;

    @Bean
    public JavaMailSender javaMailSender() {

        JavaMailSenderImpl mailSender =
                new JavaMailSenderImpl();

        mailSender.setHost(host);
        mailSender.setPort(port);
        mailSender.setUsername(username);
        mailSender.setPassword(password);

        Properties properties =
                mailSender.getJavaMailProperties();

        properties.put(
                "mail.transport.protocol",
                "smtp"
        );

        properties.put(
                "mail.smtp.auth",
                String.valueOf(auth)
        );

        properties.put(
                "mail.smtp.starttls.enable",
                String.valueOf(startTls)
        );

        properties.put(
                "mail.debug",
                "false"
        );

        return mailSender;
    }
}