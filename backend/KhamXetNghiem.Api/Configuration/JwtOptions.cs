namespace KhamXetNghiem.Api.Configuration;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Secret { get; set; } = string.Empty;

    public long ExpirationSeconds { get; set; } = 86400;
}
