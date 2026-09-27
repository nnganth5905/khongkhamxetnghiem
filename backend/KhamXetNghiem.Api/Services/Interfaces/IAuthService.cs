using System.Security.Claims;
using KhamXetNghiem.Api.DTOs.Requests;

namespace KhamXetNghiem.Api.Services.Interfaces;

public interface IAuthService
{
    Task<object> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default
    );

    Task<object> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default
    );

    Task<object> GetCurrentUserAsync(
        ClaimsPrincipal principal,
        CancellationToken cancellationToken = default
    );

    Task<object> ForgotPasswordAsync(
        ForgotPasswordRequest request,
        CancellationToken cancellationToken = default
    );

    Task<object> VerifyResetTokenAsync(
        string token,
        string email,
        CancellationToken cancellationToken = default
    );

    Task<object> ResetPasswordAsync(
        ResetPasswordRequest request,
        CancellationToken cancellationToken = default
    );

    Task<object> ConfirmEmailAsync(
        string token,
        CancellationToken cancellationToken = default
    );
}
