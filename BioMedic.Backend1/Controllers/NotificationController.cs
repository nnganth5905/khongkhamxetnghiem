using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BioMedic.Backend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BioMedic.Backend.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public NotificationController(ApplicationDbContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> GetNotifications()
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var data = await _context.Thongbaos.AsNoTracking()
            .Where(x => x.UserIdnhan == userId)
            .OrderByDescending(x => x.ThoiGianTao)
            .Take(100)
            .Select(x => new
            {
                id = x.IdthongBao,
                type = x.LoaiThongBao,
                title = x.TieuDe,
                content = x.NoiDung,
                objectType = x.LoaiDoiTuong,
                objectId = x.IddoiTuong,
                read = x.DaDoc,
                createdAt = x.ThoiGianTao,
                readAt = x.ThoiGianDoc,
            }).ToListAsync();
        return Ok(data);
    }

    [HttpPatch("{id}/read")]
    public async Task<IActionResult> MarkRead(ulong id)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var item = await _context.Thongbaos.FirstOrDefaultAsync(x => x.IdthongBao == id && x.UserIdnhan == userId);
        if (item is null) return NotFound(new { message = "Không tìm thấy thông báo." });
        item.DaDoc = true;
        item.ThoiGianDoc = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return Ok(new { success = true });
    }

    [HttpPost("read-all")]
    public async Task<IActionResult> MarkAllRead()
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var items = await _context.Thongbaos.Where(x => x.UserIdnhan == userId && !x.DaDoc).ToListAsync();
        var now = DateTime.UtcNow;
        foreach (var item in items) { item.DaDoc = true; item.ThoiGianDoc = now; }
        await _context.SaveChangesAsync();
        return Ok(new { success = true, updated = items.Count });
    }

    private bool TryGetUserId(out int userId) => int.TryParse(User.FindFirstValue("userId"), out userId);
}
