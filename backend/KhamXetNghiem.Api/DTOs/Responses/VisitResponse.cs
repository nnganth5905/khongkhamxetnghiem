namespace KhamXetNghiem.Api.DTOs.Responses;

public sealed class VisitResponse
{
    public string Id { get; init; } = string.Empty;

    public string AppointmentId { get; init; } = string.Empty;

    public string CustomerId { get; init; } = string.Empty;

    public string? DoctorId { get; init; }

    public string? RoomId { get; init; }

    public string Type { get; init; } = string.Empty;

    public int? QueueNumber { get; init; }

    public string Status { get; init; } = string.Empty;

    public DateTime? ReceivedAt { get; init; }

    public DateTime? StartedAt { get; init; }

    public DateTime? EndedAt { get; init; }

    public string? Note { get; init; }
}