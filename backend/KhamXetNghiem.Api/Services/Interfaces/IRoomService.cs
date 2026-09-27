using KhamXetNghiem.Api.DTOs.Requests;
using KhamXetNghiem.Api.DTOs.Responses;

namespace KhamXetNghiem.Api.Services.Interfaces;

public interface IRoomService
{
    Task<List<RoomResponse>> GetAllAsync(
        string? query,
        bool activeOnly,
        CancellationToken cancellationToken = default
    );

    Task<RoomResponse> GetByIdAsync(
        string id,
        CancellationToken cancellationToken = default
    );

    Task<RoomResponse> CreateAsync(
        RoomRequest request,
        CancellationToken cancellationToken = default
    );

    Task<RoomResponse> UpdateAsync(
        string id,
        RoomRequest request,
        CancellationToken cancellationToken = default
    );

    Task DeleteAsync(
        string id,
        CancellationToken cancellationToken = default
    );
}