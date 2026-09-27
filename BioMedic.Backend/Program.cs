using System.IdentityModel.Tokens.Jwt;
using System.Text;
using BioMedic.Backend.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.ParameterLocation.Header,
        Description = "Nhập JWT token"
    });

    options.AddSecurityRequirement(document => new Microsoft.OpenApi.OpenApiSecurityRequirement
    {
        [new Microsoft.OpenApi.OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

// Database
var connectionString = builder.Configuration.GetConnectionString("BioMedic")
    ?? Environment.GetEnvironmentVariable("BIOMEDIC_DB_CONNECTION_STRING");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Configure ConnectionStrings:BioMedic or BIOMEDIC_DB_CONNECTION_STRING.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)));

// JWT
var jwtSecret = builder.Configuration["Jwt:Secret"]
    ?? Environment.GetEnvironmentVariable("BIOMEDIC_JWT_SECRET");

if (string.IsNullOrWhiteSpace(jwtSecret) || Encoding.UTF8.GetByteCount(jwtSecret) < 32)
{
    throw new InvalidOperationException(
        "Configure Jwt:Secret or BIOMEDIC_JWT_SECRET with at least 32 bytes.");
}

builder.Configuration["Jwt:Secret"] = jwtSecret;

var signingKey = new SymmetricSecurityKey(
    Encoding.UTF8.GetBytes(jwtSecret));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1),
            NameClaimType = JwtRegisteredClaimNames.Sub,
            RoleClaimType = "role"
        };
    });

builder.Services.AddAuthorization();

// CORS for React
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "http://127.0.0.1:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .WithExposedHeaders(
                "Authorization",
                "Content-Disposition");
    });
});

var app = builder.Build();

// Swagger in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
