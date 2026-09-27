using BioMedic.Backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BioMedic.Backend.Controllers;

[Route("api/tests")]
[ApiController]
public class TestController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TestController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Danh mục xét nghiệm dùng cho dropdown/đặt lịch.
    [HttpGet]
    public async Task<IActionResult> GetTests()
    {
        var tests = await _context.Loaixetnghiems
            .AsNoTracking()
            .Where(test => test.Status == "yes" || test.Status == "active")
            .OrderBy(test => test.TenXetNghiem)
            .Select(test => new
            {
                id = test.IdxetNghiem,
                name = test.TenXetNghiem,
                price = test.Gia,
                categoryId = test.ChuyenKhoaId,
                type = test.Loai,
                description = test.MoTa,
                sampleType = test.LoaiMauMacDinh,
                expectedMinutes = test.ThoiGianDuKienPhut,
            })
            .ToListAsync();

        return Ok(new { success = true, data = tests });
    }
}
