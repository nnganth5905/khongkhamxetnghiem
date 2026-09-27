namespace KhamXetNghiem.Api.DTOs.Requests;

public sealed class ResultEntryRequest
{
    public List<ResultIndicatorInput> Indicators { get; set; }
        = new();

    public string? Notes { get; set; }
}

public sealed class ResultIndicatorInput
{
    public string IndicatorId { get; set; } = string.Empty;

    public string? Value { get; set; }

    public bool Abnormal { get; set; }
}