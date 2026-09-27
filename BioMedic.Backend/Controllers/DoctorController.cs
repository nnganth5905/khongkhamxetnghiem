using BioMedic.Backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BioMedic.Backend.Controllers;

[Route("api/doctors")]
[ApiController]
public class DoctorController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DoctorController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllDoctors()
    {
        var doctors = await _context.Bacsis
            .AsNoTracking()
            .Where(doctor => doctor.TrangThai == "active")
            .OrderBy(doctor => doctor.TenBacSi)
            .Select(doctor => new
            {
                id = doctor.IdbacSi,
                name = doctor.TenBacSi,
                degree = doctor.HocVi,
                title = doctor.ChucDanh,
                departmentId = doctor.KhoaId,
                departmentName = doctor.Khoa.TenChuyenKhoa,
                experience = doctor.NamKinhNghiem,
                rating = doctor.SoSao,
                image = doctor.HinhAnh,
                description = doctor.MoTa,
            })
            .ToListAsync();

        return Ok(new
        {
            success = true,
            data = doctors,
        });
    }
}
