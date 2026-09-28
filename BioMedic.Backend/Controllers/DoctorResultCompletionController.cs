using System.Security.Claims;
using BioMedic.Backend.Data;
using BioMedic.Backend.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BioMedic.Backend.Controllers;

[ApiController]
[Route("api/doctor/results")]
[Authorize(Roles = "DOCTOR")]
public class DoctorResultCompletionController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DoctorResultCompletionController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Gọi sau khi POST /api/doctor/results/{id}/approve thành công.
    // Endpoint tự lần ngược:
    // ketquaxetnghiem -> ctphieuxetnghiem -> phieuxetnghiem -> kham -> luotkham
    // và hoàn tất ca khám, không cần frontend biết luotKhamId.
    [HttpPost("{resultId}/complete-visit")]
    public async Task<IActionResult> CompleteVisitFromApprovedResult(string resultId)
    {
        var doctorId = await GetDoctorId();

        if (doctorId is null)
        {
            return Unauthorized(new
            {
                success = false,
                message = "Không xác định được bác sĩ đang đăng nhập."
            });
        }

        var result = await _context.Ketquaxetnghiems
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdketQua == resultId);

        if (result is null)
        {
            return NotFound(new
            {
                success = false,
                message = "Không tìm thấy kết quả xét nghiệm."
            });
        }

        if (result.TrangThai != "da_duyet")
        {
            return BadRequest(new
            {
                success = false,
                message = "Kết quả xét nghiệm chưa được duyệt."
            });
        }

        var detail = await _context.Ctphieuxetnghiems
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Idctphieu == result.Idctphieu);

        if (detail is null)
        {
            return BadRequest(new
            {
                success = false,
                message = "Không tìm thấy chi tiết phiếu xét nghiệm."
            });
        }

        var order = await _context.Phieuxetnghiems
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.IdphieuXetNghiem == detail.IdphieuXetNghiem);

        if (order is null)
        {
            return BadRequest(new
            {
                success = false,
                message = "Không tìm thấy phiếu xét nghiệm."
            });
        }

        // Nếu đây là xét nghiệm đặt trực tiếp, không gắn với một ca khám,
        // thì không có ca khám để hoàn tất.
        if (order.Idkham is null)
        {
            return Ok(new
            {
                success = true,
                completed = false,
                reason = "no_examination",
                message = "Kết quả đã duyệt nhưng phiếu xét nghiệm không thuộc một ca khám."
            });
        }

        var examination = await _context.Khams
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Idkham == order.Idkham.Value);

        if (examination is null)
        {
            return BadRequest(new
            {
                success = false,
                message = "Không tìm thấy hồ sơ khám liên quan."
            });
        }

        var visit = await _context.Luotkhams
            .Include(x => x.IddatLichKhamNavigation)
            .FirstOrDefaultAsync(x => x.IdluotKham == examination.IdluotKham);

        if (visit is null)
        {
            return BadRequest(new
            {
                success = false,
                message = "Không tìm thấy lượt khám liên quan."
            });
        }

        if (visit.IdbacSi != doctorId.Value)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new
            {
                success = false,
                message = "Lượt khám này không thuộc bác sĩ đang đăng nhập."
            });
        }

        // Không hoàn tất ca khám nếu vẫn còn xét nghiệm khác của cùng hồ sơ khám
        // chưa được duyệt.
        var orderIds = await _context.Phieuxetnghiems
            .AsNoTracking()
            .Where(x => x.Idkham == examination.Idkham)
            .Select(x => x.IdphieuXetNghiem)
            .ToListAsync();

        foreach (var orderId in orderIds)
        {
            var hasNotApprovedDetail = await _context.Ctphieuxetnghiems
                .AsNoTracking()
                .AnyAsync(x =>
                    x.IdphieuXetNghiem == orderId &&
                    x.TrangThai != "da_duyet");

            if (hasNotApprovedDetail)
            {
                return Ok(new
                {
                    success = true,
                    completed = false,
                    reason = "pending_other_results",
                    visitId = visit.IdluotKham,
                    message = "Đã duyệt kết quả này. Ca khám chưa hoàn tất vì vẫn còn xét nghiệm khác chưa được duyệt."
                });
            }
        }

        // Idempotent: nếu đã hoàn tất thì trả thành công luôn.
        if (visit.TrangThai == "hoan_tat" ||
            visit.TrangThai == "completed" ||
            visit.TrangThai == "da_hoan_tat")
        {
            return Ok(new
            {
                success = true,
                completed = true,
                visitId = visit.IdluotKham,
                message = "Ca khám đã được hoàn tất trước đó."
            });
        }

        visit.TrangThai = "hoan_tat";
        visit.ThoiGianKetThuc = DateTime.Now;

        if (visit.IddatLichKhamNavigation is not null)
        {
            visit.IddatLichKhamNavigation.TrangThai = "completed";
        }

        AddTracking(visit, "Bác sĩ hoàn tất khám sau khi duyệt kết quả xét nghiệm");

        await _context.SaveChangesAsync();

        return Ok(new
        {
            success = true,
            completed = true,
            visitId = visit.IdluotKham,
            status = visit.TrangThai,
            message = "Đã duyệt kết quả và hoàn tất ca khám."
        });
    }

    private async Task<int?> GetDoctorId()
    {
        if (!int.TryParse(User.FindFirstValue("userId"), out var userId))
        {
            return null;
        }

        return await _context.Users
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.IsActive == true)
            .Select(x => x.IdbacSi)
            .FirstOrDefaultAsync();
    }

    private void AddTracking(Luotkham visit, string action)
    {
        int? userId = int.TryParse(
            User.FindFirstValue("userId"),
            out var parsed)
                ? parsed
                : null;

        _context.Truyvets.Add(new Truyvet
        {
            IdkhachHang = visit.IdkhachHang,
            LoaiDoiTuong = "luotkham",
            IddoiTuong = visit.IdluotKham.ToString(),
            HanhDong = action,
            UserIdthucHien = userId,
            NguonThucHien = "user",
            ThoiGian = DateTime.Now
        });
    }
}
