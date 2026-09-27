using System.Globalization;
using System.Security.Claims;
using BioMedic.Backend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BioMedic.Backend.Controllers;

[ApiController]
[Route("api/results")]
public class CustomerResultsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public CustomerResultsController(ApplicationDbContext context) => _context = context;

    [HttpGet("mine")]
    [Authorize(Roles = "CUSTOMER")]
    public async Task<IActionResult> Mine()
    {
        var customerId = await GetCustomerId();
        if (customerId is null) return Unauthorized();
        var rows = await _context.Ketquaxetnghiems.AsNoTracking()
            .Where(x => x.TrangThai == "da_duyet" && x.IdctphieuNavigation.IdphieuXetNghiemNavigation.IdkhachHang == customerId)
            .Include(x => x.IdctphieuNavigation).ThenInclude(x => x.IdxetNghiemNavigation)
            .Include(x => x.IdmauNavigation)
            .OrderByDescending(x => x.ThoiGianDuyet)
            .Select(x => new { id = x.IdketQua, code = x.IdketQua, specimenCode = x.IdmauNavigation == null ? null : x.IdmauNavigation.MaBarcode, testName = x.IdctphieuNavigation.IdxetNghiemNavigation.TenXetNghiem, approvedAt = x.ThoiGianDuyet, status = "APPROVED", conclusion = x.KetLuanBacSi })
            .ToListAsync();
        return Ok(rows);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> Detail(string id)
    {
        var result = await _context.Ketquaxetnghiems.AsNoTracking()
            .Include(x => x.IdctphieuNavigation).ThenInclude(x => x.IdxetNghiemNavigation)
            .Include(x => x.IdctphieuNavigation).ThenInclude(x => x.IdphieuXetNghiemNavigation).ThenInclude(x => x.IdkhachHangNavigation)
            .Include(x => x.IdmauNavigation)
            .Include(x => x.Ketquachisos).ThenInclude(x => x.IdchiSoNavigation)
            .FirstOrDefaultAsync(x => x.IdketQua == id);
        if (result is null) return NotFound();
        if (User.IsInRole("CUSTOMER"))
        {
            var customerId = await GetCustomerId();
            if (customerId != result.IdctphieuNavigation.IdphieuXetNghiemNavigation.IdkhachHang || result.TrangThai != "da_duyet") return Forbid();
        }
        return Ok(new
        {
            id = result.IdketQua,
            patientName = result.IdctphieuNavigation.IdphieuXetNghiemNavigation.IdkhachHangNavigation.TenKhachHang,
            specimenCode = result.IdmauNavigation?.MaBarcode,
            testName = result.IdctphieuNavigation.IdxetNghiemNavigation.TenXetNghiem,
            status = result.TrangThai,
            conclusion = result.KetLuanBacSi,
            approvedAt = result.ThoiGianDuyet,
            notes = result.GhiChu,
            indicators = result.Ketquachisos.Where(x => x.Status == "yes").Select(x => new { indicatorId = x.IdchiSo, name = x.IdchiSoNavigation.TenChiSo, value = x.GiaTriText ?? x.GiaTriSo?.ToString(CultureInfo.InvariantCulture) ?? "", unit = x.IdchiSoNavigation.DonVi, reference = x.GiaTriThamChieu, abnormal = x.DanhGia is "bat_thuong" or "thap" or "cao" })
        });
    }

    private async Task<string?> GetCustomerId()
    {
        if (!int.TryParse(User.FindFirstValue("userId"), out var uid)) return null;
        return await _context.Users.AsNoTracking().Where(x => x.UserId == uid && x.IsActive == true).Select(x => x.IdkhachHang).FirstOrDefaultAsync();
    }
}
