namespace KhamXetNghiem.Api.DTOs.Responses;

public sealed class SpecimenActionResponse
{
    public bool Success { get; init; } = true;

    public string Message { get; init; } = string.Empty;

    public string Id { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public long? WorklistId { get; init; }
}