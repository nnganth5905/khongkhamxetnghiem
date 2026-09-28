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

    public CustomerResultsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("mine")]
    [Authorize(Roles = "CUSTOMER")]
    public async Task<IActionResult> Mine()
    {
        var customerId = await GetCustomerId();

        if (customerId is null)
        {
            return Unauthorized();
        }

        var rows = await _context.Ketquaxetnghiems
            .AsNoTracking()
            .Where(x =>
                x.TrangThai == "da_duyet" &&
                x.IdctphieuNavigation
                 .IdphieuXetNghiemNavigation
                 .IdkhachHang == customerId)
            .OrderByDescending(x => x.ThoiGianDuyet)
            .Select(x => new
            {
                id = x.IdketQua,
                code = x.IdketQua,

                specimenCode =
                    x.IdmauNavigation == null
                        ? null
                        : x.IdmauNavigation.MaBarcode,

                orderCode =
                    x.IdctphieuNavigation
                     .IdphieuXetNghiemNavigation
                     .IdphieuXetNghiem,

                testName =
                    x.IdctphieuNavigation
                     .IdxetNghiemNavigation
                     .TenXetNghiem,

                patientName =
                    x.IdctphieuNavigation
                     .IdphieuXetNghiemNavigation
                     .IdkhachHangNavigation
                     .TenKhachHang,

                // Bác sĩ trực tiếp khám bệnh.
                doctorName =
                    x.IdctphieuNavigation
                     .IdphieuXetNghiemNavigation
                     .IdkhamNavigation != null
                        ? x.IdctphieuNavigation
                           .IdphieuXetNghiemNavigation
                           .IdkhamNavigation
                           .IdbacSiNavigation
                           .TenBacSi
                        : (
                            x.IdctphieuNavigation
                             .IdphieuXetNghiemNavigation
                             .IdbacSiChiDinhNavigation != null
                                ? x.IdctphieuNavigation
                                   .IdphieuXetNghiemNavigation
                                   .IdbacSiChiDinhNavigation
                                   .TenBacSi
                                : (
                                    x.IdctphieuNavigation
                                     .IdphieuXetNghiemNavigation
                                     .IdbacSiPhuTrachNavigation != null
                                        ? x.IdctphieuNavigation
                                           .IdphieuXetNghiemNavigation
                                           .IdbacSiPhuTrachNavigation
                                           .TenBacSi
                                        : null
                                )
                        ),

                approvedDoctorName =
                    x.IdbacSiDuyetNavigation == null
                        ? null
                        : x.IdbacSiDuyetNavigation.TenBacSi,

                testDate =
                    x.ThoiGianHoanThanh ??
                    x.ThoiGianDuyet ??
                    x.ThoiGianNhap,

                approvedAt = x.ThoiGianDuyet,

                status = "APPROVED",

                conclusion = x.KetLuanBacSi,

                abnormal = x.Ketquachisos.Any(c =>
                    c.Status == "yes" &&
                    (c.DanhGia == "bat_thuong" ||
                     c.DanhGia == "thap" ||
                     c.DanhGia == "cao"))
            })
            .ToListAsync();

        return Ok(rows);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> Detail(string id)
    {
        var result = await _context.Ketquaxetnghiems
            .AsNoTracking()
            .Include(x => x.IdbacSiDuyetNavigation)
            .Include(x => x.IdctphieuNavigation)
                .ThenInclude(x => x.IdxetNghiemNavigation)
            .Include(x => x.IdctphieuNavigation)
                .ThenInclude(x => x.IdphieuXetNghiemNavigation)
                    .ThenInclude(x => x.IdkhachHangNavigation)
            .Include(x => x.IdctphieuNavigation)
                .ThenInclude(x => x.IdphieuXetNghiemNavigation)
                    .ThenInclude(x => x.IdkhamNavigation)
                        .ThenInclude(x => x!.IdbacSiNavigation)
            .Include(x => x.IdctphieuNavigation)
                .ThenInclude(x => x.IdphieuXetNghiemNavigation)
                    .ThenInclude(x => x.IdbacSiChiDinhNavigation)
            .Include(x => x.IdctphieuNavigation)
                .ThenInclude(x => x.IdphieuXetNghiemNavigation)
                    .ThenInclude(x => x.IdbacSiPhuTrachNavigation)
            .Include(x => x.IdmauNavigation)
            .Include(x => x.Ketquachisos)
                .ThenInclude(x => x.IdchiSoNavigation)
            .FirstOrDefaultAsync(x => x.IdketQua == id);

        if (result is null)
        {
            return NotFound(new
            {
                success = false,
                message = "Không tìm thấy kết quả xét nghiệm."
            });
        }

        if (User.IsInRole("CUSTOMER"))
        {
            var customerId = await GetCustomerId();

            if (customerId !=
                    result.IdctphieuNavigation
                          .IdphieuXetNghiemNavigation
                          .IdkhachHang ||
                result.TrangThai != "da_duyet")
            {
                return Forbid();
            }
        }

        var order = result.IdctphieuNavigation.IdphieuXetNghiemNavigation;

        var examiningDoctorName =
            order.IdkhamNavigation?.IdbacSiNavigation?.TenBacSi
            ?? order.IdbacSiChiDinhNavigation?.TenBacSi
            ?? order.IdbacSiPhuTrachNavigation?.TenBacSi;

        return Ok(new
        {
            id = result.IdketQua,
            code = result.IdketQua,

            patientCode = order.IdkhachHang,
            patientName = order.IdkhachHangNavigation.TenKhachHang,

            orderCode = order.IdphieuXetNghiem,

            specimenCode = result.IdmauNavigation?.MaBarcode,

            testName =
                result.IdctphieuNavigation
                      .IdxetNghiemNavigation
                      .TenXetNghiem,

            // Bác sĩ khám / chỉ định.
            doctorName = examiningDoctorName,
            examiningDoctorName,

            // Bác sĩ phê duyệt kết quả.
            approvedDoctorName =
                result.IdbacSiDuyetNavigation?.TenBacSi,

            testDate =
                result.ThoiGianHoanThanh ??
                result.ThoiGianDuyet ??
                result.ThoiGianNhap,

            approvedAt = result.ThoiGianDuyet,

            status = result.TrangThai,

            conclusion = result.KetLuanBacSi,

            notes = result.GhiChu,

            indicators = result.Ketquachisos
                .Where(x => x.Status == "yes")
                .Select(x => new
                {
                    indicatorId = x.IdchiSo,

                    name =
                        x.IdchiSoNavigation.TenChiSo,

                    value =
                        x.GiaTriText
                        ?? x.GiaTriSo?.ToString(
                            CultureInfo.InvariantCulture)
                        ?? "",

                    unit =
                        x.IdchiSoNavigation.DonVi,

                    reference =
                        x.GiaTriThamChieu,

                    abnormal =
                        x.DanhGia == "bat_thuong" ||
                        x.DanhGia == "thap" ||
                        x.DanhGia == "cao"
                })
        });
    }

    private async Task<string?> GetCustomerId()
    {
        if (!int.TryParse(
                User.FindFirstValue("userId"),
                out var userId))
        {
            return null;
        }

        return await _context.Users
            .AsNoTracking()
            .Where(x =>
                x.UserId == userId &&
                x.IsActive == true)
            .Select(x => x.IdkhachHang)
            .FirstOrDefaultAsync();
    }
}
