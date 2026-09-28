using System.ComponentModel.DataAnnotations;

namespace KhamXetNghiem.Api.DTOs.Requests;

public sealed class WalkInVisitRequest
{
    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public string Phone { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public string? Address { get; set; }

    [Required]
    public string ServiceType { get; set; } = string.Empty;

    public string? SpecialtyId { get; set; }

    /// <summary>
    /// Bắt buộc nếu ServiceType = TEST.
    /// </summary>
    public string? TestId { get; set; }

    public string? Reason { get; set; }

    public string? Notes { get; set; }
}