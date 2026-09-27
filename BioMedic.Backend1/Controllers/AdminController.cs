using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System;
using System.Collections.Generic;
using BioMedic.Backend.Data;

namespace BioMedic.Backend.Controllers
{
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("dashboard")]
        [HttpGet("stats")]
        public IActionResult GetAdminDashboardStats()
        {
            try
            {
                int khachHang = 0, bacSi = 0, phieuXetNghiem = 0, datLich = 0;

                var connection = _context.Database.GetDbConnection();
                if (connection.State != ConnectionState.Open) connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT 
                            (SELECT COUNT(*) FROM khachhang) AS TotalKhachHang,
                            (SELECT COUNT(*) FROM bacsi) AS TotalBacSi,
                            (SELECT COUNT(*) FROM phieuxetnghiem) AS TotalPhieuXN,
                            (SELECT COUNT(*) FROM datlichkham) AS TotalDatLich";

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            khachHang = reader["TotalKhachHang"] != DBNull.Value ? Convert.ToInt32(reader["TotalKhachHang"]) : 0;
                            bacSi = reader["TotalBacSi"] != DBNull.Value ? Convert.ToInt32(reader["TotalBacSi"]) : 0;
                            phieuXetNghiem = reader["TotalPhieuXN"] != DBNull.Value ? Convert.ToInt32(reader["TotalPhieuXN"]) : 0;
                            datLich = reader["TotalDatLich"] != DBNull.Value ? Convert.ToInt32(reader["TotalDatLich"]) : 0;
                        }
                    }
                }

                return Ok(new {
                    success = true,
                    data = new {
                        customersCount = khachHang,
                        employeesCount = 5,
                        doctorsCount = bacSi,
                        techniciansCount = 2,
                        testOrdersCount = phieuXetNghiem,
                        resultsCount = phieuXetNghiem,
                        todayAppointments = datLich,
                        pendingResults = 0,
                        revenue = 0
                    }
                });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Lỗi truy vấn: " + ex.Message });
            }
        }

        [HttpGet("results")]
        [HttpGet("ket-qua")]
        public IActionResult GetAdminResults()
        {
            try
            {
                var list = new List<object> {
                    new { id = 1, name = "Nguyen Van A", status = "pending" },
                    new { id = 2, name = "Tran Thi B", status = "completed" },
                    new { id = 3, name = "Le Van C", status = "pending" }
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