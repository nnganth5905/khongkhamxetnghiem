using KhamXetNghiem.Api.DTOs.Requests;
using KhamXetNghiem.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KhamXetNghiem.Api.Controllers;

[ApiController]
[Route("api")]
public sealed class RoomController
    : ControllerBase
{
    private readonly IRoomService
        _roomService;

    public RoomController(
        IRoomService roomService
    )
    {
        _roomService =
            roomService;
    }

    // =====================================================
    // ACTIVE ROOMS
    // =====================================================

    [Authorize]
    [HttpGet("rooms")]
    public async Task<IActionResult> GetActiveRooms(
        [FromQuery] string? q,
        CancellationToken cancellationToken
    )
    {
        var result =
            await _roomService
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
    [HttpGet("admin/rooms")]
    public async Task<IActionResult> GetRooms(
        [FromQuery] string? q,
        CancellationToken cancellationToken
    )
    {
        var result =
            await _roomService
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
    [HttpGet("admin/rooms/{id}")]
    public async Task<IActionResult> GetRoom(
        string id,
        CancellationToken cancellationToken
    )
    {
        var result =
            await _roomService
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
    [HttpPost("admin/rooms")]
    public async Task<IActionResult> CreateRoom(
        [FromBody] RoomRequest request,
        CancellationToken cancellationToken
    )
    {
        var result =
            await _roomService
                .CreateAsync(
                    request,
                    cancellationToken
                );

        return CreatedAtAction(
            nameof(GetRoom),
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
    [HttpPut("admin/rooms/{id}")]
    public async Task<IActionResult> UpdateRoom(
        string id,
        [FromBody] RoomRequest request,
        CancellationToken cancellationToken
    )
    {
        var result =
            await _roomService
                .UpdateAsync(
                    id,
                    request,
                    cancellationToken
                );

        return Ok(result);
    }

    // =====================================================
    // ADMIN SOFT DELETE
    // =====================================================

    [Authorize(Roles = "ADMIN")]
    [HttpDelete("admin/rooms/{id}")]
    public async Task<IActionResult> DeleteRoom(
        string id,
        CancellationToken cancellationToken
    )
    {
        await _roomService
            .DeleteAsync(
                id,
                cancellationToken
            );

        return NoContent();
    }
}