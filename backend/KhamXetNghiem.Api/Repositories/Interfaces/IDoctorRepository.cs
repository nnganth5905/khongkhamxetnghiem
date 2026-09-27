using KhamXetNghiem.Api.DTOs.Requests;
using KhamXetNghiem.Api.DTOs.Responses;

namespace KhamXetNghiem.Api.Repositories.Interfaces;

public interface IDoctorRepository
{
    Task<List<DoctorResponse>> GetAllAsync(
        string? specialtyId,
        string? query,
        decimal? minimumStars,
        bool activeOnly,
        CancellationToken cancellationToken = default
    );

    Task<DoctorResponse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default
    );

    Task<DoctorResponse> CreateAsync(
        DoctorRequest request,
        CancellationToken cancellationToken = default
    );

    Task<DoctorResponse> UpdateAsync(
        int id,
        DoctorRequest request,
        CancellationToken cancellationToken = default
    );

    Task SoftDeleteAsync(
        int id,
        CancellationToken cancellationToken = default
    );
}