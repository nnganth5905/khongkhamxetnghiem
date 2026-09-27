using System.Security.Claims;
using BioMedic.Backend.Data;
using BioMedic.Backend.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BioMedic.Backend.Controllers;

[ApiController]
[Route("api/doctor/examinations")]
[Authorize(Roles = "DOCTOR")]
public class ExaminationController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ExaminationController(ApplicationDbContext context) => _context = context;

    [HttpGet("visit/{visitId}")]
    public async Task<IActionResult> GetByVisit(string visitId)
    {
        var doctorId = await GetDoctorId();
        if (doctorId is null) return Forbid();

        var visit = await ResolveVisit(visitId, doctorId.Value);
        if (visit is null) return NotFound(new { message = "Không tìm thấy lượt khám thuộc bác sĩ hiện tại." });

        if (visit.TrangThai is "da_tiep_nhan" or "da_den_luot" or "cho_goi_lai" or "cho_kham")
        {
            visit.TrangThai = "dang_kham";
            visit.ThoiGianBatDau ??= DateTime.Now;
            AddTracking(visit, "Bác sĩ bắt đầu khám");
            await _context.SaveChangesAsync();
        }

        var exam = await _context.Khams
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdluotKham == visit.IdluotKham);

        var customer = await _context.Khachhangs
            .AsNoTracking()
            .FirstAsync(x => x.IdkhachHang == visit.IdkhachHang);

        return Ok(new
        {
            visitId = visit.IdluotKham,
            appointmentCode = visit.IddatLichKhamNavigation.MaDatLich,
            IDKham = exam?.Idkham,
            TrieuChung = exam?.TrieuChung,
            TienSuBenh = exam?.TienSuBenh,
            ChanDoan = exam?.ChanDoan,
            KetLuan = exam?.KetLuan,
            HuongDieuTri = exam?.HuongDieuTri,
            TenKhachHang = customer.TenKhachHang,
            GioiTinh = customer.GioiTinh,
            NgaySinh = customer.NgaySinh,
            SoDienThoai = customer.SoDienThoai,
            status = visit.TrangThai,
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ExaminationRequest request)
    {
        var doctorId = await GetDoctorId();
        if (doctorId is null) return Forbid();

        var visit = await ResolveVisit(request.VisitId, doctorId.Value);
        if (visit is null) return NotFound(new { message = "Không tìm thấy lượt khám thuộc bác sĩ hiện tại." });

        if (await _context.Khams.AnyAsync(x => x.IdluotKham == visit.IdluotKham))
            return Conflict(new { message = "Bệnh án đã tồn tại; hãy dùng chức năng cập nhật." });

        var exam = NewExam(visit, request);
        _context.Khams.Add(exam);
        AddTracking(visit, "Bác sĩ tạo bệnh án");
        await _context.SaveChangesAsync();
        return Ok(ToResponse(exam));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] ExaminationRequest request)
    {
        var doctorId = await GetDoctorId();
        if (doctorId is null) return Forbid();

        var visit = await ResolveVisit(request.VisitId, doctorId.Value);
        if (visit is null) return NotFound(new { message = "Không tìm thấy lượt khám thuộc bác sĩ hiện tại." });

        Kham? exam = null;
        if (ulong.TryParse(id, out var examId))
        {
            exam = await _context.Khams.FirstOrDefaultAsync(x => x.Idkham == examId && x.IdluotKham == visit.IdluotKham);
        }

        exam ??= await _context.Khams.FirstOrDefaultAsync(x => x.IdluotKham == visit.IdluotKham);

        if (exam is null)
        {
            exam = NewExam(visit, request);
            _context.Khams.Add(exam);
        }
        else
        {
            Apply(exam, request);
        }

        AddTracking(visit, "Bác sĩ cập nhật bệnh án");
        await _context.SaveChangesAsync();
        return Ok(ToResponse(exam));
    }

    [HttpPost("{visitId}/complete")]
    public async Task<IActionResult> Complete(string visitId)
    {
        var doctorId = await GetDoctorId();
        if (doctorId is null) return Forbid();

        var visit = await ResolveVisit(visitId, doctorId.Value);
        if (visit is null) return NotFound(new { message = "Không tìm thấy lượt khám thuộc bác sĩ hiện tại." });

        if (!await _context.Khams.AnyAsync(x => x.IdluotKham == visit.IdluotKham))
        {
            return BadRequest(new { success = false, message = "Chưa có bệnh án cho lượt khám này." });
        }

        visit.TrangThai = "hoan_tat";
        visit.ThoiGianKetThuc = DateTime.Now;
        visit.IddatLichKhamNavigation.TrangThai = "completed";
        AddTracking(visit, "Bác sĩ hoàn tất khám");

        await _context.SaveChangesAsync();
        return Ok(new { success = true, id = visit.IdluotKham, status = "COMPLETED", message = "Đã hoàn tất phiên khám." });
    }

    private async Task<Luotkham?> ResolveVisit(string value, int doctorId)
    {
        var query = _context.Luotkhams
            .Include(x => x.IddatLichKhamNavigation)
            .Where(x => x.IdbacSi == doctorId);

        if (ulong.TryParse(value, out var numeric))
        {
            var byId = await query.FirstOrDefaultAsync(x => x.IdluotKham == numeric);
            if (byId is not null) return byId;
        }

        return await query.FirstOrDefaultAsync(x => x.IddatLichKhamNavigation.MaDatLich == value);
    }

    private async Task<int?> GetDoctorId()
    {
        if (!int.TryParse(User.FindFirstValue("userId"), out var userId)) return null;

        var account = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.UserId == userId && user.IsActive == true);

        if (account is null) return null;
        if (account.IdbacSi.HasValue) return account.IdbacSi.Value;

        var employeeId = account.IdnhanVien;
        if (string.IsNullOrWhiteSpace(employeeId) && !string.IsNullOrWhiteSpace(account.Email))
        {
            employeeId = await _context.Nhanviens
                .AsNoTracking()
                .Where(employee => employee.Email == account.Email)
                .Select(employee => employee.IdnhanVien)
                .FirstOrDefaultAsync();
        }

        if (string.IsNullOrWhiteSpace(employeeId)) return null;

        return await _context.Bacsis
            .AsNoTracking()
            .Where(doctor => doctor.IdnhanVien == employeeId)
            .Select(doctor => (int?)doctor.IdbacSi)
            .SingleOrDefaultAsync();
    }

    private void AddTracking(Luotkham visit, string action)
    {
        int? userId = int.TryParse(User.FindFirstValue("userId"), out var parsed) ? parsed : null;
        _context.Truyvets.Add(new Truyvet
        {
            IdkhachHang = visit.IdkhachHang,
            LoaiDoiTuong = "luotkham",
            IddoiTuong = visit.IdluotKham.ToString(),
            HanhDong = action,
            UserIdthucHien = userId,
            NguonThucHien = "user",
            ThoiGian = DateTime.Now,
        });
    }

    private static Kham NewExam(Luotkham visit, ExaminationRequest request)
    {
        var exam = new Kham
        {
            IdluotKham = visit.IdluotKham,
            IdbacSi = visit.IdbacSi,
            ThoiGianKham = DateTime.Now,
        };
        Apply(exam, request);
        return exam;
    }

    private static void Apply(Kham exam, ExaminationRequest request)
    {
        exam.TrieuChung = request.Symptoms;
        exam.TienSuBenh = request.History;
        exam.ChanDoan = request.Diagnosis;
        exam.KetLuan = request.Conclusion;
        exam.HuongDieuTri = request.Advice;
        exam.UpdatedAt = DateTime.Now;
    }

    private static object ToResponse(Kham exam) => new
    {
        id = exam.Idkham,
        visitId = exam.IdluotKham,
        symptoms = exam.TrieuChung,
        history = exam.TienSuBenh,
        diagnosis = exam.ChanDoan,
        conclusion = exam.KetLuan,
        advice = exam.HuongDieuTri,
    };
}

public class ExaminationRequest
{
    public string VisitId { get; set; } = "";
    public string? Symptoms { get; set; }
    public string? History { get; set; }
    public string? Diagnosis { get; set; }
    public string? Conclusion { get; set; }
    public string? Advice { get; set; }
}
