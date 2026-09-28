namespace KhamXetNghiem.Api.DTOs.Responses;

public sealed class SpecimenResponse
{
    public string Id { get; init; } = string.Empty;

    public string Code => Id;

    public long TestOrderItemId { get; init; }

    public string TestOrderId { get; init; } = string.Empty;

    public string TestId { get; init; } = string.Empty;

    public string TestName { get; init; } = string.Empty;

    public string CustomerId { get; init; } = string.Empty;

    public string PatientName { get; init; } = string.Empty;

    public string SpecimenType { get; init; } = string.Empty;

    public string Barcode { get; init; } = string.Empty;

    public DateTime? CollectedAt { get; init; }

    public int? CollectorDoctorId { get; init; }

    public string? CollectorName { get; init; }

    public DateTime? HandedOverAt { get; init; }

    public string? HandoverBy { get; init; }

    public DateTime? ReceivedAt { get; init; }

    public string? TechnicianId { get; init; }

    public string? TechnicianName { get; init; }

    public string Status { get; init; } = string.Empty;

    public string? Note { get; init; }
}