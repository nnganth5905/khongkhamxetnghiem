using KhamXetNghiem.Api.DTOs.Requests;
using KhamXetNghiem.Api.Services.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KhamXetNghiem.Api.Controllers;

[ApiController]
[Route("api")]
public sealed class SpecimenController
    : ControllerBase
{
    private readonly ISpecimenService
        _specimenService;

    public SpecimenController(
        ISpecimenService specimenService
    )
    {
        _specimenService =
            specimenService;
    }

    // =====================================================
    // DOCTOR - PENDING TEST ITEMS
    // =====================================================

    [Authorize(Roles = "DOCTOR")]
    [HttpGet("doctor/specimens/pending")]
    public async Task<IActionResult> GetPendingItems(
        [FromQuery] string appointmentId,
        CancellationToken cancellationToken
    )
    {
        return Ok(
            await _specimenService
                .GetPendingItemsAsync(
                    appointmentId,
                    User,
                    cancellationToken
                )
        );
    }

    // =====================================================
    // DOCTOR - COLLECT
    // =====================================================

    [Authorize(Roles = "DOCTOR")]
    [HttpPost("doctor/specimens")]
    public async Task<IActionResult> Collect(
        [FromBody] CollectSpecimenRequest request,
        CancellationToken cancellationToken
    )
    {
        var result =
            await _specimenService
                .CollectAsync(
                    request,
                    User,
                    cancellationToken
                );

        return StatusCode(
            StatusCodes.Status201Created,
            result
        );
    }

    // =====================================================
    // DOCTOR - HANDOVER
    // =====================================================

    [Authorize(Roles = "DOCTOR")]
    [HttpPost("doctor/specimens/{id}/handover")]
    public async Task<IActionResult> Handover(
        string id,
        [FromBody] HandoverSpecimenRequest request,
        CancellationToken cancellationToken
    )
    {
        return Ok(
            await _specimenService
                .HandoverAsync(
                    id,
                    request,
                    User,
                    cancellationToken
                )
        );
    }

    // =====================================================
    // TECHNICIAN OPTIONS
    // =====================================================

    [Authorize(Roles = "DOCTOR,ADMIN")]
    [HttpGet("technicians")]
    public async Task<IActionResult> GetTechnicians(
        CancellationToken cancellationToken
    )
    {
        return Ok(
            await _specimenService
                .GetTechniciansAsync(
                    cancellationToken
                )
        );
    }

    // =====================================================
    // TECHNICIAN - SPECIMENS
    // =====================================================

    [Authorize(Roles = "TECHNICIAN")]
    [HttpGet("technician/specimens")]
    public async Task<IActionResult> GetSpecimens(
        [FromQuery] string? status,
        CancellationToken cancellationToken
    )
    {
        return Ok(
            await _specimenService
                .GetSpecimensAsync(
                    status,
                    User,
                    cancellationToken
                )
        );
    }

    // =====================================================
    // TECHNICIAN - DETAIL
    // =====================================================

    [Authorize(Roles = "TECHNICIAN")]
    [HttpGet("technician/specimens/{id}")]
    public async Task<IActionResult> GetSpecimen(
        string id,
        CancellationToken cancellationToken
    )
    {
        return Ok(
            await _specimenService
                .GetSpecimenAsync(
                    id,
                    User,
                    cancellationToken
                )
        );
    }

    // =====================================================
    // TECHNICIAN - RECEIVE
    // =====================================================

    [Authorize(Roles = "TECHNICIAN")]
    [HttpPost("technician/specimens/{id}/receive")]
    public async Task<IActionResult> Receive(
        string id,
        [FromBody] ReceiveSpecimenRequest request,
        CancellationToken cancellationToken
    )
    {
        return Ok(
            await _specimenService
                .ReceiveAsync(
                    id,
                    request,
                    User,
                    cancellationToken
                )
        );
    }

    // =====================================================
    // TECHNICIAN - REJECT
    // =====================================================

    [Authorize(Roles = "TECHNICIAN")]
    [HttpPost("technician/specimens/{id}/reject")]
    public async Task<IActionResult> Reject(
        string id,
        [FromBody] RejectSpecimenRequest request,
        CancellationToken cancellationToken
    )
    {
        return Ok(
            await _specimenService
                .RejectAsync(
                    id,
                    request,
                    User,
                    cancellationToken
                )
        );
    }
}