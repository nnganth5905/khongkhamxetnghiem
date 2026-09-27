using KhamXetNghiem.Api.DTOs.Requests;
using KhamXetNghiem.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KhamXetNghiem.Api.Controllers;

[ApiController]
[Route("api")]
public sealed class DoctorController
    : ControllerBase
{
    private readonly IDoctorService
        _doctorService;

    public DoctorController(
        IDoctorService doctorService
    )
    {
        _doctorService =
            doctorService;
    }

    // =====================================================
    // PUBLIC - DOCTORS
    // =====================================================

    [AllowAnonymous]
    [HttpGet("doctors")]
    public async Task<IActionResult> GetPublicDoctors(
        [FromQuery] string? q,
        [FromQuery] string? specialtyId,
        [FromQuery] decimal? star,
        CancellationToken cancellationToken
    )
    {
        var result =
            await _doctorService
                .GetDoctorsAsync(
                    specialtyId,
                    q,
                    star,
                    true,
                    cancellationToken
                );

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("doctors/{id:int}")]
    public async Task<IActionResult> GetPublicDoctor(
        int id,
        CancellationToken cancellationToken
    )
    {
        var result =
            await _doctorService
                .GetDoctorByIdAsync(
                    id,
                    cancellationToken
                );

        if (
            !string.Equals(
                result.TrangThai,
                "active",
                StringComparison.OrdinalIgnoreCase
            )
        )
        {
            return NotFound();
        }

        return Ok(result);
    }

    // =====================================================
    // ADMIN - LIST
    // =====================================================

    [Authorize(Roles = "ADMIN")]
    [HttpGet("admin/doctors")]
    public async Task<IActionResult> GetAdminDoctors(
        [FromQuery] string? q,
        [FromQuery] string? specialtyId,
        CancellationToken cancellationToken
    )
    {
        var result =
            await _doctorService
                .GetDoctorsAsync(
                    specialtyId,
                    q,
                    null,
                    false,
                    cancellationToken
                );

        return Ok(result);
    }

    // =====================================================
    // ADMIN - DETAIL
    // =====================================================

    [Authorize(Roles = "ADMIN")]
    [HttpGet("admin/doctors/{id:int}")]
    public async Task<IActionResult> GetAdminDoctor(
        int id,
        CancellationToken cancellationToken
    )
    {
        var result =
            await _doctorService
                .GetDoctorByIdAsync(
                    id,
                    cancellationToken
                );

        return Ok(result);
    }

    // =====================================================
    // ADMIN - CREATE
    // =====================================================

    [Authorize(Roles = "ADMIN")]
    [HttpPost("admin/doctors")]
    public async Task<IActionResult> CreateDoctor(
        [FromBody] DoctorRequest request,
        CancellationToken cancellationToken
    )
    {
        var result =
            await _doctorService
                .CreateDoctorAsync(
                    request,
                    cancellationToken
                );

        return CreatedAtAction(
            nameof(GetAdminDoctor),
            new
            {
                id = result.Id
            },
            result
        );
    }

    // =====================================================
    // ADMIN - UPDATE
    // =====================================================

    [Authorize(Roles = "ADMIN")]
    [HttpPut("admin/doctors/{id:int}")]
    public async Task<IActionResult> UpdateDoctor(
        int id,
        [FromBody] DoctorRequest request,
        CancellationToken cancellationToken
    )
    {
        var result =
            await _doctorService
                .UpdateDoctorAsync(
                    id,
                    request,
                    cancellationToken
                );

        return Ok(result);
    }

    // =====================================================
    // ADMIN - SOFT DELETE
    // =====================================================

    [Authorize(Roles = "ADMIN")]
    [HttpDelete("admin/doctors/{id:int}")]
    public async Task<IActionResult> DeleteDoctor(
        int id,
        CancellationToken cancellationToken
    )
    {
        await _doctorService
            .DeleteDoctorAsync(
                id,
                cancellationToken
            );

        return NoContent();
    }

    // =====================================================
    // DOCTOR - PENDING RESULTS
    // =====================================================

    [Authorize(Roles = "DOCTOR")]
    [HttpGet("doctor/results/pending")]
    public async Task<IActionResult> GetPendingResults(
        CancellationToken cancellationToken
    )
    {
        var login =
            User.Identity?.Name
            ?? string.Empty;

        var result =
            await _doctorService
                .GetPendingResultsAsync(
                    login,
                    cancellationToken
                );

        return Ok(result);
    }

    // =====================================================
    // DOCTOR - RESULT DETAIL
    // =====================================================

    [Authorize(Roles = "DOCTOR")]
    [HttpGet("doctor/results/{id}")]
    public async Task<IActionResult> GetResultDetail(
        string id,
        CancellationToken cancellationToken
    )
    {
        var login =
            User.Identity?.Name
            ?? string.Empty;

        var result =
            await _doctorService
                .GetResultDetailAsync(
                    id,
                    login,
                    cancellationToken
                );

        return Ok(result);
    }

    // =====================================================
    // DOCTOR - APPROVE RESULT
    // =====================================================

    [Authorize(Roles = "DOCTOR")]
    [HttpPost("doctor/results/{id}/approve")]
    public async Task<IActionResult> ApproveResult(
        string id,
        [FromBody] ApproveResultRequest request,
        CancellationToken cancellationToken
    )
    {
        var login =
            User.Identity?.Name
            ?? string.Empty;

        var result =
            await _doctorService
                .ApproveResultAsync(
                    id,
                    request.Conclusion,
                    login,
                    cancellationToken
                );

        return Ok(result);
    }
}