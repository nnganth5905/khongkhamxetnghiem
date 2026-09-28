using System.ComponentModel.DataAnnotations;

namespace KhamXetNghiem.Api.DTOs.Requests;

public sealed class RejectSpecimenRequest
{
    [Required]
    public string Reason { get; set; } = string.Empty;
}