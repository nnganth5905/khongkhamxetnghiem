using System.IdentityModel.Tokens.Jwt;
using System.Text;
using KhamXetNghiem.Api.Configuration;
using KhamXetNghiem.Api.Data;
using KhamXetNghiem.Api.DTOs.Responses;
using KhamXetNghiem.Api.Middleware;
using KhamXetNghiem.Api.Repositories.Implementations;
using KhamXetNghiem.Api.Repositories.Interfaces;
using KhamXetNghiem.Api.Security;
using KhamXetNghiem.Api.Services.Implementations;
using KhamXetNghiem.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder =
    WebApplication.CreateBuilder(
        args
    );

// =====================================================
// CONTROLLERS / JSON
// =====================================================

builder.Services
    .AddControllers()
    .AddJsonOptions(
        options =>
        {
            options
                .JsonSerializerOptions
                .PropertyNamingPolicy =
                    System.Text.Json
                        .JsonNamingPolicy
                        .CamelCase;
        }
    );

// =====================================================
// VALIDATION RESPONSE
// =====================================================

builder.Services
    .Configure<ApiBehaviorOptions>(
        options =>
        {
            options.InvalidModelStateResponseFactory =
                context =>
                {
                    var errors =
                        context.ModelState
                            .Where(
                                x =>
                                    x.Value
                                        ?.Errors
                                        .Count
                                    > 0
                            )
                            .ToDictionary(
                                x => x.Key,

                                x =>
                                    x.Value!
                                        .Errors
                                        .First()
                                        .ErrorMessage
                            );

                    var result =
                        new ApiResponse<
                            Dictionary<
                                string,
                                string
                            >
                        >(
                            false,

                            "Dữ liệu gửi lên không hợp lệ.",

                            errors,

                            DateTime.Now
                        );

                    return new
                        BadRequestObjectResult(
                            result
                        );
                };
        }
    );

// =====================================================
// DATABASE
// =====================================================

var connectionString =
    builder.Configuration
        .GetConnectionString(
            "DefaultConnection"
        );

if (
    string.IsNullOrWhiteSpace(
        connectionString
    )
)
{
    var host =
        Environment
            .GetEnvironmentVariable(
                "DB_HOST"
            )
        ?? "localhost";

    var port =
        Environment
            .GetEnvironmentVariable(
                "DB_PORT"
            )
        ?? "3306";

    var database =
        Environment
            .GetEnvironmentVariable(
                "DB_DATABASE"
            )
        ?? "phongkham_xetnghiem";

    var username =
        Environment
            .GetEnvironmentVariable(
                "DB_USERNAME"
            )
        ?? "root";

    var password =
        Environment
            .GetEnvironmentVariable(
                "DB_PASSWORD"
            )
        ?? string.Empty;

    connectionString =
        $"Server={host};"
        +
        $"Port={port};"
        +
        $"Database={database};"
        +
        $"User={username};"
        +
        $"Password={password};"
        +
        "AllowPublicKeyRetrieval=True;"
        +
        "SslMode=None;"
        +
        "CharSet=utf8mb4;";
}

builder.Services
    .AddDbContext<AppDbContext>(
        options =>
            options.UseMySQL(
                connectionString
            )
    );

// =====================================================
// CORS
// =====================================================

builder.Services
    .AddCors(
        options =>
        {
            options.AddPolicy(
                "Frontend",

                policy =>
                {
                    policy
                        .WithOrigins(
                            "http://localhost:5173",
                            "http://127.0.0.1:5173"
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                        .WithExposedHeaders(
                            "Authorization",
                            "Content-Disposition"
                        );
                }
            );
        }
    );

// =====================================================
// JWT OPTIONS
// =====================================================

builder.Services
    .Configure<JwtOptions>(
        builder.Configuration
            .GetSection(
                JwtOptions.SectionName
            )
    );

var jwtSecret =
    builder.Configuration[
        "Jwt:Secret"
    ];

if (
    string.IsNullOrWhiteSpace(
        jwtSecret
    )
)
{
    jwtSecret =
        Environment
            .GetEnvironmentVariable(
                "JWT_SECRET"
            );
}

if (
    string.IsNullOrWhiteSpace(
        jwtSecret
    )
)
{
    throw new InvalidOperationException(
        "Thiếu JWT_SECRET."
    );
}

if (
    jwtSecret.Length < 32
)
{
    throw new InvalidOperationException(
        "JWT_SECRET phải dài tối thiểu 32 ký tự."
    );
}

builder.Services
    .PostConfigure<JwtOptions>(
        options =>
        {
            options.Secret =
                jwtSecret;

            var expirationString =
                Environment
                    .GetEnvironmentVariable(
                        "JWT_EXPIRATION_SECONDS"
                    );

            if (
                long.TryParse(
                    expirationString,
                    out var expiration
                )
            )
            {
                options.ExpirationSeconds =
                    expiration;
            }
        }
    );

// =====================================================
// JWT AUTHENTICATION
// =====================================================

builder.Services
    .AddAuthentication(
        JwtBearerDefaults
            .AuthenticationScheme
    )
    .AddJwtBearer(
        options =>
        {
            options.MapInboundClaims =
                false;

            options.TokenValidationParameters =
                new TokenValidationParameters
                {
                    ValidateIssuer =
                        false,

                    ValidateAudience =
                        false,

                    ValidateIssuerSigningKey =
                        true,

                    ValidateLifetime =
                        true,

                    IssuerSigningKey =
                        new SymmetricSecurityKey(
                            Encoding.UTF8
                                .GetBytes(
                                    jwtSecret
                                )
                        ),

                    ClockSkew =
                        TimeSpan.Zero,

                    NameClaimType =
                        JwtRegisteredClaimNames
                            .Sub,

                    RoleClaimType =
                        "role"
                };
        }
    );

builder.Services
    .AddAuthorization();

// =====================================================
// REPOSITORIES
// =====================================================

builder.Services
    .AddScoped<
        IAccountRepository,
        AccountRepository
    >();

builder.Services
    .AddSingleton<
        IRoleRepository,
        RoleRepository
    >();

builder.Services
    .AddScoped<
        ICustomerRepository,
        CustomerRepository
    >();

// =====================================================
// SECURITY
// =====================================================

builder.Services
    .AddScoped<
        IJwtService,
        JwtService
    >();

// =====================================================
// SERVICES
// =====================================================

builder.Services
    .AddScoped<
        IAuthService,
        AuthService
    >();

builder.Services
    .AddScoped<
        ICustomerService,
        CustomerService
    >();

// =====================================================
// BUILD
// =====================================================

var app =
    builder.Build();

// =====================================================
// MIDDLEWARE
// =====================================================

app.UseMiddleware<
    ExceptionHandlingMiddleware
>();

app.UseCors(
    "Frontend"
);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

// =====================================================
// HEALTH
// =====================================================

app.MapGet(
    "/api/health",

    () =>
        Results.Ok(
            new
            {
                status =
                    "ok",

                service =
                    "KhamXetNghiem.Api",

                time =
                    DateTimeOffset.UtcNow
            }
        )
);

// =====================================================
// RUN
// =====================================================

app.Run();