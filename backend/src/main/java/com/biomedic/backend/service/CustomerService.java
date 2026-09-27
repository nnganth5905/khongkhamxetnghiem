package com.biomedic.backend.service;

import java.time.LocalDate;
import java.util.Collections;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;
import java.util.UUID;
import java.util.stream.Collectors;

import com.biomedic.backend.entity.Customer;
import com.biomedic.backend.repository.CustomerRepository;
import com.biomedic.backend.exception.ResourceNotFoundException;

import org.springframework.security.core.Authentication;
import org.springframework.stereotype.Service;

@Service
public class CustomerService {

    private final CustomerRepository customerRepository;

    public CustomerService(CustomerRepository customerRepository) {
        this.customerRepository = customerRepository;
    }

    public Map<String, Object> getCurrentCustomer(Authentication authentication) {
        Map<String, Object> result = new LinkedHashMap<>();
        result.put("username", authentication != null ? authentication.getName() : null);
        result.put("message", "CustomerService đã sẵn sàng.");
        return result;
    }

    public Map<String, Object> updateCurrentCustomer(
            Authentication authentication, String tenKhachHang, LocalDate ngaySinh,
            String gioiTinh, String soDienThoai, String cccd,
            String diaChi, String email, String ghiChu
    ) {
        return new LinkedHashMap<>();
    }

    // LẤY DANH SÁCH KHÁCH HÀNG
    public List<Map<String, Object>> getAllCustomers(String q) {
        try {
            List<Customer> customers;
            
            if (q != null && !q.trim().isEmpty() && !"undefined".equals(q) && !"null".equals(q)) {
                customers = customerRepository.search(q.trim());
            } else {
                customers = customerRepository.findAll();
            }

            return customers.stream()
                    .sorted((c1, c2) -> {
                        if (c1.getCreatedAt() == null && c2.getCreatedAt() == null) return 0;
                        if (c1.getCreatedAt() == null) return 1;
                        if (c2.getCreatedAt() == null) return -1;
                        return c2.getCreatedAt().compareTo(c1.getCreatedAt());
                    })
                    .map(this::mapToResponse)
                    .collect(Collectors.toList());

        } catch (Exception e) {
            System.err.println(">>> [LỖI] Lỗi khi tải danh sách khách hàng: " + e.getMessage());
            e.printStackTrace();
            return Collections.emptyList();
        }
    }

    // LẤY CHI TIẾT KHÁCH HÀNG
    public Map<String, Object> getCustomerById(String id) {
        Customer customer = customerRepository.findById(id)
                .orElseThrow(() -> new ResourceNotFoundException("Không tìm thấy khách hàng với mã: " + id));
        return mapToResponse(customer);
    }

    // THÊM KHÁCH HÀNG
    public Map<String, Object> createCustomer(
            String tenKhachHang, LocalDate ngaySinh, String gioiTinh,
            String soDienThoai, String cccd, String diaChi,
            String email, String status
    ) {
        Customer newCustomer = new Customer();
        String shortId = UUID.randomUUID().toString().substring(0, 6).toUpperCase();
        newCustomer.setId("KH" + shortId);

        newCustomer.setFullName(tenKhachHang);
        newCustomer.setBirthDate(ngaySinh);
        newCustomer.setGender(gioiTinh);
        newCustomer.setPhone(soDienThoai);
        
        // Xử lý CCCD rỗng để không bị lỗi UNIQUE KEY
        newCustomer.setCitizenId((cccd != null && !cccd.trim().isEmpty()) ? cccd.trim() : null);
        newCustomer.setAddress(diaChi);
        newCustomer.setEmail(email);
        
        String finalStatus = ("no".equalsIgnoreCase(status)) ? "no" : "yes";
        newCustomer.setStatus(finalStatus);

        Customer savedCustomer = customerRepository.save(newCustomer);
        return mapToResponse(savedCustomer);
    }

    // CẬP NHẬT KHÁCH HÀNG
    public Map<String, Object> updateCustomer(
            String id, String tenKhachHang, LocalDate ngaySinh, String gioiTinh,
            String soDienThoai, String cccd, String diaChi,
            String email, String status
    ) {
        Customer existingCustomer = customerRepository.findById(id)
                .orElseThrow(() -> new ResourceNotFoundException("Không tìm thấy khách hàng với mã: " + id));

        existingCustomer.setFullName(tenKhachHang);
        existingCustomer.setBirthDate(ngaySinh);
        existingCustomer.setGender(gioiTinh);
        existingCustomer.setPhone(soDienThoai);
        
        existingCustomer.setCitizenId((cccd != null && !cccd.trim().isEmpty()) ? cccd.trim() : null);
        existingCustomer.setAddress(diaChi);
        existingCustomer.setEmail(email);

        String finalStatus = ("no".equalsIgnoreCase(status)) ? "no" : "yes";
        existingCustomer.setStatus(finalStatus);

        Customer updatedCustomer = customerRepository.save(existingCustomer);
        return mapToResponse(updatedCustomer);
    }

    // XÓA KHÁCH HÀNG
    public void deleteCustomer(String id) {
        Customer existingCustomer = customerRepository.findById(id)
                .orElseThrow(() -> new ResourceNotFoundException("Không tìm thấy khách hàng với mã: " + id));
        
        // Xóa mềm: Chuyển trạng thái thành 'no'
        existingCustomer.setStatus("no");
        customerRepository.save(existingCustomer);
    }

    // MAPPING DỮ LIỆU TRẢ VỀ FRONTEND
    private Map<String, Object> mapToResponse(Customer c) {
        Map<String, Object> map = new LinkedHashMap<>();
        
        // BẮT BUỘC PHẢI CÓ KEY "id" ĐỂ COMPONENT BẢNG CỦA REACT HOẠT ĐỘNG
        map.put("id", c.getId()); 
        
        map.put("idKhachHang", c.getId());
        map.put("tenKhachHang", c.getFullName());
        map.put("ngaySinh", c.getBirthDate());
        map.put("soDienThoai", c.getPhone());
        map.put("gioiTinh", c.getGender());
        map.put("cccd", c.getCitizenId());
        map.put("diaChi", c.getAddress());
        map.put("email", c.getEmail());
        map.put("status", c.getStatus());
        
        return map;
    }
}