using System.ComponentModel.DataAnnotations;

namespace KhamXetNghiem.Api.DTOs.Requests;

public sealed class QrCheckInRequest
{
    [Required]
    public string QrCode { get; set; } = string.Empty;

    public string? Note { get; set; }
}