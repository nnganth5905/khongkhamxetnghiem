package com.biomedic.backend.service;

import java.time.LocalDate;
import java.util.Collections;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

import org.springframework.stereotype.Service;

@Service
public class EmployeeService {

    public List<Map<String, Object>> getAllEmployees(
            String q,
            String role
    ) {
        return Collections.emptyList();
    }

    public Map<String, Object> getEmployeeById(
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
                "Chưa nối repository bảng nhanvien."
        );

        return result;
    }

    public Map<String, Object> createEmployee(
            String hoTen,
            LocalDate ngaySinh,
            String gioiTinh,
            String soDienThoai,
            String email,
            String idChuyenKhoa,
            String idCoSo,
            String vaiTro,
            LocalDate ngayVaoLam,
            String trangThai,
            String ghiChu
    ) {
        return buildEmployee(
                null,
                hoTen,
                ngaySinh,
                gioiTinh,
                soDienThoai,
                email,
                idChuyenKhoa,
                idCoSo,
                vaiTro,
                ngayVaoLam,
                trangThai,
                ghiChu
        );
    }

    public Map<String, Object> updateEmployee(
            String id,
            String hoTen,
            LocalDate ngaySinh,
            String gioiTinh,
            String soDienThoai,
            String email,
            String idChuyenKhoa,
            String idCoSo,
            String vaiTro,
            LocalDate ngayVaoLam,
            String trangThai,
            String ghiChu
    ) {
        return buildEmployee(
                id,
                hoTen,
                ngaySinh,
                gioiTinh,
                soDienThoai,
                email,
                idChuyenKhoa,
                idCoSo,
                vaiTro,
                ngayVaoLam,
                trangThai,
                ghiChu
        );
    }

    public void deleteEmployee(
            String id
    ) {
        // TODO: employeeRepository.deleteById(...)
    }

    private Map<String, Object> buildEmployee(
            String id,
            String hoTen,
            LocalDate ngaySinh,
            String gioiTinh,
            String soDienThoai,
            String email,
            String idChuyenKhoa,
            String idCoSo,
            String vaiTro,
            LocalDate ngayVaoLam,
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
        result.put("email", email);
        result.put("idChuyenKhoa", idChuyenKhoa);
        result.put("idCoSo", idCoSo);
        result.put("vaiTro", vaiTro);
        result.put("ngayVaoLam", ngayVaoLam);
        result.put("trangThai", trangThai);
        result.put("ghiChu", ghiChu);

        return result;
    }
}