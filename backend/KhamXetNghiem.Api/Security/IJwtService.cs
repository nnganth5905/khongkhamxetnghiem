using KhamXetNghiem.Api.Enums;

namespace KhamXetNghiem.Api.Security;

public interface IJwtService
{
    string GenerateToken(
        string subject,
        RoleName role
    );

    long ExpirationSeconds { get; }
}
