namespace KhamXetNghiem.Api.DTOs.Responses;

public sealed class ResultEntryResponse
{
    public long WorklistId { get; init; }

    public string? ResultId { get; init; }

    public string PatientName { get; init; } = string.Empty;

    public string SpecimenCode { get; init; } = string.Empty;

    public string TestName { get; init; } = string.Empty;

    public string? Notes { get; init; }

    public string Status { get; init; } = string.Empty;

    public List<ResultIndicatorResponse> Indicators { get; init; }
        = new();
}

public sealed class ResultIndicatorResponse
{
    public string IndicatorId { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public string? Value { get; init; }

    public string? Unit { get; init; }

    public string? Reference { get; init; }

    public bool Abnormal { get; init; }
}