namespace KhamXetNghiem.Api.DTOs.Responses;

public sealed class CurrentTechnicianResponse
{
    public bool Authenticated { get; init; }

    public int UserId { get; init; }

    public string IdNhanVien { get; init; } = string.Empty;

    public string HoTen { get; init; } = string.Empty;

    public string? Email { get; init; }

    public string? SoDienThoai { get; init; }

    public string? IdCoSo { get; init; }
}