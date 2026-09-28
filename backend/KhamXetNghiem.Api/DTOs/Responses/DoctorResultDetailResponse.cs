namespace KhamXetNghiem.Api.DTOs.Responses;

public sealed class DoctorResultDetailResponse
{
    public string Id { get; init; } = string.Empty;

    public string SpecimenId { get; init; } = string.Empty;

    public string SpecimenCode { get; init; } = string.Empty;

    public string CustomerId { get; init; } = string.Empty;

    public string PatientName { get; init; } = string.Empty;

    public string TestName { get; init; } = string.Empty;

    public string TechnicianName { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public DateTime? SubmittedAt { get; init; }

    public string? TechnicianNotes { get; init; }

    public string? GeneralResult { get; init; }

    public string? DoctorConclusion { get; init; }

    public List<ResultIndicatorResponse> Indicators { get; init; } = [];
}