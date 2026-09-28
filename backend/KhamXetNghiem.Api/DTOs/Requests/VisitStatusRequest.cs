using System.ComponentModel.DataAnnotations;

namespace KhamXetNghiem.Api.DTOs.Requests;

public sealed class VisitStatusRequest
{
    [Required]
    public string Status { get; set; } = string.Empty;

    public string? Note { get; set; }
}