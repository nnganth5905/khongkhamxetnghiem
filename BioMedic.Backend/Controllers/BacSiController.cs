using Microsoft.AspNetCore.Mvc;
using BioMedic.Backend.Data;

namespace BioMedic.Backend.Controllers
{
    [ApiController]
    public class BacSiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BacSiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Danh sách hàng chờ (Danh sách chờ khám & Lấy mẫu)
        [HttpGet("api/doctor/waiting-list")]
        [HttpGet("api/bac-si/danh-sach-cho")]
        [HttpGet("api/doctor/danh-sach-cho")]
        public IActionResult GetDoctorQueue()
        {
            try
            {
                // Trả về danh sách bệnh nhân mẫu để hiển thị đẹp trên bảng giao diện
                var list = new List<object> {
                    new { id = 1, name = "Nguyen Van A", status = "waiting" },
                    new { id = 2, name = "Tran Thi B", status = "waiting" },
                    new { id = 3, name = "Le Van C", status = "waiting" }
                };  
                return Ok(new { success = true, data = list });
            }
            catch (Exception)
            {
                return Ok(new { success = false, data = new List<object>() });
            }
        }
    }
}