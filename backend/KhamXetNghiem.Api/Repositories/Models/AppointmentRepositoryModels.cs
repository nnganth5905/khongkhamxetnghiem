namespace KhamXetNghiem.Api.Repositories.Models;

public sealed record AppointmentAccountContext(
    int UserId,
    string? CustomerId,
    string Email
);

public sealed record AppointmentCustomerData(
    string Id,
    string FullName,
    string? Phone,
    string? Email,
    DateTime? BirthDate,
    string? Gender
);

public sealed record AppointmentDoctorData(
    int Id,
    string Name,
    string SpecialtyId,
    string? FacilityId,
    string Status
);

public sealed record AppointmentTestData(
    string Id,
    string Name,
    string SpecialtyId,
    decimal Price,
    int? EstimatedMinutes
);

public sealed record AppointmentWorkWindow(
    TimeSpan Start,
    TimeSpan End
);

public sealed record AppointmentEditData(
    long DbId,
    string Code,
    string Type,
    int? UserId,
    string CustomerId,
    int? DoctorId,
    string? SpecialtyId,
    string FacilityId,
    DateTime Date,
    TimeSpan Time,
    string Status,
    string? Note
);

public sealed record NewExaminationBooking(
    string Code,
    string QrCode,
    int? UserId,
    string CustomerId,
    string SpecialtyId,
    int DoctorId,
    string FacilityId,
    DateTime Date,
    TimeSpan Time,
    string? Note
);

public sealed record NewTestBooking(
    string Code,
    string QrCode,
    int? UserId,
    string CustomerId,
    int DoctorId,
    string FacilityId,
    DateTime Date,
    TimeSpan Time,
    string? Note,
    IReadOnlyList<AppointmentTestData> Tests
);

public sealed record AppointmentCreateDbResult(
    long Id,
    string Code,
    string QrCode
);