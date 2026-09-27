using System.Security.Claims;
using BioMedic.Backend.Data;
using BioMedic.Backend.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BioMedic.Backend.Controllers;

[ApiController]
public class VisitController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public VisitController(ApplicationDbContext context) => _context = context;

    [HttpGet("api/doctor/visits/waiting")]
    [HttpGet("api/doctor/queue")]
    [Authorize(Roles = "DOCTOR")]
    public async Task<IActionResult> DoctorQueue()
    {
        var doctorId = await GetDoctorId();
        if (doctorId is null) return Unauthorized(new { message = "Không xác định được bác sĩ." });
        var today = DateOnly.FromDateTime(DateTime.Today);
        var rows = await _context.Luotkhams.AsNoTracking()
            .Where(x => x.IdbacSi == doctorId && x.IddatLichKhamNavigation.NgayKham == today &&
                (x.TrangThai == "da_tiep_nhan" || x.TrangThai == "cho_kham" || x.TrangThai == "da_den_luot" || x.TrangThai == "cho_goi_lai"))
            .OrderBy(x => x.TrangThai == "da_den_luot" ? 0 : 1).ThenBy(x => x.SoThuTu)
            .Select(x => new
            {
                id = x.IdluotKham,
                stt = x.SoThuTu,
                maKhachHang = x.IdkhachHang,
                tenKhachHang = x.IdkhachHangNavigation.TenKhachHang,
                gioKham = x.IddatLichKhamNavigation.GioKham.ToString("HH:mm"),
                trangThai = x.TrangThai,
            }).ToListAsync();
        return Ok(rows);
    }

    [HttpPost("api/doctor/visits/{id}/call")]
    [Authorize(Roles = "DOCTOR")]
    public async Task<IActionResult> Call(ulong id) => await UpdateVisit(id, "da_den_luot", "Bác sĩ gọi vào phòng", true);

    [HttpPost("api/doctor/visits/{id}/skip")]
    [Authorize(Roles = "DOCTOR")]
    public async Task<IActionResult> Skip(ulong id) => await UpdateVisit(id, "cho_goi_lai", "Tạm bỏ qua lượt khám", false);

    [HttpPost("api/doctor/visits/{id}/start")]
    [Authorize(Roles = "DOCTOR")]
    public async Task<IActionResult> Start(ulong id)
    {
        var visit = await _context.Luotkhams.Include(x => x.Kham).FirstOrDefaultAsync(x => x.IdluotKham == id);
        if (visit is null) return NotFound(new { message = "Không tìm thấy lượt khám." });
        var doctorId = await GetDoctorId();
        if (doctorId is null || visit.IdbacSi != doctorId) return Forbid();
        visit.TrangThai = "dang_kham";
        visit.ThoiGianBatDau ??= DateTime.Now;
        if (visit.Kham is null)
        {
            _context.Khams.Add(new Kham { IdluotKham = id, IdbacSi = doctorId.Value, ThoiGianKham = DateTime.Now });
        }
        AddTracking(visit, "Bác sĩ bắt đầu khám");
        await _context.SaveChangesAsync();
        return Ok(new { success = true, id, status = visit.TrangThai });
    }

    private async Task<IActionResult> UpdateVisit(ulong id, string status, string action, bool notify)
    {
        var visit = await _context.Luotkhams.FirstOrDefaultAsync(x => x.IdluotKham == id);
        if (visit is null) return NotFound(new { message = "Không tìm thấy lượt khám." });
        var doctorId = await GetDoctorId();
        if (doctorId is null || visit.IdbacSi != doctorId) return Forbid();
        visit.TrangThai = status;
        AddTracking(visit, action);
        if (notify)
        {
            var userId = await _context.Users.Where(x => x.IdkhachHang == visit.IdkhachHang).Select(x => (int?)x.UserId).FirstOrDefaultAsync();
            if (userId is not null)
                _context.Thongbaos.Add(new Thongbao { UserIdnhan = userId.Value, LoaiThongBao = "kham_benh", TieuDe = "Đã đến lượt khám", NoiDung = "Vui lòng di chuyển vào phòng khám để gặp bác sĩ.", DaDoc = false, ThoiGianTao = DateTime.Now });
        }
        await _context.SaveChangesAsync();
        return Ok(new { success = true, id, status });
    }

    private void AddTracking(Luotkham visit, string action)
    {
        int? uid = int.TryParse(User.FindFirstValue("userId"), out var parsed) ? parsed : null;
        _context.Truyvets.Add(new Truyvet { IdkhachHang = visit.IdkhachHang, LoaiDoiTuong = "luotkham", IddoiTuong = visit.IdluotKham.ToString(), HanhDong = action, UserIdthucHien = uid, NguonThucHien = "user", ThoiGian = DateTime.Now });
    }

    private async Task<int?> GetDoctorId()
    {
        if (!int.TryParse(User.FindFirstValue("userId"), out var uid)) return null;
        return await _context.Users.Where(x => x.UserId == uid).Select(x => x.IdbacSi).FirstOrDefaultAsync();
    }
}
