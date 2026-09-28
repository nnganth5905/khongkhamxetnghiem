namespace KhamXetNghiem.Api.DTOs.Responses;

public sealed class CheckInResponse
{
    public bool Success { get; init; } = true;

    public string Message { get; init; } = string.Empty;

    public string VisitId { get; init; } = string.Empty;

    public string AppointmentId { get; init; } = string.Empty;

    public string Type { get; init; } = string.Empty;

    public int? QueueNumber { get; init; }

    public string? RoomId { get; init; }

    public string? RoomName { get; init; }

    public DateTime ReceivedAt { get; init; }

    public string Status { get; init; } = string.Empty;
}