namespace KhamXetNghiem.Api.DTOs.Responses;

public sealed class AvailableSlotsResponse
{
    public int DoctorId { get; init; }

    public string Date { get; init; } = string.Empty;

    public List<string> Slots { get; init; } = new();

    public List<string> TakenTimes { get; init; } = new();
}