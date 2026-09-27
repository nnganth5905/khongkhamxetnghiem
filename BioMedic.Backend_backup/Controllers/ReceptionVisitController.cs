using BioMedic.Backend.Data;
using BioMedic.Backend.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BioMedic.Backend.Controllers;

[ApiController]
[Route("api/reception/visits")]
[Authorize(Roles = "RECEPTIONIST")]
public class ReceptionVisitController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ReceptionVisitController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("waiting")]
    public async Task<IActionResult> GetWaitingList([FromQuery] string? type = "ALL")
    {
        var waitingList = new List<WaitingQueueItem>();
        var startOfDay = DateTime.Today;
        var startOfNextDay = startOfDay.AddDays(1);

        if (type is null || type.Equals("ALL", StringComparison.OrdinalIgnoreCase) ||
            type.Equals("EXAMINATION", StringComparison.OrdinalIgnoreCase))
        {
            var examinations = await _context.Luotkhams
                .Where(visit => visit.ThoiGianTiepNhan >= startOfDay &&
                    visit.ThoiGianTiepNhan < startOfNextDay &&
                    (visit.TrangThai == "da_tiep_nhan" || visit.TrangThai == "cho_kham"))
                .Select(visit => new WaitingQueueItem(
                    visit.SoThuTu,
                    visit.IddatLichKhamNavigation.MaDatLich,
                    visit.IdkhachHangNavigation.TenKhachHang,
                    "Khám bệnh",
                    visit.IdphongNavigation == null ? "Chưa xếp phòng" : visit.IdphongNavigation.TenPhong,
                    visit.ThoiGianTiepNhan,
                    visit.TrangThai))
                .ToListAsync();

            waitingList.AddRange(examinations);
        }

        if (type is null || type.Equals("ALL", StringComparison.OrdinalIgnoreCase) ||
            type.Equals("TEST", StringComparison.OrdinalIgnoreCase))
        {
            var tests = await _context.Luotxetnghiems
                .Where(visit => visit.ThoiGianTiepNhan >= startOfDay &&
                    visit.ThoiGianTiepNhan < startOfNextDay &&
                    (visit.TrangThai == "da_tiep_nhan" || visit.TrangThai == "cho_xet_nghiem"))
                .Select(visit => new WaitingQueueItem(
                    visit.SoThuTu,
                    visit.IddatLichXnNavigation.MaDatLich,
                    visit.IdkhachHangNavigation.TenKhachHang,
                    "Xét nghiệm",
                    visit.IdphongNavigation == null ? "Chưa xếp phòng" : visit.IdphongNavigation.TenPhong,
                    visit.ThoiGianTiepNhan,
                    visit.TrangThai))
                .ToListAsync();

            waitingList.AddRange(tests);
        }

        return Ok(waitingList
            .OrderBy(item => item.ThoiGianTiepNhan)
            .Select(item => new
            {
                stt = item.Stt,
                maLuot = item.MaLuot,
                tenNguoiBenh = item.TenNguoiBenh,
                dichVu = item.DichVu,
                phong = item.Phong,
                checkInTime = item.ThoiGianTiepNhan?.ToString("HH:mm") ?? "",
                trangThai = item.TrangThai,
            }));
    }

    private sealed record WaitingQueueItem(
        int? Stt,
        string MaLuot,
        string TenNguoiBenh,
        string DichVu,
        string Phong,
        DateTime? ThoiGianTiepNhan,
        string TrangThai);
}