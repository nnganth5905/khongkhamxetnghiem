using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BioMedic.Backend.Data;
using BioMedic.Backend.Entities;

namespace BioMedic.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatLichKhamController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DatLichKhamController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/datlichkham
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var list = _context.Datlichkhams
                    .Include(d => d.IdkhachHangNavigation)
                    .Include(d => d.IdbacSiNavigation)
                    .ToList();
                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST: api/datlichkham
        [HttpPost]
        public IActionResult Create([FromBody] Datlichkham model)
        {
            try
            {
                model.CreatedAt = DateTime.Now;
                model.TrangThai = "pending";
                _context.Datlichkhams.Add(model);
                _context.SaveChanges();
                return Ok(new { success = true, message = "Đặt lịch thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}