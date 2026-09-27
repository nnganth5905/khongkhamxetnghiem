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
public async Task<IActionResult> GetAdminResults()
{
    try
    {
        var list = await _context.Ketquaxetnghiems
            .AsNoTracking()
            .Include(x => x.IdctphieuNavigation)
                .ThenInclude(x => x.IdxetNghiemNavigation)
            .Include(x => x.IdctphieuNavigation)
                .ThenInclude(x => x.IdphieuXetNghiemNavigation)
            .Include(x => x.IdbacSiDuyetNavigation)
            .Include(x => x.Ketquachisos)
            .OrderByDescending(x =>
                x.ThoiGianDuyet ??
                x.ThoiGianHoanThanh ??
                x.ThoiGianNhap)
            .Select(x => new
            {
                id = x.IdketQua,

                code = x.IdketQua,

                testName =
                    x.IdctphieuNavigation
                     .IdxetNghiemNavigation
                     .TenXetNghiem,

                testDate =
                    x.ThoiGianDuyet ??
                    x.ThoiGianHoanThanh ??
                    x.ThoiGianNhap,

                doctorName =
                    x.IdbacSiDuyetNavigation != null
                        ? x.IdbacSiDuyetNavigation.TenBacSi
                        : null,

                status = x.TrangThai,

                abnormal =
                    x.Ketquachisos.Any(c =>
                        c.Status == "yes" &&
                        (
                            c.DanhGia == "bat_thuong" ||
                            c.DanhGia == "thap" ||
                            c.DanhGia == "cao"
                        ))
            })
            .ToListAsync();

        return Ok(new
        {
            success = true,
            data = list
        });
    }
    catch (Exception ex)
    {
        return StatusCode(500, new
        {
            success = false,
            message = ex.Message
        });
    }
}
    }
}