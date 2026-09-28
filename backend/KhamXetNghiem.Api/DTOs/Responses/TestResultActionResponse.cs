namespace KhamXetNghiem.Api.DTOs.Responses;

public sealed class TestResultActionResponse
{
    public bool Success { get; init; } = true;

    public string Message { get; init; } = string.Empty;

    public string ResultId { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;
}