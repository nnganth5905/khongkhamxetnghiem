using System.Security.Claims;

using KhamXetNghiem.Api.DTOs.Requests;
using KhamXetNghiem.Api.DTOs.Responses;

namespace KhamXetNghiem.Api.Services.Interfaces;

public interface IWorkScheduleService
{
    Task<List<WorkScheduleResponse>> GetDoctorScheduleAsync(
        int doctorId,
        string? date,
        CancellationToken cancellationToken = default
    );

    Task<List<WorkScheduleResponse>> GetSchedulesAsync(
        string? query,
        CancellationToken cancellationToken = default
    );

    Task<WorkScheduleResponse> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default
    );

    Task<WorkScheduleResponse> CreateAsync(
        WorkScheduleRequest request,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    );

    Task<WorkScheduleResponse> UpdateAsync(
        long id,
        WorkScheduleRequest request,
        CancellationToken cancellationToken = default
    );

    Task DeleteAsync(
        long id,
        CancellationToken cancellationToken = default
    );
}