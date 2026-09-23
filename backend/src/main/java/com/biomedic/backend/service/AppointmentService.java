package com.biomedic.backend.service;

import com.biomedic.backend.controller.AppointmentController.ExaminationAppointmentRequest;
import com.biomedic.backend.controller.AppointmentController.TestAppointmentRequest;
import com.biomedic.backend.controller.AppointmentController.UpdateAppointmentRequest;
import com.biomedic.backend.dto.response.AppointmentResponse;

import com.biomedic.backend.entity.ExaminationAppointment;
import com.biomedic.backend.entity.TestAppointment;
import com.biomedic.backend.entity.TestAppointmentItem;
import com.biomedic.backend.entity.Specialty;
import com.biomedic.backend.entity.Doctor;
import com.biomedic.backend.entity.Customer;
import com.biomedic.backend.entity.Account;
import com.biomedic.backend.repository.SpecialtyRepository;
import com.biomedic.backend.repository.DoctorRepository;
import com.biomedic.backend.repository.ExaminationAppointmentRepository;
import com.biomedic.backend.repository.TestAppointmentRepository;
import com.biomedic.backend.repository.TestAppointmentItemRepository;
import com.biomedic.backend.repository.TestRepository;
import com.biomedic.backend.repository.CustomerRepository;
import com.biomedic.backend.repository.AccountRepository;

import org.springframework.jdbc.core.JdbcTemplate;
import org.springframework.security.core.Authentication;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.math.BigDecimal;
import java.time.LocalDate;
import java.time.LocalTime;
import java.time.format.DateTimeFormatter;
import java.util.ArrayList;
import java.util.Collections;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;
import java.util.Set;
import java.util.HashSet;
import java.util.UUID;

@Service
public class AppointmentService {

    private final SpecialtyRepository specialtyRepository;
    private final DoctorRepository doctorRepository;
    private final ExaminationAppointmentRepository examinationAppointmentRepository;
    private final TestAppointmentRepository testAppointmentRepository;
    private final CustomerRepository customerRepository;
    private final AccountRepository accountRepository;
    private final TestAppointmentItemRepository testAppointmentItemRepository;
    private final TestRepository testRepository;
    private final JdbcTemplate jdbcTemplate;

    public AppointmentService(SpecialtyRepository specialtyRepository, 
                              DoctorRepository doctorRepository,
                              ExaminationAppointmentRepository examinationAppointmentRepository,
                              TestAppointmentRepository testAppointmentRepository,
                              CustomerRepository customerRepository,
                              AccountRepository accountRepository,
                              TestAppointmentItemRepository testAppointmentItemRepository,
                              TestRepository testRepository,
                              JdbcTemplate jdbcTemplate) {
        this.specialtyRepository = specialtyRepository;
        this.doctorRepository = doctorRepository;
        this.examinationAppointmentRepository = examinationAppointmentRepository;
        this.testAppointmentRepository = testAppointmentRepository;
        this.customerRepository = customerRepository;
        this.accountRepository = accountRepository;
        this.testAppointmentItemRepository = testAppointmentItemRepository;
        this.testRepository = testRepository;
        this.jdbcTemplate = jdbcTemplate;
    }

    private String generateShortCode(String prefix) {
        String shortRandom = UUID.randomUUID().toString().substring(0, 6).toUpperCase();
        return prefix + "-" + shortRandom;
    }

    public Map<String, Object> getAppointmentOptions() {
        Map<String, Object> result = new LinkedHashMap<>();

        List<Specialty> specialtyList = specialtyRepository.findByStatus("yes");
        Map<String, String> departments = new LinkedHashMap<>();
        for (Specialty sp : specialtyList) {
            departments.put(sp.getId(), sp.getName());
        }

        List<Doctor> doctorList = doctorRepository.findByStatus("active");
        List<Map<String, Object>> doctors = new ArrayList<>();
        for (Doctor d : doctorList) {
            doctors.add(Map.of(
                "id", String.valueOf(d.getId()),
                "name", d.getName(),
                "specialtyId", d.getSpecialtyId()
            ));
        }
        
        result.put("departments", departments);
        result.put("doctors", doctors);
        result.put("tests", testRepository.findAll()); 
        result.put("facilities", Collections.emptyList());

        return result;
    }

