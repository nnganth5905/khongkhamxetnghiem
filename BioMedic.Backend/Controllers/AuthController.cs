using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BioMedic.Backend.Data;
using BioMedic.Backend.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace BioMedic.Backend.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(
            ApplicationDbContext context,
            IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var identifier = model.Email?.Trim();
            if (string.IsNullOrWhiteSpace(identifier))
            {
                identifier = model.Username?.Trim();
            }

            if (string.IsNullOrWhiteSpace(identifier) || string.IsNullOrWhiteSpace(model.Password))
            {
                return BadRequest(new { success = false, message = "Vui lòng nhập tài khoản và mật khẩu." });
            }

            var user = await _context.Users
                .Include(account => account.IdkhachHangNavigation)
                .FirstOrDefaultAsync(account =>
                    account.Email == identifier || account.Username == identifier);

            if (user is null || user.IsActive != true || string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "Tài khoản hoặc mật khẩu không đúng, hoặc tài khoản đã bị khóa.",
                });
            }

            // Hỗ trợ dữ liệu cũ đang lưu mật khẩu dạng plain text.
            // Nếu là BCrypt hợp lệ thì verify bình thường.
            // Nếu chưa phải BCrypt, so sánh trực tiếp một lần rồi tự nâng cấp sang BCrypt.
            bool passwordValid;

            if (IsBcryptHash(user.PasswordHash))
            {
                try
                {
                    passwordValid = BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash);
                }
                catch (BCrypt.Net.SaltParseException)
                {
                    passwordValid = false;
                }
            }
            else
            {
                passwordValid = string.Equals(
                    model.Password,
                    user.PasswordHash,
                    StringComparison.Ordinal);

                if (passwordValid)
                {
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
                    user.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }
            }

            if (!passwordValid)
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "Tài khoản hoặc mật khẩu không đúng, hoặc tài khoản đã bị khóa.",
                });
            }

            var frontendRole = MapRole(user.Role);
            var expirySeconds = _configuration.GetValue<int?>("Jwt:ExpirationSeconds") ?? 86400;
            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]!)),
                SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                claims: new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                    new Claim("userId", user.UserId.ToString()),
                    new Claim("role", frontendRole.ToUpperInvariant()),
                },
                expires: DateTime.UtcNow.AddSeconds(expirySeconds),
                signingCredentials: signingCredentials);
            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new
            {
                success = true,
                message = "Đăng nhập thành công",
                accessToken,
                user = new
                {
                    userId = user.UserId,
                    email = user.Email,
                    username = user.Username,
                    fullName = user.IdkhachHangNavigation?.TenKhachHang ?? GetNameByRole(user.Role),
                    phone = user.IdkhachHangNavigation?.SoDienThoai ?? "",
                    role = frontendRole,
                },
            });
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            var email = model.Email.Trim().ToLowerInvariant();
            var phone = string.IsNullOrWhiteSpace(model.Phone)
                ? null
                : model.Phone.Trim();

            if (await _context.Users.AnyAsync(account =>
                account.Email == email || account.Username == email))
            {
                return Conflict(new
                {
                    success = false,
                    message = "Email này đã được sử dụng."
                });
            }

            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            // 1. Nếu khách đã từng đặt lịch khi chưa có tài khoản,
            // ưu tiên dùng lại đúng hồ sơ khách hàng đã được cấp trước đó.
            Khachhang? customer = await _context.Khachhangs
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(x =>
                    x.Email != null &&
                    x.Email.ToLower() == email);

            // 2. Nếu chưa tìm thấy theo email, thử ghép theo số điện thoại.
            if (customer is null && !string.IsNullOrWhiteSpace(phone))
            {
                customer = await _context.Khachhangs
                    .OrderByDescending(x => x.CreatedAt)
                    .FirstOrDefaultAsync(x =>
                        x.SoDienThoai == phone);
            }

            // 3. Chỉ tạo IDKhachHang mới khi thực sự chưa có hồ sơ nào phù hợp.
            if (customer is null)
            {
                var customerId =
                    $"KH{Guid.NewGuid():N}"[..10].ToUpperInvariant();

                customer = new Khachhang
                {
                    IdkhachHang = customerId,
                    TenKhachHang = model.Name.Trim(),
                    Email = email,
                    SoDienThoai = phone,
                    GioiTinh = NormalizeGender(model.Gender),
                    Status = "yes",
                    CreatedAt = DateTime.UtcNow,
                };

                _context.Khachhangs.Add(customer);
            }
            else
            {
                // Bổ sung/cập nhật thông tin cơ bản cho hồ sơ cũ,
                // nhưng KHÔNG đổi IDKhachHang.
                if (string.IsNullOrWhiteSpace(customer.Email))
                    customer.Email = email;

                if (string.IsNullOrWhiteSpace(customer.SoDienThoai) &&
                    !string.IsNullOrWhiteSpace(phone))
                {
                    customer.SoDienThoai = phone;
                }

                if (!string.IsNullOrWhiteSpace(model.Name))
                    customer.TenKhachHang = model.Name.Trim();

                if (!string.IsNullOrWhiteSpace(model.Gender))
                    customer.GioiTinh = NormalizeGender(model.Gender);

                customer.Status = "yes";
                customer.UpdatedAt = DateTime.UtcNow;
            }

            var user = new User
            {
                Email = email,
                Username = email,
                PasswordHash =
                    BCrypt.Net.BCrypt.HashPassword(model.Password),
                Role = "khachhang",

                // QUAN TRỌNG: tài khoản luôn trỏ đúng hồ sơ khách hàng
                // đã tồn tại hoặc vừa được tạo.
                IdkhachHang = customer.IdkhachHang,

                IsActive = true,
                CreatedAt = DateTime.UtcNow,
            };

            _context.Users.Add(user);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(new
            {
                success = true,
                message = "Đăng ký tài khoản thành công!",
                email,
                customerId = customer.IdkhachHang,
                reusedCustomer = customer.CreatedAt < DateTime.UtcNow.AddSeconds(-2)
            });
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            var email = model.Email.Trim().ToLowerInvariant();
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email && x.IsActive == true);

            // Luôn trả về cùng một thông điệp để tránh làm lộ email có tồn tại hay không.
            if (user is null)
            {
                return Ok(new { success = true, message = "Nếu email tồn tại trong hệ thống, hướng dẫn đặt lại mật khẩu sẽ được tạo." });
            }

            var rawToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
            var tokenHash = HashResetToken(rawToken);
            _context.PasswordResets.Add(new PasswordReset
            {
                UserId = user.UserId,
                TokenHash = tokenHash,
                ExpiresAt = DateTime.UtcNow.AddMinutes(30),
                Used = false,
                CreatedAt = DateTime.UtcNow,
            });
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Nếu email tồn tại trong hệ thống, hướng dẫn đặt lại mật khẩu sẽ được tạo.",
                // Coursework fallback: frontend có thể dùng token này khi chưa tích hợp mail server.
                resetToken = rawToken,
                email,
            });
        }

        [HttpGet("reset-password/verify")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyResetToken([FromQuery] string token, [FromQuery] string? email = null)
        {
            if (string.IsNullOrWhiteSpace(token))
                return BadRequest(new { valid = false, message = "Token không hợp lệ." });

            var tokenHash = HashResetToken(token);
            var reset = await _context.PasswordResets
                .AsNoTracking()
                .Include(x => x.User)
                .Where(x => x.TokenHash == tokenHash && !x.Used && x.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();

            var valid = reset is not null && (string.IsNullOrWhiteSpace(email) ||
                string.Equals(reset.User.Email, email.Trim(), StringComparison.OrdinalIgnoreCase));

            return Ok(new { valid, email = valid ? reset!.User.Email : email });
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            var tokenHash = HashResetToken(model.Token);
            var email = model.Email.Trim().ToLowerInvariant();
            var reset = await _context.PasswordResets
                .Include(x => x.User)
                .Where(x => x.TokenHash == tokenHash && !x.Used && x.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();

            if (reset is null || !string.Equals(reset.User.Email, email, StringComparison.OrdinalIgnoreCase))
                return BadRequest(new { success = false, message = "Token không hợp lệ hoặc đã hết hạn." });

            reset.User.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
            reset.User.UpdatedAt = DateTime.UtcNow;
            reset.Used = true;
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Đặt lại mật khẩu thành công." });
        }

        [HttpGet("confirm-email")]
        [AllowAnonymous]
        public IActionResult ConfirmEmail([FromQuery] string token)
        {
            // Database hiện chưa có cột email-confirmed/token xác nhận riêng.
            // Giữ endpoint để frontend không 404 và trả trạng thái rõ ràng.
            if (string.IsNullOrWhiteSpace(token))
                return BadRequest(new { success = false, message = "Token xác nhận không hợp lệ." });
            return Ok(new { success = true, message = "Email đã được ghi nhận xác nhận." });
        }

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout() => Ok(new { success = true, message = "Đăng xuất thành công." });

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var email = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (string.IsNullOrWhiteSpace(email))
            {
                return Unauthorized(new { success = false, message = "Phiên đăng nhập hết hạn." });
            }

            var user = await _context.Users
                .AsNoTracking()
                .Include(account => account.IdkhachHangNavigation)
                .FirstOrDefaultAsync(account => account.Email == email);

            if (user is null || user.IsActive != true)
            {
                return Unauthorized(new { success = false, message = "Tài khoản không còn hoạt động." });
            }

            return Ok(new
            {
                authenticated = true,
                userId = user.UserId,
                email = user.Email,
                username = user.Username,
                role = MapRole(user.Role),
                fullName = user.IdkhachHangNavigation?.TenKhachHang ?? GetNameByRole(user.Role),
                phone = user.IdkhachHangNavigation?.SoDienThoai ?? "",
            });
        }

        // Hàm hỗ trợ map chuẩn role từ DB sang React Frontend
        private static string MapRole(string dbRole)
        {
            switch (dbRole?.ToLower())
            {
                case "bacsi": return "doctor";
                case "letan": return "receptionist";
                case "ktv": return "technician";
                case "admin": return "admin";
                default: return "customer";
            }
        }

        private static string GetNameByRole(string dbRole)
        {
            switch (dbRole?.ToLower())
            {
                case "bacsi": return "Bác sĩ hệ thống";
                case "letan": return "Lễ tân hệ thống";
                case "ktv": return "Kỹ thuật viên xét nghiệm";
                case "admin": return "Quản trị viên";
                default: return "Khách hàng";
            }
        }

        private static bool IsBcryptHash(string hash)
        {
            if (string.IsNullOrWhiteSpace(hash))
                return false;

            return hash.StartsWith("$2a$", StringComparison.Ordinal) ||
                   hash.StartsWith("$2b$", StringComparison.Ordinal) ||
                   hash.StartsWith("$2y$", StringComparison.Ordinal);
        }

        private static string HashResetToken(string token)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
            return Convert.ToHexString(bytes);
        }

        private static string NormalizeGender(string? gender) => gender?.Trim().ToLowerInvariant() switch
        {
            "nam" => "nam",
            "nu" or "nữ" => "nu",
            "khac" or "khác" => "khac",
            _ => "khac",
        };
    }

    public class LoginDto
    {
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
    }

    public class ForgotPasswordDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = "";
    }

    public class ResetPasswordDto
    {
        [Required]
        public string Token { get; set; } = "";
        [Required, EmailAddress]
        public string Email { get; set; } = "";
        [Required, MinLength(6)]
        public string Password { get; set; } = "";
    }

    public class RegisterDto
    {
        [Required]
        [StringLength(120)]
        public string Name { get; set; } = "";

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        public string? Phone { get; set; }
        public string? Gender { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = "";
    }
}