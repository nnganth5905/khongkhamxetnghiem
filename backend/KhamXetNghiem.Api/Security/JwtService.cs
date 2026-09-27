using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using KhamXetNghiem.Api.Configuration;
using KhamXetNghiem.Api.Enums;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace KhamXetNghiem.Api.Security;

public sealed class JwtService : IJwtService
{
    private readonly JwtOptions _options;

    public JwtService(
        IOptions<JwtOptions> options
    )
    {
        _options = options.Value;

        if (string.IsNullOrWhiteSpace(_options.Secret))
        {
            throw new InvalidOperationException(
                "JWT secret chưa được cấu hình. Hãy cấu hình Jwt:Secret bằng user-secrets hoặc biến môi trường."
            );
        }

        if (_options.Secret.Length < 32)
        {
            throw new InvalidOperationException(
                "Jwt:Secret phải dài tối thiểu 32 ký tự."
            );
        }
    }

    public long ExpirationSeconds =>
        _options.ExpirationSeconds;

    public string GenerateToken(
        string subject,
        RoleName role
    )
    {
        var now =
            DateTimeOffset.UtcNow;

        var claims =
            new List<Claim>
            {
                new(
                    JwtRegisteredClaimNames.Sub,
                    subject
                ),

                new(
                    "role",
                    role.ToString()
                ),

                new(
                    JwtRegisteredClaimNames.Iat,
                    now.ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64
                )
            };

        var key =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _options.Secret
                )
            );

        var credentials =
            new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

        var token =
            new JwtSecurityToken(
                claims: claims,
                notBefore: now.UtcDateTime,
                expires: now
                    .AddSeconds(
                        _options.ExpirationSeconds
                    )
                    .UtcDateTime,
                signingCredentials: credentials
            );

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}
