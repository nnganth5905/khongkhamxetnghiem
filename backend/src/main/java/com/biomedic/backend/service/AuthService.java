package com.biomedic.backend.service;

import com.biomedic.backend.entity.Account;
import com.biomedic.backend.entity.Customer;
import com.biomedic.backend.enums.RoleName;
import com.biomedic.backend.repository.AccountRepository;
import com.biomedic.backend.repository.CustomerRepository;
import com.biomedic.backend.security.JwtService;

import java.util.LinkedHashMap;
import java.util.Map;
import java.util.Optional;
import java.util.UUID;

import org.springframework.security.core.Authentication;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

@Service
public class AuthService {

    private final AccountRepository accountRepository;
    private final PasswordEncoder passwordEncoder;
    private final JwtService jwtService;
    private final CustomerRepository customerRepository;

    public AuthService(AccountRepository accountRepository, 
                       PasswordEncoder passwordEncoder, 
                       JwtService jwtService,
                       CustomerRepository customerRepository) {
        this.accountRepository = accountRepository;
        this.passwordEncoder = passwordEncoder;
        this.jwtService = jwtService;
        this.customerRepository = customerRepository;
    }

    public Map<String, Object> login(String identifier, String password) {
        Map<String, Object> response = new LinkedHashMap<>();

        Optional<Account> accountOpt = identifier.contains("@") 
                ? accountRepository.findByEmail(identifier) 
                : accountRepository.findByUsername(identifier);

        if (accountOpt.isEmpty()) {
            throw new IllegalArgumentException("Tài khoản không tồn tại!");
        }

        Account account = accountOpt.get();

        if (!passwordEncoder.matches(password, account.getPasswordHash())) {
            throw new IllegalArgumentException("Mật khẩu không chính xác!");
        }

        if (!account.getIsActive()) {
            throw new IllegalArgumentException("Tài khoản đã bị khóa!");
        }

        RoleName roleName = mapDbRoleToEnum(account.getRole());
        String token = jwtService.generateToken(account.getEmail(), roleName);

        response.put("message", "Đăng nhập thành công");
        response.put("accessToken", token);
        
        // Lấy thông tin chi tiết từ bảng khachhang nếu có liên kết
        String fullName = "";
        String phone = "";
        String customerId = account.getIdKhachHang();
        if (customerId != null) {
            Customer customer = customerRepository.findById(customerId).orElse(null);
            if (customer != null) {
                fullName = customer.getFullName() != null ? customer.getFullName() : "";
                phone = customer.getPhone() != null ? customer.getPhone() : "";
            }
        }

        Map<String, Object> userInfo = new LinkedHashMap<>();
        userInfo.put("userId", account.getUserId());
        userInfo.put("email", account.getEmail());
        userInfo.put("username", account.getUsername());
        userInfo.put("role", account.getRole()); 
        userInfo.put("fullName", fullName);   // Đã bổ sung
        userInfo.put("phone", phone);         // Đã bổ sung SĐT để Frontend nhận diện
        
        response.put("user", userInfo);

        return response;
    }

    private RoleName mapDbRoleToEnum(String dbRole) {
        if (dbRole == null) return RoleName.CUSTOMER;
        return switch (dbRole.toLowerCase()) {
            case "admin" -> RoleName.ADMIN;
            case "bacsi" -> RoleName.DOCTOR;
            case "letan" -> RoleName.RECEPTIONIST;
            case "ktv" -> RoleName.TECHNICIAN;
            default -> RoleName.CUSTOMER;
        };
    }

    @Transactional 
    public Map<String, Object> register(String name, String email, String phone, String gender, String password) {
        Map<String, Object> response = new LinkedHashMap<>();

        if (accountRepository.existsByEmail(email)) {
            throw new IllegalArgumentException("Email này đã được sử dụng!");
        }

        String customerId = "KH" + UUID.randomUUID().toString().substring(0, 8).toUpperCase();

        Customer newCustomer = new Customer();
        newCustomer.setId(customerId); 
        newCustomer.setFullName(name);
        newCustomer.setEmail(email);
        newCustomer.setPhone(phone);
        
        newCustomer.setGender(gender != null ? gender.toLowerCase() : "khac");
        newCustomer.setStatus("yes");

        customerRepository.save(newCustomer);

        Account newAccount = new Account();
        newAccount.setEmail(email);
        newAccount.setUsername(email); 
        newAccount.setPasswordHash(passwordEncoder.encode(password));
        newAccount.setRole("khachhang");
        newAccount.setIsActive(true);
        newAccount.setIdKhachHang(customerId); 

        accountRepository.save(newAccount);

        response.put("message", "Đăng ký tài khoản thành công!");
        response.put("email", email);

        return response;
    }

    public Map<String, Object> getCurrentUser(Authentication authentication) {
        Map<String, Object> response = new LinkedHashMap<>();
        if (authentication == null || !authentication.isAuthenticated() || "anonymousUser".equals(authentication.getPrincipal())) {
            response.put("authenticated", false);
            response.put("message", "Chưa có người dùng đăng nhập.");
            return response;
        }

        String email = authentication.getName();
        Account account = accountRepository.findByEmail(email).orElse(null);

        if (account == null) {
            response.put("authenticated", false);
            response.put("message", "Không tìm thấy thông tin tài khoản.");
            return response;
        }

        String fullName = "";
        String phone = "";
        String customerId = account.getIdKhachHang();
        if (customerId != null) {
            Customer customer = customerRepository.findById(customerId).orElse(null);
            if (customer != null) {
                fullName = customer.getFullName() != null ? customer.getFullName() : "";
                phone = customer.getPhone() != null ? customer.getPhone() : "";
            }
        }

        response.put("authenticated", true);
        response.put("userId", account.getUserId());
        response.put("email", account.getEmail());
        response.put("username", account.getUsername());
        response.put("role", account.getRole());
        response.put("fullName", fullName);   // Đã bổ sung đầy đủ cho Frontend
        response.put("phone", phone);         // Đã bổ sung SĐT chuẩn xác từ DB
        response.put("authorities", authentication.getAuthorities());

        return response;
    }

    public Map<String, Object> forgotPassword(String email) {
        Map<String, Object> response = new LinkedHashMap<>();
        response.put("message", "Nếu email tồn tại trong hệ thống, hướng dẫn đặt lại mật khẩu sẽ được gửi.");
        response.put("email", email);
        return response;
    }

    public Map<String, Object> verifyResetToken(String token, String email) {
        Map<String, Object> response = new LinkedHashMap<>();
        response.put("valid", token != null && !token.isBlank() && email != null && !email.isBlank());
        response.put("email", email);
        return response;
    }

    public Map<String, Object> resetPassword(String token, String email, String password) {
        Map<String, Object> response = new LinkedHashMap<>();
        response.put("message", "Yêu cầu đặt lại mật khẩu đã được tiếp nhận.");
        response.put("email", email);
        return response;
    }

    public Map<String, Object> confirmEmail(String token) {
        Map<String, Object> response = new LinkedHashMap<>();
        response.put("success", token != null && !token.isBlank());
        response.put("message", "Token xác nhận đã được tiếp nhận.");
        return response;
    }
}