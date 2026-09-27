package com.biomedic.backend.util;

import java.time.LocalDate;
import java.time.LocalDateTime;
import java.time.LocalTime;
import java.time.ZoneId;

import java.time.format.DateTimeFormatter;
import java.time.format.DateTimeParseException;

public final class DateTimeUtil {

    public static final ZoneId VIETNAM_ZONE =
            ZoneId.of(
                    "Asia/Ho_Chi_Minh"
            );

    public static final DateTimeFormatter DATE_FORMATTER =
            DateTimeFormatter.ofPattern(
                    "dd/MM/yyyy"
            );

    public static final DateTimeFormatter TIME_FORMATTER =
            DateTimeFormatter.ofPattern(
                    "HH:mm"
            );

    public static final DateTimeFormatter DATE_TIME_FORMATTER =
            DateTimeFormatter.ofPattern(
                    "dd/MM/yyyy HH:mm"
            );

    private DateTimeUtil() {
    }

    public static LocalDate today() {
        return LocalDate.now(
                VIETNAM_ZONE
        );
    }

    public static LocalDateTime now() {
        return LocalDateTime.now(
                VIETNAM_ZONE
        );
    }

    public static String formatDate(
            LocalDate date
    ) {
        return date == null
                ? ""
                : date.format(
                        DATE_FORMATTER
                );
    }

    public static String formatTime(
            LocalTime time
    ) {
        return time == null
                ? ""
                : time.format(
                        TIME_FORMATTER
                );
    }

    public static String formatDateTime(
            LocalDateTime dateTime
    ) {
        return dateTime == null
                ? ""
                : dateTime.format(
                        DATE_TIME_FORMATTER
                );
    }

    public static LocalDate parseDate(
            String value
    ) {
        if (
                value == null
                || value.isBlank()
        ) {
            return null;
        }

        String text =
                value.trim();

        try {
            return LocalDate.parse(
                    text
            );
        } catch (
                DateTimeParseException ignored
        ) {
        }

        try {
            return LocalDate.parse(
                    text,
                    DATE_FORMATTER
            );
        } catch (
                DateTimeParseException exception
        ) {
            throw new IllegalArgumentException(
                    "Ngày không hợp lệ: "
                            + value
            );
        }
    }

    public static LocalTime parseTime(
            String value
    ) {
        if (
                value == null
                || value.isBlank()
        ) {
            return null;
        }

        String text =
                value.trim();

        try {
            return LocalTime.parse(
                    text
            );
        } catch (
                DateTimeParseException ignored
        ) {
        }

        try {
            return LocalTime.parse(
                    text,
                    TIME_FORMATTER
            );
        } catch (
                DateTimeParseException exception
        ) {
            throw new IllegalArgumentException(
                    "Giờ không hợp lệ: "
                            + value
            );
        }
    }
}