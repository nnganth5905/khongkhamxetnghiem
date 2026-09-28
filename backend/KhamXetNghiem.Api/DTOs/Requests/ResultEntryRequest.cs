namespace KhamXetNghiem.Api.DTOs.Requests;

public sealed class ResultEntryRequest
{
    public List<ResultIndicatorRequest> Indicators { get; set; } = [];

    public string? GeneralResult { get; set; }

    public string? Notes { get; set; }
}