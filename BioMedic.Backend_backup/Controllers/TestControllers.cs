using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BioMedic.Backend.Data;
using System;
using System.Collections.Generic;

namespace BioMedic.Backend.Controllers
{
    [Route("api/tests")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TestController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/tests -> Đổ danh sách xét nghiệm ra dropdown
        [HttpGet]
        public IActionResult GetTests()
        {
            try
            {
                var connection = _context.Database.GetDbConnection();
                if (connection.State != System.Data.ConnectionState.Open)
                    connection.Open();

                using var command = connection.CreateCommand();
                // Truy vấn thẳng vào bảng loaixetnghiem
                command.CommandText = "SELECT IDXetNghiem as id, TenXetNghiem as name, Gia as price FROM loaixetnghiem";

                using var reader = command.ExecuteReader();
                var list = new List<object>();
                while (reader.Read())
                {
                    list.Add(new
                    {
                        id = reader["id"],
                        name = reader["name"],
                        price = reader["price"]
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