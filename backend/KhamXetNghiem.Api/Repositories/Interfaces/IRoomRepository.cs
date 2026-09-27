using KhamXetNghiem.Api.DTOs.Responses;
using KhamXetNghiem.Api.Entities;

namespace KhamXetNghiem.Api.Repositories.Interfaces;

public interface IRoomRepository
{
    Task<List<RoomResponse>> GetAllAsync(
        string? query,
        bool activeOnly,
        CancellationToken cancellationToken = default
    );

    Task<Room?> FindByIdAsync(
        string id,
        bool tracking,
        CancellationToken cancellationToken = default
    );

    Task<RoomResponse?> GetResponseByIdAsync(
        string id,
        CancellationToken cancellationToken = default
    );

    Task<List<string>> GetAllIdsAsync(
        CancellationToken cancellationToken = default
    );

    Task<bool> FacilityExistsAsync(
        string facilityId,
        CancellationToken cancellationToken = default
    );

    Task<bool> SpecialtyExistsAsync(
        string specialtyId,
        CancellationToken cancellationToken = default
    );

    Task AddAsync(
        Room room,
        CancellationToken cancellationToken = default
    );

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default
    );
}