using Microsoft.AspNetCore.Mvc;
using BioMedic.Backend.Data;

namespace BioMedic.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class XetNghiemController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public XetNghiemController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var list = _context.Loaixetnghiems.ToList();
                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}