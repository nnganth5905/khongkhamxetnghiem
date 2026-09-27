using KhamXetNghiem.Api.Enums;
using KhamXetNghiem.Api.Repositories.Interfaces;

namespace KhamXetNghiem.Api.Repositories.Implementations;

public sealed class RoleRepository : IRoleRepository
{
    public IReadOnlyList<Role> FindAll()
    {
        return Enum
            .GetValues<Role>()
            .ToArray();
    }

    public Role? FindByName(
        string? value
    )
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return RoleExtensions
            .FromDatabaseValue(value);
    }
}
