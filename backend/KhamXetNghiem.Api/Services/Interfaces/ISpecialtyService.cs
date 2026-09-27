using KhamXetNghiem.Api.DTOs.Requests;
using KhamXetNghiem.Api.DTOs.Responses;

namespace KhamXetNghiem.Api.Services.Interfaces;

public interface ISpecialtyService
{
    Task<List<SpecialtyResponse>> GetAllAsync(
        string? query,
        bool activeOnly,
        CancellationToken cancellationToken = default
    );

    Task<SpecialtyResponse> GetByIdAsync(
        string id,
        CancellationToken cancellationToken = default
    );

    Task<SpecialtyResponse> CreateAsync(
        SpecialtyRequest request,
        CancellationToken cancellationToken = default
    );

    Task<SpecialtyResponse> UpdateAsync(
        string id,
        SpecialtyRequest request,
        CancellationToken cancellationToken = default
    );

    Task DeleteAsync(
        string id,
        CancellationToken cancellationToken = default
    );
}