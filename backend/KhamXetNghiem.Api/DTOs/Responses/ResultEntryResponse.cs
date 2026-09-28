namespace KhamXetNghiem.Api.DTOs.Responses;

public sealed class ResultEntryResponse
{
    public long WorklistId { get; init; }

    public string? ResultId { get; init; }

    public string PatientName { get; init; } = string.Empty;

    public string CustomerId { get; init; } = string.Empty;

    public string SpecimenId { get; init; } = string.Empty;

    public string SpecimenCode { get; init; } = string.Empty;

    public string TestId { get; init; } = string.Empty;

    public string TestName { get; init; } = string.Empty;

    public string WorklistStatus { get; init; } = string.Empty;

    public string? ResultStatus { get; init; }

    public string? GeneralResult { get; init; }

    public string? Notes { get; init; }

    public bool Editable { get; init; }

    public List<ResultIndicatorResponse> Indicators { get; init; } = [];
}