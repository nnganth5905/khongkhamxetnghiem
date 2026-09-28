namespace KhamXetNghiem.Api.DTOs.Responses;

public sealed class AppointmentOptionsResponse
{
    public Dictionary<string, string> Departments { get; init; }
        = new();

    public List<AppointmentDoctorOption> Doctors { get; init; }
        = new();

    public List<AppointmentTestOption> Tests { get; init; }
        = new();

    public List<AppointmentFacilityOption> Facilities { get; init; }
        = new();
}

public sealed class AppointmentDoctorOption
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string SpecialtyId { get; init; } = string.Empty;

    public string? FacilityId { get; init; }
}

public sealed class AppointmentTestOption
{
    public string Id { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public string SpecialtyId { get; init; } = string.Empty;

    public decimal Price { get; init; }

    public int? EstimatedMinutes { get; init; }
}

public sealed class AppointmentFacilityOption
{
    public string Id { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;
}