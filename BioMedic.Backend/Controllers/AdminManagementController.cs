using BioMedic.Backend.Data;
using BioMedic.Backend.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BioMedic.Backend.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "ADMIN")]
public class AdminManagementController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public AdminManagementController(ApplicationDbContext context) => _context = context;

    [HttpGet("customers")]
    public async Task<IActionResult> Customers([FromQuery] string? q = null)
    {
        var query = _context.Khachhangs.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(q)) query = query.Where(x => x.TenKhachHang.Contains(q) || (x.Email ?? "").Contains(q) || (x.SoDienThoai ?? "").Contains(q));
        return Ok(await query.OrderByDescending(x => x.CreatedAt).Select(x => new { id = x.IdkhachHang, name = x.TenKhachHang, email = x.Email, phone = x.SoDienThoai, gender = x.GioiTinh, birthday = x.NgaySinh, address = x.DiaChi, cccd = x.Cccd, status = x.Status }).ToListAsync());
    }

    [HttpGet("customers/{id}")]
    public async Task<IActionResult> Customer(string id) => await _context.Khachhangs.AsNoTracking().FirstOrDefaultAsync(x => x.IdkhachHang == id) is { } x ? Ok(x) : NotFound();

    [HttpPost("customers")]
    public async Task<IActionResult> CreateCustomer([FromBody] CustomerAdminRequest r)
    {
        if (string.IsNullOrWhiteSpace(r.Name)) return BadRequest(new { message = "Họ tên không được để trống." });
        var id = NewCode("KH");
        var item = new Khachhang { IdkhachHang = id, TenKhachHang = r.Name.Trim(), Email = Clean(r.Email), SoDienThoai = Clean(r.Phone), GioiTinh = Clean(r.Gender) ?? "khac", DiaChi = Clean(r.Address), Cccd = Clean(r.Cccd), NgaySinh = r.Birthday, Status = r.Status ?? "yes", CreatedAt = DateTime.Now };
        _context.Khachhangs.Add(item); await _context.SaveChangesAsync(); return Ok(new { id, success = true });
    }

    [HttpPut("customers/{id}")]
    public async Task<IActionResult> UpdateCustomer(string id, [FromBody] CustomerAdminRequest r)
    {
        var x = await _context.Khachhangs.FindAsync(id); if (x is null) return NotFound();
        if (!string.IsNullOrWhiteSpace(r.Name)) x.TenKhachHang = r.Name.Trim(); x.Email = Clean(r.Email) ?? x.Email; x.SoDienThoai = Clean(r.Phone) ?? x.SoDienThoai; x.GioiTinh = Clean(r.Gender) ?? x.GioiTinh; x.DiaChi = Clean(r.Address) ?? x.DiaChi; x.Cccd = Clean(r.Cccd) ?? x.Cccd; x.NgaySinh = r.Birthday ?? x.NgaySinh; x.Status = r.Status ?? x.Status; x.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync(); return Ok(new { success = true });
    }

    [HttpDelete("customers/{id}")]
    public async Task<IActionResult> DeleteCustomer(string id) { var x = await _context.Khachhangs.FindAsync(id); if (x is null) return NotFound(); x.Status = "no"; x.UpdatedAt = DateTime.Now; await _context.SaveChangesAsync(); return Ok(new { success = true }); }

    [HttpGet("employees")]
    public async Task<IActionResult> Employees() => Ok(await _context.Nhanviens.AsNoTracking().OrderBy(x => x.TenNhanVien).Select(x => new { id = x.IdnhanVien, name = x.TenNhanVien, position = x.ViTri, phone = x.SoDienThoai, email = x.Email, facilityId = x.CoSoId, status = x.Status }).ToListAsync());

    [HttpGet("employees/{id}")]
    public async Task<IActionResult> Employee(string id) => await _context.Nhanviens.AsNoTracking().FirstOrDefaultAsync(x => x.IdnhanVien == id) is { } x ? Ok(x) : NotFound();

    [HttpPost("employees")]
    public async Task<IActionResult> CreateEmployee([FromBody] EmployeeAdminRequest r)
    {
        if (string.IsNullOrWhiteSpace(r.Name))
            return BadRequest(new { success = false, message = "Họ tên không được để trống." });

        var position = NormalizeEmployeePosition(r.Position);
        if (position is null)
        {
            return BadRequest(new
            {
                success = false,
                message = "Vai trò không hợp lệ. Chỉ chấp nhận: bacsi, letan, ktv, admin, dieu_duong, khac."
            });
        }

        var status = NormalizeYesNo(r.Status);
        if (status is null)
        {
            return BadRequest(new
            {
                success = false,
                message = "Trạng thái không hợp lệ. Chỉ chấp nhận yes hoặc no."
            });
        }

        var facilityId = Clean(r.FacilityId);
        if (!string.IsNullOrWhiteSpace(facilityId))
        {
            var facilityExists = await _context.Cosos
                .AsNoTracking()
                .AnyAsync(x => x.CoSoId == facilityId);

            if (!facilityExists)
            {
                return BadRequest(new
                {
                    success = false,
                    message = $"Mã cơ sở '{facilityId}' không tồn tại."
                });
            }
        }

        var email = Clean(r.Email);
        if (!string.IsNullOrWhiteSpace(email))
        {
            email = email.ToLowerInvariant();

            var duplicateEmail = await _context.Nhanviens
                .AsNoTracking()
                .AnyAsync(x => x.Email == email);

            if (duplicateEmail)
            {
                return Conflict(new
                {
                    success = false,
                    message = "Email này đã được sử dụng cho một nhân viên khác."
                });
            }
        }

        var id = NewCode("NV");

        var employee = new Nhanvien
        {
            IdnhanVien = id,
            TenNhanVien = r.Name.Trim(),
            ViTri = position,
            SoDienThoai = Clean(r.Phone),
            Email = email,
            CoSoId = facilityId,
            Status = status,
            CreatedAt = DateTime.Now
        };

        try
        {
            _context.Nhanviens.Add(employee);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                id,
                success = true,
                message = "Thêm nhân viên thành công."
            });
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                success = false,
                message = "Không thể thêm nhân viên vào cơ sở dữ liệu.",
                detail = ex.InnerException?.Message ?? ex.Message
            });
        }
    }

    [HttpPut("employees/{id}")]
    public async Task<IActionResult> UpdateEmployee(string id, [FromBody] EmployeeAdminRequest r)
    {
        var employee = await _context.Nhanviens.FindAsync(id);
        if (employee is null)
            return NotFound(new { success = false, message = "Không tìm thấy nhân viên." });

        if (!string.IsNullOrWhiteSpace(r.Name))
            employee.TenNhanVien = r.Name.Trim();

        if (!string.IsNullOrWhiteSpace(r.Position))
        {
            var position = NormalizeEmployeePosition(r.Position);
            if (position is null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Vai trò không hợp lệ. Chỉ chấp nhận: bacsi, letan, ktv, admin, dieu_duong, khac."
                });
            }

            employee.ViTri = position;
        }

        if (r.Phone is not null)
            employee.SoDienThoai = Clean(r.Phone);

        if (r.Email is not null)
        {
            var email = Clean(r.Email)?.ToLowerInvariant();

            if (!string.IsNullOrWhiteSpace(email))
            {
                var duplicateEmail = await _context.Nhanviens
                    .AsNoTracking()
                    .AnyAsync(x => x.IdnhanVien != id && x.Email == email);

                if (duplicateEmail)
                {
                    return Conflict(new
                    {
                        success = false,
                        message = "Email này đã được sử dụng cho một nhân viên khác."
                    });
                }
            }

            employee.Email = email;
        }

        if (r.FacilityId is not null)
        {
            var facilityId = Clean(r.FacilityId);

            if (!string.IsNullOrWhiteSpace(facilityId))
            {
                var facilityExists = await _context.Cosos
                    .AsNoTracking()
                    .AnyAsync(x => x.CoSoId == facilityId);

                if (!facilityExists)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = $"Mã cơ sở '{facilityId}' không tồn tại."
                    });
                }
            }

            employee.CoSoId = facilityId;
        }

        if (!string.IsNullOrWhiteSpace(r.Status))
        {
            var status = NormalizeYesNo(r.Status);
            if (status is null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Trạng thái không hợp lệ. Chỉ chấp nhận yes hoặc no."
                });
            }

            employee.Status = status;
        }

        try
        {
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Cập nhật nhân viên thành công." });
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                success = false,
                message = "Không thể cập nhật nhân viên.",
                detail = ex.InnerException?.Message ?? ex.Message
            });
        }
    }

    [HttpDelete("employees/{id}")]
    public async Task<IActionResult> DeleteEmployee(string id) { var x = await _context.Nhanviens.FindAsync(id); if (x is null) return NotFound(); x.Status = "no"; await _context.SaveChangesAsync(); return Ok(new { success = true }); }

    [HttpGet("doctors")]
    public async Task<IActionResult> Doctors() => Ok(await _context.Bacsis.AsNoTracking().Select(x => new { id = x.IdbacSi, name = x.TenBacSi, degree = x.HocVi, title = x.ChucDanh, specialtyId = x.KhoaId, specialty = x.Khoa.TenChuyenKhoa, facilityId = x.CoSoId, description = x.MoTa, image = x.HinhAnh, rating = x.SoSao, experience = x.NamKinhNghiem, status = x.TrangThai, employeeId = x.IdnhanVien }).ToListAsync());

    [HttpPost("doctors")]
    public async Task<IActionResult> CreateDoctor([FromBody] DoctorAdminRequest r) { if (string.IsNullOrWhiteSpace(r.Name) || string.IsNullOrWhiteSpace(r.SpecialtyId)) return BadRequest(new { message = "Thiếu tên bác sĩ hoặc chuyên khoa." }); var x = new Bacsi { TenBacSi = r.Name.Trim(), HocVi = Clean(r.Degree), ChucDanh = Clean(r.Title), KhoaId = r.SpecialtyId.Trim(), CoSoId = Clean(r.FacilityId), MoTa = Clean(r.Description), HinhAnh = Clean(r.Image), SoSao = r.Rating ?? 5m, NamKinhNghiem = r.Experience, TrangThai = r.Status ?? "active", IdnhanVien = Clean(r.EmployeeId) }; _context.Bacsis.Add(x); await _context.SaveChangesAsync(); return Ok(new { id = x.IdbacSi, success = true }); }

    [HttpPut("doctors/{id:int}")]
    public async Task<IActionResult> UpdateDoctor(int id, [FromBody] DoctorAdminRequest r) { var x = await _context.Bacsis.FindAsync(id); if (x is null) return NotFound(); if (!string.IsNullOrWhiteSpace(r.Name)) x.TenBacSi = r.Name.Trim(); x.HocVi = Clean(r.Degree) ?? x.HocVi; x.ChucDanh = Clean(r.Title) ?? x.ChucDanh; x.KhoaId = Clean(r.SpecialtyId) ?? x.KhoaId; x.CoSoId = Clean(r.FacilityId) ?? x.CoSoId; x.MoTa = Clean(r.Description) ?? x.MoTa; x.HinhAnh = Clean(r.Image) ?? x.HinhAnh; x.SoSao = r.Rating ?? x.SoSao; x.NamKinhNghiem = r.Experience ?? x.NamKinhNghiem; x.TrangThai = r.Status ?? x.TrangThai; await _context.SaveChangesAsync(); return Ok(new { success = true }); }

    [HttpDelete("doctors/{id:int}")]
    public async Task<IActionResult> DeleteDoctor(int id) { var x = await _context.Bacsis.FindAsync(id); if (x is null) return NotFound(); x.TrangThai = "inactive"; await _context.SaveChangesAsync(); return Ok(new { success = true }); }

    [HttpGet("technicians")]
    public async Task<IActionResult> Technicians() => Ok(await _context.Nhanviens.AsNoTracking().Where(x => x.ViTri == "ktv" || x.ViTri.Contains("Kỹ thuật") || x.ViTri.Contains("ky thuat")).Select(x => new { id = x.IdnhanVien, name = x.TenNhanVien, phone = x.SoDienThoai, email = x.Email, facilityId = x.CoSoId, status = x.Status }).ToListAsync());

    [HttpPost("technicians")]
    public Task<IActionResult> CreateTechnician([FromBody] EmployeeAdminRequest r) { r.Position = "ktv"; return CreateEmployee(r); }
    [HttpPut("technicians/{id}")]
    public Task<IActionResult> UpdateTechnician(string id, [FromBody] EmployeeAdminRequest r) { r.Position = "ktv"; return UpdateEmployee(id, r); }
    [HttpDelete("technicians/{id}")]
    public Task<IActionResult> DeleteTechnician(string id) => DeleteEmployee(id);

    [HttpGet("tests")]
    public async Task<IActionResult> Tests() => Ok(await _context.Loaixetnghiems.AsNoTracking().Select(x => new { id = x.IdxetNghiem, name = x.TenXetNghiem, specialtyId = x.ChuyenKhoaId, type = x.Loai, description = x.MoTa, price = x.Gia, sampleType = x.LoaiMauMacDinh, expectedMinutes = x.ThoiGianDuKienPhut, status = x.Status }).ToListAsync());
    [HttpPost("tests")]
    public async Task<IActionResult> CreateTest([FromBody] TestAdminRequest r) { if (string.IsNullOrWhiteSpace(r.Name) || string.IsNullOrWhiteSpace(r.SpecialtyId)) return BadRequest(); var id = NewCode("XN"); var x = new Loaixetnghiem { IdxetNghiem = id, TenXetNghiem = r.Name.Trim(), ChuyenKhoaId = r.SpecialtyId.Trim(), Loai = Clean(r.Type), MoTa = Clean(r.Description), Gia = r.Price, LoaiMauMacDinh = Clean(r.SampleType), ThoiGianDuKienPhut = r.ExpectedMinutes, Status = r.Status ?? "yes" }; _context.Loaixetnghiems.Add(x); await _context.SaveChangesAsync(); return Ok(new { id, success = true }); }
    [HttpPut("tests/{id}")]
    public async Task<IActionResult> UpdateTest(string id, [FromBody] TestAdminRequest r) { var x = await _context.Loaixetnghiems.FindAsync(id); if (x is null) return NotFound(); if (!string.IsNullOrWhiteSpace(r.Name)) x.TenXetNghiem = r.Name.Trim(); x.ChuyenKhoaId = Clean(r.SpecialtyId) ?? x.ChuyenKhoaId; x.Loai = Clean(r.Type) ?? x.Loai; x.MoTa = Clean(r.Description) ?? x.MoTa; x.Gia = r.Price > 0 ? r.Price : x.Gia; x.LoaiMauMacDinh = Clean(r.SampleType) ?? x.LoaiMauMacDinh; x.ThoiGianDuKienPhut = r.ExpectedMinutes ?? x.ThoiGianDuKienPhut; x.Status = r.Status ?? x.Status; await _context.SaveChangesAsync(); return Ok(new { success = true }); }
    [HttpDelete("tests/{id}")]
    public async Task<IActionResult> DeleteTest(string id) { var x = await _context.Loaixetnghiems.FindAsync(id); if (x is null) return NotFound(); x.Status = "no"; await _context.SaveChangesAsync(); return Ok(new { success = true }); }

    [HttpGet("specialties")]
    public async Task<IActionResult> Specialties() => Ok(await _context.Chuyenkhoas.AsNoTracking().Select(x => new { id = x.IdchuyenKhoa, name = x.TenChuyenKhoa, description = x.MoTa, status = x.Status }).ToListAsync());
    [HttpPost("specialties")]
    public async Task<IActionResult> CreateSpecialty([FromBody] SpecialtyAdminRequest r) { var id = NewCode("CK"); var x = new Chuyenkhoa { IdchuyenKhoa = id, TenChuyenKhoa = r.Name.Trim(), MoTa = Clean(r.Description), Status = r.Status ?? "yes" }; _context.Chuyenkhoas.Add(x); await _context.SaveChangesAsync(); return Ok(new { id, success = true }); }
    [HttpPut("specialties/{id}")]
    public async Task<IActionResult> UpdateSpecialty(string id, [FromBody] SpecialtyAdminRequest r) { var x = await _context.Chuyenkhoas.FindAsync(id); if (x is null) return NotFound(); if (!string.IsNullOrWhiteSpace(r.Name)) x.TenChuyenKhoa = r.Name.Trim(); x.MoTa = Clean(r.Description) ?? x.MoTa; x.Status = r.Status ?? x.Status; await _context.SaveChangesAsync(); return Ok(new { success = true }); }
    [HttpDelete("specialties/{id}")]
    public async Task<IActionResult> DeleteSpecialty(string id) { var x = await _context.Chuyenkhoas.FindAsync(id); if (x is null) return NotFound(); x.Status = "no"; await _context.SaveChangesAsync(); return Ok(new { success = true }); }

    [HttpGet("rooms")]
    public async Task<IActionResult> Rooms() => Ok(await _context.Phongs.AsNoTracking().Select(x => new { id = x.Idphong, name = x.TenPhong, facilityId = x.CoSoId, specialtyId = x.IdchuyenKhoa, type = x.LoaiPhong, floor = x.Tang, status = x.TrangThai }).ToListAsync());
    [HttpPost("rooms")]
    public async Task<IActionResult> CreateRoom([FromBody] RoomAdminRequest r) { var id = NewCode("P"); var x = new Phong { Idphong = id, CoSoId = r.FacilityId.Trim(), IdchuyenKhoa = Clean(r.SpecialtyId), TenPhong = r.Name.Trim(), LoaiPhong = r.Type.Trim(), Tang = Clean(r.Floor), TrangThai = r.Status ?? "active" }; _context.Phongs.Add(x); await _context.SaveChangesAsync(); return Ok(new { id, success = true }); }
    [HttpPut("rooms/{id}")]
    public async Task<IActionResult> UpdateRoom(string id, [FromBody] RoomAdminRequest r) { var x = await _context.Phongs.FindAsync(id); if (x is null) return NotFound(); if (!string.IsNullOrWhiteSpace(r.Name)) x.TenPhong = r.Name.Trim(); x.CoSoId = Clean(r.FacilityId) ?? x.CoSoId; x.IdchuyenKhoa = Clean(r.SpecialtyId) ?? x.IdchuyenKhoa; x.LoaiPhong = Clean(r.Type) ?? x.LoaiPhong; x.Tang = Clean(r.Floor) ?? x.Tang; x.TrangThai = r.Status ?? x.TrangThai; await _context.SaveChangesAsync(); return Ok(new { success = true }); }
    [HttpDelete("rooms/{id}")]
    public async Task<IActionResult> DeleteRoom(string id) { var x = await _context.Phongs.FindAsync(id); if (x is null) return NotFound(); x.TrangThai = "inactive"; await _context.SaveChangesAsync(); return Ok(new { success = true }); }

    [HttpGet("work-schedules")]
    public async Task<IActionResult> Schedules() => Ok(await _context.Lichlamviecs.AsNoTracking().Select(x => new { id = x.LichId, doctorId = x.IdbacSi, doctorName = x.IdbacSiNavigation.TenBacSi, roomId = x.Idphong, date = x.Ngay, shift = x.Ca, start = x.GioBatDau, end = x.GioKetThuc, status = x.TrangThai, note = x.GhiChu }).ToListAsync());
    [HttpPost("work-schedules")]
    public async Task<IActionResult> CreateSchedule([FromBody] ScheduleAdminRequest r) { var x = new Lichlamviec { IdbacSi = r.DoctorId, Idphong = Clean(r.RoomId), Ngay = r.Date, Ca = r.Shift, GioBatDau = r.Start, GioKetThuc = r.End, NguonTao = "admin", TrangThai = r.Status ?? "active", GhiChu = Clean(r.Note), CreatedAt = DateTime.Now }; _context.Lichlamviecs.Add(x); await _context.SaveChangesAsync(); return Ok(new { id = x.LichId, success = true }); }
    [HttpPut("work-schedules/{id}")]
    public async Task<IActionResult> UpdateSchedule(ulong id, [FromBody] ScheduleAdminRequest r) { var x = await _context.Lichlamviecs.FindAsync(id); if (x is null) return NotFound(); x.IdbacSi = r.DoctorId; x.Idphong = Clean(r.RoomId) ?? x.Idphong; x.Ngay = r.Date; x.Ca = r.Shift; x.GioBatDau = r.Start; x.GioKetThuc = r.End; x.TrangThai = r.Status ?? x.TrangThai; x.GhiChu = Clean(r.Note) ?? x.GhiChu; x.UpdatedAt = DateTime.Now; await _context.SaveChangesAsync(); return Ok(new { success = true }); }
    [HttpDelete("work-schedules/{id}")]
    public async Task<IActionResult> DeleteSchedule(ulong id) { var x = await _context.Lichlamviecs.FindAsync(id); if (x is null) return NotFound(); _context.Lichlamviecs.Remove(x); await _context.SaveChangesAsync(); return Ok(new { success = true }); }

    private static string? NormalizeEmployeePosition(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var input = value.Trim().ToLowerInvariant();

        return input switch
        {
            "bacsi" or "doctor" or "bác sĩ" or "bac si" => "bacsi",
            "letan" or "receptionist" or "lễ tân" or "le tan" => "letan",
            "ktv" or "technician" or "kỹ thuật viên" or "ky thuat vien" => "ktv",
            "admin" or "administrator" or "quản trị viên" or "quan tri vien" => "admin",
            "dieu_duong" or "nurse" or "điều dưỡng" or "dieu duong" => "dieu_duong",
            "khac" or "staff" or "nhân viên" or "nhan vien" => "khac",
            _ => null
        };
    }

    private static string? NormalizeYesNo(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "yes";

        var input = value.Trim().ToLowerInvariant();

        return input switch
        {
            "yes" or "active" or "1" or "true" or "đang làm việc" or "dang lam viec" => "yes",
            "no" or "inactive" or "0" or "false" or "ngừng làm việc" or "ngung lam viec" => "no",
            _ => null
        };
    }

    private static string NewCode(string prefix) => (prefix + Guid.NewGuid().ToString("N")[..8]).ToUpperInvariant();
    private static string? Clean(string? v) => string.IsNullOrWhiteSpace(v) ? null : v.Trim();
}

