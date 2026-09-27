package com.biomedic.backend.util;

import com.google.zxing.BarcodeFormat;
import com.google.zxing.WriterException;

import com.google.zxing.client.j2se.MatrixToImageWriter;

import com.google.zxing.common.BitMatrix;

import com.google.zxing.qrcode.QRCodeWriter;

import java.io.ByteArrayOutputStream;
import java.io.IOException;

import java.util.Base64;

public final class QRCodeUtil {

    private QRCodeUtil() {
    }

    public static byte[] generatePng(
            String content,
            int width,
            int height
    ) {
        if (
                content == null
                || content.isBlank()
        ) {
            throw new IllegalArgumentException(
                    "Nội dung QR không được để trống."
            );
        }

        if (
                width <= 0
                || height <= 0
        ) {
            throw new IllegalArgumentException(
                    "Kích thước QR phải lớn hơn 0."
            );
        }

        try {
            QRCodeWriter writer =
                    new QRCodeWriter();

            BitMatrix bitMatrix =
                    writer.encode(
                            content,
                            BarcodeFormat.QR_CODE,
                            width,
                            height
                    );

            ByteArrayOutputStream output =
                    new ByteArrayOutputStream();

            MatrixToImageWriter.writeToStream(
                    bitMatrix,
                    "PNG",
                    output
            );

            return output.toByteArray();

        } catch (
                WriterException
                | IOException exception
        ) {
            throw new IllegalStateException(
                    "Không thể tạo mã QR.",
                    exception
            );
        }
    }

    public static String generateBase64(
            String content,
            int width,
            int height
    ) {
        return Base64
                .getEncoder()
                .encodeToString(
                        generatePng(
                                content,
                                width,
                                height
                        )
                );
    }

    public static String generateDataUrl(
            String content,
            int width,
            int height
    ) {
        return "data:image/png;base64,"
                + generateBase64(
                        content,
                        width,
                        height
                );
    }
}