using KhamXetNghiem.Api.DTOs.Requests;
using KhamXetNghiem.Api.Services.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KhamXetNghiem.Api.Controllers;

[ApiController]
[Route("api")]
public sealed class TestResultController
    : ControllerBase
{
    private readonly ITestResultService
        _service;

    public TestResultController(
        ITestResultService service
    )
    {
        _service =
            service;
    }

    // =====================================================
    // TECHNICIAN - RESULT ENTRY
    // =====================================================

    [Authorize(Roles = "TECHNICIAN")]
    [HttpGet(
        "technician/results/{worklistId:long}"
    )]
    public async Task<IActionResult> GetEntry(
        long worklistId,
        CancellationToken cancellationToken
    )
    {
        return Ok(
            await _service
                .GetEntryAsync(
                    worklistId,
                    User,
                    cancellationToken
                )
        );
    }

    [Authorize(Roles = "TECHNICIAN")]
    [HttpPut(
        "technician/results/{worklistId:long}"
    )]
    public async Task<IActionResult> SaveDraft(
        long worklistId,
        [FromBody] ResultEntryRequest request,
        CancellationToken cancellationToken
    )
    {
        return Ok(
            await _service
                .SaveDraftAsync(
                    worklistId,
                    request,
                    User,
                    cancellationToken
                )
        );
    }

    [Authorize(Roles = "TECHNICIAN")]
    [HttpPost(
        "technician/results/{worklistId:long}/submit"
    )]
    public async Task<IActionResult> Submit(
        long worklistId,
        [FromBody] ResultEntryRequest request,
        CancellationToken cancellationToken
    )
    {
        return Ok(
            await _service
                .SubmitAsync(
                    worklistId,
                    request,
                    User,
                    cancellationToken
                )
        );
    }

    // =====================================================
    // DOCTOR
    // =====================================================

    [Authorize(Roles = "DOCTOR")]
    [HttpGet(
        "doctor/results/pending"
    )]
    public async Task<IActionResult> GetPending(
        CancellationToken cancellationToken
    )
    {
        return Ok(
            await _service
                .GetPendingAsync(
                    User,
                    cancellationToken
                )
        );
    }

    [Authorize(Roles = "DOCTOR")]
    [HttpGet(
        "doctor/results/{resultId}"
    )]
    public async Task<IActionResult> GetDoctorResult(
        string resultId,
        CancellationToken cancellationToken
    )
    {
        return Ok(
            await _service
                .GetDoctorResultAsync(
                    resultId,
                    User,
                    cancellationToken
                )
        );
    }

    [Authorize(Roles = "DOCTOR")]
    [HttpPost(
        "doctor/results/{resultId}/approve"
    )]
    public async Task<IActionResult> Approve(
        string resultId,
        [FromBody] ApproveTestResultRequest request,
        CancellationToken cancellationToken
    )
    {
        return Ok(
            await _service
                .ApproveAsync(
                    resultId,
                    request,
                    User,
                    cancellationToken
                )
        );
    }
}