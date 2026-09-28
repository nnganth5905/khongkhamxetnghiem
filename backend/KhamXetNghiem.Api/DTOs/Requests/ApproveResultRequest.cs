using System.ComponentModel.DataAnnotations;

namespace KhamXetNghiem.Api.DTOs.Requests;

public sealed class ApproveTestResultRequest
{
    [Required]
    public string Conclusion { get; set; } = string.Empty;
}