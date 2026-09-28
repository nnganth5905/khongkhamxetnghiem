namespace KhamXetNghiem.Api.DTOs.Requests;

public sealed class ReceiveSpecimenRequest
{
    public string Condition { get; set; } = "GOOD";

    public string? Notes { get; set; }
}