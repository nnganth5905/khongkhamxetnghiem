namespace KhamXetNghiem.Api.DTOs.Responses;

public sealed class SpecimenDetailResponse
{
    public string Code { get; init; } = string.Empty;

    public string Barcode { get; init; } = string.Empty;

    public string? PatientName { get; init; }

    public string SpecimenType { get; init; } = string.Empty;

    public DateTime? CollectedAt { get; init; }

    public string Status { get; init; } = "UNKNOWN";

    public string? Notes { get; init; }

    public string? HandoverBy { get; init; }
}