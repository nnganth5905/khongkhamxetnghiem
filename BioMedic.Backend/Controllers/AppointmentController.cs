using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BioMedic.Backend.Data;
using BioMedic.Backend.Entities;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace BioMedic.Backend.Controllers
{
    [Route("api/appointments")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AppointmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("options")]
        public async Task<IActionResult> GetAppointmentOptions()
        {
            var departments = await _context.Chuyenkhoas
                .AsNoTracking()
                .Where(specialty => specialty.Status == "yes")
                .ToDictionaryAsync(
                    specialty => specialty.IdchuyenKhoa,
                    specialty => specialty.TenChuyenKhoa);

            var doctors = await _context.Bacsis
                .AsNoTracking()
                .Where(doctor => doctor.TrangThai == "active")
                .Select(doctor => new
                {
                    id = doctor.IdbacSi.ToString(),
                    name = doctor.TenBacSi,
                    specialtyId = doctor.KhoaId,
                })
                .ToListAsync();

            var tests = await _context.Loaixetnghiems
                .AsNoTracking()
                .Select(test => new
                {
                    id = test.IdxetNghiem,
                    name = test.TenXetNghiem,
                    categoryId = test.ChuyenKhoaId,
                    price = test.Gia,
                    description = test.MoTa,
                    sampleType = test.LoaiMauMacDinh,
                })
                .ToListAsync();

            return Ok(new
            {
                departments,
                doctors,
                tests,
                facilities = Array.Empty<object>(),
            });
        }

        [HttpGet("my")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> GetMyAppointments()
        {
            if (!int.TryParse(User.FindFirstValue("userId"), out var userId))
            {
                return Unauthorized(new { success = false, message = "Không xác định được tài khoản." });
            }

            var customerId = await _context.Users
                .AsNoTracking()
                .Where(user => user.UserId == userId && user.IsActive == true)
                .Select(user => user.IdkhachHang)
                .FirstOrDefaultAsync();

            if (string.IsNullOrWhiteSpace(customerId))
            {
                return Ok(Array.Empty<AppointmentListItem>());
            }

            var appointments = await ExaminationAppointments(userId, customerId).ToListAsync();
            var testAppointments = await TestAppointments(userId, customerId).ToListAsync();

            return Ok(appointments
                .Concat(testAppointments)
                .OrderByDescending(appointment => appointment.AppointmentDate)
                .ThenByDescending(appointment => appointment.AppointmentTime)
                .ToList());
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN,RECEPTIONIST")]
        public async Task<IActionResult> GetAppointments([FromQuery] DateOnly? date)
        {
            var appointmentDate = date ?? DateOnly.FromDateTime(DateTime.Today);
            var appointments = await ExaminationAppointments(appointmentDate: appointmentDate)
                .ToListAsync();
            var testAppointments = await TestAppointments(appointmentDate: appointmentDate)
                .ToListAsync();

            return Ok(appointments
                .Concat(testAppointments)
                .OrderBy(appointment => appointment.AppointmentTime)
                .ToList());
        }

        [HttpGet("{id}", Order = 1)]
        [Authorize(Roles = "CUSTOMER,ADMIN,RECEPTIONIST,DOCTOR")]
        public async Task<IActionResult> GetAppointmentDetail(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || id == "undefined")
            {
                return BadRequest(new { success = false, message = "Mã lịch hẹn không hợp lệ." });
            }

            ulong? numericId = ulong.TryParse(id, out var parsedId) ? parsedId : null;
            var examination = await _context.Datlichkhams
                .AsNoTracking()
                .Where(appointment => appointment.MaDatLich == id ||
                    (numericId.HasValue && appointment.IddatLichKham == numericId.Value))
                .Select(appointment => new
                {
                    appointment.IddatLichKham,
                    appointment.UserId,
                    appointment.IdkhachHang,
                    appointment.MaDatLich,
                    appointment.NgayKham,
                    appointment.GioKham,
                    appointment.TrangThai,
                    appointment.CreatedAt,
                    appointment.MaQr,
                    appointment.GhiChu,
                    CustomerName = appointment.IdkhachHangNavigation.TenKhachHang,
                    CustomerPhone = appointment.IdkhachHangNavigation.SoDienThoai,
                    DoctorName = appointment.IdbacSiNavigation.TenBacSi,
                })
                .FirstOrDefaultAsync();

            if (examination is not null)
            {
                if (!await CanAccessAppointment(examination.UserId, examination.IdkhachHang))
                {
                    return Forbid();
                }

                var visit = await _context.Luotkhams
                    .AsNoTracking()
                    .FirstOrDefaultAsync(item => item.IddatLichKham == examination.IddatLichKham);
                var timeline = new List<object>
                {
                    new { time = examination.CreatedAt, @event = "Đặt lịch khám thành công" },
                };
                if (visit?.ThoiGianTiepNhan is not null)
                {
                    timeline.Add(new { time = visit.ThoiGianTiepNhan, @event = "Check-in tại quầy" });
                }
                if (visit?.ThoiGianBatDau is not null)
                {
                    timeline.Add(new { time = visit.ThoiGianBatDau, @event = "Bác sĩ bắt đầu khám" });
                }
                if (visit?.ThoiGianKetThuc is not null)
                {
                    timeline.Add(new { time = visit.ThoiGianKetThuc, @event = "Hoàn tất khám" });
                }

                return Ok(new
                {
                    id = examination.MaDatLich,
                    type = "EXAMINATION",
                    patientCode = examination.IdkhachHang,
                    date = examination.NgayKham,
                    appointmentDate = examination.NgayKham,
                    time = examination.GioKham,
                    appointmentTime = examination.GioKham,
                    status = visit?.TrangThai ?? examination.TrangThai,
                    customerName = examination.CustomerName,
                    customerPhone = examination.CustomerPhone,
                    doctorName = examination.DoctorName,
                    qrCode = examination.MaQr,
                    note = examination.GhiChu,
                    timeline,
                    testOrders = Array.Empty<object>(),
                });
            }

            var testAppointment = await _context.Datlichxetnghiems
                .AsNoTracking()
                .Where(appointment => appointment.MaDatLich == id ||
                    (numericId.HasValue && appointment.IddatLichXn == numericId.Value))
                .Select(appointment => new
                {
                    appointment.IddatLichXn,
                    appointment.UserId,
                    appointment.IdkhachHang,
                    appointment.MaDatLich,
                    appointment.NgayXetNghiem,
                    appointment.GioXetNghiem,
                    appointment.TrangThai,
                    appointment.CreatedAt,
                    appointment.MaQr,
                    appointment.GhiChu,
                    CustomerName = appointment.IdkhachHangNavigation.TenKhachHang,
                    CustomerPhone = appointment.IdkhachHangNavigation.SoDienThoai,
                    DoctorName = appointment.IdbacSiNavigation == null
                        ? "Được sắp xếp khi đến nơi"
                        : appointment.IdbacSiNavigation.TenBacSi,
                })
                .FirstOrDefaultAsync();

            if (testAppointment is null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy lịch hẹn." });
            }
            if (!await CanAccessAppointment(testAppointment.UserId, testAppointment.IdkhachHang))
            {
                return Forbid();
            }

            var testVisit = await _context.Luotxetnghiems
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.IddatLichXn == testAppointment.IddatLichXn);
            var registeredTests = await _context.Ctdatlichxetnghiems
                .AsNoTracking()
                .Where(item => item.IddatLichXn == testAppointment.IddatLichXn)
                .Select(item => item.IdxetNghiemNavigation.TenXetNghiem)
                .ToListAsync();
            var testTimeline = new List<object>
            {
                new { time = testAppointment.CreatedAt, @event = "Đặt lịch xét nghiệm thành công" },
            };
            if (testVisit?.ThoiGianTiepNhan is not null)
            {
                testTimeline.Add(new { time = testVisit.ThoiGianTiepNhan, @event = "Check-in tại quầy" });
            }
            if (testVisit?.ThoiGianBatDau is not null)
            {
                testTimeline.Add(new { time = testVisit.ThoiGianBatDau, @event = "Bắt đầu lấy mẫu" });
            }
            if (testVisit?.ThoiGianKetThuc is not null)
            {
                testTimeline.Add(new { time = testVisit.ThoiGianKetThuc, @event = "Hoàn tất quy trình" });
            }

            return Ok(new
            {
                id = testAppointment.MaDatLich,
                type = "TEST",
                patientCode = testAppointment.IdkhachHang,
                date = testAppointment.NgayXetNghiem,
                appointmentDate = testAppointment.NgayXetNghiem,
                time = testAppointment.GioXetNghiem,
                appointmentTime = testAppointment.GioXetNghiem,
                status = testVisit?.TrangThai ?? testAppointment.TrangThai,
                customerName = testAppointment.CustomerName,
                customerPhone = testAppointment.CustomerPhone,
                doctorName = testAppointment.DoctorName,
                qrCode = testAppointment.MaQr,
                note = testAppointment.GhiChu,
                timeline = testTimeline,
                registeredTests,
            });
        }

        [HttpPost("{id}/check-in")]
        [Authorize(Roles = "ADMIN,RECEPTIONIST")]
        public async Task<IActionResult> CheckInAppointment(
            string id,
            [FromBody] CheckInRequest request)
        {
            var type = request.Type?.Trim().ToUpperInvariant();
            if (type is not ("EXAMINATION" or "TEST"))
            {
                return BadRequest(new { success = false, message = "Loại lịch hẹn không hợp lệ." });
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();
            var receivedAt = DateTime.Now;

            if (type == "EXAMINATION")
            {
                var appointment = await _context.Datlichkhams
                    .FirstOrDefaultAsync(item => item.MaDatLich == id);
                if (appointment is null)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy lịch khám." });
                }

                var existingVisit = await _context.Luotkhams
                    .AnyAsync(visit => visit.IddatLichKham == appointment.IddatLichKham);
                if (existingVisit)
                {
                    return Conflict(new { success = false, message = "Lịch khám đã được check-in." });
                }

                var today = DateOnly.FromDateTime(receivedAt);
                var lastQueueNumber = await _context.Luotkhams
                    .Where(visit => visit.IdbacSi == appointment.IdbacSi &&
                        visit.IddatLichKhamNavigation.NgayKham == today)
                    .MaxAsync(visit => (int?)visit.SoThuTu) ?? 0;

                appointment.TrangThai = "checked_in";
                _context.Luotkhams.Add(new Luotkham
                {
                    IddatLichKham = appointment.IddatLichKham,
                    IdkhachHang = appointment.IdkhachHang,
                    IdbacSi = appointment.IdbacSi,
                    SoThuTu = lastQueueNumber + 1,
                    ThoiGianTiepNhan = receivedAt,
                    TrangThai = "da_tiep_nhan",
                });
            }
            else
            {
                var appointment = await _context.Datlichxetnghiems
                    .FirstOrDefaultAsync(item => item.MaDatLich == id);
                if (appointment is null)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy lịch xét nghiệm." });
                }

                var existingVisit = await _context.Luotxetnghiems
                    .AnyAsync(visit => visit.IddatLichXn == appointment.IddatLichXn);
                if (existingVisit)
                {
                    return Conflict(new { success = false, message = "Lịch xét nghiệm đã được check-in." });
                }

                var today = DateOnly.FromDateTime(receivedAt);
                var lastQueueNumber = await _context.Luotxetnghiems
                    .Where(visit => visit.IddatLichXnNavigation.NgayXetNghiem == today)
                    .MaxAsync(visit => (int?)visit.SoThuTu) ?? 0;

                appointment.TrangThai = "checked_in";
                _context.Luotxetnghiems.Add(new Luotxetnghiem
                {
                    IddatLichXn = appointment.IddatLichXn,
                    IdkhachHang = appointment.IdkhachHang,
                    SoThuTu = lastQueueNumber + 1,
                    ThoiGianTiepNhan = receivedAt,
                    TrangThai = "da_tiep_nhan",
                });
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return Ok(new { success = true, message = "Check-in thành công." });
        }

        [HttpPost("tests")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateTestAppointment([FromBody] TestAppointmentRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Hoten) ||
                string.IsNullOrWhiteSpace(request.Sodienthoai) ||
                string.IsNullOrWhiteSpace(request.Ngay) ||
                string.IsNullOrWhiteSpace(request.Gio) ||
                string.IsNullOrWhiteSpace(request.Idxetnghiem))
            {
                return BadRequest(new { success = false, message = "Vui lòng nhập đủ thông tin đặt lịch." });
            }

            if (!DateOnly.TryParseExact(
                    request.Ngay,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var appointmentDate) ||
                !TimeOnly.TryParse(
                    request.Gio,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var appointmentTime))
            {
                return BadRequest(new { success = false, message = "Ngày hoặc giờ đặt lịch không hợp lệ." });
            }

            int? userId = null;
            if (int.TryParse(User.FindFirstValue("userId"), out var authenticatedUserId))
            {
                userId = authenticatedUserId;
            }

            var customerName = request.Hoten.Trim();
            var phone = request.Sodienthoai.Trim();
            var gender = NormalizeGender(request.Gioitinh);
            DateOnly? birthDate = null;
            if (!string.IsNullOrWhiteSpace(request.Ngaysinh) &&
                DateOnly.TryParseExact(
                    request.Ngaysinh,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var parsedBirthDate))
            {
                birthDate = parsedBirthDate;
            }

            var testIds = request.Idxetnghiem
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
            if (testIds.Length == 0)
            {
                return BadRequest(new { success = false, message = "Vui lòng chọn ít nhất một xét nghiệm." });
            }

            var tests = await _context.Loaixetnghiems
                .AsNoTracking()
                .Where(test => testIds.Contains(test.IdxetNghiem))
                .Select(test => test.IdxetNghiem)
                .ToListAsync();
            if (tests.Count != testIds.Length)
            {
                return BadRequest(new { success = false, message = "Một hoặc nhiều xét nghiệm không tồn tại." });
            }

            int? doctorId = null;
            if (!string.IsNullOrWhiteSpace(request.Idbacsi))
            {
                if (!int.TryParse(request.Idbacsi, out var parsedDoctorId) ||
                    !await _context.Bacsis.AnyAsync(doctor => doctor.IdbacSi == parsedDoctorId))
                {
                    return BadRequest(new { success = false, message = "Bác sĩ được chọn không tồn tại." });
                }
                doctorId = parsedDoctorId;
            }

            var facilityId = string.IsNullOrWhiteSpace(request.Idcoso) ? "CS001" : request.Idcoso.Trim();
            if (!await _context.Cosos.AnyAsync(facility => facility.CoSoId == facilityId))
            {
                return BadRequest(new { success = false, message = "Cơ sở tiếp nhận không tồn tại." });
            }

            var customer = await _context.Khachhangs.FirstOrDefaultAsync(existing =>
                existing.TenKhachHang == customerName &&
                existing.SoDienThoai == phone &&
                existing.GioiTinh == gender &&
                existing.NgaySinh == birthDate);

            await using var transaction = await _context.Database.BeginTransactionAsync();

            if (customer is null)
            {
                customer = new Khachhang
                {
                    IdkhachHang = $"KH{Guid.NewGuid():N}"[..10].ToUpperInvariant(),
                    TenKhachHang = customerName,
                    Email = request.Email?.Trim(),
                    SoDienThoai = phone,
                    GioiTinh = gender,
                    NgaySinh = birthDate,
                    Status = "yes",
                };
                _context.Khachhangs.Add(customer);
                await _context.SaveChangesAsync();
            }

            var appointment = new Datlichxetnghiem
            {
                MaDatLich = $"DLXN-{Guid.NewGuid():N}"[..11].ToUpperInvariant(),
                UserId = userId,
                IdkhachHang = customer.IdkhachHang,
                IdbacSi = doctorId,
                CoSoId = facilityId,
                NgayXetNghiem = appointmentDate,
                GioXetNghiem = appointmentTime,
                GhiChu = request.Ghichu,
                TrangThai = "pending",
                StatusMail = "pending",
            };

            var hasExamConflict = await _context.Datlichkhams.AnyAsync(existing =>
                existing.IdkhachHang == customer.IdkhachHang &&
                existing.NgayKham == appointmentDate &&
                existing.GioKham == appointmentTime &&
                existing.TrangThai != "huy" && existing.TrangThai != "cancelled");
            var hasTestConflict = await _context.Datlichxetnghiems.AnyAsync(existing =>
                existing.IdkhachHang == customer.IdkhachHang &&
                existing.NgayXetNghiem == appointmentDate &&
                existing.GioXetNghiem == appointmentTime &&
                existing.TrangThai != "huy" && existing.TrangThai != "cancelled");

            if (hasExamConflict || hasTestConflict)
            {
                await transaction.RollbackAsync();
                return Conflict(new { success = false, message = "Bạn đã có lịch hẹn vào khung giờ này." });
            }

            _context.Datlichxetnghiems.Add(appointment);
            await _context.SaveChangesAsync();

            foreach (var testId in testIds)
            {
                _context.Ctdatlichxetnghiems.Add(new Ctdatlichxetnghiem
                {
                    IddatLichXn = appointment.IddatLichXn,
                    IdxetNghiem = testId,
                    DonGia = 0,
                    GhiChu = request.Ghichu,
                });
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return StatusCode(StatusCodes.Status201Created, new
            {
                id = appointment.IddatLichXn,
                type = "TEST",
                message = "Đã lưu lịch xét nghiệm thành công.",
            });
        }

        [HttpPost("examinations")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateExaminationAppointment(
            [FromBody] ExaminationAppointmentRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Hoten) ||
                string.IsNullOrWhiteSpace(request.Sodienthoai) ||
                string.IsNullOrWhiteSpace(request.Ngay) ||
                string.IsNullOrWhiteSpace(request.Gio) ||
                string.IsNullOrWhiteSpace(request.Idbacsi) ||
                string.IsNullOrWhiteSpace(request.Idchuyenkhoa))
            {
                return BadRequest(new { success = false, message = "Vui lòng nhập đủ thông tin đặt lịch khám." });
            }

            if (!DateOnly.TryParseExact(
                    request.Ngay,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var appointmentDate) ||
                !TimeOnly.TryParse(
                    request.Gio,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var appointmentTime))
            {
                return BadRequest(new { success = false, message = "Ngày hoặc giờ đặt lịch không hợp lệ." });
            }

            if (!int.TryParse(request.Idbacsi, out var doctorId) ||
                !await _context.Bacsis.AnyAsync(doctor => doctor.IdbacSi == doctorId))
            {
                return BadRequest(new { success = false, message = "Bác sĩ được chọn không tồn tại." });
            }

            var specialtyId = request.Idchuyenkhoa.Trim();
            if (!await _context.Chuyenkhoas.AnyAsync(specialty => specialty.IdchuyenKhoa == specialtyId))
            {
                return BadRequest(new { success = false, message = "Chuyên khoa được chọn không tồn tại." });
            }

            var facilityId = string.IsNullOrWhiteSpace(request.Idcoso) ? "CS001" : request.Idcoso.Trim();
            if (!await _context.Cosos.AnyAsync(facility => facility.CoSoId == facilityId))
            {
                return BadRequest(new { success = false, message = "Cơ sở tiếp nhận không tồn tại." });
            }

            var customerName = request.Hoten.Trim();
            var phone = request.Sodienthoai.Trim();
            var gender = NormalizeGender(request.Gioitinh);
            DateOnly? birthDate = null;
            if (!string.IsNullOrWhiteSpace(request.Ngaysinh) &&
                DateOnly.TryParseExact(
                    request.Ngaysinh,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var parsedBirthDate))
            {
                birthDate = parsedBirthDate;
            }

            var customer = await _context.Khachhangs.FirstOrDefaultAsync(existing =>
                existing.TenKhachHang == customerName &&
                existing.SoDienThoai == phone &&
                existing.GioiTinh == gender &&
                existing.NgaySinh == birthDate);

            await using var transaction = await _context.Database.BeginTransactionAsync();

            if (customer is null)
            {
                customer = new Khachhang
                {
                    IdkhachHang = $"KH{Guid.NewGuid():N}"[..10].ToUpperInvariant(),
                    TenKhachHang = customerName,
                    Email = request.Email?.Trim(),
                    SoDienThoai = phone,
                    GioiTinh = gender,
                    NgaySinh = birthDate,
                    Status = "yes",
                };
                _context.Khachhangs.Add(customer);
                await _context.SaveChangesAsync();
            }

            var hasExamConflict = await _context.Datlichkhams.AnyAsync(existing =>
                existing.IdkhachHang == customer.IdkhachHang &&
                existing.NgayKham == appointmentDate &&
                existing.GioKham == appointmentTime &&
                existing.TrangThai != "huy" && existing.TrangThai != "cancelled");
            var hasTestConflict = await _context.Datlichxetnghiems.AnyAsync(existing =>
                existing.IdkhachHang == customer.IdkhachHang &&
                existing.NgayXetNghiem == appointmentDate &&
                existing.GioXetNghiem == appointmentTime &&
                existing.TrangThai != "huy" && existing.TrangThai != "cancelled");

            if (hasExamConflict || hasTestConflict)
            {
                await transaction.RollbackAsync();
                return Conflict(new { success = false, message = "Bạn đã có lịch hẹn vào khung giờ này." });
            }

            var reasonAndNote = string.Join(
                Environment.NewLine,
                new[] { request.Lydokham?.Trim(), request.Ghichu?.Trim() }
                    .Where(value => !string.IsNullOrWhiteSpace(value)));
            var appointment = new Datlichkham
            {
                MaDatLich = $"DLK-{Guid.NewGuid():N}"[..10].ToUpperInvariant(),
                UserId = int.TryParse(User.FindFirstValue("userId"), out var userId) ? userId : null,
                IdkhachHang = customer.IdkhachHang,
                IdchuyenKhoa = specialtyId,
                IdbacSi = doctorId,
                CoSoId = facilityId,
                NgayKham = appointmentDate,
                GioKham = appointmentTime,
                GhiChu = string.IsNullOrWhiteSpace(reasonAndNote) ? null : reasonAndNote,
                TrangThai = "pending",
                StatusMail = "pending",
            };

            _context.Datlichkhams.Add(appointment);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return StatusCode(StatusCodes.Status201Created, new
            {
                id = appointment.IddatLichKham,
                type = "EXAMINATION",
                message = "Đã lưu lịch khám thành công.",
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> UpdateAppointment(string id, [FromBody] UpdateAppointmentRequest request)
        {
            if (!DateOnly.TryParseExact(request.NewDate, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var newDate) ||
                !TimeOnly.TryParse(request.NewTime, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var newTime))
            {
                return BadRequest(new { success = false, message = "Ngày hoặc giờ mới không hợp lệ." });
            }
            if (newDate < DateOnly.FromDateTime(DateTime.Today))
            {
                return BadRequest(new { success = false, message = "Không thể dời lịch về ngày trong quá khứ." });
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();
            var userId = int.TryParse(User.FindFirstValue("userId"), out var parsedUserId)
                ? parsedUserId
                : (int?)null;

            var examination = await _context.Datlichkhams
                .FirstOrDefaultAsync(appointment => appointment.MaDatLich == id);
            if (examination is not null)
            {
                if (!await CanAccessAppointment(examination.UserId, examination.IdkhachHang))
                {
                    return Forbid();
                }
                if (IsAppointmentLocked(examination.NgayKham, examination.TrangThai))
                {
                    return Conflict(new { success = false, message = "Lịch đã qua hạn hoặc đã check-in, không thể chỉnh sửa." });
                }
                var conflict = await _context.Datlichkhams.AnyAsync(other =>
                    other.MaDatLich != id && other.IdkhachHang == examination.IdkhachHang &&
                    other.NgayKham == newDate && other.GioKham == newTime &&
                    other.TrangThai != "cancelled" && other.TrangThai != "huy") ||
                    await _context.Datlichxetnghiems.AnyAsync(other =>
                        other.IdkhachHang == examination.IdkhachHang &&
                        other.NgayXetNghiem == newDate && other.GioXetNghiem == newTime &&
                        other.TrangThai != "cancelled" && other.TrangThai != "huy");
                if (conflict)
                {
                    return Conflict(new { success = false, message = "Khách hàng đã có lịch hẹn vào khung giờ này." });
                }

                var oldStatus = examination.TrangThai;
                examination.NgayKham = newDate;
                examination.GioKham = newTime;
                examination.GhiChu = request.Note;
                examination.TrangThai = "pending";
                AddAppointmentAudit(examination.IdkhachHang, "datlichkham",
                    examination.IddatLichKham.ToString(), "Khách hàng thay đổi lịch hẹn",
                    oldStatus, examination.TrangThai, userId,
                    $"Khách hàng dời lịch sang {newTime:HH:mm} ngày {newDate:yyyy-MM-dd}");
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok(new { message = "Thay đổi lịch hẹn thành công.", maDatLich = id });
            }

            var testAppointment = await _context.Datlichxetnghiems
                .FirstOrDefaultAsync(appointment => appointment.MaDatLich == id);
            if (testAppointment is null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy lịch hẹn." });
            }
            if (!await CanAccessAppointment(testAppointment.UserId, testAppointment.IdkhachHang))
            {
                return Forbid();
            }
            if (IsAppointmentLocked(testAppointment.NgayXetNghiem, testAppointment.TrangThai))
            {
                return Conflict(new { success = false, message = "Lịch đã qua hạn hoặc đã check-in, không thể chỉnh sửa." });
            }

            var testConflict = await _context.Datlichkhams.AnyAsync(other =>
                other.IdkhachHang == testAppointment.IdkhachHang &&
                other.NgayKham == newDate && other.GioKham == newTime &&
                other.TrangThai != "cancelled" && other.TrangThai != "huy") ||
                await _context.Datlichxetnghiems.AnyAsync(other =>
                    other.MaDatLich != id && other.IdkhachHang == testAppointment.IdkhachHang &&
                    other.NgayXetNghiem == newDate && other.GioXetNghiem == newTime &&
                    other.TrangThai != "cancelled" && other.TrangThai != "huy");
            if (testConflict)
            {
                return Conflict(new { success = false, message = "Khách hàng đã có lịch hẹn vào khung giờ này." });
            }

            var oldTestStatus = testAppointment.TrangThai;
            testAppointment.NgayXetNghiem = newDate;
            testAppointment.GioXetNghiem = newTime;
            testAppointment.GhiChu = request.Note;
            testAppointment.TrangThai = "pending";
            AddAppointmentAudit(testAppointment.IdkhachHang, "datlichxetnghiem",
                testAppointment.IddatLichXn.ToString(), "Khách hàng thay đổi lịch hẹn",
                oldTestStatus, testAppointment.TrangThai, userId,
                $"Khách hàng dời lịch sang {newTime:HH:mm} ngày {newDate:yyyy-MM-dd}");
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return Ok(new { message = "Thay đổi lịch hẹn thành công.", maDatLich = id });
        }

        [HttpPost("{id}/cancel")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> CancelAppointment(string id)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            var userId = int.TryParse(User.FindFirstValue("userId"), out var parsedUserId)
                ? parsedUserId
                : (int?)null;

            var examination = await _context.Datlichkhams
                .FirstOrDefaultAsync(appointment => appointment.MaDatLich == id);
            if (examination is not null)
            {
                if (!await CanAccessAppointment(examination.UserId, examination.IdkhachHang))
                {
                    return Forbid();
                }
                if (IsAppointmentLocked(examination.NgayKham, examination.TrangThai, allowPast: true))
                {
                    return Conflict(new { success = false, message = "Không thể hủy lịch đã check-in hoặc hoàn thành." });
                }

                var oldStatus = examination.TrangThai;
                examination.TrangThai = "cancelled";
                AddAppointmentAudit(examination.IdkhachHang, "datlichkham",
                    examination.IddatLichKham.ToString(), "Khách hàng hủy lịch hẹn",
                    oldStatus, examination.TrangThai, userId, null);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok(new { message = "Hủy lịch hẹn thành công.", maDatLich = id });
            }

            var testAppointment = await _context.Datlichxetnghiems
                .FirstOrDefaultAsync(appointment => appointment.MaDatLich == id);
            if (testAppointment is null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy lịch hẹn." });
            }
            if (!await CanAccessAppointment(testAppointment.UserId, testAppointment.IdkhachHang))
            {
                return Forbid();
            }
            if (IsAppointmentLocked(testAppointment.NgayXetNghiem, testAppointment.TrangThai, allowPast: true))
            {
                return Conflict(new { success = false, message = "Không thể hủy lịch đã check-in hoặc hoàn thành." });
            }

            var oldTestStatus = testAppointment.TrangThai;
            testAppointment.TrangThai = "cancelled";
            AddAppointmentAudit(testAppointment.IdkhachHang, "datlichxetnghiem",
                testAppointment.IddatLichXn.ToString(), "Khách hàng hủy lịch hẹn",
                oldTestStatus, testAppointment.TrangThai, userId, null);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return Ok(new { message = "Hủy lịch hẹn thành công.", maDatLich = id });
        }

        private async Task<bool> CanAccessAppointment(int? appointmentUserId, string customerId)
        {
            if (User.IsInRole("ADMIN") || User.IsInRole("RECEPTIONIST") || User.IsInRole("DOCTOR"))
            {
                return true;
            }

            if (!User.IsInRole("CUSTOMER") ||
                !int.TryParse(User.FindFirstValue("userId"), out var userId))
            {
                return false;
            }

            if (appointmentUserId == userId)
            {
                return true;
            }

            var linkedCustomerId = await _context.Users
                .AsNoTracking()
                .Where(user => user.UserId == userId && user.IsActive == true)
                .Select(user => user.IdkhachHang)
                .FirstOrDefaultAsync();

            return linkedCustomerId == customerId;
        }

        private static bool IsAppointmentLocked(DateOnly appointmentDate, string status, bool allowPast = false)
        {
            var isPast = appointmentDate < DateOnly.FromDateTime(DateTime.Today);
            var isFinal = status.Equals("checked_in", StringComparison.OrdinalIgnoreCase) ||
                status.Equals("completed", StringComparison.OrdinalIgnoreCase) ||
                status.Equals("no_show", StringComparison.OrdinalIgnoreCase) ||
                status.Equals("huy", StringComparison.OrdinalIgnoreCase) ||
                status.Equals("cancelled", StringComparison.OrdinalIgnoreCase);
            return isFinal || (!allowPast && isPast);
        }

        private void AddAppointmentAudit(
            string customerId,
            string entityType,
            string entityId,
            string action,
            string oldStatus,
            string newStatus,
            int? userId,
            string? description)
        {
            _context.Truyvets.Add(new Truyvet
            {
                IdkhachHang = customerId,
                LoaiDoiTuong = entityType,
                IddoiTuong = entityId,
                HanhDong = action,
                TrangThaiCu = oldStatus,
                TrangThaiMoi = newStatus,
                UserIdthucHien = userId,
                NguonThucHien = "user",
                ThoiGian = DateTime.Now,
                MoTa = description,
                Ipaddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
            });
        }

        private static string NormalizeGender(string? gender) => gender?.Trim().ToLowerInvariant() switch
        {
            "nam" => "nam",
            "nu" or "nữ" => "nu",
            "khac" or "khác" => "khac",
            _ => "khac",
        };

        private IQueryable<AppointmentListItem> ExaminationAppointments(
            int? userId = null,
            string? customerId = null,
            DateOnly? appointmentDate = null)
        {
            var query = _context.Datlichkhams.AsNoTracking();
            if (appointmentDate.HasValue)
            {
                query = query.Where(appointment => appointment.NgayKham == appointmentDate.Value);
            }
            if (userId.HasValue)
            {
                query = query.Where(appointment =>
                    appointment.UserId == userId || appointment.IdkhachHang == customerId);
            }

            return query.Select(appointment => new AppointmentListItem(
                appointment.MaDatLich,
                "EXAMINATION",
                appointment.IdkhachHang,
                appointment.IdkhachHangNavigation.TenKhachHang,
                appointment.IdkhachHangNavigation.SoDienThoai,
                appointment.IdbacSi.ToString(),
                appointment.IdbacSiNavigation.TenBacSi,
                appointment.NgayKham,
                appointment.GioKham,
                appointment.Luotkham != null ? appointment.Luotkham.TrangThai : appointment.TrangThai,
                appointment.MaQr,
                appointment.GhiChu));
        }

        private IQueryable<AppointmentListItem> TestAppointments(
            int? userId = null,
            string? customerId = null,
            DateOnly? appointmentDate = null)
        {
            var query = _context.Datlichxetnghiems.AsNoTracking();
            if (appointmentDate.HasValue)
            {
                query = query.Where(appointment => appointment.NgayXetNghiem == appointmentDate.Value);
            }
            if (userId.HasValue)
            {
                query = query.Where(appointment =>
                    appointment.UserId == userId || appointment.IdkhachHang == customerId);
            }

            return query.Select(appointment => new AppointmentListItem(
                appointment.MaDatLich,
                "TEST",
                appointment.IdkhachHang,
                appointment.IdkhachHangNavigation.TenKhachHang,
                appointment.IdkhachHangNavigation.SoDienThoai,
                appointment.IdbacSi.HasValue ? appointment.IdbacSi.Value.ToString() : null,
                appointment.IdbacSiNavigation != null
                    ? appointment.IdbacSiNavigation.TenBacSi
                    : "Được sắp xếp khi đến nơi",
                appointment.NgayXetNghiem,
                appointment.GioXetNghiem,
                appointment.Luotxetnghiem != null ? appointment.Luotxetnghiem.TrangThai : appointment.TrangThai,
                appointment.MaQr,
                appointment.GhiChu));
            }

        private sealed record AppointmentListItem(
            string Id,
            string Type,
            string CustomerId,
            string CustomerName,
            string? CustomerPhone,
            string? DoctorId,
            string? DoctorName,
            DateOnly AppointmentDate,
            TimeOnly AppointmentTime,
            string Status,
            string? QrCode,
            string? Note);

        public sealed class TestAppointmentRequest
        {
            public string? Hoten { get; set; }
            public string? Email { get; set; }
            public string? Sodienthoai { get; set; }
            public string? Gioitinh { get; set; }
            public string? Ngaysinh { get; set; }
            public string? Ngay { get; set; }
            public string? Gio { get; set; }
            public string? Idbacsi { get; set; }
            public string? Idxetnghiem { get; set; }
            public string? Idcoso { get; set; }
            public string? Ghichu { get; set; }
        }

        public sealed class ExaminationAppointmentRequest
        {
            public string? Hoten { get; set; }
            public string? Email { get; set; }
            public string? Sodienthoai { get; set; }
            public string? Gioitinh { get; set; }
            public string? Ngaysinh { get; set; }
            public string? Ngay { get; set; }
            public string? Gio { get; set; }
            public string? Idbacsi { get; set; }
            public string? Idchuyenkhoa { get; set; }
            public string? Idcoso { get; set; }
            public string? Lydokham { get; set; }
            public string? Ghichu { get; set; }
        }

        public sealed class CheckInRequest
        {
            public string? Type { get; set; }
        }

        public sealed class UpdateAppointmentRequest
        {
            [JsonPropertyName("new_date")]
            public string? NewDate { get; set; }

            [JsonPropertyName("new_time")]
            public string? NewTime { get; set; }

            [JsonPropertyName("ghichu")]
            public string? Note { get; set; }
        }
    }
}