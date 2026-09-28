namespace KhamXetNghiem.Api.DTOs.Responses;

public sealed class PendingDoctorResultResponse
{
    public string Id { get; init; } = string.Empty;

    public string SpecimenCode { get; init; } = string.Empty;

    public string PatientName { get; init; } = string.Empty;

    public string TestName { get; init; } = string.Empty;

    public string TechnicianName { get; init; } = string.Empty;

    public DateTime? SubmittedAt { get; init; }

    public bool HasAbnormalIndicator { get; init; }

    public string Status { get; init; } = "PENDING_APPROVAL";
}