    private Customer resolveCustomerForAppointment(Authentication authentication, String hoten, String email, String phone, String gender, String dob) {
        Account loggedInAccount = null;
        Customer loggedInCustomer = null;
        
        String reqName = (hoten != null) ? hoten.trim() : "";
        String reqPhone = (phone != null) ? phone.trim() : "";
        String reqGender = (gender != null && !gender.trim().isEmpty()) ? gender.trim() : "khac";
        LocalDate reqDob = (dob != null && !dob.trim().isEmpty()) ? LocalDate.parse(dob) : null;

        if (authentication != null && authentication.isAuthenticated() && !"anonymousUser".equals(authentication.getPrincipal())) {
            loggedInAccount = accountRepository.findByEmail(authentication.getName()).orElse(null);
            if (loggedInAccount != null && loggedInAccount.getIdKhachHang() != null) {
                loggedInCustomer = customerRepository.findById(loggedInAccount.getIdKhachHang()).orElse(null);
            }
        }

        if (loggedInCustomer != null) {
            boolean matchName = reqName.equalsIgnoreCase(loggedInCustomer.getFullName() != null ? loggedInCustomer.getFullName().trim() : "");
            boolean matchPhone = reqPhone.equals(loggedInCustomer.getPhone() != null ? loggedInCustomer.getPhone().trim() : "");
            boolean matchGender = reqGender.equalsIgnoreCase(loggedInCustomer.getGender() != null ? loggedInCustomer.getGender().trim() : "khac");
            boolean matchDob = (reqDob == null && loggedInCustomer.getBirthDate() == null) || 
                               (reqDob != null && reqDob.equals(loggedInCustomer.getBirthDate()));

            if (matchName && matchPhone && matchGender && matchDob) {
                return loggedInCustomer;
            }
        }

        if (!reqName.isEmpty() && !reqPhone.isEmpty()) {
            List<Customer> possibleMatches = customerRepository.search(reqPhone);
            for (Customer c : possibleMatches) {
                boolean matchName = reqName.equalsIgnoreCase(c.getFullName() != null ? c.getFullName().trim() : "");
                boolean matchPhone = reqPhone.equals(c.getPhone() != null ? c.getPhone().trim() : "");
                boolean matchGender = reqGender.equalsIgnoreCase(c.getGender() != null ? c.getGender().trim() : "khac");
                boolean matchDob = (reqDob == null && c.getBirthDate() == null) || 
                                   (reqDob != null && reqDob.equals(c.getBirthDate()));

                if (matchName && matchPhone && matchGender && matchDob) {
                    return c;
                }
            }
        }

        Customer newCustomer = new Customer();
        String shortId = UUID.randomUUID().toString().substring(0, 8).toUpperCase();
        newCustomer.setId("KH" + shortId);
        newCustomer.setFullName(reqName);
        newCustomer.setPhone(reqPhone); 
        newCustomer.setEmail(email);
        newCustomer.setGender(reqGender);
        newCustomer.setBirthDate(reqDob);
        newCustomer.setStatus("yes");
        
        return customerRepository.save(newCustomer);
    }

    // Logic kiểm tra trùng giờ trên cả Khám bệnh và Xét nghiệm
    private void checkTimeConflict(String customerId, LocalDate reqDate, LocalTime reqTime) {
        if (reqDate == null || reqTime == null) return;

        List<ExaminationAppointment> existingExams = examinationAppointmentRepository.findByCustomerIdAndExaminationDate(customerId, reqDate);
        boolean examConflict = existingExams.stream()
                .anyMatch(a -> !a.getStatus().equalsIgnoreCase("huy") 
                            && !a.getStatus().equalsIgnoreCase("cancelled")
                            && a.getExaminationTime() != null
                            && a.getExaminationTime().equals(reqTime));

        List<TestAppointment> existingTests = testAppointmentRepository.findByCustomerIdAndTestDate(customerId, reqDate);
        boolean testConflict = existingTests.stream()
                .anyMatch(a -> !a.getStatus().equalsIgnoreCase("huy") 
                            && !a.getStatus().equalsIgnoreCase("cancelled")
                            && a.getTestTime() != null
                            && a.getTestTime().equals(reqTime));

        if (examConflict || testConflict) {
            throw new IllegalArgumentException("Khách hàng đã có lịch hẹn vào lúc " + reqTime.toString() + " ngày " + reqDate.toString() + ". Vui lòng chọn khung giờ khác.");
        }
    }

