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
                    (visit.TrangThai == "da_tiep_nhan" ||
                     visit.TrangThai == "cho_kham" ||
                     visit.TrangThai == "da_den_luot")) +
                await _context.Luotxetnghiems.CountAsync(visit =>
                    visit.IddatLichXnNavigation.NgayXetNghiem == today &&
                    (visit.TrangThai == "da_tiep_nhan" ||
                     visit.TrangThai == "cho_xet_nghiem" ||
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

        [HttpGet("customer")]
        public IActionResult GetCustomerDashboard() => Ok();

        [HttpGet("doctor")]
        [Authorize(Roles = "DOCTOR")]
        public async Task<IActionResult> GetDoctorDashboard()
        {
            try
            {
                var doctorId = await GetCurrentDoctorId();

                if (doctorId is null)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new
                    {
                        success = false,
                        message = "Tài khoản bác sĩ chưa được liên kết với hồ sơ bác sĩ trong hệ thống."
                    });
                }

                // Sau khi đã kiểm tra null, chỉ dùng int thường trong LINQ.
                // Việc này tránh lỗi EF Core khi đánh giá Nullable<int>.Value
                // bên trong biểu thức truy vấn.
                var currentDoctorId = doctorId.Value;

                var today = DateOnly.FromDateTime(DateTime.Today);

                // Tổng lượt khám hôm nay
                var todayVisits = await _context.Luotkhams
                    .AsNoTracking()
                    .CountAsync(x =>
                        x.IdbacSi == currentDoctorId &&
                        x.IddatLichKhamNavigation.NgayKham == today);

                // Số bệnh nhân đang chờ
                var waitingCount = await _context.Luotkhams
                    .AsNoTracking()
                    .CountAsync(x =>
                        x.IdbacSi == currentDoctorId &&
                        x.IddatLichKhamNavigation.NgayKham == today &&
                        (x.TrangThai == "da_tiep_nhan" ||
                         x.TrangThai == "cho_kham" ||
                         x.TrangThai == "da_den_luot" ||
                         x.TrangThai == "cho_goi_lai"));

                // Số lượt hoàn tất hôm nay
                var completedToday = await _context.Luotkhams
                    .AsNoTracking()
                    .CountAsync(x =>
                        x.IdbacSi == currentDoctorId &&
                        x.IddatLichKhamNavigation.NgayKham == today &&
                        (x.TrangThai == "completed" ||
                         x.TrangThai == "hoan_tat" ||
                         x.TrangThai == "da_hoan_tat"));

                // Kết quả chờ duyệt.
                // Dùng điều kiện đơn giản để tránh chuỗi navigation quá sâu.
                var pendingApprovals = await _context.Ketquaxetnghiems
                    .AsNoTracking()
                    .CountAsync(x =>
                        x.TrangThai == "cho_duyet" &&
                        (x.IdbacSiDuyet == null || x.IdbacSiDuyet == currentDoctorId));

                // Phòng làm việc hiện tại
                var schedule = await _context.Lichlamviecs
                    .AsNoTracking()
                    .Where(x =>
                        x.IdbacSi == currentDoctorId &&
                        x.Ngay == today &&
                        x.TrangThai != "inactive" &&
                        x.TrangThai != "cancelled")
                    .OrderBy(x => x.GioBatDau)
                    .Select(x => new
                    {
                        x.Idphong
                    })
                    .FirstOrDefaultAsync();

                var currentRoom = "—";

                if (!string.IsNullOrWhiteSpace(schedule?.Idphong))
                {
                    currentRoom = await _context.Phongs
                        .AsNoTracking()
                        .Where(x => x.Idphong == schedule.Idphong)
                        .Select(x => x.TenPhong)
                        .FirstOrDefaultAsync()
                        ?? schedule.Idphong;
                }

                // Danh sách bệnh nhân tiếp theo
                var nextPatients = await _context.Luotkhams
                    .AsNoTracking()
                    .Where(x =>
                        x.IdbacSi == currentDoctorId &&
                        x.IddatLichKhamNavigation.NgayKham == today &&
                        (x.TrangThai == "da_tiep_nhan" ||
                         x.TrangThai == "cho_kham" ||
                         x.TrangThai == "da_den_luot" ||
                         x.TrangThai == "cho_goi_lai"))
                    .OrderBy(x => x.SoThuTu)
                    .ThenBy(x => x.IddatLichKhamNavigation.GioKham)
                    .Take(8)
                    .Select(x => new
                    {
                        id = x.IdluotKham,
                        queueNumber = x.SoThuTu,
                        patientCode = x.IdkhachHang,
                        patientName = x.IdkhachHangNavigation.TenKhachHang,
                        time = x.IddatLichKhamNavigation.GioKham.ToString("HH:mm"),
                        reason = x.IddatLichKhamNavigation.GhiChu ?? "Khám bệnh",
                        status = x.TrangThai
                    })
                    .ToListAsync();

                // Tạm để danh sách chi tiết kết quả chờ duyệt rỗng.
                // Màn Duyệt kết quả có API riêng và vẫn hoạt động độc lập.
                var pendingResults = Array.Empty<object>();

                return Ok(new
                {
                    success = true,
                    waitingCount,
                    todayVisits,
                    todayExamsCount = todayVisits,
                    pendingApprovals,
                    pendingResultsCount = pendingApprovals,
                    completedToday,
                    completedTodayCount = completedToday,
                    currentRoom,
                    nextPatients,
                    pendingResults
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = ex.Message,
                    innerMessage = ex.InnerException?.Message,
                    innerInnerMessage = ex.InnerException?.InnerException?.Message
                });
            }
        }
        [HttpGet("technician")]
        [Authorize(Roles = "TECHNICIAN")]
        public async Task<IActionResult> GetTechnicianDashboard()
        {
            try
            {
                var technicianId = await GetCurrentTechnicianId();

                if (string.IsNullOrWhiteSpace(technicianId))
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new
                    {
                        success = false,
                        message = "Tài khoản kỹ thuật viên chưa được liên kết với hồ sơ nhân viên."
                    });
                }

                // 1. Mẫu đã được bác sĩ bàn giao, đang chờ KTV tiếp nhận.
                // Chưa lọc theo IDKTV vì ở giai đoạn này mẫu có thể chưa tạo worklist.
                var waitingSpecimens = await _context.Maubenhphams
                    .AsNoTracking()
                    .CountAsync(x => x.TrangThai == "da_ban_giao");

                // 2. Mẫu đã được KTV tiếp nhận.
                var receivedSpecimens = await _context.Maubenhphams
                    .AsNoTracking()
                    .CountAsync(x => x.TrangThai == "ktv_tiep_nhan");

                // 3. Worklist KTV hiện đang chạy.
                var inProgress = await _context.Worklists
                    .AsNoTracking()
                    .CountAsync(x =>
                        x.Idktv == technicianId &&
                        x.Status == "running");

                // 4. Worklist đã chạy xong và đang chờ nhập kết quả.
                var pendingResultEntries = await _context.Worklists
                    .AsNoTracking()
                    .CountAsync(x =>
                        x.Idktv == technicianId &&
                        x.Status == "to_result");

                // 5. Chỉ hiển thị các worklist còn cần xử lý.
                // Không lấy finished/cancelled để tránh dashboard hiển thị dữ liệu cũ đã hoàn tất.
                // Worklist chưa được gán KTV (IDKTV = NULL) vẫn có thể hiện ở trạng thái queue
                // để KTV tiếp nhận/xử lý theo workflow hiện tại.
                var worklist = await _context.Worklists
                    .AsNoTracking()
                    .Where(x =>
                        (x.Idktv == null || x.Idktv == technicianId) &&
                        (
                            x.Status == "queue" ||
                            x.Status == "running" ||
                            x.Status == "to_result" ||
                            x.Status == "rerun"
                        ))
                    .OrderBy(x => x.Status == "running" ? 0 :
                                  x.Status == "to_result" ? 1 :
                                  x.Status == "rerun" ? 2 : 3)
                    .ThenByDescending(x => x.ReceivedAt)
                    .Take(10)
                    .Select(x => new
                    {
                        id = x.Id,
                        specimenId = x.Idmau,
                        specimenCode = x.IdmauNavigation == null
                            ? x.Idmau
                            : x.IdmauNavigation.MaBarcode,

                        patientName =
                            x.IdctphieuNavigation
                             .IdphieuXetNghiemNavigation
                             .IdkhachHangNavigation
                             .TenKhachHang,

                        testName =
                            x.IdctphieuNavigation
                             .IdxetNghiemNavigation
                             .TenXetNghiem,

                        technicianId = x.Idktv,
                        priority = "NORMAL",
                        status = x.Status,
                        receivedAt = x.ReceivedAt,
                        startedAt = x.StartedAt,
                        finishedAt = x.FinishedAt
                    })
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    technicianId,
                    waitingSpecimens,
                    receivedSpecimens,
                    inProgress,
                    pendingResultEntries,
                    worklist
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = ex.Message,
                    innerMessage = ex.InnerException?.Message
                });
            }
        }

        [HttpGet("admin")]
        public IActionResult GetAdminDashboard() => Ok();

        private async Task<int?> GetCurrentDoctorId()
        {
            if (!int.TryParse(User.FindFirst("userId")?.Value, out var userId))
            {
                return null;
            }

            var account = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == userId && x.IsActive == true);

            if (account is null)
            {
                return null;
            }

            if (account.IdbacSi.HasValue)
            {
                return account.IdbacSi.Value;
            }

            var employeeId = account.IdnhanVien;

            if (string.IsNullOrWhiteSpace(employeeId) &&
                !string.IsNullOrWhiteSpace(account.Email))
            {
                employeeId = await _context.Nhanviens
                    .AsNoTracking()
                    .Where(x => x.Email == account.Email)
                    .Select(x => x.IdnhanVien)
                    .FirstOrDefaultAsync();
            }

            if (string.IsNullOrWhiteSpace(employeeId))
            {
                return null;
            }

            return await _context.Bacsis
                .AsNoTracking()
                .Where(x => x.IdnhanVien == employeeId)
                .Select(x => (int?)x.IdbacSi)
                .FirstOrDefaultAsync();
        }
        private async Task<string?> GetCurrentTechnicianId()
        {
            if (!int.TryParse(User.FindFirst("userId")?.Value, out var userId))
            {
                return null;
            }

            var account = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == userId && x.IsActive == true);

            if (account is null)
            {
                return null;
            }

            // Ưu tiên liên kết trực tiếp users.IDNhanVien.
            if (!string.IsNullOrWhiteSpace(account.IdnhanVien))
            {
                return account.IdnhanVien;
            }

            // Fallback cho dữ liệu cũ: tìm nhân viên theo email của tài khoản.
            if (!string.IsNullOrWhiteSpace(account.Email))
            {
                return await _context.Nhanviens
                    .AsNoTracking()
                    .Where(x =>
                        x.Email == account.Email &&
                        x.Status != "no" &&
                        x.Status != "inactive")
                    .Select(x => x.IdnhanVien)
                    .FirstOrDefaultAsync();
            }

            return null;
        }

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
