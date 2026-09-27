using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BioMedic.Backend.Data;

namespace BioMedic.Backend.Controllers
{
    [ApiController]
    public class CommonController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CommonController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Lấy danh sách chuyên khoa trực tiếp từ bảng 'chuyenkhoa'
        [HttpGet("api/chuyenkhoa")]
        [HttpGet("api/departments")]
        public IActionResult GetChuyenKhoa()
        {
            try
            {
                var connection = _context.Database.GetDbConnection();
                if (connection.State != System.Data.ConnectionState.Open)
                    connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = "SELECT IDChuyenKhoa as id, TenChuyenKhoa as name FROM chuyenkhoa WHERE Status = 'yes'";
                
                using var reader = command.ExecuteReader();
                var list = new List<object>();
                while (reader.Read())   
                {
                    list.Add(new
                    {
                        id = reader["id"],
                        name = reader["name"]
                    });
                }   
                return Ok(new { success = true, data = list });
            }
            catch (Exception)
            {
                return Ok(new { success = false, data = new List<object>() });
            }       
        }
    }
}   