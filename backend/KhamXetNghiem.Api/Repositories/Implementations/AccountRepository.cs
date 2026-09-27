using KhamXetNghiem.Api.Data;
using KhamXetNghiem.Api.Entities;
using KhamXetNghiem.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KhamXetNghiem.Api.Repositories.Implementations;

public sealed class AccountRepository
    : IAccountRepository
{
    private readonly AppDbContext
        _dbContext;

    public AccountRepository(
        AppDbContext dbContext
    )
    {
        _dbContext =
            dbContext;
    }

    public Task<Account?> FindByIdAsync(
        int userId,
        CancellationToken cancellationToken = default
    )
    {
        return _dbContext.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x =>
                    x.UserId == userId,
                cancellationToken
            );
    }

    public Task<Account?> FindByUsernameAsync(
        string username,
        CancellationToken cancellationToken = default
    )
    {
        return _dbContext.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x =>
                    x.Username == username,
                cancellationToken
            );
    }

    public Task<Account?> FindByEmailAsync(
        string email,
        CancellationToken cancellationToken = default
    )
    {
        return _dbContext.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x =>
                    x.Email == email,
                cancellationToken
            );
    }

    public Task<Account?> FindByIdBacSiAsync(
        int idBacSi,
        CancellationToken cancellationToken = default
    )
    {
        return _dbContext.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x =>
                    x.IdBacSi ==
                    idBacSi,
                cancellationToken
            );
    }

    public Task<bool> ExistsByUsernameAsync(
        string username,
        CancellationToken cancellationToken = default
    )
    {
        return _dbContext.Accounts
            .AnyAsync(
                x =>
                    x.Username ==
                    username,
                cancellationToken
            );
    }

    public Task<bool> ExistsByEmailAsync(
        string email,
        CancellationToken cancellationToken = default
    )
    {
        return _dbContext.Accounts
            .AnyAsync(
                x =>
                    x.Email ==
                    email,
                cancellationToken
            );
    }

    public async Task AddAsync(
        Account account,
        CancellationToken cancellationToken = default
    )
    {
        await _dbContext.Accounts
            .AddAsync(
                account,
                cancellationToken
            );
    }

    public void Update(
        Account account
    )
    {
        _dbContext.Accounts
            .Update(account);
    }

    public Task SaveChangesAsync(
        CancellationToken cancellationToken = default
    )
    {
        return _dbContext
            .SaveChangesAsync(
                cancellationToken
            );
    }
}