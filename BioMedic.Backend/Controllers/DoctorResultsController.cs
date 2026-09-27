using System.Globalization;
using BioMedic.Backend.Data;
using BioMedic.Backend.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BioMedic.Backend.Controllers;

[ApiController]
[Route("api/doctor/results")]
[Authorize(Roles = "DOCTOR")]
public class DoctorResultsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DoctorResultsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingResults()
    {
        var doctorId = await GetDoctorId();
        if (doctorId is null)
        {
            return Forbid();
        }

        var results = await ResultsForDoctor(doctorId.Value)
            .Where(result => result.TrangThai == "cho_duyet")
            .Include(result => result.IdctphieuNavigation)
                .ThenInclude(item => item.IdxetNghiemNavigation)
            .Include(result => result.IdctphieuNavigation)
                .ThenInclude(item => item.IdphieuXetNghiemNavigation)
                    .ThenInclude(item => item.IdkhachHangNavigation)
            .Include(result => result.IdktvNavigation)
            .Include(result => result.IdmauNavigation)
            .OrderByDescending(result => result.ThoiGianHoanThanh)
            .ToListAsync();

        return Ok(results.Select(result => new
        {
            id = result.IdketQua,
            specimenCode = result.IdmauNavigation?.MaBarcode ?? "Chưa rõ",
            patientName = result.IdctphieuNavigation.IdphieuXetNghiemNavigation.IdkhachHangNavigation.TenKhachHang,
            testName = result.IdctphieuNavigation.IdxetNghiemNavigation.TenXetNghiem,
            technicianName = result.IdktvNavigation.TenNhanVien,
            submittedAt = result.ThoiGianHoanThanh,
            status = "PENDING_APPROVAL",
        }));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetResultDetail(string id)
    {
        var doctorId = await GetDoctorId();
        if (doctorId is null)
        {
            return Forbid();
        }

        var result = await ResultsForDoctor(doctorId.Value)
            .Include(item => item.IdctphieuNavigation)
                .ThenInclude(item => item.IdxetNghiemNavigation)
            .Include(item => item.IdctphieuNavigation)
                .ThenInclude(item => item.IdphieuXetNghiemNavigation)
                    .ThenInclude(item => item.IdkhachHangNavigation)
            .Include(item => item.IdktvNavigation)
            .Include(item => item.IdmauNavigation)
            .Include(item => item.Ketquachisos)
                .ThenInclude(item => item.IdchiSoNavigation)
            .FirstOrDefaultAsync(item => item.IdketQua == id);

        if (result is null)
        {
            return NotFound(new { success = false, message = "Không tìm thấy kết quả xét nghiệm." });
        }

        return Ok(new
        {
            id = result.IdketQua,
            specimenCode = result.IdmauNavigation?.MaBarcode ?? "Chưa rõ",
            patientName = result.IdctphieuNavigation.IdphieuXetNghiemNavigation.IdkhachHangNavigation.TenKhachHang,
            testName = result.IdctphieuNavigation.IdxetNghiemNavigation.TenXetNghiem,
            technicianNotes = result.GhiChu,
            indicators = result.Ketquachisos.Select(indicator => new
            {
                name = indicator.IdchiSoNavigation.TenChiSo,
                value = indicator.GiaTriText ?? indicator.GiaTriSo?.ToString("0.####", CultureInfo.InvariantCulture) ?? "",
                unit = indicator.IdchiSoNavigation.DonVi,
                abnormal = indicator.DanhGia is "bat_thuong" or "thap" or "cao",
            }),
        });
    }

    [HttpPost("{id}/approve")]
    public async Task<IActionResult> ApproveResult(string id, [FromBody] ApproveResultRequest request)
    {
        var doctorId = await GetDoctorId();
        if (doctorId is null)
        {
            return Forbid();
        }

        if (string.IsNullOrWhiteSpace(request.Conclusion))
        {
            return BadRequest(new { success = false, message = "Vui lòng nhập kết luận của bác sĩ." });
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();
        var result = await ResultsForDoctor(doctorId.Value)
            .Include(item => item.IdctphieuNavigation)
                .ThenInclude(item => item.IdphieuXetNghiemNavigation)
                    .ThenInclude(item => item.IdluotXetNghiemNavigation)
                        .ThenInclude(item => item!.IddatLichXnNavigation)
            .Include(item => item.IdctphieuNavigation)
                .ThenInclude(item => item.IdphieuXetNghiemNavigation)
                    .ThenInclude(item => item.IdkhachHangNavigation)
            .Include(item => item.IdmauNavigation)
            .FirstOrDefaultAsync(item => item.IdketQua == id && item.TrangThai == "cho_duyet");

        if (result is null)
        {
            return NotFound(new { success = false, message = "Không tìm thấy kết quả đang chờ duyệt." });
        }

        var now = DateTime.Now;
        result.TrangThai = "da_duyet";
        result.KetLuanBacSi = request.Conclusion.Trim();
        result.IdbacSiDuyet = doctorId.Value;
        result.ThoiGianDuyet = now;

        var detail = result.IdctphieuNavigation;
        detail.TrangThai = "da_duyet";

        var order = detail.IdphieuXetNghiemNavigation;
        order.TrangThai = "da_co_kq";

        if (result.IdmauNavigation is not null)
        {
            result.IdmauNavigation.TrangThai = "hoan_tat";
        }

        if (order.IdluotXetNghiemNavigation is { } visit)
        {
            visit.TrangThai = "hoan_tat";
            visit.ThoiGianKetThuc ??= now;
            visit.IddatLichXnNavigation.TrangThai = "completed";
        }

        var customerUserId = await _context.Users
            .Where(user => user.IdkhachHang == order.IdkhachHang && user.IsActive == true)
            .Select(user => (int?)user.UserId)
            .FirstOrDefaultAsync();
        if (customerUserId is not null)
        {
            _context.Thongbaos.Add(new Thongbao
            {
                UserIdnhan = customerUserId.Value,
                LoaiThongBao = "ket_qua_xet_nghiem",
                TieuDe = "Đã có kết quả xét nghiệm",
                NoiDung = "Kết quả xét nghiệm của bạn đã được bác sĩ phê duyệt.",
                LoaiDoiTuong = "ketquaxetnghiem",
                IddoiTuong = result.IdketQua,
                DaDoc = false,
                ThoiGianTao = now,
            });
        }

        int? userId = int.TryParse(User.FindFirst("userId")?.Value, out var parsedUserId) ? parsedUserId : null;
        _context.Truyvets.Add(new Truyvet
        {
            IdkhachHang = order.IdkhachHang,
            LoaiDoiTuong = "ketquaxetnghiem",
            IddoiTuong = result.IdketQua,
            HanhDong = "Bác sĩ phê duyệt kết quả xét nghiệm",
            TrangThaiCu = "cho_duyet",
            TrangThaiMoi = "da_duyet",
            UserIdthucHien = userId,
            NguonThucHien = "user",
            ThoiGian = now,
            MoTa = request.Conclusion.Trim(),
        });

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return Ok(new
        {
            status = "SUCCESS",
            message = "Đã phê duyệt kết quả xét nghiệm thành công.",
        });
    }

    private IQueryable<Ketquaxetnghiem> ResultsForDoctor(int doctorId) =>
        _context.Ketquaxetnghiems.Where(result =>
            result.IdctphieuNavigation.IdphieuXetNghiemNavigation.IdbacSiPhuTrach == doctorId ||
            (result.IdctphieuNavigation.IdphieuXetNghiemNavigation.IdbacSiPhuTrach == null &&
             (result.IdctphieuNavigation.IdphieuXetNghiemNavigation.IdluotXetNghiemNavigation == null ||
              result.IdctphieuNavigation.IdphieuXetNghiemNavigation.IdluotXetNghiemNavigation.IddatLichXnNavigation.IdbacSi == null ||
              result.IdctphieuNavigation.IdphieuXetNghiemNavigation.IdluotXetNghiemNavigation.IddatLichXnNavigation.IdbacSi == doctorId)));

    private async Task<int?> GetDoctorId()
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        User? account = null;
        if (int.TryParse(userIdClaim, out var userId))
        {
            account = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(user => user.UserId == userId);
        }

        var email = User.Identity?.Name;
        if (account is null && !string.IsNullOrWhiteSpace(email))
        {
            account = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(user => user.Email == email || user.Username == email);
        }

        if (account is null)
        {
            return null;
        }

        if (account.IdbacSi.HasValue)
        {
            return account.IdbacSi.Value;
        }

        if (string.IsNullOrWhiteSpace(account.IdnhanVien))
        {
            return null;
        }

        return await _context.Bacsis
            .Where(doctor => doctor.IdnhanVien == account.IdnhanVien)
            .Select(doctor => (int?)doctor.IdbacSi)
            .SingleOrDefaultAsync();
    }

    public sealed record ApproveResultRequest(string? Conclusion);
}