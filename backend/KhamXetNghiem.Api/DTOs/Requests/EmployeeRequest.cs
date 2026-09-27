using System.ComponentModel.DataAnnotations;

namespace KhamXetNghiem.Api.DTOs.Requests;

public sealed class EmployeeRequest
{
    [Required(
        ErrorMessage = "Họ tên không được để trống."
    )]
    public string HoTen { get; set; } = string.Empty;

    public string? SoDienThoai { get; set; }

    [EmailAddress(
        ErrorMessage = "Email không hợp lệ."
    )]
    public string? Email { get; set; }

    public string? IdCoSo { get; set; }

    [Required(
        ErrorMessage = "Vai trò không được để trống."
    )]
    public string VaiTro { get; set; } = string.Empty;

    public string? TrangThai { get; set; }
}