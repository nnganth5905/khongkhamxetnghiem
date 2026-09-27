using Microsoft.AspNetCore.Mvc;
using BioMedic.Backend.Data;

namespace BioMedic.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KhachHangController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public KhachHangController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var list = _context.Khachhangs.ToList();
                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}