    @Transactional
    public Map<String, Object> createExaminationAppointment(ExaminationAppointmentRequest request, Authentication authentication) {
        Integer userId = null;
        if (authentication != null && authentication.isAuthenticated() && !"anonymousUser".equals(authentication.getPrincipal())) {
            Account acc = accountRepository.findByEmail(authentication.getName()).orElse(null);
            if (acc != null) userId = acc.getUserId();
        }

        Customer customer = resolveCustomerForAppointment(
            authentication, request.hoten(), request.email(), request.sodienthoai(), request.gioitinh(), request.ngaysinh()
        );

        boolean hasPhoneInRequest = request.sodienthoai() != null && !request.sodienthoai().trim().isEmpty();
        boolean hasPhoneInDb = customer != null && customer.getPhone() != null && !customer.getPhone().trim().isEmpty();

        if (!hasPhoneInRequest && !hasPhoneInDb) {
            throw new IllegalArgumentException("Số điện thoại không được để trống");
        }

        LocalDate reqDate = null;
        LocalTime reqTime = null;

        if (request.ngay() != null && !request.ngay().isEmpty()) {
            reqDate = LocalDate.parse(request.ngay(), DateTimeFormatter.ISO_LOCAL_DATE);
        }
        
        if (request.gio() != null && !request.gio().isEmpty()) {
            reqTime = LocalTime.parse(request.gio(), DateTimeFormatter.ISO_LOCAL_TIME);
        }

        checkTimeConflict(customer.getId(), reqDate, reqTime);

        ExaminationAppointment appointment = new ExaminationAppointment();
        appointment.setMaDatLich(generateShortCode("DLK"));
        appointment.setUserId(userId); 
        appointment.setCustomerId(customer.getId()); 
        appointment.setExaminationDate(reqDate);
        appointment.setExaminationTime(reqTime);
        
        if (request.idbacsi() != null && !request.idbacsi().isEmpty()) {
            appointment.setDoctorId(Integer.parseInt(request.idbacsi()));
        }
        
        appointment.setSpecialtyId(request.idchuyenkhoa());
        appointment.setRoomId(request.idcoso() != null ? request.idcoso() : "CS001"); 
        appointment.setReason(request.lydokham());
        appointment.setNote(request.ghichu());
        appointment.setStatus("pending"); 
        
        examinationAppointmentRepository.save(appointment);

        Map<String, Object> result = new LinkedHashMap<>();
        result.put("id", appointment.getId()); 
        result.put("type", "EXAMINATION");
        result.put("message", "Đã lưu lịch khám thành công.");
        return result;
    }

    @Transactional
    public Map<String, Object> createTestAppointment(TestAppointmentRequest request, Authentication authentication) {
        Integer userId = null;
        if (authentication != null && authentication.isAuthenticated() && !"anonymousUser".equals(authentication.getPrincipal())) {
            Account acc = accountRepository.findByEmail(authentication.getName()).orElse(null);
            if (acc != null) userId = acc.getUserId();
        }

        Customer customer = resolveCustomerForAppointment(
            authentication, request.hoten(), request.email(), request.sodienthoai(), request.gioitinh(), request.ngaysinh()
        );

        boolean hasPhoneInRequest = request.sodienthoai() != null && !request.sodienthoai().trim().isEmpty();
        boolean hasPhoneInDb = customer != null && customer.getPhone() != null && !customer.getPhone().trim().isEmpty();

        if (!hasPhoneInRequest && !hasPhoneInDb) {
            throw new IllegalArgumentException("Số điện thoại không được để trống");
        }

        LocalDate reqDate = null;
        LocalTime reqTime = null;

        if (request.ngay() != null && !request.ngay().isEmpty()) {
            reqDate = LocalDate.parse(request.ngay(), DateTimeFormatter.ISO_LOCAL_DATE);
        }
        
        if (request.gio() != null && !request.gio().isEmpty()) {
            reqTime = LocalTime.parse(request.gio(), DateTimeFormatter.ISO_LOCAL_TIME);
        }

        checkTimeConflict(customer.getId(), reqDate, reqTime);

        TestAppointment appointment = new TestAppointment();
        appointment.setMaDatLich(generateShortCode("DLXN"));
        appointment.setUserId(userId); 
        appointment.setCustomerId(customer.getId());
        appointment.setTestDate(reqDate);
        appointment.setTestTime(reqTime);
        
        if (request.idbacsi() != null && !request.idbacsi().isEmpty()) {
            appointment.setDoctorId(Integer.parseInt(request.idbacsi()));
        }
        
        appointment.setFacilityId(request.idcoso() != null ? request.idcoso() : "CS001");
        appointment.setNote(request.ghichu());
        appointment.setStatus("pending");

        TestAppointment savedAppointment = testAppointmentRepository.save(appointment);

        if (request.idxetnghiem() != null && !request.idxetnghiem().isEmpty()) {
            String[] testIds = request.idxetnghiem().split(",");
            Set<String> uniqueTestIds = new HashSet<>();
            
            for (String testId : testIds) {
                String cleanTestId = testId.trim();
                if (!cleanTestId.isEmpty() && uniqueTestIds.add(cleanTestId)) {
                    TestAppointmentItem item = new TestAppointmentItem();
                    item.setTestAppointmentId(savedAppointment.getId());
                    item.setTestId(cleanTestId);
                    item.setPrice(BigDecimal.ZERO);
                    item.setNote(request.ghichu());
                    
                    testAppointmentItemRepository.save(item);
                }
            }
        }

        Map<String, Object> result = new LinkedHashMap<>();
        result.put("id", savedAppointment.getId()); 
        result.put("type", "TEST");
        result.put("message", "Đã lưu lịch xét nghiệm thành công.");
        return result;
    }

