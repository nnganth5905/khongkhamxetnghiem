using KhamXetNghiem.Api.Services.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KhamXetNghiem.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize]
public sealed class DashboardController
    : ControllerBase
{
    private readonly IDashboardService
        _dashboardService;

    public DashboardController(
        IDashboardService dashboardService
    )
    {
        _dashboardService =
            dashboardService;
    }

    // =====================================================
    // RECEPTIONIST
    // =====================================================

    [Authorize(Roles = "RECEPTIONIST")]
    [HttpGet("receptionist")]
    public async Task<IActionResult>
        GetReceptionistDashboard(
            CancellationToken cancellationToken
        )
    {
        return Ok(
            await _dashboardService
                .GetReceptionistAsync(
                    cancellationToken
                )
        );
    }

    // =====================================================
    // DOCTOR
    // =====================================================

    [Authorize(Roles = "DOCTOR")]
    [HttpGet("doctor")]
    public async Task<IActionResult>
        GetDoctorDashboard(
            CancellationToken cancellationToken
        )
    {
        return Ok(
            await _dashboardService
                .GetDoctorAsync(
                    User,
                    cancellationToken
                )
        );
    }

    // =====================================================
    // TECHNICIAN
    // =====================================================

    [Authorize(Roles = "TECHNICIAN")]
    [HttpGet("technician")]
    public async Task<IActionResult>
        GetTechnicianDashboard(
            CancellationToken cancellationToken
        )
    {
        return Ok(
            await _dashboardService
                .GetTechnicianAsync(
                    User,
                    cancellationToken
                )
        );
    }

    // =====================================================
    // ADMIN
    // =====================================================

    [Authorize(Roles = "ADMIN")]
    [HttpGet("admin")]
    public async Task<IActionResult>
        GetAdminDashboard(
            CancellationToken cancellationToken
        )
    {
        return Ok(
            await _dashboardService
                .GetAdminAsync(
                    cancellationToken
                )
        );
    }

    // =====================================================
    // CUSTOMER
    // =====================================================

    [Authorize(Roles = "CUSTOMER")]
    [HttpGet("customer")]
    public async Task<IActionResult>
        GetCustomerDashboard(
            CancellationToken cancellationToken
        )
    {
        return Ok(
            await _dashboardService
                .GetCustomerAsync(
                    User,
                    cancellationToken
                )
        );
    }
}