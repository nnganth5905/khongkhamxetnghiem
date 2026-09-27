using BioMedic.Backend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BioMedic.Backend.Controllers;

[ApiController]
[Route("api/tracking")]
[Authorize]
public class TrackingController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TrackingController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("visits/{code}")]
    public async Task<IActionResult> GetVisitTracking(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return BadRequest(new
            {
                message = "Mã lượt khám không hợp lệ."
            });
        }

        var normalized = code.Trim();

        ulong? numericId = ulong.TryParse(normalized, out var parsed)
            ? parsed
            : null;

        var visit = await _context.Luotkhams
            .AsNoTracking()
            .Include(x => x.IdkhachHangNavigation)
            .Include(x => x.IdbacSiNavigation)
            .Include(x => x.IdphongNavigation)
            .Include(x => x.IddatLichKhamNavigation)
            .FirstOrDefaultAsync(x =>
                (numericId.HasValue && x.IdluotKham == numericId.Value) ||
                x.IddatLichKhamNavigation.MaDatLich == normalized);

        if (visit is null)
        {
            return NotFound(new
            {
                message = "Không tìm thấy lượt khám."
            });
        }

        var history = await _context.Truyvets
            .AsNoTracking()
            .Where(x =>
                x.LoaiDoiTuong == "luotkham" &&
                x.IddoiTuong == visit.IdluotKham.ToString())
            .OrderBy(x => x.ThoiGian)
            .Select(x => new
            {
                title = x.HanhDong,
                description = x.MoTa,
                time = x.ThoiGian,
                oldStatus = x.TrangThaiCu,
                newStatus = x.TrangThaiMoi
            })
            .ToListAsync();

        var currentStep = MapVisitStep(visit.TrangThai);

        return Ok(new
        {
            code = visit.IdluotKham.ToString(),
            patientName = visit.IdkhachHangNavigation.TenKhachHang,
            doctorName = visit.IdbacSiNavigation.TenBacSi,
            roomName = visit.IdphongNavigation != null
                ? visit.IdphongNavigation.TenPhong
                : null,
            status = visit.TrangThai,

            appointmentDate =
                visit.IddatLichKhamNavigation.NgayKham.ToString("dd/MM/yyyy"),

            appointmentTime =
                visit.IddatLichKhamNavigation.GioKham.ToString("HH:mm"),

            currentStep,

            steps = history
        });
    }

    [HttpGet("history")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> GetHistory()
    {
        var rows = await _context.Truyvets
            .AsNoTracking()
            .OrderByDescending(x => x.ThoiGian)
            .Take(200)
            .Select(x => new
            {
                id = x.IdtruyVet,
                customerId = x.IdkhachHang,
                objectType = x.LoaiDoiTuong,
                objectId = x.IddoiTuong,
                action = x.HanhDong,
                oldStatus = x.TrangThaiCu,
                newStatus = x.TrangThaiMoi,
                source = x.NguonThucHien,
                time = x.ThoiGian,
                description = x.MoTa
            })
            .ToListAsync();

        return Ok(rows);
    }

    private static int MapVisitStep(string? status)
    {
        return status?.ToLowerInvariant() switch
        {
            "pending" => 0,
            "confirmed" => 0,
            "da_tiep_nhan" => 1,
            "cho_kham" => 2,
            "da_den_luot" => 2,
            "cho_goi_lai" => 2,
            "dang_kham" => 3,
            "cho_xet_nghiem" => 4,
            "dang_xet_nghiem" => 5,
            "cho_duyet" => 5,
            "da_duyet" => 6,
            "completed" => 7,
            "hoan_tat" => 7,
            _ => 0
        };
    }
}