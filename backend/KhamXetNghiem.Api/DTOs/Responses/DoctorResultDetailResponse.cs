namespace KhamXetNghiem.Api.DTOs.Responses;

public sealed class DoctorResultDetailResponse
{
    public string Id { get; init; } = string.Empty;

    public string SpecimenCode { get; init; } = string.Empty;

    public string PatientName { get; init; } = string.Empty;

    public string TestName { get; init; } = string.Empty;

    public string? TechnicianNotes { get; init; }

    public List<DoctorResultIndicatorResponse> Indicators { get; init; }
        = new();
}