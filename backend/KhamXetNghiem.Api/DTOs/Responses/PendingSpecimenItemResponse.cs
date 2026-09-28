namespace KhamXetNghiem.Api.DTOs.Responses;

public sealed class PendingSpecimenItemResponse
{
    public long TestOrderItemId { get; init; }

    public string TestOrderId { get; init; } = string.Empty;

    public string TestId { get; init; } = string.Empty;

    public string TestName { get; init; } = string.Empty;

    public string CustomerId { get; init; } = string.Empty;

    public string PatientName { get; init; } = string.Empty;

    public string? DefaultSpecimenType { get; init; }

    public string Status { get; init; } = string.Empty;
}