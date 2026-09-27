namespace KhamXetNghiem.Api.DTOs.Responses;

public sealed record AuthResponse(
    string AccessToken,
    string TokenType,
    long ExpiresIn,
    UserResponse User
)
{
    public AuthResponse(
        string accessToken,
        long expiresIn,
        UserResponse user
    ) : this(
        accessToken,
        "Bearer",
        expiresIn,
        user
    )
    {
    }
}
