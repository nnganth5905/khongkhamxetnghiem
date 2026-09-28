using KhamXetNghiem.Api.DTOs.Responses;
using KhamXetNghiem.Api.Entities;

namespace KhamXetNghiem.Api.Repositories.Interfaces;

public interface IWorkScheduleRepository
{
    Task<List<WorkScheduleResponse>> GetAllAsync(
        string? query,
        CancellationToken cancellationToken = default
    );

    Task<List<WorkScheduleResponse>> GetDoctorSchedulesAsync(
        int doctorId,
        DateTime? date,
        CancellationToken cancellationToken = default
    );

    Task<WorkSchedule?> FindByIdAsync(
        long id,
        CancellationToken cancellationToken = default
    );

    Task<WorkScheduleResponse?> GetResponseByIdAsync(
        long id,
        CancellationToken cancellationToken = default
    );

    Task<bool> DoctorExistsAsync(
        int doctorId,
        CancellationToken cancellationToken = default
    );

    Task<bool> RoomExistsAndActiveAsync(
        string roomId,
        CancellationToken cancellationToken = default
    );

    Task<bool> HasDoctorOverlapAsync(
        int doctorId,
        DateTime date,
        TimeSpan start,
        TimeSpan end,
        long? excludeScheduleId,
        CancellationToken cancellationToken = default
    );

    Task<bool> HasRoomOverlapAsync(
        string roomId,
        DateTime date,
        TimeSpan start,
        TimeSpan end,
        long? excludeScheduleId,
        CancellationToken cancellationToken = default
    );

    Task<int?> FindUserIdByLoginAsync(
        string login,
        CancellationToken cancellationToken = default
    );

    Task<long> CreateAsync(
        WorkSchedule schedule,
        CancellationToken cancellationToken = default
    );

    Task UpdateAsync(
        WorkSchedule schedule,
        CancellationToken cancellationToken = default
    );
}