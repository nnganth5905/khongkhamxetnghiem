package com.biomedic.backend.service;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.mail.SimpleMailMessage;
import org.springframework.mail.javamail.JavaMailSender;
import org.springframework.stereotype.Service;

@Service
public class MailService {

    private final JavaMailSender mailSender;

    @Value("${spring.mail.username:no-reply@biomedic.local}")
    private String fromAddress;

    @Value("${app.frontend-url:http://localhost:5173}")
    private String frontendUrl;

    public MailService(
            JavaMailSender mailSender
    ) {
        this.mailSender = mailSender;
    }

    public void sendSimpleMail(
            String to,
            String subject,
            String content
    ) {
        if (to == null || to.isBlank()) {
            throw new IllegalArgumentException(
                    "Email người nhận không được để trống."
            );
        }

        SimpleMailMessage message =
                new SimpleMailMessage();

        message.setFrom(fromAddress);
        message.setTo(to);
        message.setSubject(
                subject != null
                        ? subject
                        : "Bio Medic Center"
        );
        message.setText(
                content != null
                        ? content
                        : ""
        );

        mailSender.send(message);
    }

    public void sendPasswordResetMail(
            String to,
            String token
    ) {
        String resetUrl =
                frontendUrl
                        + "/reset-password?token="
                        + safe(token)
                        + "&email="
                        + safe(to);

        String content =
                """
                BIO MEDIC CENTER

                Chúng tôi đã nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn.

                Vui lòng truy cập đường dẫn sau để tạo mật khẩu mới:
                %s

                Nếu bạn không thực hiện yêu cầu này, bạn có thể bỏ qua email.

                Trân trọng,
                Bio Medic Center
                """.formatted(resetUrl);

        sendSimpleMail(
                to,
                "Đặt lại mật khẩu - Bio Medic Center",
                content
        );
    }

    public void sendAppointmentConfirmationMail(
            String to,
            String customerName,
            String appointmentCode,
            String appointmentDate,
            String appointmentTime,
            String doctorName
    ) {
        String content =
                """
                BIO MEDIC CENTER

                Xin chào %s,

                Lịch hẹn của bạn đã được tiếp nhận.

                Mã lịch hẹn: %s
                Ngày: %s
                Giờ: %s
                Bác sĩ: %s

                Vui lòng đến trước giờ hẹn để hoàn tất thủ tục tiếp nhận.

                Hotline: 1900 56 56 56

                Trân trọng,
                Bio Medic Center
                """.formatted(
                        valueOrDash(customerName),
                        valueOrDash(appointmentCode),
                        valueOrDash(appointmentDate),
                        valueOrDash(appointmentTime),
                        valueOrDash(doctorName)
                );

        sendSimpleMail(
                to,
                "Xác nhận lịch hẹn - Bio Medic Center",
                content
        );
    }

    public void sendTestResultReadyMail(
            String to,
            String customerName,
            String resultCode
    ) {
        String resultUrl =
                frontendUrl
                        + "/ket-qua/"
                        + safe(resultCode);

        String content =
                """
                BIO MEDIC CENTER

                Xin chào %s,

                Kết quả xét nghiệm của bạn đã có trên hệ thống.

                Mã kết quả: %s

                Bạn có thể xem kết quả tại:
                %s

                Trân trọng,
                Bio Medic Center
                """.formatted(
                        valueOrDash(customerName),
                        valueOrDash(resultCode),
                        resultUrl
                );

        sendSimpleMail(
                to,
                "Kết quả xét nghiệm đã có - Bio Medic Center",
                content
        );
    }

    public void sendResultConsultationMail(
            String to,
            String customerName,
            String roomName,
            String doctorName
    ) {
        String content =
                """
                BIO MEDIC CENTER

                Xin chào %s,

                Bác sĩ đã có kết quả và yêu cầu bạn vào phòng để được tư vấn.

                Phòng: %s
                Bác sĩ: %s

                Vui lòng theo dõi thêm thông báo trên hệ thống.

                Trân trọng,
                Bio Medic Center
                """.formatted(
                        valueOrDash(customerName),
                        valueOrDash(roomName),
                        valueOrDash(doctorName)
                );

        sendSimpleMail(
                to,
                "Mời đọc kết quả - Bio Medic Center",
                content
        );
    }

    private String safe(
            String value
    ) {
        if (value == null) {
            return "";
        }

        return value
                .replace(" ", "%20")
                .replace("@", "%40");
    }

    private String valueOrDash(
            String value
    ) {
        return value == null || value.isBlank()
                ? "—"
                : value;
    }
}