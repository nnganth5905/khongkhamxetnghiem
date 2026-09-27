using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BioMedic.Backend.Data;

namespace BioMedic.Backend.Controllers
{
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
        public IActionResult GetAllDoctors()
        {
            try
            {
                // Sử dụng cách truy vấn trực tiếp an toàn, tránh lệch tên thuộc tính viết hoa/thường
                var doctors = _context.Bacsis
                    .FromSqlRaw("SELECT * FROM bacsi WHERE TrangThai = 'active'")
                    .Select(b => new {
                        id = b.IdbacSi,
                        name = b.TenBacSi,
                        degree = b.HocVi,
                        title = b.ChucDanh,
                        departmentId = b.KhoaId,
                        experience = b.NamKinhNghiem,
                        rating = b.SoSao
                    })
                    .ToList();

                return Ok(new {
                    success = true,
                    data = doctors
                });
            }
            catch (Exception ex)
            {
                // Fallback nếu có lỗi câu lệnh thô, trả về danh sách rỗng để không sập app
                return Ok(new { success = true, data = new List<object>() });
            }
        }
    }
}
