using KhamXetNghiem.Api.Entities;

namespace KhamXetNghiem.Api.Repositories.Interfaces;

public interface ICustomerRepository
{
    Task<List<Customer>> GetAllAsync(
        string? query = null,
        CancellationToken cancellationToken = default
    );

    Task<Customer?> FindByIdAsync(
        string id,
        CancellationToken cancellationToken = default
    );

    Task<Customer?> FindTrackedByIdAsync(
        string id,
        CancellationToken cancellationToken = default
    );

    Task<Customer?> FindByEmailAsync(
        string email,
        CancellationToken cancellationToken = default
    );

    Task<Customer?> FindByPhoneAsync(
        string phone,
        CancellationToken cancellationToken = default
    );

    Task<Customer?> FindByAccountIdAsync(
        int accountId,
        CancellationToken cancellationToken = default
    );

    Task<bool> ExistsByIdAsync(
        string id,
        CancellationToken cancellationToken = default
    );

    Task<bool> ExistsByCitizenIdAsync(
        string citizenId,
        string? excludeCustomerId = null,
        CancellationToken cancellationToken = default
    );

    Task AddAsync(
        Customer customer,
        CancellationToken cancellationToken = default
    );

    void Update(
        Customer customer
    );

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default
    );
}