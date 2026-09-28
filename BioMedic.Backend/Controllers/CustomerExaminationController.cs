using System.Security.Claims;
using BioMedic.Backend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BioMedic.Backend.Controllers;

[ApiController]
[Route("api/examinations")]
[Authorize(Roles = "CUSTOMER")]
public class CustomerExaminationController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CustomerExaminationController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("mine")]
    public async Task<IActionResult> GetMyExaminations()
    {
        if (!int.TryParse(User.FindFirstValue("userId"), out var userId))
        {
            return Unauthorized(new
            {
                success = false,
                message = "Không xác định được tài khoản."
            });
        }

        var customerId = await _context.Users
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.IsActive == true)
            .Select(x => x.IdkhachHang)
            .FirstOrDefaultAsync();

        if (string.IsNullOrWhiteSpace(customerId))
        {
            return Ok(Array.Empty<object>());
        }

        var rows = await _context.Khams
            .AsNoTracking()
            .Where(x =>
                x.IdluotKhamNavigation.IdkhachHang == customerId &&
                x.IdluotKhamNavigation.TrangThai == "hoan_tat")
            .OrderByDescending(x => x.ThoiGianKham)
            .Select(x => new
            {
                id = x.Idkham,

                visitId = x.IdluotKham,

                appointmentCode =
                    x.IdluotKhamNavigation
                     .IddatLichKhamNavigation
                     .MaDatLich,

                examinationDate = x.ThoiGianKham,

                doctorId = x.IdbacSi,

                doctorName =
                    x.IdbacSiNavigation.TenBacSi,

                symptoms = x.TrieuChung,

                medicalHistory = x.TienSuBenh,

                diagnosis = x.ChanDoan,

                conclusion = x.KetLuan,

                treatment = x.HuongDieuTri,

                status = x.IdluotKhamNavigation.TrangThai
            })
            .ToListAsync();

        return Ok(rows);
    }

    [HttpGet("mine/{id}")]
    public async Task<IActionResult> GetMyExamination(ulong id)
    {
        if (!int.TryParse(User.FindFirstValue("userId"), out var userId))
        {
            return Unauthorized();
        }

        var customerId = await _context.Users
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.IsActive == true)
            .Select(x => x.IdkhachHang)
            .FirstOrDefaultAsync();

        if (string.IsNullOrWhiteSpace(customerId))
        {
            return NotFound();
        }

        var item = await _context.Khams
            .AsNoTracking()
            .Where(x =>
                x.Idkham == id &&
                x.IdluotKhamNavigation.IdkhachHang == customerId)
            .Select(x => new
            {
                id = x.Idkham,

                visitId = x.IdluotKham,

                examinationDate = x.ThoiGianKham,

                doctorName =
                    x.IdbacSiNavigation.TenBacSi,

                patientName =
                    x.IdluotKhamNavigation
                     .IdkhachHangNavigation
                     .TenKhachHang,

                symptoms = x.TrieuChung,

                medicalHistory = x.TienSuBenh,

                diagnosis = x.ChanDoan,

                conclusion = x.KetLuan,

                treatment = x.HuongDieuTri,

                status = x.IdluotKhamNavigation.TrangThai
            })
            .FirstOrDefaultAsync();

        if (item is null)
        {
            return NotFound(new
            {
                message = "Không tìm thấy kết quả khám."
            });
        }

        return Ok(item);
    }
}