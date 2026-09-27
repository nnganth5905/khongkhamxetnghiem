namespace KhamXetNghiem.Api.DTOs.Responses;

public sealed class WorklistItemResponse
{
    public long Id { get; init; }

    public string SpecimenCode { get; init; } = string.Empty;

    public string TestName { get; init; } = string.Empty;

    public string PatientName { get; init; } = string.Empty;

    public DateTime? AssignedAt { get; init; }

    public string Status { get; init; } = "PENDING";
}