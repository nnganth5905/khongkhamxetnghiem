namespace KhamXetNghiem.Api.DTOs.Responses;

public sealed class ResultIndicatorResponse
{
    public string IndicatorId { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public string? Unit { get; init; }

    public string DataType { get; init; } = "number";

    public string Value { get; init; } = string.Empty;

    public decimal? NumericValue { get; init; }

    public string? TextValue { get; init; }

    public decimal? Min { get; init; }

    public decimal? Max { get; init; }

    public string? Reference { get; init; }

    public string Evaluation { get; init; } = "chua_danh_gia";

    public bool Abnormal =>
        Evaluation is
            "thap"
            or "cao"
            or "bat_thuong";

    public string? Note { get; init; }
}