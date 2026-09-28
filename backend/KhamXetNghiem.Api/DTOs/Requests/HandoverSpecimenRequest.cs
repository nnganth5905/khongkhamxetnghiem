using System.ComponentModel.DataAnnotations;

namespace KhamXetNghiem.Api.DTOs.Requests;

public sealed class HandoverSpecimenRequest
{
    [Required]
    public string ReceiverId { get; set; } = string.Empty;

    public string? Note { get; set; }
}