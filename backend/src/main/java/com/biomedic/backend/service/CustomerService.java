package com.biomedic.backend.service;

import java.time.LocalDate;
import java.util.Collections;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

import org.springframework.security.core.Authentication;
import org.springframework.stereotype.Service;

@Service
public class CustomerService {

    public Map<String, Object> getCurrentCustomer(
            Authentication authentication
    ) {
        Map<String, Object> result =
                new LinkedHashMap<>();

        result.put(
                "username",
                authentication != null
                    ? authentication.getName()
                    : null
        );

        result.put(
                "message",
                "CustomerService đã sẵn sàng. Dữ liệu MySQL sẽ được nối sau khi map bảng khachhang."
        );

        return result;
    }

    public Map<String, Object> updateCurrentCustomer(
            Authentication authentication,
            String hoTen,
            LocalDate ngaySinh,
            String gioiTinh,
            String soDienThoai,
            String cccd,
            String diaChi,
            String email,
            String ghiChu
    ) {
        return buildCustomer(
                null,
                hoTen,
                ngaySinh,
                gioiTinh,
                soDienThoai,
                cccd,
                diaChi,
                email,
                null,
                ghiChu
        );
    }

    public List<Map<String, Object>> getAllCustomers(
            String q
    ) {
        return Collections.emptyList();
    }

    public Map<String, Object> getCustomerById(
            String id
    ) {
        Map<String, Object> result =
                new LinkedHashMap<>();

        result.put(
                "id",
                id
        );

        result.put(
                "message",
                "Chưa nối repository bảng khachhang."
        );

        return result;
    }

    public Map<String, Object> createCustomer(
            String hoTen,
            LocalDate ngaySinh,
            String gioiTinh,
            String soDienThoai,
            String cccd,
            String diaChi,
            String email,
            String trangThai,
            String ghiChu
    ) {
        return buildCustomer(
                null,
                hoTen,
                ngaySinh,
                gioiTinh,
                soDienThoai,
                cccd,
                diaChi,
                email,
                trangThai,
                ghiChu
        );
    }

    public Map<String, Object> updateCustomer(
            String id,
            String hoTen,
            LocalDate ngaySinh,
            String gioiTinh,
            String soDienThoai,
            String cccd,
            String diaChi,
            String email,
            String trangThai,
            String ghiChu
    ) {
        return buildCustomer(
                id,
                hoTen,
                ngaySinh,
                gioiTinh,
                soDienThoai,
                cccd,
                diaChi,
                email,
                trangThai,
                ghiChu
        );
    }

    public void deleteCustomer(
            String id
    ) {
        // TODO: customerRepository.deleteById(...)
    }

    private Map<String, Object> buildCustomer(
            String id,
            String hoTen,
            LocalDate ngaySinh,
            String gioiTinh,
            String soDienThoai,
            String cccd,
            String diaChi,
            String email,
            String trangThai,
            String ghiChu
    ) {
        Map<String, Object> result =
                new LinkedHashMap<>();

        result.put("id", id);
        result.put("hoTen", hoTen);
        result.put("ngaySinh", ngaySinh);
        result.put("gioiTinh", gioiTinh);
        result.put("soDienThoai", soDienThoai);
        result.put("cccd", cccd);
        result.put("diaChi", diaChi);
        result.put("email", email);
        result.put("trangThai", trangThai);
        result.put("ghiChu", ghiChu);

        return result;
    }
}