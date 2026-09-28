using System.ComponentModel.DataAnnotations;

namespace KhamXetNghiem.Api.DTOs.Requests;

public sealed class TestAppointmentRequest
{
    [Required]
    public string Hoten { get; set; } = string.Empty;

    public string? Email { get; set; }

    [Required]
    public string Sodienthoai { get; set; } = string.Empty;

    public string? Gioitinh { get; set; }

    public string? Ngaysinh { get; set; }

    [Required]
    public string Ngay { get; set; } = string.Empty;

    [Required]
    public string Gio { get; set; } = string.Empty;

    [Required]
    public string Idbacsi { get; set; } = string.Empty;

    [Required]
    public string Idxetnghiem { get; set; } = string.Empty;

    public string? Idcoso { get; set; }

    public string? Ghichu { get; set; }
}