    public List<String> getTakenTimes(String type, String doctorId, LocalDate date) {
        if (doctorId == null || doctorId.trim().isEmpty() || date == null) {
            return Collections.emptyList();
        }

        List<String> takenTimes = new ArrayList<>();

        if ("EXAMINATION".equalsIgnoreCase(type)) {
            try {
                Integer docId = Integer.parseInt(doctorId);
                List<ExaminationAppointment> appointments = examinationAppointmentRepository
                        .findByDoctorIdAndExaminationDate(docId, date);

                for (ExaminationAppointment appt : appointments) {
                    String status = appt.getStatus();
                    if ("cancelled".equalsIgnoreCase(status) || "huy".equalsIgnoreCase(status) || "no_show".equalsIgnoreCase(status)) {
                        continue;
                    }
                    if (appt.getExaminationTime() != null) {
                        String timeStr = appt.getExaminationTime().format(DateTimeFormatter.ofPattern("HH:mm"));
                        takenTimes.add(timeStr);
                    }
                }
            } catch (NumberFormatException e) {
                System.err.println("Lỗi parse Doctor ID: " + e.getMessage());
            }
        } 
        return takenTimes;
    }

    // SỬA LẠI HÀM NÀY ĐỂ THÊM SỐ ĐIỆN THOẠI
    public List<AppointmentResponse> getMyAppointments(Authentication authentication) {
        if (authentication == null || !authentication.isAuthenticated() || "anonymousUser".equals(authentication.getPrincipal())) {
            return Collections.emptyList();
        }

        Account loggedInAccount = accountRepository.findByEmail(authentication.getName()).orElse(null);
        if (loggedInAccount == null) return Collections.emptyList();

        Integer userId = loggedInAccount.getUserId();
        String customerId = loggedInAccount.getIdKhachHang();
        List<AppointmentResponse> myAppointments = new ArrayList<>();

        String sqlExam = "SELECT d.MaDatLich, d.NgayKham, d.GioKham, d.MaQR, d.GhiChu, " +
                         "k.IDKhachHang, k.TenKhachHang, k.SoDienThoai, d.IDBacSi, b.TenBacSi, " +
                         "COALESCE(lk.TrangThai, d.TrangThai) as RealStatus " +
                         "FROM datlichkham d " +
                         "JOIN khachhang k ON d.IDKhachHang = k.IDKhachHang " +
                         "LEFT JOIN bacsi b ON d.IDBacSi = b.IDBacSi " +
                         "LEFT JOIN luotkham lk ON d.IDDatLichKham = lk.IDDatLichKham " +
                         "WHERE d.UserID = ? OR (d.IDKhachHang = ? AND ? != '')";
        
        List<Map<String, Object>> exams = jdbcTemplate.queryForList(sqlExam, userId, customerId, customerId);
        for (Map<String, Object> row : exams) {
            myAppointments.add(new AppointmentResponse(
                    (String) row.get("MaDatLich"),
                    "EXAMINATION",
                    (String) row.get("IDKhachHang"),
                    (String) row.get("TenKhachHang"),
                    (String) row.get("SoDienThoai"), // Bổ sung
                    row.get("IDBacSi") != null ? String.valueOf(row.get("IDBacSi")) : null,
                    (String) row.get("TenBacSi"),
                    row.get("NgayKham") != null ? LocalDate.parse(row.get("NgayKham").toString()) : null,
                    row.get("GioKham") != null ? LocalTime.parse(row.get("GioKham").toString()) : null,
                    (String) row.get("RealStatus"), 
                    (String) row.get("MaQR"),
                    (String) row.get("GhiChu")
            ));
        }

        String sqlTest = "SELECT d.MaDatLich, d.NgayXetNghiem, d.GioXetNghiem, d.MaQR, d.GhiChu, " +
                         "k.IDKhachHang, k.TenKhachHang, k.SoDienThoai, d.IDBacSi, b.TenBacSi, " +
                         "COALESCE(lxn.TrangThai, d.TrangThai) as RealStatus " +
                         "FROM datlichxetnghiem d " +
                         "JOIN khachhang k ON d.IDKhachHang = k.IDKhachHang " +
                         "LEFT JOIN bacsi b ON d.IDBacSi = b.IDBacSi " +
                         "LEFT JOIN luotxetnghiem lxn ON d.IDDatLichXN = lxn.IDDatLichXN " +
                         "WHERE d.UserID = ? OR (d.IDKhachHang = ? AND ? != '')";
                         
        List<Map<String, Object>> tests = jdbcTemplate.queryForList(sqlTest, userId, customerId, customerId);
        for (Map<String, Object> row : tests) {
            myAppointments.add(new AppointmentResponse(
                    (String) row.get("MaDatLich"),
                    "TEST",
                    (String) row.get("IDKhachHang"),
                    (String) row.get("TenKhachHang"),
                    (String) row.get("SoDienThoai"), // Bổ sung
                    row.get("IDBacSi") != null ? String.valueOf(row.get("IDBacSi")) : null,
                    (String) row.get("TenBacSi") != null ? (String) row.get("TenBacSi") : "Được sắp xếp khi đến nơi",
                    row.get("NgayXetNghiem") != null ? LocalDate.parse(row.get("NgayXetNghiem").toString()) : null,
                    row.get("GioXetNghiem") != null ? LocalTime.parse(row.get("GioXetNghiem").toString()) : null,
                    (String) row.get("RealStatus"), 
                    (String) row.get("MaQR"),
                    (String) row.get("GhiChu")
            ));
        }

        myAppointments.sort((a, b) -> {
            if (a.appointmentDate() == null || b.appointmentDate() == null) return 0;
            int dateCompare = b.appointmentDate().compareTo(a.appointmentDate());
            if (dateCompare != 0) return dateCompare;
            
            if (a.appointmentTime() == null || b.appointmentTime() == null) return 0;
            return b.appointmentTime().compareTo(a.appointmentTime());
        });

        return myAppointments;
    }

