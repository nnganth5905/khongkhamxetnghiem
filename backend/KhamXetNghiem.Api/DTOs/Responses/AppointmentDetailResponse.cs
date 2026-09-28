namespace KhamXetNghiem.Api.DTOs.Responses;

public sealed class AppointmentDetailResponse
{
    public string Id { get; init; } = string.Empty;

    public string Type { get; init; } = string.Empty;

    public string CustomerId { get; init; } = string.Empty;

    public string? CustomerName { get; init; }

    public string? CustomerPhone { get; init; }

    public string? DoctorId { get; init; }

    public string? DoctorName { get; init; }

    public string? SpecialtyId { get; init; }

    public string? FacilityId { get; init; }

    public string Date { get; init; } = string.Empty;

    public string Time { get; init; } = string.Empty;

    // aliases cho LichHenSua
    public string AppointmentDate => Date;

    public string AppointmentTime => Time;

    public string Status { get; init; } = string.Empty;

    public string? QrCode { get; init; }

    public string? Note { get; init; }

    public List<AppointmentTimelineItem> Timeline { get; init; }
        = new();

    public List<AppointmentTestOrderSummary> TestOrders { get; init; }
        = new();

    public List<string> RegisteredTests { get; init; }
        = new();
}

public sealed class AppointmentTimelineItem
{
    /// <summary>
    /// Giờ local Việt Nam từ MySQL DATETIME/TIMESTAMP.
    /// Không convert UTC lần hai.
    /// </summary>
    public string Time { get; init; } = string.Empty;

    public string Event { get; init; } = string.Empty;
}

public sealed class AppointmentTestOrderSummary
{
    public string IDPhieuXetNghiem { get; init; } = string.Empty;

    public string? TrangThai { get; init; }

    public string? NgayTao { get; init; }

    public List<string> Tests { get; init; } = new();
}

public sealed class CreateAppointmentResponse
{
    public long Id { get; init; }

    public string AppointmentCode { get; init; } = string.Empty;

    public string Type { get; init; } = string.Empty;

    public string QrCode { get; init; } = string.Empty;

    public string Message { get; init; } = string.Empty;
}