using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BioMedic.Backend.Data;

namespace BioMedic.Backend.Controllers
{
    [Route("api/dashboard")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("receptionist")]
        [Authorize(Roles = "RECEPTIONIST")]
        public async Task<IActionResult> GetReceptionistDashboard()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var upcomingThrough = today.AddDays(7);

            var todayExamAppointments = _context.Datlichkhams
                .Where(appointment => appointment.NgayKham == today &&
                    appointment.TrangThai != "cancelled" && appointment.TrangThai != "huy");
            var todayTestAppointments = _context.Datlichxetnghiems
                .Where(appointment => appointment.NgayXetNghiem == today &&
                    appointment.TrangThai != "cancelled" && appointment.TrangThai != "huy");

            var todayAppointments =
                await todayExamAppointments.CountAsync() +
                await todayTestAppointments.CountAsync();
            var checkedIn =
                await _context.Luotkhams.CountAsync(visit =>
                    visit.IddatLichKhamNavigation.NgayKham == today) +
                await _context.Luotxetnghiems.CountAsync(visit =>
                    visit.IddatLichXnNavigation.NgayXetNghiem == today);
            var waiting =
                await _context.Luotkhams.CountAsync(visit =>
                    visit.IddatLichKhamNavigation.NgayKham == today &&
                    (visit.TrangThai == "da_tiep_nhan" || visit.TrangThai == "cho_kham" ||
                     visit.TrangThai == "da_den_luot")) +
                await _context.Luotxetnghiems.CountAsync(visit =>
                    visit.IddatLichXnNavigation.NgayXetNghiem == today &&
                    (visit.TrangThai == "da_tiep_nhan" || visit.TrangThai == "cho_xet_nghiem" ||
                     visit.TrangThai == "da_den_luot"));
            var walkIns =
                await todayExamAppointments.CountAsync(appointment => appointment.UserId == null) +
                await todayTestAppointments.CountAsync(appointment => appointment.UserId == null);

            var examRows = await _context.Datlichkhams
                .AsNoTracking()
                .Where(appointment => appointment.NgayKham >= today &&
                    appointment.NgayKham <= upcomingThrough &&
                    (appointment.TrangThai == "pending" || appointment.TrangThai == "confirmed"))
                .OrderBy(appointment => appointment.NgayKham)
                .ThenBy(appointment => appointment.GioKham)
                .Select(appointment => new
                {
                    appointment.MaDatLich,
                    appointment.NgayKham,
                    appointment.GioKham,
                    PatientName = appointment.IdkhachHangNavigation.TenKhachHang,
                    ServiceName = appointment.IdchuyenKhoaNavigation.TenChuyenKhoa,
                    appointment.TrangThai,
                })
                .ToListAsync();

            var testRows = await _context.Datlichxetnghiems
                .AsNoTracking()
                .Where(appointment => appointment.NgayXetNghiem >= today &&
                    appointment.NgayXetNghiem <= upcomingThrough &&
                    (appointment.TrangThai == "pending" || appointment.TrangThai == "confirmed"))
                .OrderBy(appointment => appointment.NgayXetNghiem)
                .ThenBy(appointment => appointment.GioXetNghiem)
                .Select(appointment => new
                {
                    appointment.MaDatLich,
                    appointment.NgayXetNghiem,
                    appointment.GioXetNghiem,
                    PatientName = appointment.IdkhachHangNavigation.TenKhachHang,
                    ServiceName = appointment.Ctdatlichxetnghiems
                        .Select(item => item.IdxetNghiemNavigation.TenXetNghiem)
                        .FirstOrDefault() ?? "Xét nghiệm",
                    appointment.TrangThai,
                })
                .ToListAsync();

            var upcomingAppointments = examRows
                .Select(appointment => new ReceptionistUpcomingAppointment(
                    appointment.MaDatLich,
                    appointment.NgayKham,
                    appointment.GioKham.ToString("HH:mm"),
                    appointment.PatientName,
                    "Khám bệnh",
                    appointment.ServiceName,
                    appointment.TrangThai))
                .Concat(testRows.Select(appointment => new ReceptionistUpcomingAppointment(
                    appointment.MaDatLich,
                    appointment.NgayXetNghiem,
                    appointment.GioXetNghiem.ToString("HH:mm"),
                    appointment.PatientName,
                    "Xét nghiệm",
                    appointment.ServiceName,
                    appointment.TrangThai)))
                .OrderBy(appointment => appointment.Date)
                .ThenBy(appointment => appointment.Time)
                .Take(10)
                .ToList();

            return Ok(new
            {
                todayAppointments,
                checkedIn,
                waiting,
                walkIns,
                upcomingAppointments,
            });
        }

        [HttpGet("customer")] public IActionResult GetCustomerDashboard() => Ok();
        [HttpGet("doctor")] public IActionResult GetDoctorDashboard() => Ok();
        [HttpGet("technician")] public IActionResult GetTechnicianDashboard() => Ok();
        [HttpGet("admin")] public IActionResult GetAdminDashboard() => Ok();

        private sealed record ReceptionistUpcomingAppointment(
            string Id,
            DateOnly Date,
            string Time,
            string PatientName,
            string Type,
            string ServiceName,
            string Status);
    }
}