    // THÊM HÀM MỚI: Lấy tất cả lịch hẹn trong ngày cho Admin
    public List<AppointmentResponse> getAllAppointments(LocalDate date) {
        List<AppointmentResponse> list = new ArrayList<>();

        String sqlExam = "SELECT d.MaDatLich, d.NgayKham, d.GioKham, d.MaQR, d.GhiChu, " +
                         "k.IDKhachHang, k.TenKhachHang, k.SoDienThoai, d.IDBacSi, b.TenBacSi, d.TrangThai " +
                         "FROM datlichkham d " +
                         "JOIN khachhang k ON d.IDKhachHang = k.IDKhachHang " +
                         "LEFT JOIN bacsi b ON d.IDBacSi = b.IDBacSi " +
                         "WHERE d.NgayKham = ?";
        List<Map<String, Object>> exams = jdbcTemplate.queryForList(sqlExam, date);
        for (Map<String, Object> row : exams) {
            list.add(new AppointmentResponse(
                    (String) row.get("MaDatLich"), "EXAMINATION", (String) row.get("IDKhachHang"),
                    (String) row.get("TenKhachHang"), (String) row.get("SoDienThoai"),
                    row.get("IDBacSi") != null ? String.valueOf(row.get("IDBacSi")) : null,
                    (String) row.get("TenBacSi"),
                    row.get("NgayKham") != null ? LocalDate.parse(row.get("NgayKham").toString()) : null,
                    row.get("GioKham") != null ? LocalTime.parse(row.get("GioKham").toString()) : null,
                    (String) row.get("TrangThai"), (String) row.get("MaQR"), (String) row.get("GhiChu")
            ));
        }

        String sqlTest = "SELECT d.MaDatLich, d.NgayXetNghiem, d.GioXetNghiem, d.MaQR, d.GhiChu, " +
                         "k.IDKhachHang, k.TenKhachHang, k.SoDienThoai, d.IDBacSi, b.TenBacSi, d.TrangThai " +
                         "FROM datlichxetnghiem d " +
                         "JOIN khachhang k ON d.IDKhachHang = k.IDKhachHang " +
                         "LEFT JOIN bacsi b ON d.IDBacSi = b.IDBacSi " +
                         "WHERE d.NgayXetNghiem = ?";
        List<Map<String, Object>> tests = jdbcTemplate.queryForList(sqlTest, date);
        for (Map<String, Object> row : tests) {
            list.add(new AppointmentResponse(
                    (String) row.get("MaDatLich"), "TEST", (String) row.get("IDKhachHang"),
                    (String) row.get("TenKhachHang"), (String) row.get("SoDienThoai"),
                    row.get("IDBacSi") != null ? String.valueOf(row.get("IDBacSi")) : null,
                    (String) row.get("TenBacSi") != null ? (String) row.get("TenBacSi") : "Được sắp xếp khi đến",
                    row.get("NgayXetNghiem") != null ? LocalDate.parse(row.get("NgayXetNghiem").toString()) : null,
                    row.get("GioXetNghiem") != null ? LocalTime.parse(row.get("GioXetNghiem").toString()) : null,
                    (String) row.get("TrangThai"), (String) row.get("MaQR"), (String) row.get("GhiChu")
            ));
        }

        list.sort((a, b) -> {
            if (a.appointmentTime() == null || b.appointmentTime() == null) return 0;
            return a.appointmentTime().compareTo(b.appointmentTime());
        });
        return list;
    }

