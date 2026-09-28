using KhamXetNghiem.Api.DTOs.Responses;
using KhamXetNghiem.Api.Repositories.Models;

namespace KhamXetNghiem.Api.Repositories.Interfaces;

public interface IAppointmentRepository
{
    Task<AppointmentOptionsResponse> GetOptionsAsync(
        CancellationToken cancellationToken = default
    );

    Task<AppointmentAccountContext?> FindAccountAsync(
        string login,
        CancellationToken cancellationToken = default
    );

    Task<AppointmentCustomerData?> FindCustomerByIdAsync(
        string customerId,
        CancellationToken cancellationToken = default
    );

    Task<AppointmentCustomerData?> FindCustomerAsync(
        string fullName,
        string phone,
        CancellationToken cancellationToken = default
    );

    Task<AppointmentCustomerData> CreateCustomerAsync(
        string id,
        string fullName,
        string phone,
        string? email,
        DateTime? birthDate,
        string gender,
        CancellationToken cancellationToken = default
    );

    Task<AppointmentDoctorData?> FindDoctorAsync(
        int doctorId,
        CancellationToken cancellationToken = default
    );

    Task<AppointmentTestData?> FindTestAsync(
        string testId,
        CancellationToken cancellationToken = default
    );

    Task<bool> IsFacilityActiveAsync(
        string facilityId,
        CancellationToken cancellationToken = default
    );

    Task<List<AppointmentWorkWindow>> GetDoctorWorkWindowsAsync(
        int doctorId,
        DateTime date,
        CancellationToken cancellationToken = default
    );

    Task<List<string>> GetDoctorOccupiedTimesAsync(
        int doctorId,
        DateTime date,
        string? excludeAppointmentCode = null,
        CancellationToken cancellationToken = default
    );

    Task<List<string>> GetTakenTimesAsync(
        string type,
        int doctorId,
        DateTime date,
        CancellationToken cancellationToken = default
    );

    Task<AppointmentCreateDbResult> CreateExaminationAsync(
        NewExaminationBooking booking,
        CancellationToken cancellationToken = default
    );

    Task<AppointmentCreateDbResult> CreateTestAsync(
        NewTestBooking booking,
        CancellationToken cancellationToken = default
    );

    Task<List<AppointmentResponse>> GetMyAppointmentsAsync(
        int userId,
        string? customerId,
        CancellationToken cancellationToken = default
    );

    Task<List<AppointmentResponse>> GetAllAppointmentsAsync(
        DateTime date,
        CancellationToken cancellationToken = default
    );

    Task<AppointmentDetailResponse?> GetDetailAsync(
        string code,
        CancellationToken cancellationToken = default
    );

    Task<AppointmentEditData?> GetEditDataAsync(
        string code,
        CancellationToken cancellationToken = default
    );

    Task<List<string>> GetTestIdsAsync(
        string appointmentCode,
        CancellationToken cancellationToken = default
    );

    Task UpdateAppointmentAsync(
        AppointmentEditData current,
        int doctorId,
        DateTime newDate,
        TimeSpan newTime,
        string? note,
        int? actorUserId,
        CancellationToken cancellationToken = default
    );

    Task CancelAppointmentAsync(
        AppointmentEditData current,
        int? actorUserId,
        CancellationToken cancellationToken = default
    );

    Task<int> MarkExpiredAppointmentsNoShowAsync(
        DateTime today,
        CancellationToken cancellationToken = default
    );
}