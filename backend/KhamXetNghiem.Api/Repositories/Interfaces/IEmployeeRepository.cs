using KhamXetNghiem.Api.Entities;

namespace KhamXetNghiem.Api.Repositories.Interfaces;

public interface IEmployeeRepository
{
    Task<List<Employee>> GetAllAsync(
        string? query,
        string? position,
        CancellationToken cancellationToken = default
    );

    Task<Employee?> FindByIdAsync(
        string id,
        CancellationToken cancellationToken = default
    );

    Task<Employee?> FindTrackedByIdAsync(
        string id,
        CancellationToken cancellationToken = default
    );

    Task<Employee?> FindByEmailAsync(
        string email,
        CancellationToken cancellationToken = default
    );

    Task<bool> ExistsByIdAsync(
        string id,
        CancellationToken cancellationToken = default
    );

    Task<bool> FacilityExistsAsync(
        string facilityId,
        CancellationToken cancellationToken = default
    );

    Task AddAsync(
        Employee employee,
        CancellationToken cancellationToken = default
    );

    Task DisableLinkedAccountAsync(
        string employeeId,
        CancellationToken cancellationToken = default
    );

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default
    );
}