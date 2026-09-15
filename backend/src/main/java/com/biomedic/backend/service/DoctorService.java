package com.biomedic.backend.service;

import java.util.Collections;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

import org.springframework.stereotype.Service;

@Service
public class DoctorService {

    public List<Map<String, Object>> getDoctors(
            String specialtyId,
            String q
    ) {
        return Collections.emptyList();
    }

    public Map<String, Object> getDoctorById(
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
                "Chưa nối repository bảng bacsi."
        );

        return result;
    }

    public Map<String, Object> createDoctor(
            String hoTen,
            String idChuyenKhoa,
            String hocVi,
            String chucDanh,
            String soDienThoai,
            String email,
            String idPhong,
            String hinhAnh,
            String gioiThieu,
            String trangThai
    ) {
        return buildDoctor(
                null,
                hoTen,
                idChuyenKhoa,
                hocVi,
                chucDanh,
                soDienThoai,
                email,
                idPhong,
                hinhAnh,
                gioiThieu,
                trangThai
        );
    }

    public Map<String, Object> updateDoctor(
            String id,
            String hoTen,
            String idChuyenKhoa,
            String hocVi,
            String chucDanh,
            String soDienThoai,
            String email,
            String idPhong,
            String hinhAnh,
            String gioiThieu,
            String trangThai
    ) {
        return buildDoctor(
                id,
                hoTen,
                idChuyenKhoa,
                hocVi,
                chucDanh,
                soDienThoai,
                email,
                idPhong,
                hinhAnh,
                gioiThieu,
                trangThai
        );
    }

    public void deleteDoctor(
            String id
    ) {
        // TODO: doctorRepository.deleteById(...)
    }

    private Map<String, Object> buildDoctor(
            String id,
            String hoTen,
            String idChuyenKhoa,
            String hocVi,
            String chucDanh,
            String soDienThoai,
            String email,
            String idPhong,
            String hinhAnh,
            String gioiThieu,
            String trangThai
    ) {
        Map<String, Object> result =
                new LinkedHashMap<>();

        result.put("id", id);
        result.put("hoTen", hoTen);
        result.put("idChuyenKhoa", idChuyenKhoa);
        result.put("hocVi", hocVi);
        result.put("chucDanh", chucDanh);
        result.put("soDienThoai", soDienThoai);
        result.put("email", email);
        result.put("idPhong", idPhong);
        result.put("hinhAnh", hinhAnh);
        result.put("gioiThieu", gioiThieu);
        result.put("trangThai", trangThai);

        return result;
    }
}