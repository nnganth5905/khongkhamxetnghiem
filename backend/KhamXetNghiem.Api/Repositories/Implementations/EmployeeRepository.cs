using KhamXetNghiem.Api.Data;
using KhamXetNghiem.Api.Entities;
using KhamXetNghiem.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KhamXetNghiem.Api.Repositories.Implementations;

public sealed class EmployeeRepository
    : IEmployeeRepository
{
    private readonly AppDbContext _dbContext;

    public EmployeeRepository(
        AppDbContext dbContext
    )
    {
        _dbContext = dbContext;
    }

    // =====================================================
    // LIST / SEARCH
    // =====================================================

    public async Task<List<Employee>> GetAllAsync(
        string? query,
        string? position,
        CancellationToken cancellationToken = default
    )
    {
        IQueryable<Employee> employees =
            _dbContext.Employees
                .AsNoTracking();

        if (
            !string.IsNullOrWhiteSpace(
                position
            )
        )
        {
            var normalizedPosition =
                position.Trim();

            employees =
                employees.Where(
                    x =>
                        x.Position ==
                        normalizedPosition
                );
        }

        if (
            !string.IsNullOrWhiteSpace(
                query
            )
            &&
            !string.Equals(
                query,
                "undefined",
                StringComparison.OrdinalIgnoreCase
            )
            &&
            !string.Equals(
                query,
                "null",
                StringComparison.OrdinalIgnoreCase
            )
        )
        {
            var keyword =
                query.Trim();

            employees =
                employees.Where(
                    x =>
                        x.Id.Contains(
                            keyword
                        )
                        ||
                        x.FullName.Contains(
                            keyword
                        )
                        ||
                        x.Position.Contains(
                            keyword
                        )
                        ||
                        (
                            x.Phone != null
                            &&
                            x.Phone.Contains(
                                keyword
                            )
                        )
                        ||
                        (
                            x.Email != null
                            &&
                            x.Email.Contains(
                                keyword
                            )
                        )
                        ||
                        (
                            x.FacilityId != null
                            &&
                            x.FacilityId.Contains(
                                keyword
                            )
                        )
                );
        }

        return await employees
            .OrderByDescending(
                x => x.CreatedAt
            )
            .ThenBy(
                x => x.Id
            )
            .ToListAsync(
                cancellationToken
            );
    }

    // =====================================================
    // DETAIL
    // =====================================================

    public Task<Employee?> FindByIdAsync(
        string id,
        CancellationToken cancellationToken = default
    )
    {
        return _dbContext.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x =>
                    x.Id == id,
                cancellationToken
            );
    }

    public Task<Employee?> FindTrackedByIdAsync(
        string id,
        CancellationToken cancellationToken = default
    )
    {
        return _dbContext.Employees
            .FirstOrDefaultAsync(
                x =>
                    x.Id == id,
                cancellationToken
            );
    }

    public Task<Employee?> FindByEmailAsync(
        string email,
        CancellationToken cancellationToken = default
    )
    {
        return _dbContext.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x =>
                    x.Email == email,
                cancellationToken
            );
    }

    // =====================================================
    // EXISTS
    // =====================================================

    public Task<bool> ExistsByIdAsync(
        string id,
        CancellationToken cancellationToken = default
    )
    {
        return _dbContext.Employees
            .AnyAsync(
                x =>
                    x.Id == id,
                cancellationToken
            );
    }

    public Task<bool> FacilityExistsAsync(
        string facilityId,
        CancellationToken cancellationToken = default
    )
    {
        return _dbContext.Facilities
            .AnyAsync(
                x =>
                    x.Id == facilityId,
                cancellationToken
            );
    }

    // =====================================================
    // CREATE
    // =====================================================

    public async Task AddAsync(
        Employee employee,
        CancellationToken cancellationToken = default
    )
    {
        await _dbContext.Employees
            .AddAsync(
                employee,
                cancellationToken
            );
    }

    // =====================================================
    // ACCOUNT CONSISTENCY
    // =====================================================

    public async Task DisableLinkedAccountAsync(
        string employeeId,
        CancellationToken cancellationToken = default
    )
    {
        var account =
            await _dbContext.Accounts
                .FirstOrDefaultAsync(
                    x =>
                        x.IdNhanVien ==
                        employeeId,
                    cancellationToken
                );

        if (account is null)
        {
            return;
        }

        account.IsActive =
            false;
    }

    // =====================================================
    // SAVE
    // =====================================================

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