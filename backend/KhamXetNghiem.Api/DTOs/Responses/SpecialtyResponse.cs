namespace KhamXetNghiem.Api.DTOs.Responses;

public sealed class SpecialtyResponse
{
    public string Id { get; init; } = string.Empty;

    public string IdChuyenKhoa => Id;

    public string MaChuyenKhoa => Id;

    public string TenChuyenKhoa { get; init; } = string.Empty;

    public string Name => TenChuyenKhoa;

    public string? MoTa { get; init; }

    /// <summary>
    /// ACTIVE / INACTIVE
    /// </summary>
    public string TrangThai { get; init; } = "ACTIVE";

    /// <summary>
    /// yes / no trong MySQL.
    /// </summary>
    public string Status { get; init; } = "yes";
}