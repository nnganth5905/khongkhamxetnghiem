using KhamXetNghiem.Api.DTOs.Requests;
using KhamXetNghiem.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KhamXetNghiem.Api.Controllers;

[ApiController]
[Route("api")]
public sealed class SpecialtyController
    : ControllerBase
{
    private readonly ISpecialtyService
        _specialtyService;

    public SpecialtyController(
        ISpecialtyService specialtyService
    )
    {
        _specialtyService =
            specialtyService;
    }

    // =====================================================
    // PUBLIC / COMMON LOOKUP
    // =====================================================

    [AllowAnonymous]
    [HttpGet("specialties")]
    public async Task<IActionResult> GetActiveSpecialties(
        [FromQuery] string? q,
        CancellationToken cancellationToken
    )
    {
        var result =
            await _specialtyService
                .GetAllAsync(
                    q,
                    true,
                    cancellationToken
                );

        return Ok(result);
    }

    // =====================================================
    // ADMIN LIST
    // =====================================================

    [Authorize(Roles = "ADMIN")]
    [HttpGet("admin/specialties")]
    public async Task<IActionResult> GetSpecialties(
        [FromQuery] string? q,
        CancellationToken cancellationToken
    )
    {
        var result =
            await _specialtyService
                .GetAllAsync(
                    q,
                    false,
                    cancellationToken
                );

        return Ok(result);
    }

    // =====================================================
    // ADMIN DETAIL
    // =====================================================

    [Authorize(Roles = "ADMIN")]
    [HttpGet("admin/specialties/{id}")]
    public async Task<IActionResult> GetSpecialty(
        string id,
        CancellationToken cancellationToken
    )
    {
        var result =
            await _specialtyService
                .GetByIdAsync(
                    id,
                    cancellationToken
                );

        return Ok(result);
    }

    // =====================================================
    // ADMIN CREATE
    // =====================================================

    [Authorize(Roles = "ADMIN")]
    [HttpPost("admin/specialties")]
    public async Task<IActionResult> CreateSpecialty(
        [FromBody] SpecialtyRequest request,
        CancellationToken cancellationToken
    )
    {
        var result =
            await _specialtyService
                .CreateAsync(
                    request,
                    cancellationToken
                );

        return CreatedAtAction(
            nameof(GetSpecialty),
            new
            {
                id = result.Id
            },
            result
        );
    }

    // =====================================================
    // ADMIN UPDATE
    // =====================================================

    [Authorize(Roles = "ADMIN")]
    [HttpPut("admin/specialties/{id}")]
    public async Task<IActionResult> UpdateSpecialty(
        string id,
        [FromBody] SpecialtyRequest request,
        CancellationToken cancellationToken
    )
    {
        var result =
            await _specialtyService
                .UpdateAsync(
                    id,
                    request,
                    cancellationToken
                );

        return Ok(result);
    }

    // =====================================================
    // ADMIN DISABLE
    // =====================================================

    [Authorize(Roles = "ADMIN")]
    [HttpDelete("admin/specialties/{id}")]
    public async Task<IActionResult> DeleteSpecialty(
        string id,
        CancellationToken cancellationToken
    )
    {
        await _specialtyService
            .DeleteAsync(
                id,
                cancellationToken
            );

        return NoContent();
    }
}