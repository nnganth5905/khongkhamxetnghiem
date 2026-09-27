namespace KhamXetNghiem.Api.DTOs.Responses;

public sealed class DoctorResultIndicatorResponse
{
    public string Name { get; init; } = string.Empty;

    public string? Value { get; init; }

    public string? Unit { get; init; }

    public string? Evaluation { get; init; }

    public bool Abnormal { get; init; }
}