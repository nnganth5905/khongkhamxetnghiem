using KhamXetNghiem.Api.Entities;

namespace KhamXetNghiem.Api.Repositories.Interfaces;

public interface IAccountRepository
{
    Task<Account?> FindByIdAsync(
        int userId,
        CancellationToken cancellationToken = default
    );

    Task<Account?> FindByUsernameAsync(
        string username,
        CancellationToken cancellationToken = default
    );

    Task<Account?> FindByEmailAsync(
        string email,
        CancellationToken cancellationToken = default
    );

    Task<Account?> FindByIdBacSiAsync(
        int idBacSi,
        CancellationToken cancellationToken = default
    );

    Task<bool> ExistsByUsernameAsync(
        string username,
        CancellationToken cancellationToken = default
    );

    Task<bool> ExistsByEmailAsync(
        string email,
        CancellationToken cancellationToken = default
    );

    Task AddAsync(
        Account account,
        CancellationToken cancellationToken = default
    );

    void Update(
        Account account
    );

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default
    );
}