using KhamXetNghiem.Api.DTOs.Requests;
using KhamXetNghiem.Api.Services.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KhamXetNghiem.Api.Controllers;

[ApiController]
[Route("api/appointments")]
public sealed class AppointmentController
    : ControllerBase
{
    private readonly IAppointmentService
        _appointmentService;

    public AppointmentController(
        IAppointmentService appointmentService
    )
    {
        _appointmentService =
            appointmentService;
    }

    // =====================================================
    // OPTIONS
    // =====================================================

    [AllowAnonymous]
    [HttpGet("options")]
    public async Task<IActionResult> GetOptions(
        CancellationToken cancellationToken
    )
    {
        return Ok(
            await _appointmentService
                .GetOptionsAsync(
                    cancellationToken
                )
        );
    }

    // =====================================================
    // COMPATIBILITY: TAKEN TIMES
    // =====================================================

    [AllowAnonymous]
    [HttpGet("taken-times")]
    public async Task<IActionResult> GetTakenTimes(
        [FromQuery] string type,
        [FromQuery] string doctorId,
        [FromQuery] string date,
        CancellationToken cancellationToken
    )
    {
        return Ok(
            await _appointmentService
                .GetTakenTimesAsync(
                    type,
                    doctorId,
                    date,
                    cancellationToken
                )
        );
    }

    // =====================================================
    // REAL SLOTS FROM lichlamviec
    // =====================================================

    [AllowAnonymous]
    [HttpGet("time-slots")]
    public async Task<IActionResult> GetAvailableSlots(
        [FromQuery] string type,
        [FromQuery] string doctorId,
        [FromQuery] string date,
        [FromQuery] string? excludeId,
        CancellationToken cancellationToken
    )
    {
        return Ok(
            await _appointmentService
                .GetAvailableSlotsAsync(
                    type,
                    doctorId,
                    date,
                    excludeId,
                    cancellationToken
                )
        );
    }

    // =====================================================
    // CREATE EXAMINATION
    // =====================================================

    [AllowAnonymous]
    [HttpPost("examinations")]
    public async Task<IActionResult> CreateExamination(
        [FromBody]
        ExaminationAppointmentRequest request,
        CancellationToken cancellationToken
    )
    {
        var created =
            await _appointmentService
                .CreateExaminationAsync(
                    request,
                    User,
                    cancellationToken
                );

        return StatusCode(
            StatusCodes.Status201Created,
            created
        );
    }

    // =====================================================
    // CREATE TEST
    // =====================================================

    [AllowAnonymous]
    [HttpPost("tests")]
    public async Task<IActionResult> CreateTest(
        [FromBody]
        TestAppointmentRequest request,
        CancellationToken cancellationToken
    )
    {
        var created =
            await _appointmentService
                .CreateTestAsync(
                    request,
                    User,
                    cancellationToken
                );

        return StatusCode(
            StatusCodes.Status201Created,
            created
        );
    }

    // =====================================================
    // QUICK
    // =====================================================

    [Authorize(Roles = "CUSTOMER")]
    [HttpPost("quick")]
    public async Task<IActionResult> CreateQuick(
        [FromBody]
        QuickAppointmentRequest request,
        CancellationToken cancellationToken
    )
    {
        var created =
            await _appointmentService
                .CreateQuickAsync(
                    request,
                    User,
                    cancellationToken
                );

        return StatusCode(
            StatusCodes.Status201Created,
            created
        );
    }

    // =====================================================
    // MY
    // =====================================================

    [Authorize]
    [HttpGet("my")]
    public async Task<IActionResult> GetMyAppointments(
        CancellationToken cancellationToken
    )
    {
        return Ok(
            await _appointmentService
                .GetMyAppointmentsAsync(
                    User,
                    cancellationToken
                )
        );
    }

    // =====================================================
    // STAFF LIST
    // =====================================================

    [Authorize(
        Roles = "ADMIN,RECEPTIONIST"
    )]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? date,
        CancellationToken cancellationToken
    )
    {
        return Ok(
            await _appointmentService
                .GetAllAppointmentsAsync(
                    date,
                    cancellationToken
                )
        );
    }

    // =====================================================
    // DETAIL
    // =====================================================

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetail(
        string id,
        CancellationToken cancellationToken
    )
    {
        return Ok(
            await _appointmentService
                .GetDetailAsync(
                    id,
                    User,
                    cancellationToken
                )
        );
    }

    // =====================================================
    // UPDATE
    // =====================================================

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        string id,
        [FromBody]
        UpdateAppointmentRequest request,
        CancellationToken cancellationToken
    )
    {
        return Ok(
            await _appointmentService
                .UpdateAsync(
                    id,
                    request,
                    User,
                    cancellationToken
                )
        );
    }

    // =====================================================
    // CANCEL
    // =====================================================

    [Authorize]
    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> Cancel(
        string id,
        CancellationToken cancellationToken
    )
    {
        await _appointmentService
            .CancelAsync(
                id,
                User,
                cancellationToken
            );

        return Ok(
            new
            {
                success = true,
                message = "Hủy lịch hẹn thành công.",
                maDatLich = id
            }
        );
    }
}