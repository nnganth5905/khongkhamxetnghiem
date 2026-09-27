using System.ComponentModel.DataAnnotations;

namespace KhamXetNghiem.Api.DTOs.Requests;

public sealed class CustomerRequest
{
    [Required(
        ErrorMessage = "Họ tên không được để trống."
    )]
    public string TenKhachHang { get; set; } = string.Empty;

    public DateOnly? NgaySinh { get; set; }

    public string? GioiTinh { get; set; }

    public string? SoDienThoai { get; set; }

    public string? Cccd { get; set; }

    public string? DiaChi { get; set; }

    [EmailAddress(
        ErrorMessage = "Email không hợp lệ."
    )]
    public string? Email { get; set; }

    public string? Status { get; set; }
}