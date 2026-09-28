namespace KhamXetNghiem.Api.DTOs.Responses;

public sealed class AppointmentResponse
{
    public string Id { get; init; } = string.Empty;

    public string Type { get; init; } = string.Empty;

    public string CustomerId { get; init; } = string.Empty;

    public string? CustomerName { get; init; }

    public string? CustomerPhone { get; init; }

    public string? DoctorId { get; init; }

    public string? DoctorName { get; init; }

    public string AppointmentDate { get; init; } = string.Empty;

    public string AppointmentTime { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public string? QrCode { get; init; }

    public string? Note { get; init; }
}