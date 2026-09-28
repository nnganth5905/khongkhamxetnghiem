namespace KhamXetNghiem.Api.DTOs.Responses;

public sealed class WalkInVisitResponse
{
    public bool Success { get; init; } = true;

    public string Message { get; init; } = string.Empty;

    public string AppointmentId { get; init; } = string.Empty;

    public string VisitId { get; init; } = string.Empty;

    public string CustomerId { get; init; } = string.Empty;

    public string Type { get; init; } = string.Empty;

    public int QueueNumber { get; init; }

    public string? RoomId { get; init; }

    public string? RoomName { get; init; }

    public string? TestOrderId { get; init; }

    public DateTime ReceivedAt { get; init; }
}