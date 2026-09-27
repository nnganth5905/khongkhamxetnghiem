using System.ComponentModel.DataAnnotations;

namespace KhamXetNghiem.Api.DTOs.Requests;

public sealed class LoginRequest
{
    public string? Login { get; set; }

    public string? Email { get; set; }

    public string? Username { get; set; }

    [Required(ErrorMessage = "Mật khẩu không được để trống.")]
    public string Password { get; set; } = string.Empty;

    public string Identifier()
    {
        if (!string.IsNullOrWhiteSpace(Login))
        {
            return Login.Trim();
        }

        if (!string.IsNullOrWhiteSpace(Email))
        {
            return Email.Trim();
        }

        if (!string.IsNullOrWhiteSpace(Username))
        {
            return Username.Trim();
        }

        return string.Empty;
    }
}
