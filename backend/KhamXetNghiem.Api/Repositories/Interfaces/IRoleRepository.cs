using KhamXetNghiem.Api.Enums;

namespace KhamXetNghiem.Api.Repositories.Interfaces;

public interface IRoleRepository
{
    IReadOnlyList<Role> FindAll();

    Role? FindByName(string? value);
}
