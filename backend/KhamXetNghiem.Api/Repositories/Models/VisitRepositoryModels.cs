namespace KhamXetNghiem.Api.Repositories.Models;

public sealed record VisitActorContext(
    int UserId,
    int? DoctorId,
    string? EmployeeId,
    string? CustomerId
);

public sealed record VisitAppointmentSource(
    long DbId,
    string Code,
    string Type,
    int? BookedUserId,
    string CustomerId,
    int? DoctorId,
    string? SpecialtyId,
    string FacilityId,
    DateTime AppointmentDate,
    TimeSpan AppointmentTime,
    string Status,
    string? Note,
    string? QrCode
);

public sealed record VisitRoomData(
    string Id,
    string Name,
    string FacilityId,
    string? SpecialtyId,
    string Type
);

public sealed record WalkInVisitData(
    string FullName,
    string Phone,
    string? Email,
    DateTime? BirthDate,
    string Gender,
    string? Address,
    string Type,
    string? SpecialtyId,
    string? TestId,
    string? Reason,
    string? Note
);