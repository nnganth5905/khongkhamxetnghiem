using KhamXetNghiem.Api.DTOs.Requests;
using KhamXetNghiem.Api.Services.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KhamXetNghiem.Api.Controllers;

[ApiController]
[Route("api")]
public sealed class ScheduleController
    : ControllerBase
{
    private readonly IWorkScheduleService
        _scheduleService;

    public ScheduleController(
        IWorkScheduleService scheduleService
    )
    {
        _scheduleService =
            scheduleService;
    }

    // =====================================================
    // PUBLIC / BOOKING
    // =====================================================

    [AllowAnonymous]
    [HttpGet("schedules/doctors/{doctorId:int}")]
    public async Task<IActionResult> GetDoctorSchedule(
        int doctorId,
        [FromQuery] string? date,
        CancellationToken cancellationToken
    )
    {
        var result =
            await _scheduleService
                .GetDoctorScheduleAsync(
                    doctorId,
                    date,
                    cancellationToken
                );

        return Ok(result);
    }

    // =====================================================
    // ADMIN LIST
    // =====================================================

    [Authorize(Roles = "ADMIN")]
    [HttpGet("admin/work-schedules")]
    public async Task<IActionResult> GetSchedules(
        [FromQuery] string? q,
        CancellationToken cancellationToken
    )
    {
        var result =
            await _scheduleService
                .GetSchedulesAsync(
                    q,
                    cancellationToken
                );

        return Ok(result);
    }

    // =====================================================
    // ADMIN DETAIL
    // =====================================================

    [Authorize(Roles = "ADMIN")]
    [HttpGet("admin/work-schedules/{id:long}")]
    public async Task<IActionResult> GetSchedule(
        long id,
        CancellationToken cancellationToken
    )
    {
        var result =
            await _scheduleService
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
    [HttpPost("admin/work-schedules")]
    public async Task<IActionResult> CreateSchedule(
        [FromBody] WorkScheduleRequest request,
        CancellationToken cancellationToken
    )
    {
        var result =
            await _scheduleService
                .CreateAsync(
                    request,
                    User,
                    cancellationToken
                );

        return CreatedAtAction(
            nameof(GetSchedule),
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
    [HttpPut("admin/work-schedules/{id:long}")]
    public async Task<IActionResult> UpdateSchedule(
        long id,
        [FromBody] WorkScheduleRequest request,
        CancellationToken cancellationToken
    )
    {
        var result =
            await _scheduleService
                .UpdateAsync(
                    id,
                    request,
                    cancellationToken
                );

        return Ok(result);
    }

    // =====================================================
    // ADMIN CANCEL
    // =====================================================

    [Authorize(Roles = "ADMIN")]
    [HttpDelete("admin/work-schedules/{id:long}")]
    public async Task<IActionResult> DeleteSchedule(
        long id,
        CancellationToken cancellationToken
    )
    {
        await _scheduleService
            .DeleteAsync(
                id,
                cancellationToken
            );

        return NoContent();
    }
}