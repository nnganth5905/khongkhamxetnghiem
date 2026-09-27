using KhamXetNghiem.Api.DTOs.Requests;
using KhamXetNghiem.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KhamXetNghiem.Api.Controllers;

[ApiController]
[Route("api")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(
        IAuthService authService
    )
    {
        _authService =
            authService;
    }

    // =====================================================
    // LOGIN
    // =====================================================

    [AllowAnonymous]
    [HttpPost("auth/login")]
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken
    )
    {
        var response =
            await _authService
                .LoginAsync(
                    request,
                    cancellationToken
                );

        return Ok(response);
    }

    // =====================================================
    // REGISTER
    // =====================================================

    [AllowAnonymous]
    [HttpPost("auth/register")]
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken
    )
    {
        var response =
            await _authService
                .RegisterAsync(
                    request,
                    cancellationToken
                );

        return Ok(response);
    }

    // =====================================================
    // CURRENT USER
    // =====================================================

    [Authorize]
    [HttpGet("auth/me")]
    public async Task<IActionResult> GetCurrentUser(
        CancellationToken cancellationToken
    )
    {
        var response =
            await _authService
                .GetCurrentUserAsync(
                    User,
                    cancellationToken
                );

        return Ok(response);
    }

    // =====================================================
    // LOGOUT
    // =====================================================

    [AllowAnonymous]
    [HttpPost("auth/logout")]
    public IActionResult Logout()
    {
        /*
         * JWT stateless:
         * backend không giữ session.
         * Frontend xóa token là đủ,
         * trừ khi sau này thêm blacklist/refresh-token.
         */
        return Ok(
            new
            {
                message =
                    "Đăng xuất thành công."
            }
        );
    }

    // =====================================================
    // FORGOT PASSWORD
    // =====================================================

    [AllowAnonymous]
    [HttpPost("auth/forgot-password")]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordRequest request,
        CancellationToken cancellationToken
    )
    {
        var response =
            await _authService
                .ForgotPasswordAsync(
                    request,
                    cancellationToken
                );

        return Ok(response);
    }

    // =====================================================
    // VERIFY RESET TOKEN
    // =====================================================

    [AllowAnonymous]
    [HttpGet("auth/reset-password/verify")]
    [HttpGet("auth/verify-reset-token")]
    [HttpGet("verify-reset-token")]
    public async Task<IActionResult> VerifyResetToken(
        [FromQuery] string token,
        [FromQuery] string email,
        CancellationToken cancellationToken
    )
    {
        var response =
            await _authService
                .VerifyResetTokenAsync(
                    token,
                    email,
                    cancellationToken
                );

        return Ok(response);
    }

    // =====================================================
    // RESET PASSWORD
    // =====================================================

    [AllowAnonymous]
    [HttpPost("auth/reset-password")]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordRequest request,
        CancellationToken cancellationToken
    )
    {
        var response =
            await _authService
                .ResetPasswordAsync(
                    request,
                    cancellationToken
                );

        return Ok(response);
    }

    // =====================================================
    // CONFIRM EMAIL
    // =====================================================

    [AllowAnonymous]
    [HttpGet("auth/confirm-email")]
    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(
        [FromQuery] string token,
        CancellationToken cancellationToken
    )
    {
        var response =
            await _authService
                .ConfirmEmailAsync(
                    token,
                    cancellationToken
                );

        return Ok(response);
    }
}
