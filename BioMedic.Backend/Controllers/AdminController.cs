using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        /// <summary>
        /// Dashboard + báo cáo thống kê cho Admin.
        ///
        /// range:
        /// TODAY = hôm nay
        /// WEEK  = 7 ngày gần nhất
        /// MONTH = 30 ngày gần nhất
        /// YEAR  = từ đầu năm đến hôm nay
        /// </summary>
        [HttpGet("dashboard")]
        [HttpGet("stats")]
        public async Task<IActionResult> GetAdminDashboardStats(
            [FromQuery] string range = "MONTH")
        {
            try
            {
                var today = DateOnly.FromDateTime(DateTime.Today);
                var tomorrow = today.AddDays(1);

                var normalizedRange = (range ?? "MONTH")
                    .Trim()
                    .ToUpperInvariant();

                DateOnly startDate = normalizedRange switch
                {
                    "TODAY" => today,
                    "WEEK" => today.AddDays(-6),
                    "YEAR" => new DateOnly(today.Year, 1, 1),
                    _ => today.AddDays(-29), // MONTH
                };

                DateOnly endDateExclusive = tomorrow;

                // DateTime tương ứng dùng cho các bảng có cột datetime.
                var startDateTime = startDate.ToDateTime(TimeOnly.MinValue);
                var endDateTimeExclusive =
                    endDateExclusive.ToDateTime(TimeOnly.MinValue);

                // =========================================================
                // 1. DỮ LIỆU DANH MỤC / NHÂN SỰ
                // =========================================================
                var totalCustomers = await _context.Khachhangs
                    .AsNoTracking()
                    .CountAsync();

                var totalEmployees = await _context.Nhanviens
                    .AsNoTracking()
                    .CountAsync(x => x.Status != "no");

                var totalDoctors = await _context.Bacsis
                    .AsNoTracking()
                    .CountAsync(x =>
                        x.TrangThai != "inactive");

                var totalTechnicians = await _context.Nhanviens
                    .AsNoTracking()
                    .CountAsync(x =>
                        x.Status != "no" &&
                        x.ViTri == "ktv");

                // =========================================================
                // 2. PHIẾU XÉT NGHIỆM + KẾT QUẢ TRONG KHOẢNG CHỌN
                // =========================================================
                var totalTestOrders = await _context.Phieuxetnghiems
                    .AsNoTracking()
                    .CountAsync(x =>
                        x.NgayTao >= startDateTime &&
                        x.NgayTao < endDateTimeExclusive &&
                        x.TrangThai != "huy");

                var totalResults = await _context.Ketquaxetnghiems
                    .AsNoTracking()
                    .CountAsync(x =>
                        x.ThoiGianNhap >= startDateTime &&
                        x.ThoiGianNhap < endDateTimeExclusive);

                var pendingResults = await _context.Ketquaxetnghiems
                    .AsNoTracking()
                    .CountAsync(x =>
                        x.TrangThai == "cho_duyet");

                // =========================================================
                // 3. LỊCH HÔM NAY
                //    Bao gồm cả lịch khám và lịch xét nghiệm.
                // =========================================================
                var todayExamAppointments = await _context.Datlichkhams
                    .AsNoTracking()
                    .CountAsync(x =>
                        x.NgayKham == today &&
                        x.TrangThai != "cancelled");

                var todayTestAppointments = await _context.Datlichxetnghiems
                    .AsNoTracking()
                    .CountAsync(x =>
                        x.NgayXetNghiem == today &&
                        x.TrangThai != "cancelled");

                var todayAppointments =
                    todayExamAppointments +
                    todayTestAppointments;

                // =========================================================
                // 4. HOÀN TẤT HÔM NAY
                // =========================================================
                var completedExamToday = await _context.Datlichkhams
                    .AsNoTracking()
                    .CountAsync(x =>
                        x.NgayKham == today &&
                        x.TrangThai == "completed");

                var completedTestToday = await _context.Datlichxetnghiems
                    .AsNoTracking()
                    .CountAsync(x =>
                        x.NgayXetNghiem == today &&
                        x.TrangThai == "completed");

                var completedToday =
                    completedExamToday +
                    completedTestToday;

                // =========================================================
                // 5. DOANH THU TRONG KHOẢNG CHỌN
                //    Chỉ tính hóa đơn đã thanh toán, còn hiệu lực.
                // =========================================================
                var revenue = await _context.Hoadons
                    .AsNoTracking()
                    .Where(x =>
                        x.NgayTaoHoaDon >= startDateTime &&
                        x.NgayTaoHoaDon < endDateTimeExclusive &&
                        x.Status == "yes" &&
                        x.TrangThaiThanhToan == "da_thanh_toan")
                    .SumAsync(x => (decimal?)x.TongTien)
                    ?? 0m;

                // =========================================================
                // 6. TRẠNG THÁI QUY TRÌNH XÉT NGHIỆM
                // =========================================================
                var rawStatusBreakdown = await _context.Phieuxetnghiems
                    .AsNoTracking()
                    .Where(x =>
                        x.NgayTao >= startDateTime &&
                        x.NgayTao < endDateTimeExclusive &&
                        x.TrangThai != "huy")
                    .GroupBy(x => x.TrangThai)
                    .Select(g => new
                    {
                        status = g.Key,
                        count = g.Count()
                    })
                    .OrderByDescending(x => x.count)
                    .ToListAsync();

                var statusBreakdown = rawStatusBreakdown
                    .Select(x => new
                    {
                        status = x.status,
                        label = TranslateTestOrderStatus(x.status),
                        count = x.count
                    })
                    .ToList();

                // =========================================================
                // 7. TOP XÉT NGHIỆM ĐƯỢC SỬ DỤNG NHIỀU
                //
                // Lấy chi tiết phiếu trong khoảng đã chọn, sau đó group
                // ở memory để tránh LINQ quá phức tạp với Pomelo/EF Core.
                // =========================================================
                var testUsageRows = await _context.Ctphieuxetnghiems
                    .AsNoTracking()
                    .Where(x =>
                        x.Status == "yes" &&
                        x.IdphieuXetNghiemNavigation.NgayTao >= startDateTime &&
                        x.IdphieuXetNghiemNavigation.NgayTao < endDateTimeExclusive &&
                        x.IdphieuXetNghiemNavigation.TrangThai != "huy")
                    .Select(x => new
                    {
                        id = x.IdxetNghiem,
                        testName =
                            x.IdxetNghiemNavigation.TenXetNghiem,
                        quantity = x.SoLuong,
                        unitPrice = x.DonGia
                    })
                    .ToListAsync();

                var topTests = testUsageRows
                    .GroupBy(x => new
                    {
                        x.id,
                        x.testName
                    })
                    .Select(g => new
                    {
                        id = g.Key.id,
                        testName = g.Key.testName,
                        count = g.Sum(x => Convert.ToInt32(x.quantity)),
                        revenue = g.Sum(x =>
                            x.unitPrice *
                            Convert.ToDecimal(x.quantity))
                    })
                    .OrderByDescending(x => x.count)
                    .ThenByDescending(x => x.revenue)
                    .Take(10)
                    .ToList();

                // =========================================================
                // RESPONSE
                //
                // Trả cả tên mới và tên cũ để những màn Admin cũ vẫn dùng
                // được trong khi BaoCaoThongKe dùng totalCustomers...
                // =========================================================
                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        range = normalizedRange,
                        fromDate = startDate,
                        toDate = today,

                        // Tên field chuẩn dùng bởi BaoCaoThongKe.jsx
                        totalCustomers,
                        totalEmployees,
                        totalDoctors,
                        totalTechnicians,
                        totalTestOrders,
                        totalResults,
                        todayAppointments,
                        completedToday,
                        pendingResults,
                        revenue,
                        statusBreakdown,
                        topTests,

                        // Alias giữ tương thích code cũ
                        customersCount = totalCustomers,
                        employeesCount = totalEmployees,
                        doctorsCount = totalDoctors,
                        techniciansCount = totalTechnicians,
                        testOrdersCount = totalTestOrders,
                        resultsCount = totalResults
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        success = false,
                        message = "Không thể tải báo cáo thống kê.",
                        detail = ex.Message,
                        innerMessage = ex.InnerException?.Message
                    });
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
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        success = false,
                        message = ex.Message,
                        innerMessage = ex.InnerException?.Message
                    });
            }
        }

        private static string TranslateTestOrderStatus(string? status)
        {
            return status switch
            {
                "moi_tao" => "Mới tạo",
                "cho_lay_mau" => "Chờ lấy mẫu",
                "da_lay_mau" => "Đã lấy mẫu",
                "ktv_tiep_nhan" => "KTV tiếp nhận",
                "dang_xet_nghiem" => "Đang xét nghiệm",
                "cho_duyet" => "Chờ duyệt",
                "da_co_kq" => "Đã có kết quả",
                "hoan_tat" => "Hoàn tất",
                "huy" => "Đã hủy",
                _ => string.IsNullOrWhiteSpace(status)
                    ? "Khác"
                    : status
            };
        }
    }
}