    // THÊM HÀM MỚI: Xử lý Check-in và tạo hàng đợi
    @Transactional
    public Map<String, Object> checkInAppointmentAdmin(String maDatLich, String type) {
        if ("EXAMINATION".equalsIgnoreCase(type)) {
            jdbcTemplate.update("UPDATE datlichkham SET TrangThai = 'checked_in' WHERE MaDatLich = ?", maDatLich);
            Map<String, Object> apt = jdbcTemplate.queryForMap("SELECT IDDatLichKham, IDKhachHang, IDBacSi FROM datlichkham WHERE MaDatLich = ?", maDatLich);
            
            Integer maxStt = jdbcTemplate.queryForObject("SELECT MAX(SoThuTu) FROM luotkham lk JOIN datlichkham d ON lk.IDDatLichKham = d.IDDatLichKham WHERE d.IDBacSi = ? AND d.NgayKham = CURRENT_DATE", Integer.class, apt.get("IDBacSi"));
            int stt = (maxStt != null ? maxStt : 0) + 1;

            jdbcTemplate.update("INSERT INTO luotkham (IDDatLichKham, IDKhachHang, IDBacSi, SoThuTu, ThoiGianTiepNhan, TrangThai) VALUES (?, ?, ?, ?, NOW(), 'da_tiep_nhan')",
                apt.get("IDDatLichKham"), apt.get("IDKhachHang"), apt.get("IDBacSi"), stt);
                
        } else if ("TEST".equalsIgnoreCase(type)) {
            jdbcTemplate.update("UPDATE datlichxetnghiem SET TrangThai = 'checked_in' WHERE MaDatLich = ?", maDatLich);
            Map<String, Object> apt = jdbcTemplate.queryForMap("SELECT IDDatLichXN, IDKhachHang FROM datlichxetnghiem WHERE MaDatLich = ?", maDatLich);
            
            Integer maxStt = jdbcTemplate.queryForObject("SELECT MAX(SoThuTu) FROM luotxetnghiem lx JOIN datlichxetnghiem d ON lx.IDDatLichXN = d.IDDatLichXN WHERE d.NgayXetNghiem = CURRENT_DATE", Integer.class);
            int stt = (maxStt != null ? maxStt : 0) + 1;

            jdbcTemplate.update("INSERT INTO luotxetnghiem (IDDatLichXN, IDKhachHang, SoThuTu, ThoiGianTiepNhan, TrangThai) VALUES (?, ?, ?, NOW(), 'da_tiep_nhan')",
                apt.get("IDDatLichXN"), apt.get("IDKhachHang"), stt);
        }
        
        return Map.of("success", true, "message", "Check-in thành công");
    }

