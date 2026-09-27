using BioMedic.Backend.Data;
using BioMedic.Backend.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BioMedic.Backend.Controllers;

[ApiController]
[Route("api/appointments")]
[Authorize(Roles = "DOCTOR")]
public class DoctorQueueController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DoctorQueueController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("waiting-queue", Order = 0)]
    [HttpGet("~/api/doctor/queue", Order = 0)]
    public async Task<IActionResult> GetWaitingQueue([FromQuery] string? type)
    {
        var doctorId = await GetDoctorId();
        if (doctorId is null)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new
            {
                success = false,
                message = "Tài khoản bác sĩ chưa được liên kết với hồ sơ bác sĩ trong hệ thống.",
            });
        }

        var queueType = string.IsNullOrWhiteSpace(type) ? "ALL" : type.Trim().ToUpperInvariant();
        if (queueType is not ("ALL" or "EXAMINATION" or "TEST"))
        {
            return BadRequest(new { success = false, message = "Loại hàng chờ không hợp lệ." });
        }

        var today = DateOnly.FromDateTime(DateTime.Today);
        var queue = new List<DoctorQueueEntry>();

        if (queueType is "ALL" or "EXAMINATION")
        {
            var visits = await _context.Luotkhams
                .AsNoTracking()
                .Where(visit => visit.IdbacSi == doctorId &&
                    visit.IddatLichKhamNavigation.NgayKham == today &&
                    (visit.TrangThai == "da_tiep_nhan" ||
                     visit.TrangThai == "cho_kham" ||
                     visit.TrangThai == "da_den_luot" ||
                     visit.TrangThai == "cho_goi_lai"))
                .Select(visit => new
                {
                    visit.IddatLichKhamNavigation.MaDatLich,
                    visit.IdkhachHang,
                    visit.IdkhachHangNavigation.TenKhachHang,
                    visit.IddatLichKhamNavigation.GioKham,
                    visit.IddatLichKhamNavigation.GhiChu,
                    visit.SoThuTu,
                    visit.TrangThai,
                    visit.ThoiGianTiepNhan,
                })
                .ToListAsync();

            queue.AddRange(visits.Select(visit => new DoctorQueueEntry(
                visit.MaDatLich,
                "EXAMINATION",
                visit.IdkhachHang,
                visit.TenKhachHang,
                visit.GioKham.ToString("HH:mm"),
                visit.TrangThai,
                visit.ThoiGianTiepNhan,
                string.IsNullOrWhiteSpace(visit.GhiChu) ? "Khám bệnh" : visit.GhiChu,
                visit.SoThuTu ?? 0)));
        }

        if (queueType is "ALL" or "TEST")
        {
            var visits = await _context.Luotxetnghiems
                .AsNoTracking()
                .Where(visit => visit.IddatLichXnNavigation.IdbacSi == doctorId &&
                    visit.IddatLichXnNavigation.NgayXetNghiem == today &&
                    (visit.TrangThai == "da_tiep_nhan" ||
                     visit.TrangThai == "cho_xet_nghiem" ||
                     visit.TrangThai == "da_den_luot" ||
                     visit.TrangThai == "cho_goi_lai"))
                .Select(visit => new
                {
                    visit.IddatLichXn,
                    visit.IddatLichXnNavigation.MaDatLich,
                    visit.IdkhachHang,
                    visit.IdkhachHangNavigation.TenKhachHang,
                    visit.IddatLichXnNavigation.GioXetNghiem,
                    visit.SoThuTu,
                    visit.TrangThai,
                    visit.ThoiGianTiepNhan,
                })
                .ToListAsync();

            var appointmentIds = visits.Select(visit => visit.IddatLichXn).ToList();
            var testNames = await _context.Ctdatlichxetnghiems
                .AsNoTracking()
                .Where(item => appointmentIds.Contains(item.IddatLichXn))
                .Select(item => new
                {
                    item.IddatLichXn,
                    item.IdxetNghiemNavigation.TenXetNghiem,
                })
                .ToListAsync();

            queue.AddRange(visits.Select(visit => new DoctorQueueEntry(
                visit.MaDatLich,
                "TEST",
                visit.IdkhachHang,
                visit.TenKhachHang,
                visit.GioXetNghiem.ToString("HH:mm"),
                visit.TrangThai,
                visit.ThoiGianTiepNhan,
                string.Join(", ", testNames
                    .Where(item => item.IddatLichXn == visit.IddatLichXn)
                    .Select(item => item.TenXetNghiem)),
                visit.SoThuTu ?? 0)));
        }

        return Ok(queue
            .OrderByDescending(item => item.StatusCode == "da_den_luot")
            .ThenBy(item => item.Stt)
            .Select(item => new
            {
                id = item.Id,
                type = item.Type,
                patientCode = item.PatientCode,
                hoten = item.FullName,
                gio = item.Time,
                statusCode = item.StatusCode,
                status = GetStatusText(item.StatusCode),
                thoiGianTiepNhan = item.CheckedInAt,
                dichVu = item.Service,
                stt = item.Stt,
            }));
    }

    [HttpPost("queue/{id}/call")]
    [HttpPost("~/api/doctor/queue/{id}/call")]
    public Task<IActionResult> CallPatient(string id) => UpdateQueueItem(id, QueueAction.Call);

    [HttpPost("queue/{id}/hold")]
    [HttpPost("~/api/doctor/queue/{id}/hold")]
    public Task<IActionResult> HoldPatient(string id) => UpdateQueueItem(id, QueueAction.Hold);

    [HttpPost("queue/{id}/skip")]
    [HttpPost("~/api/doctor/queue/{id}/skip")]
    public Task<IActionResult> SkipPatient(string id) => UpdateQueueItem(id, QueueAction.Skip);

    private async Task<IActionResult> UpdateQueueItem(string id, QueueAction action)
    {
        var doctorId = await GetDoctorId();
        if (doctorId is null)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new
            {
                success = false,
                message = "Tài khoản bác sĩ chưa được liên kết với hồ sơ bác sĩ trong hệ thống.",
            });
        }

        var today = DateOnly.FromDateTime(DateTime.Today);
        if (id.StartsWith("DLK", StringComparison.OrdinalIgnoreCase))
        {
            var visit = await _context.Luotkhams
                .Include(item => item.IddatLichKhamNavigation)
                .FirstOrDefaultAsync(item => item.IddatLichKhamNavigation.MaDatLich == id &&
                    item.IdbacSi == doctorId &&
                    item.IddatLichKhamNavigation.NgayKham == today);
            if (visit is null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy lượt khám trong hàng chờ của bác sĩ." });
            }

            ApplyQueueAction(visit, action);
            if (action == QueueAction.Skip)
            {
                visit.TrangThai = "bo_luot";
                visit.IddatLichKhamNavigation.TrangThai = "no_show";
            }
        }
        else if (id.StartsWith("DLXN", StringComparison.OrdinalIgnoreCase))
        {
            var visit = await _context.Luotxetnghiems
                .Include(item => item.IddatLichXnNavigation)
                .FirstOrDefaultAsync(item => item.IddatLichXnNavigation.MaDatLich == id &&
                    item.IddatLichXnNavigation.IdbacSi == doctorId &&
                    item.IddatLichXnNavigation.NgayXetNghiem == today);
            if (visit is null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy lượt xét nghiệm trong hàng chờ của bác sĩ." });
            }

            ApplyQueueAction(visit, action);
            if (action == QueueAction.Skip)
            {
                visit.TrangThai = "huy";
                visit.IddatLichXnNavigation.TrangThai = "no_show";
            }
        }
        else
        {
            return BadRequest(new { success = false, message = "Mã lượt không hợp lệ." });
        }

        await _context.SaveChangesAsync();
        return Ok(new { success = true, message = GetActionMessage(action) });
    }

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

        var employeeId = account.IdnhanVien;
        if (string.IsNullOrWhiteSpace(employeeId) && !string.IsNullOrWhiteSpace(account.Email))
        {
            employeeId = await _context.Nhanviens
                .AsNoTracking()
                .Where(employee => employee.Email == account.Email)
                .Select(employee => employee.IdnhanVien)
                .FirstOrDefaultAsync();
        }

        if (string.IsNullOrWhiteSpace(employeeId))
        {
            return null;
        }

        return await _context.Bacsis
            .AsNoTracking()
            .Where(doctor => doctor.IdnhanVien == employeeId)
            .Select(doctor => (int?)doctor.IdbacSi)
            .SingleOrDefaultAsync();
    }

    private static void ApplyQueueAction(Luotkham visit, QueueAction action)
    {
        visit.TrangThai = action switch
        {
            QueueAction.Call => "da_den_luot",
            QueueAction.Hold => "da_tiep_nhan",
            _ => visit.TrangThai,
        };

        if (action == QueueAction.Hold)
        {
            visit.ThoiGianTiepNhan = DateTime.Now;
        }
    }

    private static void ApplyQueueAction(Luotxetnghiem visit, QueueAction action)
    {
        visit.TrangThai = action switch
        {
            QueueAction.Call => "da_den_luot",
            QueueAction.Hold => "da_tiep_nhan",
            _ => visit.TrangThai,
        };

        if (action == QueueAction.Hold)
        {
            visit.ThoiGianTiepNhan = DateTime.Now;
        }
    }

    private static string GetStatusText(string status) => status switch
    {
        "da_den_luot" => "Đang gọi vào...",
        "cho_goi_lai" => "Chờ gọi lại",
        _ => "Chờ khám",
    };

    private static string GetActionMessage(QueueAction action) => action switch
    {
        QueueAction.Call => "Đã gọi bệnh nhân.",
        QueueAction.Hold => "Đã chuyển bệnh nhân xuống cuối hàng chờ.",
        _ => "Đã đánh dấu bệnh nhân bỏ lượt.",
    };

    private enum QueueAction
    {
        Call,
        Hold,
        Skip,
    }

    private sealed record DoctorQueueEntry(
        string Id,
        string Type,
        string PatientCode,
        string FullName,
        string Time,
        string StatusCode,
        DateTime? CheckedInAt,
        string Service,
        int Stt);
}