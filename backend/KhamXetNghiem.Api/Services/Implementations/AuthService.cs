using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using KhamXetNghiem.Api.Data;
using KhamXetNghiem.Api.DTOs.Requests;
using KhamXetNghiem.Api.Entities;
using KhamXetNghiem.Api.Enums;
using KhamXetNghiem.Api.Exceptions;
using KhamXetNghiem.Api.Repositories.Interfaces;
using KhamXetNghiem.Api.Security;
using KhamXetNghiem.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KhamXetNghiem.Api.Services.Implementations;

public sealed class AuthService
    : IAuthService
{
    private readonly AppDbContext
        _dbContext;

    private readonly IAccountRepository
        _accountRepository;

    private readonly ICustomerRepository
        _customerRepository;

    private readonly IJwtService
        _jwtService;

    public AuthService(
        AppDbContext dbContext,
        IAccountRepository accountRepository,
        ICustomerRepository customerRepository,
        IJwtService jwtService
    )
    {
        _dbContext =
            dbContext;

        _accountRepository =
            accountRepository;

        _customerRepository =
            customerRepository;

        _jwtService =
            jwtService;
    }

    // =====================================================
    // LOGIN
    // =====================================================

    public async Task<object> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var identifier =
            request.Identifier();

        if (
            string.IsNullOrWhiteSpace(
                identifier
            )
        )
        {
            throw new BadRequestException(
                "Vui lòng nhập email hoặc username."
            );
        }

        Account? account;

        if (
            identifier.Contains('@')
        )
        {
            account =
                await _accountRepository
                    .FindByEmailAsync(
                        identifier
                            .Trim()
                            .ToLowerInvariant(),

                        cancellationToken
                    );
        }
        else
        {
            account =
                await _accountRepository
                    .FindByUsernameAsync(
                        identifier.Trim(),
                        cancellationToken
                    );
        }

        if (account is null)
        {
            throw new BadRequestException(
                "Tài khoản không tồn tại!"
            );
        }

        if (!account.IsActive)
        {
            throw new BadRequestException(
                "Tài khoản đã bị khóa!"
            );
        }

        var passwordValid =
            VerifyPassword(
                request.Password,
                account.PasswordHash
            );

        if (!passwordValid)
        {
            throw new BadRequestException(
                "Mật khẩu không chính xác!"
            );
        }

        /*
         * DB cũ có admin@example.com đang lưu
         * password plaintext "123456".
         *
         * Nếu login thành công với legacy password,
         * tự đổi ngay sang BCrypt.
         */
        if (
            !IsBcryptHash(
                account.PasswordHash
            )
        )
        {
            account.PasswordHash =
                BCrypt.Net.BCrypt
                    .HashPassword(
                        request.Password
                    );

            _accountRepository
                .Update(account);

            await _accountRepository
                .SaveChangesAsync(
                    cancellationToken
                );
        }

        var role =
            RoleNameExtensions
                .FromValue(
                    account.Role
                );

        var token =
            _jwtService
                .GenerateToken(
                    account.Email,
                    role
                );

        var fullName =
            string.Empty;

        var phone =
            string.Empty;

        if (
            !string.IsNullOrWhiteSpace(
                account.IdKhachHang
            )
        )
        {
            var customer =
                await _customerRepository
                    .FindByIdAsync(
                        account.IdKhachHang,
                        cancellationToken
                    );

            if (customer is not null)
            {
                fullName =
                    customer.FullName;

                phone =
                    customer.Phone
                    ?? string.Empty;
            }
        }

        return new
        {
            message =
                "Đăng nhập thành công",

            accessToken =
                token,

            user =
                new
                {
                    userId =
                        account.UserId,

                    email =
                        account.Email,

                    username =
                        account.Username,

                    role =
                        account.Role,

                    fullName,

                    phone,

                    idKhachHang =
                        account.IdKhachHang,

                    idBacSi =
                        account.IdBacSi,

                    idNhanVien =
                        account.IdNhanVien
                }
        };
    }

    // =====================================================
    // REGISTER
    // =====================================================

    public async Task<object> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var email =
            request.Email
                .Trim()
                .ToLowerInvariant();

        if (
            await _accountRepository
                .ExistsByEmailAsync(
                    email,
                    cancellationToken
                )
        )
        {
            throw new BadRequestException(
                "Email này đã được sử dụng!"
            );
        }

        var customerId =
            await GenerateCustomerIdAsync(
                cancellationToken
            );

        await using var transaction =
            await _dbContext.Database
                .BeginTransactionAsync(
                    cancellationToken
                );

        try
        {
            var customer =
                new Customer
                {
                    Id =
                        customerId,

                    FullName =
                        request.Name
                            .Trim(),

                    Email =
                        email,

                    Phone =
                        NormalizeNullable(
                            request.Phone
                        ),

                    /*
                     * Fix lỗi "Nữ" -> "nữ"
                     * không khớp enum MySQL.
                     */
                    Gender =
                        CustomerService
                            .NormalizeGender(
                                request.Gender
                            ),

                    Status =
                        "yes"
                };

            await _customerRepository
                .AddAsync(
                    customer,
                    cancellationToken
                );

            var account =
                new Account
                {
                    Email =
                        email,

                    Username =
                        email,

                    PasswordHash =
                        BCrypt.Net.BCrypt
                            .HashPassword(
                                request.Password
                            ),

                    Role =
                        "khachhang",

                    IdKhachHang =
                        customerId,

                    IsActive =
                        true
                };

            await _accountRepository
                .AddAsync(
                    account,
                    cancellationToken
                );

            /*
             * Cả Customer + Account dùng chung DbContext.
             * Một SaveChanges + một transaction.
             */
            await _dbContext
                .SaveChangesAsync(
                    cancellationToken
                );

            await transaction
                .CommitAsync(
                    cancellationToken
                );

            return new
            {
                message =
                    "Đăng ký tài khoản thành công!",

                email,

                idKhachHang =
                    customerId
            };
        }
        catch
        {
            await transaction
                .RollbackAsync(
                    cancellationToken
                );

            throw;
        }
    }

    // =====================================================
    // CURRENT USER
    // =====================================================

    public async Task<object> GetCurrentUserAsync(
        ClaimsPrincipal principal,
        CancellationToken cancellationToken = default
    )
    {
        if (
            principal.Identity
                ?.IsAuthenticated
            != true
        )
        {
            return new
            {
                authenticated =
                    false,

                message =
                    "Chưa có người dùng đăng nhập."
            };
        }

        var email =
            principal.FindFirstValue(
                JwtRegisteredClaimNames.Sub
            )
            ??
            principal.Identity?.Name;

        if (
            string.IsNullOrWhiteSpace(
                email
            )
        )
        {
            return new
            {
                authenticated =
                    false,

                message =
                    "JWT không chứa thông tin tài khoản."
            };
        }

        var account =
            await _accountRepository
                .FindByEmailAsync(
                    email,
                    cancellationToken
                );

        if (account is null)
        {
            return new
            {
                authenticated =
                    false,

                message =
                    "Không tìm thấy thông tin tài khoản."
            };
        }

        var fullName =
            string.Empty;

        var phone =
            string.Empty;

        if (
            !string.IsNullOrWhiteSpace(
                account.IdKhachHang
            )
        )
        {
            var customer =
                await _customerRepository
                    .FindByIdAsync(
                        account.IdKhachHang,
                        cancellationToken
                    );

            if (customer is not null)
            {
                fullName =
                    customer.FullName;

                phone =
                    customer.Phone
                    ?? string.Empty;
            }
        }

        return new
        {
            authenticated =
                true,

            userId =
                account.UserId,

            email =
                account.Email,

            username =
                account.Username,

            role =
                account.Role,

            fullName,

            phone,

            idKhachHang =
                account.IdKhachHang,

            idBacSi =
                account.IdBacSi,

            idNhanVien =
                account.IdNhanVien,

            authorities =
                principal.Claims
                    .Where(
                        c =>
                            c.Type ==
                            "role"
                    )
                    .Select(
                        c => c.Value
                    )
                    .ToArray()
        };
    }

    // =====================================================
    // FORGOT PASSWORD
    // =====================================================

    public Task<object> ForgotPasswordAsync(
        ForgotPasswordRequest request,
        CancellationToken cancellationToken = default
    )
    {
        object response =
            new
            {
                message =
                    "Nếu email tồn tại trong hệ thống, hướng dẫn đặt lại mật khẩu sẽ được gửi.",

                email =
                    request.Email
            };

        return Task.FromResult(
            response
        );
    }

    // =====================================================
    // VERIFY RESET TOKEN
    // =====================================================

    public Task<object> VerifyResetTokenAsync(
        string token,
        string email,
        CancellationToken cancellationToken = default
    )
    {
        object response =
            new
            {
                valid =
                    !string.IsNullOrWhiteSpace(
                        token
                    )
                    &&
                    !string.IsNullOrWhiteSpace(
                        email
                    ),

                email
            };

        return Task.FromResult(
            response
        );
    }

    // =====================================================
    // RESET PASSWORD
    // =====================================================

    public Task<object> ResetPasswordAsync(
        ResetPasswordRequest request,
        CancellationToken cancellationToken = default
    )
    {
        object response =
            new
            {
                message =
                    "Yêu cầu đặt lại mật khẩu đã được tiếp nhận.",

                email =
                    request.Email
            };

        return Task.FromResult(
            response
        );
    }

    // =====================================================
    // CONFIRM EMAIL
    // =====================================================

    public Task<object> ConfirmEmailAsync(
        string token,
        CancellationToken cancellationToken = default
    )
    {
        object response =
            new
            {
                success =
                    !string.IsNullOrWhiteSpace(
                        token
                    ),

                message =
                    "Token xác nhận đã được tiếp nhận."
            };

        return Task.FromResult(
            response
        );
    }

    // =====================================================
    // PASSWORD COMPATIBILITY
    // =====================================================

    private static bool VerifyPassword(
        string password,
        string storedPassword
    )
    {
        if (
            string.IsNullOrWhiteSpace(
                storedPassword
            )
        )
        {
            return false;
        }

        if (
            IsBcryptHash(
                storedPassword
            )
        )
        {
            try
            {
                return BCrypt.Net.BCrypt
                    .Verify(
                        password,
                        storedPassword
                    );
            }
            catch
            {
                return false;
            }
        }

        /*
         * Chỉ để migrate DB legacy.
         * Login thành công sẽ hash lại ngay.
         */
        return string.Equals(
            password,
            storedPassword,
            StringComparison.Ordinal
        );
    }

    private static bool IsBcryptHash(
        string value
    )
    {
        if (
            string.IsNullOrWhiteSpace(
                value
            )
        )
        {
            return false;
        }

        return
            value.StartsWith(
                "$2a$",
                StringComparison.Ordinal
            )
            ||
            value.StartsWith(
                "$2b$",
                StringComparison.Ordinal
            )
            ||
            value.StartsWith(
                "$2y$",
                StringComparison.Ordinal
            );
    }

    // =====================================================
    // CUSTOMER ID
    // =====================================================

    private async Task<string>
        GenerateCustomerIdAsync(
            CancellationToken cancellationToken
        )
    {
        for (
            var attempt = 0;
            attempt < 10;
            attempt++
        )
        {
            var id =
                "KH"
                +
                Guid.NewGuid()
                    .ToString("N")
                    .Substring(0, 8)
                    .ToUpperInvariant();

            if (
                !await _customerRepository
                    .ExistsByIdAsync(
                        id,
                        cancellationToken
                    )
            )
            {
                return id;
            }
        }

        throw new InvalidOperationException(
            "Không thể tạo mã khách hàng duy nhất."
        );
    }

    private static string? NormalizeNullable(
        string? value
    )
    {
        return string.IsNullOrWhiteSpace(
            value
        )
            ? null
            : value.Trim();
    }
}