    public Map<String, Object> getAppointmentDetail(String id) {
        Map<String, Object> result = new LinkedHashMap<>();
        
        if (id == null || id.trim().isEmpty() || "undefined".equals(id.trim())) {
            result.put("error", "Mã lịch hẹn không hợp lệ hoặc chưa được cập nhật.");
            return result;
        }

        id = id.trim();
        Long numericId = null;
        try {
            numericId = Long.parseLong(id);
        } catch (NumberFormatException e) {
        }

        String sqlExam = "SELECT d.MaDatLich, d.NgayKham, d.GioKham, d.TrangThai, d.CreatedAt, " +
                     "k.TenKhachHang, b.TenBacSi, d.IDDatLichKham " +
                     "FROM datlichkham d " +
                     "LEFT JOIN khachhang k ON d.IDKhachHang = k.IDKhachHang " +
                     "LEFT JOIN bacsi b ON d.IDBacSi = b.IDBacSi " +
                     "WHERE d.MaDatLich = ? OR d.IDDatLichKham = ?";
        try {
            List<Map<String, Object>> examRows = jdbcTemplate.queryForList(sqlExam, id, numericId != null ? numericId : -1);
            if (!examRows.isEmpty()) {
                Map<String, Object> appointment = examRows.get(0);
                
                result.put("id", appointment.get("MaDatLich"));
                result.put("type", "EXAMINATION");
                result.put("date", appointment.get("NgayKham"));
                result.put("time", appointment.get("GioKham"));
                result.put("status", appointment.get("TrangThai"));
                result.put("customerName", appointment.get("TenKhachHang"));
                result.put("doctorName", appointment.get("TenBacSi"));

                Long idDatLichKham = ((Number) appointment.get("IDDatLichKham")).longValue();
                List<Map<String, Object>> timeline = new ArrayList<>();
                timeline.add(Map.of("time", appointment.get("CreatedAt"), "event", "Đặt lịch khám thành công"));

                String sqlLuotKham = "SELECT IDLuotKham, ThoiGianTiepNhan, ThoiGianBatDau, ThoiGianKetThuc FROM luotkham WHERE IDDatLichKham = ?";
                List<Map<String, Object>> luotKhams = jdbcTemplate.queryForList(sqlLuotKham, idDatLichKham);
                Long idLuotKham = null;
                
                if (!luotKhams.isEmpty()) {
                    Map<String, Object> lk = luotKhams.get(0);
                    idLuotKham = ((Number) lk.get("IDLuotKham")).longValue();
                    if (lk.get("ThoiGianTiepNhan") != null) timeline.add(Map.of("time", lk.get("ThoiGianTiepNhan"), "event", "Check-in tại quầy"));
                    if (lk.get("ThoiGianBatDau") != null) timeline.add(Map.of("time", lk.get("ThoiGianBatDau"), "event", "Bác sĩ bắt đầu khám"));
                    if (lk.get("ThoiGianKetThuc") != null) timeline.add(Map.of("time", lk.get("ThoiGianKetThuc"), "event", "Hoàn tất khám"));
                }
                result.put("timeline", timeline);

                List<Map<String, Object>> testOrders = new ArrayList<>();
                if (idLuotKham != null) {
                    String sqlKham = "SELECT IDKham FROM kham WHERE IDLuotKham = ?";
                    List<Map<String, Object>> khams = jdbcTemplate.queryForList(sqlKham, idLuotKham);
                    if (!khams.isEmpty()) {
                        Long idKham = ((Number) khams.get(0).get("IDKham")).longValue();
                        String sqlPhieu = "SELECT IDPhieuXetNghiem, TrangThai, NgayTao FROM phieuxetnghiem WHERE IDKham = ?";
                        testOrders = jdbcTemplate.queryForList(sqlPhieu, idKham);
                        
                        for (Map<String, Object> phieu : testOrders) {
                            String sqlCT = "SELECT l.TenXetNghiem FROM ctphieuxetnghiem ct JOIN loaixetnghiem l ON ct.IDXetNghiem = l.IDXetNghiem WHERE ct.IDPhieuXetNghiem = ?";
                            List<String> tests = jdbcTemplate.queryForList(sqlCT, String.class, phieu.get("IDPhieuXetNghiem"));
                            phieu.put("tests", tests);
                        }
                    }
                }
                result.put("testOrders", testOrders);
                return result; 
            }
        } catch (Exception e) {
            System.err.println("Lỗi truy vấn lịch khám: " + e.getMessage());
        }

        String sqlTest = "SELECT d.MaDatLich, d.NgayXetNghiem, d.GioXetNghiem, d.TrangThai, d.CreatedAt, " +
                     "k.TenKhachHang, d.IDDatLichXN, b.TenBacSi " +
                     "FROM datlichxetnghiem d " +
                     "LEFT JOIN khachhang k ON d.IDKhachHang = k.IDKhachHang " +
                     "LEFT JOIN bacsi b ON d.IDBacSi = b.IDBacSi " +
                     "WHERE d.MaDatLich = ? OR d.IDDatLichXN = ?";
        try {
            List<Map<String, Object>> testRows = jdbcTemplate.queryForList(sqlTest, id, numericId != null ? numericId : -1);
            if (!testRows.isEmpty()) {
                Map<String, Object> appointment = testRows.get(0);
                
                result.put("id", appointment.get("MaDatLich"));
                result.put("type", "TEST");
                result.put("date", appointment.get("NgayXetNghiem"));
                result.put("time", appointment.get("GioXetNghiem"));
                result.put("status", appointment.get("TrangThai"));
                result.put("customerName", appointment.get("TenKhachHang"));
                result.put("doctorName", appointment.get("TenBacSi") != null ? appointment.get("TenBacSi") : "Được sắp xếp khi đến nơi");

                Long idDatLichXN = ((Number) appointment.get("IDDatLichXN")).longValue();
                List<Map<String, Object>> timeline = new ArrayList<>();
                timeline.add(Map.of("time", appointment.get("CreatedAt"), "event", "Đặt lịch xét nghiệm thành công"));

                String sqlLuotXN = "SELECT IDLuotXetNghiem, ThoiGianTiepNhan, ThoiGianBatDau, ThoiGianKetThuc FROM luotxetnghiem WHERE IDDatLichXN = ?";
                List<Map<String, Object>> luotXNs = jdbcTemplate.queryForList(sqlLuotXN, idDatLichXN);
                
                if (!luotXNs.isEmpty()) {
                    Map<String, Object> lxn = luotXNs.get(0);
                    if (lxn.get("ThoiGianTiepNhan") != null) timeline.add(Map.of("time", lxn.get("ThoiGianTiepNhan"), "event", "Check-in tại quầy"));
                    if (lxn.get("ThoiGianBatDau") != null) timeline.add(Map.of("time", lxn.get("ThoiGianBatDau"), "event", "Bắt đầu lấy mẫu"));
                    if (lxn.get("ThoiGianKetThuc") != null) timeline.add(Map.of("time", lxn.get("ThoiGianKetThuc"), "event", "Đã có kết quả"));
                }
                result.put("timeline", timeline);
                
                String sqlTestItems = "SELECT l.TenXetNghiem FROM ctdatlichxetnghiem ct JOIN loaixetnghiem l ON ct.IDXetNghiem = l.IDXetNghiem WHERE ct.IDDatLichXN = ?";
                List<String> registeredTests = jdbcTemplate.queryForList(sqlTestItems, String.class, idDatLichXN);
                result.put("registeredTests", registeredTests);

                return result;
            }
        } catch (Exception e) {
            System.err.println("Lỗi hệ thống khi tải lịch xét nghiệm: " + e.getMessage());
        }

        result.put("error", "Không tìm thấy dữ liệu cho mã lịch hẹn: " + id);
        return result;
    }

    public Map<String, Object> createQuickAppointment(String hoten, String email, String ngay, String gio, String idbacsi, String ghichu) {
        return new LinkedHashMap<>();
    }

    public Map<String, Object> updateAppointment(String id, UpdateAppointmentRequest request) {
        return new LinkedHashMap<>();
    }

    public Map<String, Object> cancelAppointment(String id) {
        return new LinkedHashMap<>();
    }
}