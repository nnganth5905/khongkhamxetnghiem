namespace KhamXetNghiem.Api.DTOs.Requests;

public sealed class VisitCheckInRequest
{
    public string? AppointmentId { get; set; }

    public string? CustomerId { get; set; }

    /// <summary>
    /// Endpoint mới gửi visitType.
    /// </summary>
    public string? VisitType { get; set; }

    /// <summary>
    /// Endpoint compatibility từ appointmentService.js gửi type.
    /// </summary>
    public string? Type { get; set; }

    public string? Note { get; set; }

    public string? ResolveType()
    {
        return !string.IsNullOrWhiteSpace(VisitType)
            ? VisitType
            : Type;
    }
}