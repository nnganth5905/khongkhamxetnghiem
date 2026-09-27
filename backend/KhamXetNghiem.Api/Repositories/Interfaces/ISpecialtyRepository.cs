using KhamXetNghiem.Api.Entities;

namespace KhamXetNghiem.Api.Repositories.Interfaces;

public interface ISpecialtyRepository
{
    Task<List<Specialty>> GetAllAsync(
        string? query,
        bool activeOnly,
        CancellationToken cancellationToken = default
    );

    Task<Specialty?> FindByIdAsync(
        string id,
        bool tracking,
        CancellationToken cancellationToken = default
    );

    Task<bool> ExistsByNameAsync(
        string name,
        string? excludeId = null,
        CancellationToken cancellationToken = default
    );

    Task<List<string>> GetAllIdsAsync(
        CancellationToken cancellationToken = default
    );

    Task AddAsync(
        Specialty specialty,
        CancellationToken cancellationToken = default
    );

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default
    );
}