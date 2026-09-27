using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BioMedic.Backend.Data;

namespace BioMedic.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HoaDonController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public HoaDonController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var list = _context.Hoadons
                    .Include(h => h.IdkhachHangNavigation)
                    .ToList();
                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}