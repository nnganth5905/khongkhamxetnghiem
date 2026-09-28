using System.ComponentModel.DataAnnotations;

namespace KhamXetNghiem.Api.DTOs.Requests;

public sealed class CollectSpecimenRequest
{
    [Required]
    public string AppointmentId { get; set; } = string.Empty;

    /// <summary>
    /// Chính xác dòng ctphieuxetnghiem cần lấy mẫu.
    /// Fix lỗi Java cũ lấy LIMIT 1.
    /// </summary>
    [Range(1, long.MaxValue)]
    public long TestOrderItemId { get; set; }

    [Required]
    public string SpecimenType { get; set; } = string.Empty;

    /// <summary>
    /// Có thể bỏ trống để backend tự sinh.
    /// </summary>
    public string? Barcode { get; set; }

    public string? Notes { get; set; }
}