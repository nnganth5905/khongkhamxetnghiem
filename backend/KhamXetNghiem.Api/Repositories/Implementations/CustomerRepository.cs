using KhamXetNghiem.Api.Data;
using KhamXetNghiem.Api.Entities;
using KhamXetNghiem.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KhamXetNghiem.Api.Repositories.Implementations;

public sealed class CustomerRepository
    : ICustomerRepository
{
    private readonly AppDbContext _dbContext;

    public CustomerRepository(
        AppDbContext dbContext
    )
    {
        _dbContext =
            dbContext;
    }

    public async Task<List<Customer>> GetAllAsync(
        string? query = null,
        CancellationToken cancellationToken = default
    )
    {
        IQueryable<Customer> customers =
            _dbContext.Customers
                .AsNoTracking();

        var keyword =
            query?.Trim();

        if (
            !string.IsNullOrWhiteSpace(
                keyword
            )
            &&
            !string.Equals(
                keyword,
                "undefined",
                StringComparison.OrdinalIgnoreCase
            )
            &&
            !string.Equals(
                keyword,
                "null",
                StringComparison.OrdinalIgnoreCase
            )
        )
        {
            customers =
                customers.Where(
                    c =>
                        c.Id.Contains(keyword)
                        ||
                        c.FullName.Contains(keyword)
                        ||
                        (
                            c.Phone != null
                            &&
                            c.Phone.Contains(keyword)
                        )
                        ||
                        (
                            c.CitizenId != null
                            &&
                            c.CitizenId.Contains(keyword)
                        )
                        ||
                        (
                            c.Email != null
                            &&
                            c.Email.Contains(keyword)
                        )
                );
        }

        return await customers
            .OrderByDescending(
                c => c.CreatedAt
            )
            .ThenBy(
                c => c.Id
            )
            .ToListAsync(
                cancellationToken
            );
    }

    public Task<Customer?> FindByIdAsync(
        string id,
        CancellationToken cancellationToken = default
    )
    {
        return _dbContext.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(
                c => c.Id == id,
                cancellationToken
            );
    }

    public Task<Customer?> FindTrackedByIdAsync(
        string id,
        CancellationToken cancellationToken = default
    )
    {
        return _dbContext.Customers
            .FirstOrDefaultAsync(
                c => c.Id == id,
                cancellationToken
            );
    }

    public Task<Customer?> FindByEmailAsync(
        string email,
        CancellationToken cancellationToken = default
    )
    {
        return _dbContext.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(
                c =>
                    c.Email == email,
                cancellationToken
            );
    }

    public Task<Customer?> FindByPhoneAsync(
        string phone,
        CancellationToken cancellationToken = default
    )
    {
        return _dbContext.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(
                c =>
                    c.Phone == phone,
                cancellationToken
            );
    }

    public async Task<Customer?> FindByAccountIdAsync(
        int accountId,
        CancellationToken cancellationToken = default
    )
    {
        return await (
            from customer
                in _dbContext.Customers.AsNoTracking()

            join account
                in _dbContext.Accounts.AsNoTracking()

                on customer.Id
                equals account.IdKhachHang

            where
                account.UserId == accountId

            select customer
        )
        .FirstOrDefaultAsync(
            cancellationToken
        );
    }

    public Task<bool> ExistsByIdAsync(
        string id,
        CancellationToken cancellationToken = default
    )
    {
        return _dbContext.Customers
            .AnyAsync(
                c => c.Id == id,
                cancellationToken
            );
    }

    public Task<bool> ExistsByCitizenIdAsync(
        string citizenId,
        string? excludeCustomerId = null,
        CancellationToken cancellationToken = default
    )
    {
        IQueryable<Customer> query =
            _dbContext.Customers;

        query =
            query.Where(
                c =>
                    c.CitizenId ==
                    citizenId
            );

        if (
            !string.IsNullOrWhiteSpace(
                excludeCustomerId
            )
        )
        {
            query =
                query.Where(
                    c =>
                        c.Id !=
                        excludeCustomerId
                );
        }

        return query.AnyAsync(
            cancellationToken
        );
    }

    public async Task AddAsync(
        Customer customer,
        CancellationToken cancellationToken = default
    )
    {
        await _dbContext.Customers
            .AddAsync(
                customer,
                cancellationToken
            );
    }

    public void Update(
        Customer customer
    )
    {
        _dbContext.Customers
            .Update(customer);
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