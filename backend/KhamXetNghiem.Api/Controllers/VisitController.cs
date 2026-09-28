using KhamXetNghiem.Api.DTOs.Requests;
using KhamXetNghiem.Api.Services.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KhamXetNghiem.Api.Controllers;

[ApiController]
[Route("api")]
public sealed class VisitController
    : ControllerBase
{
    private readonly IVisitService
        _visitService;

    public VisitController(
        IVisitService visitService
    )
    {
        _visitService =
            visitService;
    }

    // =====================================================
    // RECEPTION CHECK-IN - API CHÍNH
    // =====================================================

    [Authorize(
        Roles = "RECEPTIONIST,ADMIN"
    )]
    [HttpPost(
        "reception/visits/check-in"
    )]
    public async Task<IActionResult> CheckIn(
        [FromBody]
        VisitCheckInRequest request,
        CancellationToken cancellationToken
    )
    {
        var result =
            await _visitService
                .CheckInAsync(
                    request.AppointmentId
                    ?? string.Empty,

                    request.CustomerId,

                    request.ResolveType(),

                    request.Note,

                    User,

                    cancellationToken
                );

        return StatusCode(
            StatusCodes.Status201Created,
            result
        );
    }

    // =====================================================
    // COMPATIBILITY:
    // POST /appointments/{id}/check-in
    // =====================================================

    [Authorize(
        Roles = "RECEPTIONIST,ADMIN"
    )]
    [HttpPost(
        "appointments/{id}/check-in"
    )]
    public async Task<IActionResult>
        CheckInAppointment(
            string id,
            [FromBody]
            VisitCheckInRequest request,
            CancellationToken cancellationToken
        )
    {
        var result =
            await _visitService
                .CheckInAsync(
                    id,
                    request.CustomerId,
                    request.ResolveType(),
                    request.Note,
                    User,
                    cancellationToken
                );

        return Ok(
            result
        );
    }

    // =====================================================
    // QR
    // =====================================================

    [Authorize(
        Roles = "RECEPTIONIST,ADMIN"
    )]
    [HttpPost(
        "appointments/check-in-qr"
    )]
    public async Task<IActionResult> CheckInByQr(
        [FromBody]
        QrCheckInRequest request,
        CancellationToken cancellationToken
    )
    {
        return Ok(
            await _visitService
                .CheckInByQrAsync(
                    request.QrCode,
                    request.Note,
                    User,
                    cancellationToken
                )
        );
    }

    // =====================================================
    // RECEPTION WAITING
    // =====================================================

    [Authorize(
        Roles = "RECEPTIONIST,ADMIN"
    )]
    [HttpGet(
        "reception/visits/waiting"
    )]
    public async Task<IActionResult>
        GetReceptionWaitingList(
            [FromQuery]
            string? type,
            CancellationToken cancellationToken
        )
    {
        return Ok(
            await _visitService
                .GetWaitingListAsync(
                    type,
                    cancellationToken
                )
        );
    }

    // Compatibility appointmentService.js
    [Authorize(
        Roles = "RECEPTIONIST,ADMIN"
    )]
    [HttpGet(
        "appointments/waiting-queue"
    )]
    public async Task<IActionResult>
        GetAppointmentWaitingQueue(
            [FromQuery]
            string? type,
            CancellationToken cancellationToken
        )
    {
        return Ok(
            await _visitService
                .GetWaitingListAsync(
                    type,
                    cancellationToken
                )
        );
    }

    // =====================================================
    // VISIT DETAIL
    // =====================================================

    [Authorize]
    [HttpGet(
        "visits/{id:long}"
    )]
    public async Task<IActionResult> GetVisit(
        long id,
        [FromQuery]
        string? type,
        CancellationToken cancellationToken
    )
    {
        return Ok(
            await _visitService
                .GetVisitByIdAsync(
                    id,
                    type,
                    cancellationToken
                )
        );
    }

    // =====================================================
    // STATUS
    // =====================================================

    [Authorize(
        Roles =
            "ADMIN,RECEPTIONIST,DOCTOR,TECHNICIAN"
    )]
    [HttpPatch(
        "visits/{id:long}/status"
    )]
    public async Task<IActionResult>
        UpdateStatus(
            long id,
            [FromQuery]
            string type,
            [FromBody]
            VisitStatusRequest request,
            CancellationToken cancellationToken
        )
    {
        return Ok(
            await _visitService
                .UpdateStatusAsync(
                    id,
                    type,
                    request,
                    User,
                    cancellationToken
                )
        );
    }

    // =====================================================
    // DOCTOR QUEUE
    // =====================================================

    [Authorize(
        Roles = "DOCTOR"
    )]
    [HttpGet(
        "doctor/queue"
    )]
    public async Task<IActionResult>
        GetDoctorQueue(
            CancellationToken cancellationToken
        )
    {
        return Ok(
            await _visitService
                .GetDoctorWaitingQueueAsync(
                    User,
                    cancellationToken
                )
        );
    }

    [Authorize(
        Roles = "DOCTOR"
    )]
    [HttpGet(
        "doctor/visits/waiting"
    )]
    public async Task<IActionResult>
        GetDoctorWaiting(
            CancellationToken cancellationToken
        )
    {
        return Ok(
            await _visitService
                .GetDoctorWaitingQueueAsync(
                    User,
                    cancellationToken
                )
        );
    }

    // =====================================================
    // DOCTOR CALL
    // =====================================================

    [Authorize(
        Roles = "DOCTOR"
    )]
    [HttpPost(
        "doctor/visits/{id:long}/call"
    )]
    public async Task<IActionResult> CallPatient(
        long id,
        CancellationToken cancellationToken
    )
    {
        await _visitService
            .CallPatientAsync(
                id,
                User,
                cancellationToken
            );

        return Ok(
            new
            {
                success = true,
                message =
                    "Đã gọi bệnh nhân."
            }
        );
    }

    // =====================================================
    // DOCTOR HOLD
    // =====================================================

    [Authorize(
        Roles = "DOCTOR"
    )]
    [HttpPost(
        "doctor/visits/{id:long}/hold"
    )]
    public async Task<IActionResult> HoldPatient(
        long id,
        CancellationToken cancellationToken
    )
    {
        await _visitService
            .HoldPatientAsync(
                id,
                User,
                cancellationToken
            );

        return Ok(
            new
            {
                success = true,
                message =
                    "Đã chuyển bệnh nhân sang chờ gọi lại."
            }
        );
    }

    // Compatibility với visitService.js cũ:
    // skip của bác sĩ = "gọi lại sau"
    [Authorize(
        Roles = "DOCTOR"
    )]
    [HttpPost(
        "doctor/visits/{id:long}/skip"
    )]
    public async Task<IActionResult>
        DoctorSkipPatient(
            long id,
            CancellationToken cancellationToken
        )
    {
        await _visitService
            .HoldPatientAsync(
                id,
                User,
                cancellationToken
            );

        return Ok(
            new
            {
                success = true,
                message =
                    "Đã chuyển bệnh nhân sang chờ gọi lại."
            }
        );
    }

    // =====================================================
    // START EXAM
    // =====================================================

    [Authorize(
        Roles = "DOCTOR"
    )]
    [HttpPost(
        "doctor/visits/{id:long}/start"
    )]
    public async Task<IActionResult> StartExam(
        long id,
        CancellationToken cancellationToken
    )
    {
        var examinationId =
            await _visitService
                .StartExamAsync(
                    id,
                    User,
                    cancellationToken
                );

        return Ok(
            new
            {
                success = true,
                message =
                    "Bắt đầu khám.",

                idKham =
                    examinationId,

                visitId =
                    id
            }
        );
    }

    // =====================================================
    // RECEPTION CALL
    // =====================================================

    [Authorize(
        Roles = "RECEPTIONIST,ADMIN"
    )]
    [HttpPost(
        "appointments/queue/{id}/call"
    )]
    public async Task<IActionResult>
        ReceptionCall(
            string id,
            CancellationToken cancellationToken
        )
    {
        await _visitService
            .CallByAppointmentAsync(
                id,
                User,
                cancellationToken
            );

        return Ok(
            new
            {
                success = true,
                message =
                    "Đã gọi bệnh nhân."
            }
        );
    }

    // =====================================================
    // RECEPTION HOLD
    // =====================================================

    [Authorize(
        Roles = "RECEPTIONIST,ADMIN"
    )]
    [HttpPost(
        "appointments/queue/{id}/hold"
    )]
    public async Task<IActionResult>
        ReceptionHold(
            string id,
            CancellationToken cancellationToken
        )
    {
        await _visitService
            .HoldByAppointmentAsync(
                id,
                User,
                cancellationToken
            );

        return Ok(
            new
            {
                success = true,
                message =
                    "Đã chuyển bệnh nhân sang gọi lại sau."
            }
        );
    }

    // =====================================================
    // RECEPTION SKIP
    // =====================================================

    [Authorize(
        Roles = "RECEPTIONIST,ADMIN"
    )]
    [HttpPost(
        "appointments/queue/{id}/skip"
    )]
    public async Task<IActionResult>
        ReceptionSkip(
            string id,
            CancellationToken cancellationToken
        )
    {
        await _visitService
            .SkipByAppointmentAsync(
                id,
                User,
                cancellationToken
            );

        return Ok(
            new
            {
                success = true,
                message =
                    "Đã đánh dấu bệnh nhân bỏ lượt."
            }
        );
    }

    // =====================================================
    // WALK-IN
    // =====================================================

    [Authorize(
        Roles = "RECEPTIONIST,ADMIN"
    )]
    [HttpPost(
        "appointments/walk-in"
    )]
    public async Task<IActionResult> CreateWalkIn(
        [FromBody]
        WalkInVisitRequest request,
        CancellationToken cancellationToken
    )
    {
        var result =
            await _visitService
                .CreateWalkInAsync(
                    request,
                    User,
                    cancellationToken
                );

        return StatusCode(
            StatusCodes.Status201Created,
            result
        );
    }
}