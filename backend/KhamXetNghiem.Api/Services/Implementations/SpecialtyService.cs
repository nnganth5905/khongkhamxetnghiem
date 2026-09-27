using KhamXetNghiem.Api.DTOs.Requests;
using KhamXetNghiem.Api.DTOs.Responses;
using KhamXetNghiem.Api.Entities;
using KhamXetNghiem.Api.Exceptions;
using KhamXetNghiem.Api.Repositories.Interfaces;
using KhamXetNghiem.Api.Services.Interfaces;

namespace KhamXetNghiem.Api.Services.Implementations;

public sealed class SpecialtyService
    : ISpecialtyService
{
    private readonly ISpecialtyRepository
        _specialtyRepository;

    public SpecialtyService(
        ISpecialtyRepository specialtyRepository
    )
    {
        _specialtyRepository =
            specialtyRepository;
    }

    // =====================================================
    // LIST
    // =====================================================

    public async Task<List<SpecialtyResponse>> GetAllAsync(
        string? query,
        bool activeOnly,
        CancellationToken cancellationToken = default
    )
    {
        var items =
            await _specialtyRepository
                .GetAllAsync(
                    query,
                    activeOnly,
                    cancellationToken
                );

        return items
            .Select(MapResponse)
            .ToList();
    }

    // =====================================================
    // DETAIL
    // =====================================================

    public async Task<SpecialtyResponse> GetByIdAsync(
        string id,
        CancellationToken cancellationToken = default
    )
    {
        var specialty =
            await _specialtyRepository
                .FindByIdAsync(
                    id,
                    false,
                    cancellationToken
                );

        if (specialty is null)
        {
            throw new ResourceNotFoundException(
                $"Không tìm thấy chuyên khoa: {id}"
            );
        }

        return MapResponse(
            specialty
        );
    }

    // =====================================================
    // CREATE
    // =====================================================

    public async Task<SpecialtyResponse> CreateAsync(
        SpecialtyRequest request,
        CancellationToken cancellationToken = default
    )
    {
        ValidateRequest(
            request
        );

        var name =
            request.TenChuyenKhoa.Trim();

        if (
            await _specialtyRepository
                .ExistsByNameAsync(
                    name,
                    null,
                    cancellationToken
                )
        )
        {
            throw new BadRequestException(
                $"Chuyên khoa '{name}' đã tồn tại."
            );
        }

        var id =
            await GenerateNextIdAsync(
                cancellationToken
            );

        var specialty =
            new Specialty
            {
                Id =
                    id,

                Name =
                    name,

                Description =
                    NormalizeNullable(
                        request.MoTa
                    ),

                Status =
                    NormalizeStatusToDb(
                        request.TrangThai
                    )
            };

        await _specialtyRepository
            .AddAsync(
                specialty,
                cancellationToken
            );

        await _specialtyRepository
            .SaveChangesAsync(
                cancellationToken
            );

        return MapResponse(
            specialty
        );
    }

    // =====================================================
    // UPDATE
    // =====================================================

    public async Task<SpecialtyResponse> UpdateAsync(
        string id,
        SpecialtyRequest request,
        CancellationToken cancellationToken = default
    )
    {
        ValidateRequest(
            request
        );

        var specialty =
            await _specialtyRepository
                .FindByIdAsync(
                    id,
                    true,
                    cancellationToken
                );

        if (specialty is null)
        {
            throw new ResourceNotFoundException(
                $"Không tìm thấy chuyên khoa: {id}"
            );
        }

        var name =
            request.TenChuyenKhoa.Trim();

        if (
            await _specialtyRepository
                .ExistsByNameAsync(
                    name,
                    id,
                    cancellationToken
                )
        )
        {
            throw new BadRequestException(
                $"Chuyên khoa '{name}' đã tồn tại."
            );
        }

        specialty.Name =
            name;

        specialty.Description =
            NormalizeNullable(
                request.MoTa
            );

        specialty.Status =
            NormalizeStatusToDb(
                request.TrangThai
            );

        await _specialtyRepository
            .SaveChangesAsync(
                cancellationToken
            );

        return MapResponse(
            specialty
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
        var specialty =
            await _specialtyRepository
                .FindByIdAsync(
                    id,
                    true,
                    cancellationToken
                );

        if (specialty is null)
        {
            throw new ResourceNotFoundException(
                $"Không tìm thấy chuyên khoa: {id}"
            );
        }

        specialty.Status =
            "no";

        await _specialtyRepository
            .SaveChangesAsync(
                cancellationToken
            );
    }

    // =====================================================
    // GENERATE ID
    // =====================================================

    private async Task<string> GenerateNextIdAsync(
        CancellationToken cancellationToken
    )
    {
        var ids =
            await _specialtyRepository
                .GetAllIdsAsync(
                    cancellationToken
                );

        var max =
            ids
                .Where(
                    id =>
                        id.StartsWith(
                            "CK",
                            StringComparison.OrdinalIgnoreCase
                        )
                )
                .Select(
                    id =>
                    {
                        var suffix =
                            id.Length > 2
                                ? id[2..]
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

        var next =
            max + 1;

        var id =
            $"CK{next:D3}";

        if (id.Length > 10)
        {
            throw new InvalidOperationException(
                "Đã vượt giới hạn mã chuyên khoa."
            );
        }

        return id;
    }

    // =====================================================
    // VALIDATE
    // =====================================================

    private static void ValidateRequest(
        SpecialtyRequest request
    )
    {
        if (
            string.IsNullOrWhiteSpace(
                request.TenChuyenKhoa
            )
        )
        {
            throw new BadRequestException(
                "Tên chuyên khoa không được để trống."
            );
        }
    }

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
            return "yes";
        }

        return status
            .Trim()
            .ToUpperInvariant()
        switch
        {
            "ACTIVE" or
            "YES" =>
                "yes",

            "INACTIVE" or
            "NO" =>
                "no",

            _ =>
                throw new BadRequestException(
                    $"Trạng thái chuyên khoa '{status}' không hợp lệ."
                )
        };
    }

    private static SpecialtyResponse MapResponse(
        Specialty specialty
    )
    {
        return new SpecialtyResponse
        {
            Id =
                specialty.Id,

            TenChuyenKhoa =
                specialty.Name,

            MoTa =
                specialty.Description,

            TrangThai =
                specialty.Status == "no"
                    ? "INACTIVE"
                    : "ACTIVE",

            Status =
                specialty.Status
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