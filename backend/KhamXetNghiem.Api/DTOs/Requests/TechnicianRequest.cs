using System.ComponentModel.DataAnnotations;

namespace KhamXetNghiem.Api.DTOs.Requests;

public sealed class TechnicianRequest
{
    [Required(
        ErrorMessage = "Họ tên kỹ thuật viên không được để trống."
    )]
    public string HoTen { get; set; } = string.Empty;

    public string? IdCoSo { get; set; }

    public string? SoDienThoai { get; set; }

    [EmailAddress(
        ErrorMessage = "Email không hợp lệ."
    )]
    public string? Email { get; set; }

    public string? TrangThai { get; set; }
}