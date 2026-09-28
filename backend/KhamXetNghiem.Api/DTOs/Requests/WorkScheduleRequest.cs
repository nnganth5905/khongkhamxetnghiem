using System.ComponentModel.DataAnnotations;

namespace KhamXetNghiem.Api.DTOs.Requests;

public sealed class WorkScheduleRequest
{
    [Required(
        ErrorMessage = "Mã bác sĩ không được để trống."
    )]
    public int IdBacSi { get; set; }

    [Required(
        ErrorMessage = "Ngày làm việc không được để trống."
    )]
    public string NgayLamViec { get; set; } = string.Empty;

    public string? CaLamViec { get; set; }

    [Required(
        ErrorMessage = "Giờ bắt đầu không được để trống."
    )]
    public string GioBatDau { get; set; } = string.Empty;

    [Required(
        ErrorMessage = "Giờ kết thúc không được để trống."
    )]
    public string GioKetThuc { get; set; } = string.Empty;

    public string? IdPhong { get; set; }

    public string? TrangThai { get; set; }

    [StringLength(
        255,
        ErrorMessage = "Ghi chú tối đa 255 ký tự."
    )]
    public string? GhiChu { get; set; }
}