public class CustomerAdminRequest { public string Name { get; set; } = ""; public string? Email { get; set; } public string? Phone { get; set; } public string? Gender { get; set; } public DateOnly? Birthday { get; set; } public string? Address { get; set; } public string? Cccd { get; set; } public string? Status { get; set; } }
public class EmployeeAdminRequest { public string Name { get; set; } = ""; public string Position { get; set; } = ""; public string? Phone { get; set; } public string? Email { get; set; } public string? FacilityId { get; set; } public string? Status { get; set; } }
public class DoctorAdminRequest { public string Name { get; set; } = ""; public string? Degree { get; set; } public string? Title { get; set; } public string? SpecialtyId { get; set; } public string? FacilityId { get; set; } public string? Description { get; set; } public string? Image { get; set; } public decimal? Rating { get; set; } public byte? Experience { get; set; } public string? Status { get; set; } public string? EmployeeId { get; set; } }
public class TestAdminRequest { public string Name { get; set; } = ""; public string? SpecialtyId { get; set; } public string? Type { get; set; } public string? Description { get; set; } public decimal Price { get; set; } public string? SampleType { get; set; } public int? ExpectedMinutes { get; set; } public string? Status { get; set; } }
public class SpecialtyAdminRequest { public string Name { get; set; } = ""; public string? Description { get; set; } public string? Status { get; set; } }
public class RoomAdminRequest { public string Name { get; set; } = ""; public string FacilityId { get; set; } = ""; public string? SpecialtyId { get; set; } public string Type { get; set; } = "kham"; public string? Floor { get; set; } public string? Status { get; set; } }
public class ScheduleAdminRequest { public int DoctorId { get; set; } public string? RoomId { get; set; } public DateOnly Date { get; set; } public string Shift { get; set; } = "sang"; public TimeOnly Start { get; set; } public TimeOnly End { get; set; } public string? Status { get; set; } public string? Note { get; set; } }
