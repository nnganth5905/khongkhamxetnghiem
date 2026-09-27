using KhamXetNghiem.Api.Data;
using KhamXetNghiem.Api.DTOs.Responses;
using KhamXetNghiem.Api.Entities;
using KhamXetNghiem.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KhamXetNghiem.Api.Repositories.Implementations;

public sealed class RoomRepository
    : IRoomRepository
{
    private readonly AppDbContext _dbContext;

    public RoomRepository(
        AppDbContext dbContext
    )
    {
        _dbContext = dbContext;
    }

    private DbSet<Room> Rooms =>
        _dbContext.Set<Room>();

    private DbSet<Facility> Facilities =>
        _dbContext.Set<Facility>();

    private DbSet<Specialty> Specialties =>
        _dbContext.Set<Specialty>();

    // =====================================================
    // LIST
    // =====================================================

    public async Task<List<RoomResponse>> GetAllAsync(
        string? query,
        bool activeOnly,
        CancellationToken cancellationToken = default
    )
    {
        var roomQuery =
            Rooms
                .AsNoTracking()
                .AsQueryable();

        if (activeOnly)
        {
            roomQuery =
                roomQuery.Where(
                    x => x.Status == "active"
                );
        }

        var joinedQuery =
            from room in roomQuery

            join facilityBase
                in Facilities.AsNoTracking()
                on room.FacilityId
                equals facilityBase.Id
                into facilityGroup

            from facility
                in facilityGroup.DefaultIfEmpty()

            join specialtyBase
                in Specialties.AsNoTracking()
                on room.SpecialtyId
                equals specialtyBase.Id
                into specialtyGroup

            from specialty
                in specialtyGroup.DefaultIfEmpty()

            select new
            {
                Room = room,

                FacilityName =
                    facility != null
                        ? facility.Name
                        : null,

                SpecialtyName =
                    specialty != null
                        ? specialty.Name
                        : null
            };

        if (
            !string.IsNullOrWhiteSpace(query)
            &&
            !string.Equals(
                query,
                "undefined",
                StringComparison.OrdinalIgnoreCase
            )
            &&
            !string.Equals(
                query,
                "null",
                StringComparison.OrdinalIgnoreCase
            )
        )
        {
            var keyword =
                query.Trim();

            joinedQuery =
                joinedQuery.Where(
                    x =>
                        x.Room.Id.Contains(keyword)
                        ||
                        x.Room.Name.Contains(keyword)
                        ||
                        x.Room.Type.Contains(keyword)
                        ||
                        x.Room.FacilityId.Contains(keyword)
                        ||
                        (
                            x.Room.Floor != null
                            &&
                            x.Room.Floor.Contains(keyword)
                        )
                        ||
                        (
                            x.FacilityName != null
                            &&
                            x.FacilityName.Contains(keyword)
                        )
                        ||
                        (
                            x.SpecialtyName != null
                            &&
                            x.SpecialtyName.Contains(keyword)
                        )
                );
        }

        var raw =
            await joinedQuery
                .OrderBy(x => x.Room.Id)
                .ToListAsync(
                    cancellationToken
                );

        return raw
            .Select(
                x => MapResponse(
                    x.Room,
                    x.FacilityName,
                    x.SpecialtyName
                )
            )
            .ToList();
    }

    // =====================================================
    // ENTITY DETAIL
    // =====================================================

    public Task<Room?> FindByIdAsync(
        string id,
        bool tracking,
        CancellationToken cancellationToken = default
    )
    {
        IQueryable<Room> query =
            Rooms;

        if (!tracking)
        {
            query =
                query.AsNoTracking();
        }

        return query.FirstOrDefaultAsync(
            x => x.Id == id,
            cancellationToken
        );
    }

    // =====================================================
    // RESPONSE DETAIL
    // =====================================================

    public async Task<RoomResponse?> GetResponseByIdAsync(
        string id,
        CancellationToken cancellationToken = default
    )
    {
        var result =
            await (
                from room
                    in Rooms.AsNoTracking()

                join facilityBase
                    in Facilities.AsNoTracking()
                    on room.FacilityId
                    equals facilityBase.Id
                    into facilityGroup

                from facility
                    in facilityGroup.DefaultIfEmpty()

                join specialtyBase
                    in Specialties.AsNoTracking()
                    on room.SpecialtyId
                    equals specialtyBase.Id
                    into specialtyGroup

                from specialty
                    in specialtyGroup.DefaultIfEmpty()

                where room.Id == id

                select new
                {
                    Room = room,

                    FacilityName =
                        facility != null
                            ? facility.Name
                            : null,

                    SpecialtyName =
                        specialty != null
                            ? specialty.Name
                            : null
                }
            )
            .FirstOrDefaultAsync(
                cancellationToken
            );

        if (result is null)
        {
            return null;
        }

        return MapResponse(
            result.Room,
            result.FacilityName,
            result.SpecialtyName
        );
    }

    // =====================================================
    // IDS
    // =====================================================

    public Task<List<string>> GetAllIdsAsync(
        CancellationToken cancellationToken = default
    )
    {
        return Rooms
            .AsNoTracking()
            .Select(x => x.Id)
            .ToListAsync(
                cancellationToken
            );
    }

    // =====================================================
    // FK CHECK
    // =====================================================

    public Task<bool> FacilityExistsAsync(
        string facilityId,
        CancellationToken cancellationToken = default
    )
    {
        return Facilities.AnyAsync(
            x => x.Id == facilityId,
            cancellationToken
        );
    }

    public Task<bool> SpecialtyExistsAsync(
        string specialtyId,
        CancellationToken cancellationToken = default
    )
    {
        return Specialties.AnyAsync(
            x => x.Id == specialtyId,
            cancellationToken
        );
    }

    // =====================================================
    // ADD
    // =====================================================

    public Task AddAsync(
        Room room,
        CancellationToken cancellationToken = default
    )
    {
        return Rooms
            .AddAsync(
                room,
                cancellationToken
            )
            .AsTask();
    }

    public Task SaveChangesAsync(
        CancellationToken cancellationToken = default
    )
    {
        return _dbContext.SaveChangesAsync(
            cancellationToken
        );
    }

    // =====================================================
    // MAPPER
    // =====================================================

    private static RoomResponse MapResponse(
        Room room,
        string? facilityName,
        string? specialtyName
    )
    {
        return new RoomResponse
        {
            Id =
                room.Id,

            TenPhong =
                room.Name,

            LoaiPhong =
                MapDbTypeToApi(
                    room.Type
                ),

            DbLoaiPhong =
                room.Type,

            IdCoSo =
                room.FacilityId,

            TenCoSo =
                facilityName,

            IdChuyenKhoa =
                room.SpecialtyId,

            TenChuyenKhoa =
                specialtyName,

            Tang =
                room.Floor,

            TrangThai =
                MapDbStatusToApi(
                    room.Status
                ),

            Status =
                room.Status
        };
    }

    private static string MapDbTypeToApi(
        string type
    )
    {
        return type switch
        {
            "kham" =>
                "EXAMINATION",

            "lay_mau" =>
                "SAMPLING",

            "xet_nghiem" =>
                "LAB",

            "tu_van" =>
                "CONSULTATION",

            _ =>
                "OTHER"
        };
    }

    private static string MapDbStatusToApi(
        string status
    )
    {
        return status switch
        {
            "maintenance" =>
                "MAINTENANCE",

            "inactive" =>
                "INACTIVE",

            _ =>
                "AVAILABLE"
        };
    }
}