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
            Authentication authentication, String hoTen, LocalDate ngaySinh,
            String gioiTinh, String soDienThoai, String cccd,
            String diaChi, String email, String ghiChu
    ) {
        return new LinkedHashMap<>();
    }

    // Lấy danh sách khách hàng cho trang Admin[cite: 15]
    public List<Map<String, Object>> getAllCustomers(String q) {
        try {
            List<Customer> customers;
            
            if (q != null && !q.trim().isEmpty() && !"undefined".equals(q) && !"null".equals(q)) {
                customers = customerRepository.search(q.trim());
            } else {
                customers = customerRepository.findAll();
            }

            // Sắp xếp an toàn đưa khách hàng mới tạo lên đầu
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

    public Map<String, Object> getCustomerById(String id) {
        Customer customer = customerRepository.findById(id)
                .orElseThrow(() -> new ResourceNotFoundException("Không tìm thấy khách hàng với mã: " + id));
        return mapToResponse(customer);
    }

    public Map<String, Object> createCustomer(
            String hoTen, LocalDate ngaySinh, String gioiTinh,
            String soDienThoai, String cccd, String diaChi,
            String email, String trangThai, String ghiChu
    ) {
        Customer newCustomer = new Customer();
        String shortId = UUID.randomUUID().toString().substring(0, 6).toUpperCase();
        newCustomer.setId("KH" + shortId);

        newCustomer.setFullName(hoTen);
        newCustomer.setBirthDate(ngaySinh);
        newCustomer.setGender(gioiTinh);
        newCustomer.setPhone(soDienThoai);
        newCustomer.setCitizenId(cccd);
        newCustomer.setAddress(diaChi);
        newCustomer.setEmail(email);
        
        String status = ("INACTIVE".equalsIgnoreCase(trangThai)) ? "no" : "yes";
        newCustomer.setStatus(status);

        Customer savedCustomer = customerRepository.save(newCustomer);
        return mapToResponse(savedCustomer);
    }

    public Map<String, Object> updateCustomer(
            String id, String hoTen, LocalDate ngaySinh, String gioiTinh,
            String soDienThoai, String cccd, String diaChi,
            String email, String trangThai, String ghiChu
    ) {
        Customer existingCustomer = customerRepository.findById(id)
                .orElseThrow(() -> new ResourceNotFoundException("Không tìm thấy khách hàng với mã: " + id));

        existingCustomer.setFullName(hoTen);
        existingCustomer.setBirthDate(ngaySinh);
        existingCustomer.setGender(gioiTinh);
        existingCustomer.setPhone(soDienThoai);
        existingCustomer.setCitizenId(cccd);
        existingCustomer.setAddress(diaChi);
        existingCustomer.setEmail(email);

        String status = ("INACTIVE".equalsIgnoreCase(trangThai)) ? "no" : "yes";
        existingCustomer.setStatus(status);

        Customer updatedCustomer = customerRepository.save(existingCustomer);
        return mapToResponse(updatedCustomer);
    }

    public void deleteCustomer(String id) {
        Customer existingCustomer = customerRepository.findById(id)
                .orElseThrow(() -> new ResourceNotFoundException("Không tìm thấy khách hàng với mã: " + id));
        
        existingCustomer.setStatus("no");
        customerRepository.save(existingCustomer);
    }

    private Map<String, Object> mapToResponse(Customer c) {
        Map<String, Object> map = new LinkedHashMap<>();
        map.put("id", c.getId());
        map.put("maKhachHang", c.getId());
        map.put("hoTen", c.getFullName());
        map.put("ngaySinh", c.getBirthDate());
        map.put("soDienThoai", c.getPhone());
        map.put("gioiTinh", c.getGender() != null ? c.getGender() : "Khác");
        map.put("cccd", c.getCitizenId());
        map.put("diaChi", c.getAddress());
        map.put("email", c.getEmail());
        map.put("trangThai", "yes".equalsIgnoreCase(c.getStatus()) ? "ACTIVE" : "INACTIVE");
        
        return map;
    }
}