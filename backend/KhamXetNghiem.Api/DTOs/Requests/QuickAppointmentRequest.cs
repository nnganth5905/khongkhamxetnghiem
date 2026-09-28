using System.ComponentModel.DataAnnotations;

namespace KhamXetNghiem.Api.DTOs.Requests;

public sealed class QuickAppointmentRequest
{
    [Required]
    public string Ngay { get; set; } = string.Empty;

    [Required]
    public string Gio { get; set; } = string.Empty;

    [Required]
    public string Idbacsi { get; set; } = string.Empty;

    public string? Ghichu { get; set; }
}