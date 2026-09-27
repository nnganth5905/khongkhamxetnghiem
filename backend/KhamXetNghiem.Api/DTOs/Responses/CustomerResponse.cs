namespace KhamXetNghiem.Api.DTOs.Responses;

public sealed class CustomerResponse
{
    public string Id { get; init; } = string.Empty;

    public string IdKhachHang { get; init; } = string.Empty;

    public string TenKhachHang { get; init; } = string.Empty;

    public DateOnly? NgaySinh { get; init; }

    public string? SoDienThoai { get; init; }

    public string? GioiTinh { get; init; }

    public string? Cccd { get; init; }

    public string? DiaChi { get; init; }

    public string? Email { get; init; }

    public string Status { get; init; } = "yes";

    public DateTime? CreatedAt { get; init; }

    public DateTime? UpdatedAt { get; init; }
}