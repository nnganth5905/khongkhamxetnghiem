using System.Globalization;
using System.Security.Claims;
using BioMedic.Backend.Data;
using BioMedic.Backend.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BioMedic.Backend.Controllers;

[ApiController]
public class TechnicianWorkflowController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TechnicianWorkflowController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("api/technicians")]
    [Authorize(Roles = "DOCTOR,ADMIN,RECEPTIONIST")]
    public async Task<IActionResult> GetTechnicians()
    {
        var rows = await _context.Nhanviens.AsNoTracking()
            .Where(x => x.Status == "yes" && (x.ViTri == "ktv" || x.ViTri.Contains("Kỹ thuật") || x.ViTri.Contains("ky thuat")))
            .OrderBy(x => x.TenNhanVien)
            .Select(x => new { id = x.IdnhanVien, name = x.TenNhanVien })
            .ToListAsync();
        return Ok(new { success = true, data = rows });
    }

    [HttpGet("api/technician/me")]
    [Authorize(Roles = "TECHNICIAN")]
    public async Task<IActionResult> GetCurrentTechnician()
    {
        var technicianId = await GetTechnicianId();
        if (technicianId is null) return Unauthorized(new { success = false, message = "Không xác định được kỹ thuật viên." });

        var technician = await _context.Nhanviens.AsNoTracking()
            .Where(x => x.IdnhanVien == technicianId)
            .Select(x => new { id = x.IdnhanVien, name = x.TenNhanVien, phone = x.SoDienThoai, email = x.Email, facilityId = x.CoSoId, status = x.Status })
            .FirstOrDefaultAsync();
        return technician is null ? NotFound() : Ok(technician);
    }

    [HttpPost("api/doctor/specimens")]
    [Authorize(Roles = "DOCTOR")]
    public async Task<IActionResult> CollectSpecimen([FromBody] CollectSpecimenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.AppointmentId) || string.IsNullOrWhiteSpace(request.Barcode))
            return BadRequest(new { success = false, message = "Thiếu mã lịch hẹn hoặc barcode." });

        var doctorId = await GetDoctorId();
        if (doctorId is null) return Forbid();

        await using var tx = await _context.Database.BeginTransactionAsync();
        var appointment = await _context.Datlichxetnghiems
            .Include(x => x.Luotxetnghiem)
            .Include(x => x.Ctdatlichxetnghiems).ThenInclude(x => x.IdxetNghiemNavigation)
            .FirstOrDefaultAsync(x => x.MaDatLich == request.AppointmentId);
        if (appointment?.Luotxetnghiem is null)
            return NotFound(new { success = false, message = "Không tìm thấy lượt xét nghiệm đã check-in." });

        if (!string.IsNullOrWhiteSpace(request.PatientCode) && appointment.IdkhachHang != request.PatientCode)
            return BadRequest(new { success = false, message = "Mã khách hàng không khớp lịch hẹn." });

        var visit = appointment.Luotxetnghiem;
        visit.TrangThai = "da_lay_mau";
        visit.ThoiGianBatDau ??= DateTime.Now;

        var orderId = $"PXN{visit.IdluotXetNghiem}";
        var order = await _context.Phieuxetnghiems
            .Include(x => x.Ctphieuxetnghiems)
            .FirstOrDefaultAsync(x => x.IdphieuXetNghiem == orderId);

        if (order is null)
        {
            order = new Phieuxetnghiem
            {
                IdphieuXetNghiem = orderId,
                IdkhachHang = appointment.IdkhachHang,
                IdluotXetNghiem = visit.IdluotXetNghiem,
                IdbacSiChiDinh = doctorId,
                IdbacSiPhuTrach = appointment.IdbacSi ?? doctorId,
                NgayTao = DateTime.Now,
                TongTien = appointment.Ctdatlichxetnghiems.Sum(x => x.DonGia),
                TrangThaiThanhToan = "chua_thanh_toan",
                TrangThai = "da_lay_mau",
                GhiChu = request.Notes
            };
            _context.Phieuxetnghiems.Add(order);
            await _context.SaveChangesAsync();

            foreach (var source in appointment.Ctdatlichxetnghiems)
            {
                _context.Ctphieuxetnghiems.Add(new Ctphieuxetnghiem
                {
                    IdphieuXetNghiem = orderId,
                    IdxetNghiem = source.IdxetNghiem,
                    SoLuong = 1,
                    DonGia = source.DonGia,
                    GhiChu = source.GhiChu,
                    TrangThai = "da_lay_mau",
                    Status = "yes"
                });
            }
            await _context.SaveChangesAsync();
            await _context.Entry(order).Collection(x => x.Ctphieuxetnghiems).LoadAsync();
        }
        else
        {
            order.TrangThai = "da_lay_mau";
            foreach (var detail in order.Ctphieuxetnghiems) detail.TrangThai = "da_lay_mau";
        }

        var detailForSample = order.Ctphieuxetnghiems.FirstOrDefault();
        if (detailForSample is null)
            return BadRequest(new { success = false, message = "Phiếu xét nghiệm chưa có dịch vụ xét nghiệm." });

        var specimenId = "M" + request.Barcode.Trim();
        var exists = await _context.Maubenhphams.AnyAsync(x => x.Idmau == specimenId || x.MaBarcode == request.Barcode.Trim());
        if (exists) return Conflict(new { success = false, message = "Barcode mẫu đã tồn tại." });

        _context.Maubenhphams.Add(new Maubenhpham
        {
            Idmau = specimenId,
            Idctphieu = detailForSample.Idctphieu,
            MaBarcode = request.Barcode.Trim(),
            LoaiMau = string.IsNullOrWhiteSpace(request.SampleType) ? detailForSample.IdxetNghiemNavigation?.LoaiMauMacDinh ?? "Khác" : request.SampleType.Trim(),
            IdbacSiLayMau = doctorId,
            ThoiGianLayMau = DateTime.Now,
            TrangThai = "da_lay_mau",
            GhiChu = request.Notes
        });
        AddTracking(appointment.IdkhachHang, "luotxetnghiem", visit.IdluotXetNghiem.ToString(), "Bác sĩ xác nhận đã lấy mẫu", $"Mã mẫu/Barcode: {request.Barcode.Trim()}");
        await _context.SaveChangesAsync();
        await tx.CommitAsync();

        return StatusCode(201, new { success = true, id = specimenId, barcode = request.Barcode.Trim(), status = "COLLECTED", message = "Đã lấy mẫu thành công." });
    }

    [HttpPost("api/doctor/specimens/{id}/handover")]
    [Authorize(Roles = "DOCTOR")]
    public async Task<IActionResult> HandoverSpecimen(string id, [FromBody] HandoverRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ReceiverId)) return BadRequest(new { success = false, message = "Vui lòng chọn kỹ thuật viên tiếp nhận." });
        var doctorId = await GetDoctorId();
        if (doctorId is null) return Forbid();

        var specimen = await _context.Maubenhphams
            .Include(x => x.IdctphieuNavigation).ThenInclude(x => x.IdphieuXetNghiemNavigation).ThenInclude(x => x.Ctphieuxetnghiems)
            .FirstOrDefaultAsync(x => x.Idmau == id || x.MaBarcode == id);
        if (specimen is null) return NotFound(new { success = false, message = "Không tìm thấy mẫu bệnh phẩm." });
        if (!await _context.Nhanviens.AnyAsync(x => x.IdnhanVien == request.ReceiverId && x.Status == "yes"))
            return BadRequest(new { success = false, message = "Kỹ thuật viên tiếp nhận không hợp lệ." });

        await using var tx = await _context.Database.BeginTransactionAsync();
        specimen.TrangThai = "da_ban_giao";
        var order = specimen.IdctphieuNavigation.IdphieuXetNghiemNavigation;
        order.TrangThai = "ktv_tiep_nhan";
        foreach (var detail in order.Ctphieuxetnghiems) detail.TrangThai = "ktv_tiep_nhan";
        if (order.IdluotXetNghiem is not null)
        {
            var visit = await _context.Luotxetnghiems.FindAsync(order.IdluotXetNghiem.Value);
            if (visit is not null) visit.TrangThai = "ktv_tiep_nhan";
        }

        _context.Bangiaomaus.Add(new Bangiaomau
        {
            Idmau = specimen.Idmau,
            IdbacSiBanGiao = doctorId.Value,
            IdktvtiepNhan = request.ReceiverId.Trim(),
            ThoiGianBanGiao = DateTime.Now,
            TrangThai = "da_ban_giao",
            GhiChu = request.Note
        });
        AddTracking(order.IdkhachHang, "maubenhpham", specimen.Idmau, "Mẫu bệnh phẩm đã bàn giao cho KTV", $"KTV tiếp nhận: {request.ReceiverId}");
        await _context.SaveChangesAsync();
        await tx.CommitAsync();
        return Ok(new { success = true, id = specimen.Idmau, receiverId = request.ReceiverId, status = "HANDED_OVER" });
    }

    [HttpGet("api/technician/specimens")]
    [Authorize(Roles = "TECHNICIAN")]
    public async Task<IActionResult> GetSpecimens([FromQuery] string? status = null)
    {
        var dbStatus = ApiSpecimenStatusToDb(status);
        var query = _context.Maubenhphams.AsNoTracking()
            .Include(x => x.IdctphieuNavigation).ThenInclude(x => x.IdphieuXetNghiemNavigation).ThenInclude(x => x.IdkhachHangNavigation)
            .AsQueryable();
        if (!string.IsNullOrWhiteSpace(dbStatus)) query = query.Where(x => x.TrangThai == dbStatus);
        else query = query.Where(x => x.TrangThai != "moi_tao" && x.TrangThai != "da_lay_mau" && x.TrangThai != "huy");

        var rows = await query.OrderByDescending(x => x.ThoiGianLayMau).ToListAsync();
        return Ok(rows.Select(x => new
        {
            id = x.Idmau,
            idMauBenhPham = x.Idmau,
            code = x.Idmau,
            maMau = x.Idmau,
            barcode = x.MaBarcode,
            maVach = x.MaBarcode,
            patientName = x.IdctphieuNavigation.IdphieuXetNghiemNavigation.IdkhachHangNavigation.TenKhachHang,
            tenKhachHang = x.IdctphieuNavigation.IdphieuXetNghiemNavigation.IdkhachHangNavigation.TenKhachHang,
            specimenType = x.LoaiMau,
            loaiMau = x.LoaiMau,
            collectedAt = x.ThoiGianLayMau,
            thoiGianLay = x.ThoiGianLayMau,
            status = DbSpecimenStatusToApi(x.TrangThai),
            trangThai = DbSpecimenStatusToApi(x.TrangThai)
        }));
    }

    [HttpGet("api/technician/specimens/{id}")]
    [Authorize(Roles = "TECHNICIAN")]
    public async Task<IActionResult> GetSpecimen(string id)
    {
        var x = await _context.Maubenhphams.AsNoTracking()
            .Include(s => s.IdctphieuNavigation).ThenInclude(d => d.IdxetNghiemNavigation)
            .Include(s => s.IdctphieuNavigation).ThenInclude(d => d.IdphieuXetNghiemNavigation).ThenInclude(o => o.IdkhachHangNavigation)
            .Include(s => s.Bangiaomaus).ThenInclude(h => h.IdbacSiBanGiaoNavigation)
            .FirstOrDefaultAsync(s => s.Idmau == id || s.MaBarcode == id);
        if (x is null) return NotFound(new { success = false, message = "Không tìm thấy mẫu bệnh phẩm." });
        return Ok(new
        {
            id = x.Idmau,
            code = x.Idmau,
            barcode = x.MaBarcode,
            patientName = x.IdctphieuNavigation.IdphieuXetNghiemNavigation.IdkhachHangNavigation.TenKhachHang,
            specimenType = x.LoaiMau,
            collectedAt = x.ThoiGianLayMau,
            status = DbSpecimenStatusToApi(x.TrangThai),
            notes = x.GhiChu,
            testName = x.IdctphieuNavigation.IdxetNghiemNavigation.TenXetNghiem,
            handoverBy = x.Bangiaomaus.OrderByDescending(h => h.ThoiGianBanGiao).Select(h => h.IdbacSiBanGiaoNavigation.TenBacSi).FirstOrDefault() ?? "Bác sĩ / Điều dưỡng"
        });
    }

    [HttpPost("api/technician/specimens/{id}/receive")]
    [Authorize(Roles = "TECHNICIAN")]
    public async Task<IActionResult> ReceiveSpecimen(string id, [FromBody] ReceiveSpecimenRequest request)
    {
        var technicianId = await GetTechnicianId();
        if (technicianId is null) return Unauthorized(new { success = false, message = "Không xác định được kỹ thuật viên." });

        var specimen = await _context.Maubenhphams
            .Include(x => x.IdctphieuNavigation).ThenInclude(x => x.IdphieuXetNghiemNavigation).ThenInclude(x => x.Ctphieuxetnghiems)
            .FirstOrDefaultAsync(x => x.Idmau == id || x.MaBarcode == id);
        if (specimen is null) return NotFound(new { success = false, message = "Không tìm thấy mẫu bệnh phẩm." });
        if (specimen.TrangThai == "tu_choi_mau" || specimen.TrangThai == "hoan_tat") return Conflict(new { success = false, message = "Mẫu hiện không thể tiếp nhận." });

        await using var tx = await _context.Database.BeginTransactionAsync();
        specimen.TrangThai = "ktv_tiep_nhan";
        specimen.GhiChu = string.IsNullOrWhiteSpace(request.Notes) ? specimen.GhiChu : request.Notes.Trim();
        var order = specimen.IdctphieuNavigation.IdphieuXetNghiemNavigation;
        order.TrangThai = "ktv_tiep_nhan";
        foreach (var detail in order.Ctphieuxetnghiems) detail.TrangThai = "ktv_tiep_nhan";

        var handover = await _context.Bangiaomaus.Where(x => x.Idmau == specimen.Idmau).OrderByDescending(x => x.ThoiGianBanGiao).FirstOrDefaultAsync();
        if (handover is not null)
        {
            handover.TrangThai = "da_tiep_nhan";
            handover.ThoiGianTiepNhan = DateTime.Now;
            handover.IdktvtiepNhan = technicianId;
        }

        var workItems = new List<Worklist>();
        foreach (var detail in order.Ctphieuxetnghiems.Where(x => x.Status == "yes"))
        {
            var work = await _context.Worklists.FirstOrDefaultAsync(x => x.Idctphieu == detail.Idctphieu && x.Idmau == specimen.Idmau);
            if (work is null)
            {
                work = new Worklist
                {
                    Idctphieu = detail.Idctphieu,
                    Idmau = specimen.Idmau,
                    Idktv = technicianId,
                    Status = "queue",
                    ReceivedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                _context.Worklists.Add(work);
            }
            else
            {
                work.Idktv ??= technicianId;
                if (work.Status == "cancelled") work.Status = "queue";
                work.UpdatedAt = DateTime.Now;
            }
            workItems.Add(work);
        }

        await _context.SaveChangesAsync();
        await tx.CommitAsync();
        return Ok(new { success = true, status = "SUCCESS", worklistId = workItems.FirstOrDefault()?.Id, worklistIds = workItems.Select(x => x.Id), message = "Đã tiếp nhận mẫu thành công." });
    }

    [HttpPost("api/technician/specimens/{id}/reject")]
    [Authorize(Roles = "TECHNICIAN")]
    public async Task<IActionResult> RejectSpecimen(string id, [FromBody] RejectSpecimenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Reason)) return BadRequest(new { success = false, message = "Vui lòng nhập lý do từ chối." });
        var specimen = await _context.Maubenhphams.Include(x => x.IdctphieuNavigation).FirstOrDefaultAsync(x => x.Idmau == id || x.MaBarcode == id);
        if (specimen is null) return NotFound(new { success = false, message = "Không tìm thấy mẫu bệnh phẩm." });
        specimen.TrangThai = "tu_choi_mau";
        specimen.IdctphieuNavigation.TrangThai = "can_lam_lai";
        var handover = await _context.Bangiaomaus.Where(x => x.Idmau == specimen.Idmau).OrderByDescending(x => x.ThoiGianBanGiao).FirstOrDefaultAsync();
        if (handover is not null)
        {
            handover.TrangThai = "tu_choi";
            handover.LyDoTuChoi = request.Reason.Trim();
            handover.ThoiGianTiepNhan = DateTime.Now;
        }
        await _context.SaveChangesAsync();
        return Ok(new { success = true, status = "REJECTED", message = "Đã từ chối mẫu bệnh phẩm." });
    }

    [HttpPut("api/technician/specimens/{id}")]
    [Authorize(Roles = "TECHNICIAN")]
    public async Task<IActionResult> UpdateSpecimen(string id, [FromBody] UpdateSpecimenRequest request)
    {
        var specimen = await _context.Maubenhphams.FirstOrDefaultAsync(x => x.Idmau == id || x.MaBarcode == id);
        if (specimen is null) return NotFound();
        var dbStatus = ApiSpecimenStatusToDb(request.Status);
        if (!string.IsNullOrWhiteSpace(dbStatus)) specimen.TrangThai = dbStatus;
        if (!string.IsNullOrWhiteSpace(request.Notes)) specimen.GhiChu = request.Notes.Trim();
        await _context.SaveChangesAsync();
        return Ok(new { success = true, status = DbSpecimenStatusToApi(specimen.TrangThai) });
    }

    [HttpGet("api/technician/worklist")]
    [Authorize(Roles = "TECHNICIAN")]
    public async Task<IActionResult> GetWorklist([FromQuery] string? status = null)
    {
        var technicianId = await GetTechnicianId();
        if (technicianId is null) return Unauthorized();
        var dbStatus = ApiWorkStatusToDb(status);
        var query = _context.Worklists.AsNoTracking()
            .Include(x => x.IdmauNavigation)
            .Include(x => x.IdctphieuNavigation).ThenInclude(x => x.IdxetNghiemNavigation)
            .Include(x => x.IdctphieuNavigation).ThenInclude(x => x.IdphieuXetNghiemNavigation).ThenInclude(x => x.IdkhachHangNavigation)
            .Where(x => x.Idktv == null || x.Idktv == technicianId);
        if (!string.IsNullOrWhiteSpace(dbStatus)) query = query.Where(x => x.Status == dbStatus);

        var rows = await query.OrderBy(x => x.ReceivedAt).ToListAsync();
        return Ok(rows.Select(x => new
        {
            id = x.Id,
            specimenCode = x.IdmauNavigation?.MaBarcode ?? x.Idmau ?? "—",
            testName = x.IdctphieuNavigation.IdxetNghiemNavigation.TenXetNghiem,
            patientName = x.IdctphieuNavigation.IdphieuXetNghiemNavigation.IdkhachHangNavigation.TenKhachHang,
            priority = "NORMAL",
            assignedAt = x.ReceivedAt,
            status = DbWorkStatusToApi(x.Status)
        }));
    }

    [HttpGet("api/technician/worklist/{id}")]
    [Authorize(Roles = "TECHNICIAN")]
    public async Task<IActionResult> GetWorkDetail(ulong id)
    {
        var technicianId = await GetTechnicianId();
        var x = await WorkForTechnician(id, technicianId).AsNoTracking()
            .Include(w => w.IdmauNavigation)
            .Include(w => w.IdctphieuNavigation).ThenInclude(d => d.IdxetNghiemNavigation)
            .Include(w => w.IdctphieuNavigation).ThenInclude(d => d.IdphieuXetNghiemNavigation).ThenInclude(o => o.IdkhachHangNavigation)
            .FirstOrDefaultAsync();
        if (x is null) return NotFound();
        return Ok(new { id = x.Id, specimenCode = x.IdmauNavigation?.MaBarcode, testName = x.IdctphieuNavigation.IdxetNghiemNavigation.TenXetNghiem, patientName = x.IdctphieuNavigation.IdphieuXetNghiemNavigation.IdkhachHangNavigation.TenKhachHang, status = DbWorkStatusToApi(x.Status), instrument = x.Instrument, reagentLot = x.ReagentLot, startedAt = x.StartedAt, finishedAt = x.FinishedAt });
    }

    [HttpPost("api/technician/worklist/{id}/start")]
    [Authorize(Roles = "TECHNICIAN")]
    public async Task<IActionResult> StartWork(ulong id, [FromBody] WorkActionRequest? request = null)
    {
        var technicianId = await GetTechnicianId();
        var work = await WorkForTechnician(id, technicianId).Include(x => x.IdmauNavigation).Include(x => x.IdctphieuNavigation).ThenInclude(x => x.IdphieuXetNghiemNavigation).FirstOrDefaultAsync();
        if (work is null) return NotFound();
        if (work.Status is not ("queue" or "rerun")) return Conflict(new { success = false, message = "Worklist không ở trạng thái có thể bắt đầu." });
        work.Idktv ??= technicianId;
        work.Status = "running";
        work.StartedAt ??= DateTime.Now;
        work.UpdatedAt = DateTime.Now;
        if (!string.IsNullOrWhiteSpace(request?.Instrument)) work.Instrument = request.Instrument.Trim();
        if (!string.IsNullOrWhiteSpace(request?.ReagentLot)) work.ReagentLot = request.ReagentLot.Trim();
        if (work.IdmauNavigation is not null) work.IdmauNavigation.TrangThai = "dang_xu_ly";
        work.IdctphieuNavigation.TrangThai = "dang_xet_nghiem";
        work.IdctphieuNavigation.IdphieuXetNghiemNavigation.TrangThai = "dang_xet_nghiem";
        await _context.SaveChangesAsync();
        return Ok(new { success = true, status = "IN_PROGRESS", message = "Đã bắt đầu xét nghiệm." });
    }

    [HttpPost("api/technician/worklist/{id}/complete")]
    [Authorize(Roles = "TECHNICIAN")]
    public async Task<IActionResult> CompleteWork(ulong id)
    {
        var technicianId = await GetTechnicianId();
        var work = await WorkForTechnician(id, technicianId).FirstOrDefaultAsync();
        if (work is null) return NotFound();
        if (work.Status != "running") return Conflict(new { success = false, message = "Worklist chưa ở trạng thái đang thực hiện." });
        work.Status = "to_result";
        work.FinishedAt = DateTime.Now;
        work.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();
        return Ok(new { success = true, status = "COMPLETED", message = "Đã hoàn tất chạy máy, chờ nhập kết quả." });
    }

    [HttpGet("api/technician/results/{worklistId}")]
    [Authorize(Roles = "TECHNICIAN")]
    public async Task<IActionResult> GetResultEntry(ulong worklistId)
    {
        var technicianId = await GetTechnicianId();
        var work = await WorkForTechnician(worklistId, technicianId).AsNoTracking()
            .Include(w => w.IdmauNavigation)
            .Include(w => w.IdctphieuNavigation).ThenInclude(d => d.IdxetNghiemNavigation).ThenInclude(t => t.Chisoxetnghiems).ThenInclude(i => i.Nguongchisoxetnghiems)
            .Include(w => w.IdctphieuNavigation).ThenInclude(d => d.IdphieuXetNghiemNavigation).ThenInclude(o => o.IdkhachHangNavigation)
            .Include(w => w.IdctphieuNavigation).ThenInclude(d => d.Ketquaxetnghiem).ThenInclude(r => r.Ketquachisos)
            .FirstOrDefaultAsync();
        if (work is null) return NotFound(new { success = false, message = "Không tìm thấy worklist." });

        var result = work.IdctphieuNavigation.Ketquaxetnghiem;
        var saved = result?.Ketquachisos.ToDictionary(x => x.IdchiSo, x => x) ?? new Dictionary<string, Ketquachiso>();
        var customer = work.IdctphieuNavigation.IdphieuXetNghiemNavigation.IdkhachHangNavigation;
        var age = customer.NgaySinh is null ? (int?)null : DateTime.Today.Year - customer.NgaySinh.Value.Year;

        var indicators = work.IdctphieuNavigation.IdxetNghiemNavigation.Chisoxetnghiems.Where(x => x.Status == "yes").Select(indicator =>
        {
            saved.TryGetValue(indicator.IdchiSo, out var existing);
            var threshold = indicator.Nguongchisoxetnghiems.FirstOrDefault(n => n.Status == "yes" &&
                (n.GioiTinhApDung == "tatca" || n.GioiTinhApDung == customer.GioiTinh) &&
                (!n.TuoiMin.HasValue || !age.HasValue || age.Value >= n.TuoiMin.Value) &&
                (!n.TuoiMax.HasValue || !age.HasValue || age.Value <= n.TuoiMax.Value));
            var reference = threshold?.GiaTriTextBinhThuong ??
                (threshold?.GiaTriMin is not null || threshold?.GiaTriMax is not null ? $"{threshold?.GiaTriMin?.ToString(CultureInfo.InvariantCulture) ?? ""} - {threshold?.GiaTriMax?.ToString(CultureInfo.InvariantCulture) ?? ""}" : "");
            return new
            {
                indicatorId = indicator.IdchiSo,
                name = indicator.TenChiSo,
                value = existing?.GiaTriText ?? existing?.GiaTriSo?.ToString(CultureInfo.InvariantCulture) ?? "",
                unit = indicator.DonVi ?? "",
                reference,
                abnormal = existing?.DanhGia is "bat_thuong" or "thap" or "cao"
            };
        });

        return Ok(new { worklistId = work.Id, patientName = customer.TenKhachHang, specimenCode = work.IdmauNavigation?.MaBarcode, testName = work.IdctphieuNavigation.IdxetNghiemNavigation.TenXetNghiem, status = DbWorkStatusToApi(work.Status), notes = result?.GhiChu ?? "", indicators });
    }

    [HttpPut("api/technician/results/{worklistId}")]
    [Authorize(Roles = "TECHNICIAN")]
    public Task<IActionResult> SaveResult(ulong worklistId, [FromBody] ResultEntryRequest request) => SaveResultInternal(worklistId, request, false);

    [HttpPost("api/technician/results/{worklistId}/submit")]
    [Authorize(Roles = "TECHNICIAN")]
    public Task<IActionResult> SubmitResult(ulong worklistId, [FromBody] ResultEntryRequest request) => SaveResultInternal(worklistId, request, true);

    private async Task<IActionResult> SaveResultInternal(ulong worklistId, ResultEntryRequest request, bool submit)
    {
        var technicianId = await GetTechnicianId();
        if (technicianId is null) return Unauthorized();
        var work = await WorkForTechnician(worklistId, technicianId)
            .Include(w => w.IdmauNavigation)
            .Include(w => w.IdctphieuNavigation).ThenInclude(d => d.IdxetNghiemNavigation).ThenInclude(t => t.Chisoxetnghiems)
            .Include(w => w.IdctphieuNavigation).ThenInclude(d => d.IdphieuXetNghiemNavigation)
            .Include(w => w.IdctphieuNavigation).ThenInclude(d => d.Ketquaxetnghiem).ThenInclude(r => r.Ketquachisos)
            .FirstOrDefaultAsync();
        if (work is null) return NotFound();
        if (work.Status is not ("to_result" or "running" or "finished")) return Conflict(new { success = false, message = "Worklist chưa sẵn sàng nhập kết quả." });

        await using var tx = await _context.Database.BeginTransactionAsync();
        var result = work.IdctphieuNavigation.Ketquaxetnghiem;
        if (result is null)
        {
            result = new Ketquaxetnghiem
            {
                IdketQua = $"KQ{work.Idctphieu}",
                Idctphieu = work.Idctphieu,
                Idmau = work.Idmau,
                Idktv = technicianId,
                ThoiGianBatDau = work.StartedAt ?? DateTime.Now,
                ThoiGianNhap = DateTime.Now,
                TrangThai = "dang_thuc_hien",
                GhiChu = request.Notes
            };
            _context.Ketquaxetnghiems.Add(result);
            await _context.SaveChangesAsync();
        }
        else if (result.TrangThai is "da_duyet" or "hoan_tat")
        {
            return Conflict(new { success = false, message = "Kết quả đã được bác sĩ duyệt và không thể chỉnh sửa." });
        }

        result.GhiChu = request.Notes;
        result.Idktv = technicianId;
        result.Idmau = work.Idmau;
        result.ThoiGianNhap = DateTime.Now;

        var validIndicatorIds = work.IdctphieuNavigation.IdxetNghiemNavigation.Chisoxetnghiems.Where(x => x.Status == "yes").Select(x => x.IdchiSo).ToHashSet();
        foreach (var input in request.Indicators ?? [])
        {
            if (string.IsNullOrWhiteSpace(input.IndicatorId) || !validIndicatorIds.Contains(input.IndicatorId)) continue;
            var row = result.Ketquachisos.FirstOrDefault(x => x.IdchiSo == input.IndicatorId);
            if (row is null)
            {
                row = new Ketquachiso { IdketQua = result.IdketQua, IdchiSo = input.IndicatorId, DanhGia = "chua_danh_gia", Status = "yes", CreatedAt = DateTime.Now };
                _context.Ketquachisos.Add(row);
                result.Ketquachisos.Add(row);
            }
            var value = input.Value?.Trim() ?? "";
            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var numeric))
            {
                row.GiaTriSo = numeric;
                row.GiaTriText = value;
            }
            else
            {
                row.GiaTriSo = null;
                row.GiaTriText = value;
            }
            row.DanhGia = input.Abnormal ? "bat_thuong" : "binh_thuong";
            row.UpdatedAt = DateTime.Now;
        }

        if (submit)
        {
            if ((request.Indicators ?? []).Any(x => string.IsNullOrWhiteSpace(x.Value))) return BadRequest(new { success = false, message = "Vui lòng nhập đầy đủ các chỉ số trước khi gửi duyệt." });
            result.TrangThai = "cho_duyet";
            result.ThoiGianHoanThanh = DateTime.Now;
            work.Status = "finished";
            work.FinishedAt ??= DateTime.Now;
            work.IdctphieuNavigation.TrangThai = "cho_duyet";
            work.IdctphieuNavigation.IdphieuXetNghiemNavigation.TrangThai = "cho_duyet";
            AddTracking(work.IdctphieuNavigation.IdphieuXetNghiemNavigation.IdkhachHang, "ketquaxetnghiem", result.IdketQua, "Kỹ thuật viên gửi kết quả sang bác sĩ duyệt", null);
        }
        else
        {
            result.TrangThai = "dang_thuc_hien";
        }
        work.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();
        await tx.CommitAsync();
        return Ok(new { success = true, id = result.IdketQua, status = submit ? "SUBMITTED" : "DRAFT", message = submit ? "Đã gửi kết quả sang bác sĩ duyệt." : "Đã lưu nháp kết quả." });
    }

    private IQueryable<Worklist> WorkForTechnician(ulong id, string? technicianId) =>
        _context.Worklists.Where(x => x.Id == id && (x.Idktv == null || x.Idktv == technicianId));

    private async Task<string?> GetTechnicianId()
    {
        if (!int.TryParse(User.FindFirstValue("userId"), out var userId)) return null;
        return await _context.Users.AsNoTracking().Where(x => x.UserId == userId && x.IsActive == true).Select(x => x.IdnhanVien).FirstOrDefaultAsync();
    }

    private async Task<int?> GetDoctorId()
    {
        if (!int.TryParse(User.FindFirstValue("userId"), out var userId)) return null;
        var account = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == userId && x.IsActive == true);
        if (account?.IdbacSi is not null) return account.IdbacSi;
        if (string.IsNullOrWhiteSpace(account?.IdnhanVien)) return null;
        return await _context.Bacsis.AsNoTracking().Where(x => x.IdnhanVien == account.IdnhanVien).Select(x => (int?)x.IdbacSi).FirstOrDefaultAsync();
    }

    private void AddTracking(string customerId, string objectType, string objectId, string action, string? description)
    {
        int? uid = int.TryParse(User.FindFirstValue("userId"), out var parsed) ? parsed : null;
        _context.Truyvets.Add(new Truyvet { IdkhachHang = customerId, LoaiDoiTuong = objectType, IddoiTuong = objectId, HanhDong = action, UserIdthucHien = uid, NguonThucHien = "user", ThoiGian = DateTime.Now, MoTa = description });
    }

    private static string DbSpecimenStatusToApi(string value) => value switch { "da_ban_giao" => "HANDED_OVER", "ktv_tiep_nhan" => "RECEIVED", "tu_choi_mau" => "REJECTED", "dang_xu_ly" => "IN_PROGRESS", "hoan_tat" => "COMPLETED", _ => value.ToUpperInvariant() };
    private static string? ApiSpecimenStatusToDb(string? value) => value?.Trim().ToUpperInvariant() switch { "HANDED_OVER" => "da_ban_giao", "RECEIVED" => "ktv_tiep_nhan", "REJECTED" => "tu_choi_mau", "IN_PROGRESS" or "PROCESSING" => "dang_xu_ly", "COMPLETED" => "hoan_tat", "" or null => null, var v => v.ToLowerInvariant() };
    private static string DbWorkStatusToApi(string value) => value switch { "queue" => "PENDING", "running" => "IN_PROGRESS", "to_result" => "COMPLETED", "finished" => "RESULT_ENTERED", "rerun" => "PENDING", _ => value.ToUpperInvariant() };
    private static string? ApiWorkStatusToDb(string? value) => value?.Trim().ToUpperInvariant() switch { "PENDING" => "queue", "IN_PROGRESS" => "running", "COMPLETED" => "to_result", "RESULT_ENTERED" or "SUBMITTED" => "finished", "" or null => null, var v => v.ToLowerInvariant() };

    public sealed record CollectSpecimenRequest(string? PatientCode, string? PatientName, string? SampleType, string? Barcode, string? SamplingTime, string? Notes, string? AppointmentId);
    public sealed record HandoverRequest(string? ReceiverId, string? Note, string? AppointmentId, string? PatientCode);
    public sealed record ReceiveSpecimenRequest(string? Condition, string? Notes);
    public sealed record RejectSpecimenRequest(string? Reason);
    public sealed record UpdateSpecimenRequest(string? Status, string? Notes);
    public sealed record WorkActionRequest(string? Instrument, string? ReagentLot);
    public sealed record ResultEntryRequest(List<ResultIndicatorRequest>? Indicators, string? Notes);
    public sealed record ResultIndicatorRequest(string? IndicatorId, string? Value, bool Abnormal);
}
