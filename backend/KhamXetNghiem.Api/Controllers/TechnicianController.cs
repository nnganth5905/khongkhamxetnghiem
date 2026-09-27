using KhamXetNghiem.Api.DTOs.Requests;
using KhamXetNghiem.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KhamXetNghiem.Api.Controllers;

[ApiController]
[Route("api")]
public sealed class TechnicianController
    : ControllerBase
{
    private readonly ITechnicianService
        _technicianService;

    public TechnicianController(
        ITechnicianService technicianService
    )
    {
        _technicianService =
            technicianService;
    }

    // =====================================================
    // CURRENT TECHNICIAN
    // =====================================================

    [Authorize(Roles = "TECHNICIAN")]
    [HttpGet("technician/me")]
    public async Task<IActionResult> GetCurrentTechnician(
        CancellationToken cancellationToken
    )
    {
        var result =
            await _technicianService
                .GetCurrentTechnicianAsync(
                    User,
                    cancellationToken
                );

        return Ok(result);
    }

    // =====================================================
    // ADMIN CRUD
    // =====================================================

    [Authorize(Roles = "ADMIN")]
    [HttpGet("admin/technicians")]
    public async Task<IActionResult> GetTechnicians(
        [FromQuery] string? q,
        CancellationToken cancellationToken
    )
    {
        var result =
            await _technicianService
                .GetTechniciansAsync(
                    q,
                    cancellationToken
                );

        return Ok(result);
    }

    [Authorize(Roles = "ADMIN")]
    [HttpGet("admin/technicians/{id}")]
    public async Task<IActionResult> GetTechnicianById(
        string id,
        CancellationToken cancellationToken
    )
    {
        var result =
            await _technicianService
                .GetTechnicianByIdAsync(
                    id,
                    cancellationToken
                );

        return Ok(result);
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPost("admin/technicians")]
    public async Task<IActionResult> CreateTechnician(
        [FromBody] TechnicianRequest request,
        CancellationToken cancellationToken
    )
    {
        var result =
            await _technicianService
                .CreateTechnicianAsync(
                    request,
                    cancellationToken
                );

        return CreatedAtAction(
            nameof(GetTechnicianById),
            new
            {
                id = result.Id
            },
            result
        );
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPut("admin/technicians/{id}")]
    public async Task<IActionResult> UpdateTechnician(
        string id,
        [FromBody] TechnicianRequest request,
        CancellationToken cancellationToken
    )
    {
        var result =
            await _technicianService
                .UpdateTechnicianAsync(
                    id,
                    request,
                    cancellationToken
                );

        return Ok(result);
    }

    [Authorize(Roles = "ADMIN")]
    [HttpDelete("admin/technicians/{id}")]
    public async Task<IActionResult> DeleteTechnician(
        string id,
        CancellationToken cancellationToken
    )
    {
        await _technicianService
            .DeleteTechnicianAsync(
                id,
                cancellationToken
            );

        return NoContent();
    }

    // =====================================================
    // SPECIMEN
    // =====================================================

    [Authorize(Roles = "TECHNICIAN")]
    [HttpGet("technician/specimens")]
    public async Task<IActionResult> GetSpecimens(
        [FromQuery] string? status,
        CancellationToken cancellationToken
    )
    {
        var result =
            await _technicianService
                .GetSpecimensAsync(
                    status,
                    User,
                    cancellationToken
                );

        return Ok(result);
    }

    [Authorize(Roles = "TECHNICIAN")]
    [HttpGet("technician/specimens/{id}")]
    public async Task<IActionResult> GetSpecimen(
        string id,
        CancellationToken cancellationToken
    )
    {
        var result =
            await _technicianService
                .GetSpecimenAsync(
                    id,
                    User,
                    cancellationToken
                );

        return Ok(result);
    }

    [Authorize(Roles = "TECHNICIAN")]
    [HttpPost("technician/specimens/{id}/receive")]
    public async Task<IActionResult> ReceiveSpecimen(
        string id,
        [FromBody] ReceiveSpecimenRequest request,
        CancellationToken cancellationToken
    )
    {
        await _technicianService
            .ReceiveSpecimenAsync(
                id,
                request,
                User,
                cancellationToken
            );

        return Ok(
            new
            {
                message =
                    "Tiếp nhận mẫu thành công."
            }
        );
    }

    [Authorize(Roles = "TECHNICIAN")]
    [HttpPost("technician/specimens/{id}/reject")]
    public async Task<IActionResult> RejectSpecimen(
        string id,
        [FromBody] RejectSpecimenRequest request,
        CancellationToken cancellationToken
    )
    {
        await _technicianService
            .RejectSpecimenAsync(
                id,
                request,
                User,
                cancellationToken
            );

        return Ok(
            new
            {
                message =
                    "Đã từ chối mẫu bệnh phẩm."
            }
        );
    }

    // =====================================================
    // WORKLIST
    // =====================================================

    [Authorize(Roles = "TECHNICIAN")]
    [HttpGet("technician/worklist")]
    public async Task<IActionResult> GetWorklist(
        [FromQuery] string? status,
        CancellationToken cancellationToken
    )
    {
        var result =
            await _technicianService
                .GetWorklistAsync(
                    status,
                    User,
                    cancellationToken
                );

        return Ok(result);
    }

    [Authorize(Roles = "TECHNICIAN")]
    [HttpPost("technician/worklist/{id:long}/start")]
    public async Task<IActionResult> StartWork(
        long id,
        CancellationToken cancellationToken
    )
    {
        await _technicianService
            .StartWorkAsync(
                id,
                User,
                cancellationToken
            );

        return Ok(
            new
            {
                message =
                    "Đã bắt đầu thực hiện xét nghiệm."
            }
        );
    }

    [Authorize(Roles = "TECHNICIAN")]
    [HttpPost("technician/worklist/{id:long}/complete")]
    public async Task<IActionResult> CompleteWork(
        long id,
        CancellationToken cancellationToken
    )
    {
        await _technicianService
            .CompleteWorkAsync(
                id,
                User,
                cancellationToken
            );

        return Ok(
            new
            {
                message =
                    "Đã hoàn tất bước thực hiện xét nghiệm."
            }
        );
    }

    // =====================================================
    // RESULT ENTRY
    // =====================================================

    [Authorize(Roles = "TECHNICIAN")]
    [HttpGet("technician/results/{id:long}")]
    public async Task<IActionResult> GetResultEntry(
        long id,
        CancellationToken cancellationToken
    )
    {
        var result =
            await _technicianService
                .GetResultEntryAsync(
                    id,
                    User,
                    cancellationToken
                );

        return Ok(result);
    }

    [Authorize(Roles = "TECHNICIAN")]
    [HttpPut("technician/results/{id:long}")]
    public async Task<IActionResult> SaveResultEntry(
        long id,
        [FromBody] ResultEntryRequest request,
        CancellationToken cancellationToken
    )
    {
        await _technicianService
            .SaveResultAsync(
                id,
                request,
                User,
                false,
                cancellationToken
            );

        return Ok(
            new
            {
                message =
                    "Đã lưu kết quả xét nghiệm."
            }
        );
    }

    [Authorize(Roles = "TECHNICIAN")]
    [HttpPost("technician/results/{id:long}/submit")]
    public async Task<IActionResult> SubmitResultEntry(
        long id,
        [FromBody] ResultEntryRequest request,
        CancellationToken cancellationToken
    )
    {
        await _technicianService
            .SaveResultAsync(
                id,
                request,
                User,
                true,
                cancellationToken
            );

        return Ok(
            new
            {
                message =
                    "Đã gửi kết quả sang bác sĩ duyệt."
            }
        );
    }

    /*
     * Alias cho service frontend cũ.
     */
    [Authorize(Roles = "TECHNICIAN")]
    [HttpPost("technician/specimens/{id}/results")]
    public async Task<IActionResult> SubmitResultBySpecimen(
        string id,
        [FromBody] ResultEntryRequest request,
        CancellationToken cancellationToken
    )
    {
        await _technicianService
            .SubmitResultBySpecimenAsync(
                id,
                request,
                User,
                cancellationToken
            );

        return Ok(
            new
            {
                message =
                    "Đã gửi kết quả xét nghiệm."
            }
        );
    }
}