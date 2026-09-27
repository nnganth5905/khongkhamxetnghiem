using KhamXetNghiem.Api.Data;
using KhamXetNghiem.Api.Entities;
using KhamXetNghiem.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KhamXetNghiem.Api.Repositories.Implementations;

public sealed class SpecialtyRepository
    : ISpecialtyRepository
{
    private readonly AppDbContext _dbContext;

    public SpecialtyRepository(
        AppDbContext dbContext
    )
    {
        _dbContext = dbContext;
    }

    private DbSet<Specialty> Specialties =>
        _dbContext.Set<Specialty>();

    public async Task<List<Specialty>> GetAllAsync(
        string? query,
        bool activeOnly,
        CancellationToken cancellationToken = default
    )
    {
        IQueryable<Specialty> specialties =
            Specialties.AsNoTracking();

        if (activeOnly)
        {
            specialties =
                specialties.Where(
                    x => x.Status == "yes"
                );
        }

        if (
            !string.IsNullOrWhiteSpace(query)
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

            specialties =
                specialties.Where(
                    x =>
                        x.Id.Contains(keyword)
                        ||
                        x.Name.Contains(keyword)
                        ||
                        (
                            x.Description != null
                            &&
                            x.Description.Contains(keyword)
                        )
                );
        }

        return await specialties
            .OrderBy(x => x.Id)
            .ToListAsync(
                cancellationToken
            );
    }

    public Task<Specialty?> FindByIdAsync(
        string id,
        bool tracking,
        CancellationToken cancellationToken = default
    )
    {
        IQueryable<Specialty> query =
            Specialties;

        if (!tracking)
        {
            query =
                query.AsNoTracking();
        }

        return query.FirstOrDefaultAsync(
            x => x.Id == id,
            cancellationToken
        );
    }

    public Task<bool> ExistsByNameAsync(
        string name,
        string? excludeId = null,
        CancellationToken cancellationToken = default
    )
    {
        return Specialties.AnyAsync(
            x =>
                x.Name == name
                &&
                (
                    excludeId == null
                    ||
                    x.Id != excludeId
                ),
            cancellationToken
        );
    }

    public Task<List<string>> GetAllIdsAsync(
        CancellationToken cancellationToken = default
    )
    {
        return Specialties
            .AsNoTracking()
            .Select(x => x.Id)
            .ToListAsync(
                cancellationToken
            );
    }

    public Task AddAsync(
        Specialty specialty,
        CancellationToken cancellationToken = default
    )
    {
        return Specialties
            .AddAsync(
                specialty,
                cancellationToken
            )
            .AsTask();
    }

    public Task SaveChangesAsync(
        CancellationToken cancellationToken = default
    )
    {
        return _dbContext.SaveChangesAsync(
            cancellationToken
        );
    }
}