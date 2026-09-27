using KhamXetNghiem.Api.DTOs.Requests;
using KhamXetNghiem.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KhamXetNghiem.Api.Controllers;

[ApiController]
[Route("api")]
public sealed class CustomerController
    : ControllerBase
{
    private readonly ICustomerService
        _customerService;

    public CustomerController(
        ICustomerService customerService
    )
    {
        _customerService =
            customerService;
    }

    // =====================================================
    // CUSTOMER - MY PROFILE
    // =====================================================

    [Authorize(Roles = "CUSTOMER")]
    [HttpGet("customers/me")]
    public async Task<IActionResult> GetMyProfile(
        CancellationToken cancellationToken
    )
    {
        var result =
            await _customerService
                .GetCurrentCustomerAsync(
                    User,
                    cancellationToken
                );

        return Ok(result);
    }

    // =====================================================
    // CUSTOMER - UPDATE MY PROFILE
    // =====================================================

    [Authorize(Roles = "CUSTOMER")]
    [HttpPut("customers/me")]
    public async Task<IActionResult> UpdateMyProfile(
        [FromBody] CustomerRequest request,
        CancellationToken cancellationToken
    )
    {
        var result =
            await _customerService
                .UpdateCurrentCustomerAsync(
                    User,
                    request,
                    cancellationToken
                );

        return Ok(result);
    }

    // =====================================================
    // ADMIN - LIST
    // =====================================================

    [Authorize(Roles = "ADMIN")]
    [HttpGet("admin/customers")]
    public async Task<IActionResult> GetAllCustomers(
        [FromQuery] string? q,
        CancellationToken cancellationToken
    )
    {
        var result =
            await _customerService
                .GetAllCustomersAsync(
                    q,
                    cancellationToken
                );

        return Ok(result);
    }

    // =====================================================
    // ADMIN - DETAIL
    // =====================================================

    [Authorize(Roles = "ADMIN")]
    [HttpGet("admin/customers/{id}")]
    public async Task<IActionResult> GetCustomerById(
        string id,
        CancellationToken cancellationToken
    )
    {
        var result =
            await _customerService
                .GetCustomerByIdAsync(
                    id,
                    cancellationToken
                );

        return Ok(result);
    }

    // =====================================================
    // ADMIN - CREATE
    // =====================================================

    [Authorize(Roles = "ADMIN")]
    [HttpPost("admin/customers")]
    public async Task<IActionResult> CreateCustomer(
        [FromBody] CustomerRequest request,
        CancellationToken cancellationToken
    )
    {
        var result =
            await _customerService
                .CreateCustomerAsync(
                    request,
                    cancellationToken
                );

        return CreatedAtAction(
            nameof(GetCustomerById),
            new
            {
                id =
                    result.IdKhachHang
            },
            result
        );
    }

    // =====================================================
    // ADMIN - UPDATE
    // =====================================================

    [Authorize(Roles = "ADMIN")]
    [HttpPut("admin/customers/{id}")]
    public async Task<IActionResult> UpdateCustomer(
        string id,
        [FromBody] CustomerRequest request,
        CancellationToken cancellationToken
    )
    {
        var result =
            await _customerService
                .UpdateCustomerAsync(
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
    [HttpDelete("admin/customers/{id}")]
    public async Task<IActionResult> DeleteCustomer(
        string id,
        CancellationToken cancellationToken
    )
    {
        await _customerService
            .DeleteCustomerAsync(
                id,
                cancellationToken
            );

        return NoContent();
    }
}