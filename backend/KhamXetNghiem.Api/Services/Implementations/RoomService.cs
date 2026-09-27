using KhamXetNghiem.Api.DTOs.Requests;
using KhamXetNghiem.Api.DTOs.Responses;
using KhamXetNghiem.Api.Entities;
using KhamXetNghiem.Api.Exceptions;
using KhamXetNghiem.Api.Repositories.Interfaces;
using KhamXetNghiem.Api.Services.Interfaces;

namespace KhamXetNghiem.Api.Services.Implementations;

public sealed class RoomService
    : IRoomService
{
    private readonly IRoomRepository
        _roomRepository;

    public RoomService(
        IRoomRepository roomRepository
    )
    {
        _roomRepository =
            roomRepository;
    }

    // =====================================================
    // LIST
    // =====================================================

    public Task<List<RoomResponse>> GetAllAsync(
        string? query,
        bool activeOnly,
        CancellationToken cancellationToken = default
    )
    {
        return _roomRepository
            .GetAllAsync(
                query,
                activeOnly,
                cancellationToken
            );
    }

    // =====================================================
    // DETAIL
    // =====================================================

    public async Task<RoomResponse> GetByIdAsync(
        string id,
        CancellationToken cancellationToken = default
    )
    {
        var room =
            await _roomRepository
                .GetResponseByIdAsync(
                    id,
                    cancellationToken
                );

        if (room is null)
        {
            throw new ResourceNotFoundException(
                $"Không tìm thấy phòng: {id}"
            );
        }

        return room;
    }

    // =====================================================
    // CREATE
    // =====================================================

    public async Task<RoomResponse> CreateAsync(
        RoomRequest request,
        CancellationToken cancellationToken = default
    )
    {
        await ValidateAsync(
            request,
            cancellationToken
        );

        var id =
            await GenerateNextIdAsync(
                cancellationToken
            );

        var room =
            new Room
            {
                Id =
                    id,

                FacilityId =
                    request.IdCoSo.Trim(),

                SpecialtyId =
                    NormalizeNullable(
                        request.IdChuyenKhoa
                    ),

                Name =
                    request.TenPhong.Trim(),

                Type =
                    NormalizeTypeToDb(
                        request.LoaiPhong
                    ),

                Floor =
                    NormalizeNullable(
                        request.Tang
                    ),

                Status =
                    NormalizeStatusToDb(
                        request.TrangThai
                    )
            };

        await _roomRepository
            .AddAsync(
                room,
                cancellationToken
            );

        await _roomRepository
            .SaveChangesAsync(
                cancellationToken
            );

        return (
            await _roomRepository
                .GetResponseByIdAsync(
                    room.Id,
                    cancellationToken
                )
        )
        ?? throw new InvalidOperationException(
            "Không đọc được phòng vừa tạo."
        );
    }

    // =====================================================
    // UPDATE
    // =====================================================

    public async Task<RoomResponse> UpdateAsync(
        string id,
        RoomRequest request,
        CancellationToken cancellationToken = default
    )
    {
        await ValidateAsync(
            request,
            cancellationToken
        );

        var room =
            await _roomRepository
                .FindByIdAsync(
                    id,
                    true,
                    cancellationToken
                );

        if (room is null)
        {
            throw new ResourceNotFoundException(
                $"Không tìm thấy phòng: {id}"
            );
        }

        room.Name =
            request.TenPhong.Trim();

        room.Type =
            NormalizeTypeToDb(
                request.LoaiPhong
            );

        room.FacilityId =
            request.IdCoSo.Trim();

        room.SpecialtyId =
            NormalizeNullable(
                request.IdChuyenKhoa
            );

        room.Floor =
            NormalizeNullable(
                request.Tang
            );

        room.Status =
            NormalizeStatusToDb(
                request.TrangThai
            );

        await _roomRepository
            .SaveChangesAsync(
                cancellationToken
            );

        return (
            await _roomRepository
                .GetResponseByIdAsync(
                    id,
                    cancellationToken
                )
        )
        ?? throw new ResourceNotFoundException(
            $"Không tìm thấy phòng: {id}"
        );
    }

    // =====================================================
    // DELETE = SOFT DELETE
    // =====================================================

    public async Task DeleteAsync(
        string id,
        CancellationToken cancellationToken = default
    )
    {
        var room =
            await _roomRepository
                .FindByIdAsync(
                    id,
                    true,
                    cancellationToken
                );

        if (room is null)
        {
            throw new ResourceNotFoundException(
                $"Không tìm thấy phòng: {id}"
            );
        }

        /*
         * Không DELETE vật lý:
         * IDPhong đang được lichlamviec /
         * luotxetnghiem tham chiếu.
         */
        room.Status =
            "inactive";

        await _roomRepository
            .SaveChangesAsync(
                cancellationToken
            );
    }

    // =====================================================
    // VALIDATE
    // =====================================================

    private async Task ValidateAsync(
        RoomRequest request,
        CancellationToken cancellationToken
    )
    {
        if (
            string.IsNullOrWhiteSpace(
                request.TenPhong
            )
        )
        {
            throw new BadRequestException(
                "Tên phòng không được để trống."
            );
        }

        if (
            string.IsNullOrWhiteSpace(
                request.IdCoSo
            )
        )
        {
            throw new BadRequestException(
                "Mã cơ sở không được để trống."
            );
        }

        if (
            !await _roomRepository
                .FacilityExistsAsync(
                    request.IdCoSo.Trim(),
                    cancellationToken
                )
        )
        {
            throw new BadRequestException(
                $"Cơ sở {request.IdCoSo} không tồn tại."
            );
        }

        var specialtyId =
            NormalizeNullable(
                request.IdChuyenKhoa
            );

        if (
            specialtyId is not null
            &&
            !await _roomRepository
                .SpecialtyExistsAsync(
                    specialtyId,
                    cancellationToken
                )
        )
        {
            throw new BadRequestException(
                $"Chuyên khoa {specialtyId} không tồn tại."
            );
        }

        /*
         * Validate enum ngay tại Service.
         */
        _ =
            NormalizeTypeToDb(
                request.LoaiPhong
            );

        _ =
            NormalizeStatusToDb(
                request.TrangThai
            );
    }

    // =====================================================
    // ID
    // =====================================================

    private async Task<string> GenerateNextIdAsync(
        CancellationToken cancellationToken
    )
    {
        var ids =
            await _roomRepository
                .GetAllIdsAsync(
                    cancellationToken
                );

        var max =
            ids
                .Where(
                    id =>
                        id.StartsWith(
                            "P",
                            StringComparison.OrdinalIgnoreCase
                        )
                )
                .Select(
                    id =>
                    {
                        var suffix =
                            id.Length > 1
                                ? id[1..]
                                : string.Empty;

                        return int.TryParse(
                            suffix,
                            out var number
                        )
                            ? number
                            : 0;
                    }
                )
                .DefaultIfEmpty(0)
                .Max();

        var id =
            $"P{max + 1:D3}";

        if (id.Length > 20)
        {
            throw new InvalidOperationException(
                "Đã vượt giới hạn mã phòng."
            );
        }

        return id;
    }

    // =====================================================
    // TYPE
    // =====================================================

    private static string NormalizeTypeToDb(
        string? type
    )
    {
        if (
            string.IsNullOrWhiteSpace(
                type
            )
        )
        {
            throw new BadRequestException(
                "Loại phòng không được để trống."
            );
        }

        return type
            .Trim()
            .ToUpperInvariant()
        switch
        {
            "EXAMINATION" or
            "KHAM" =>
                "kham",

            "SAMPLING" or
            "LAY_MAU" =>
                "lay_mau",

            "LAB" or
            "XET_NGHIEM" =>
                "xet_nghiem",

            "CONSULTATION" or
            "TU_VAN" =>
                "tu_van",

            "OTHER" or
            "KHAC" =>
                "khac",

            _ =>
                throw new BadRequestException(
                    $"Loại phòng '{type}' không hợp lệ."
                )
        };
    }

    // =====================================================
    // STATUS
    // =====================================================

    private static string NormalizeStatusToDb(
        string? status
    )
    {
        if (
            string.IsNullOrWhiteSpace(
                status
            )
        )
        {
            return "active";
        }

        return status
            .Trim()
            .ToUpperInvariant()
        switch
        {
            "AVAILABLE" or
            "ACTIVE" =>
                "active",

            "MAINTENANCE" =>
                "maintenance",

            "INACTIVE" =>
                "inactive",

            _ =>
                throw new BadRequestException(
                    $"Trạng thái phòng '{status}' không hợp lệ."
                )
        };
    }

    private static string? NormalizeNullable(
        string? value
    )
    {
        return string.IsNullOrWhiteSpace(
            value
        )
            ? null
            : value.Trim();
    }
}