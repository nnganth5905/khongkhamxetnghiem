using System.Data;
using System.Data.Common;

using KhamXetNghiem.Api.Data;
using KhamXetNghiem.Api.DTOs.Responses;
using KhamXetNghiem.Api.Entities;
using KhamXetNghiem.Api.Repositories.Interfaces;

namespace KhamXetNghiem.Api.Repositories.Implementations;

public sealed class WorkScheduleRepository
    : IWorkScheduleRepository
{
    private readonly AppDbContext _dbContext;

    public WorkScheduleRepository(
        AppDbContext dbContext
    )
    {
        _dbContext = dbContext;
    }

    // =====================================================
    // ADMIN LIST
    // =====================================================

    public async Task<List<WorkScheduleResponse>> GetAllAsync(
        string? query,
        CancellationToken cancellationToken = default
    )
    {
        var connection =
            _dbContext.Database.GetDbConnection();

        var closeAfter =
            connection.State != ConnectionState.Open;

        if (closeAfter)
        {
            await connection.OpenAsync(
                cancellationToken
            );
        }

        try
        {
            await using var command =
                connection.CreateCommand();

            command.CommandText =
                """
                SELECT
                    l.LichID,
                    l.IDBacSi,
                    bs.TenBacSi,
                    l.IDPhong,
                    p.TenPhong,
                    l.Ngay,
                    l.Ca,
                    l.GioBatDau,
                    l.GioKetThuc,
                    l.NguonTao,
                    l.NguoiTaoUserID,
                    l.TrangThai,
                    l.GhiChu,
                    l.CreatedAt

                FROM lichlamviec l

                INNER JOIN bacsi bs
                    ON l.IDBacSi =
                       bs.IDBacSi

                LEFT JOIN phong p
                    ON l.IDPhong =
                       p.IDPhong

                WHERE
                    @query IS NULL

                    OR CAST(
                        l.LichID AS CHAR
                    ) LIKE @search

                    OR CAST(
                        l.IDBacSi AS CHAR
                    ) LIKE @search

                    OR bs.TenBacSi
                        LIKE @search

                    OR l.IDPhong
                        LIKE @search

                    OR p.TenPhong
                        LIKE @search

                    OR CAST(
                        l.Ngay AS CHAR
                    ) LIKE @search

                ORDER BY
                    l.Ngay DESC,
                    l.GioBatDau ASC,
                    l.LichID DESC
                """;

            var keyword =
                NormalizeNullable(query);

            AddParameter(
                command,
                "@query",
                keyword
            );

            AddParameter(
                command,
                "@search",
                keyword is null
                    ? null
                    : $"%{keyword}%"
            );

            return await ReadScheduleListAsync(
                command,
                cancellationToken
            );
        }
        finally
        {
            if (closeAfter)
            {
                await connection.CloseAsync();
            }
        }
    }

    // =====================================================
    // DOCTOR SCHEDULE
    // =====================================================

    public async Task<List<WorkScheduleResponse>>
        GetDoctorSchedulesAsync(
            int doctorId,
            DateTime? date,
            CancellationToken cancellationToken = default
        )
    {
        var connection =
            _dbContext.Database.GetDbConnection();

        var closeAfter =
            connection.State != ConnectionState.Open;

        if (closeAfter)
        {
            await connection.OpenAsync(
                cancellationToken
            );
        }

        try
        {
            await using var command =
                connection.CreateCommand();

            command.CommandText =
                """
                SELECT
                    l.LichID,
                    l.IDBacSi,
                    bs.TenBacSi,
                    l.IDPhong,
                    p.TenPhong,
                    l.Ngay,
                    l.Ca,
                    l.GioBatDau,
                    l.GioKetThuc,
                    l.NguonTao,
                    l.NguoiTaoUserID,
                    l.TrangThai,
                    l.GhiChu,
                    l.CreatedAt

                FROM lichlamviec l

                INNER JOIN bacsi bs
                    ON l.IDBacSi =
                       bs.IDBacSi

                LEFT JOIN phong p
                    ON l.IDPhong =
                       p.IDPhong

                WHERE
                    l.IDBacSi =
                        @doctorId

                    AND l.TrangThai =
                        'duoc_duyet'

                    AND
                    (
                        @date IS NULL
                        OR l.Ngay =
                           @date
                    )

                ORDER BY
                    l.Ngay ASC,
                    l.GioBatDau ASC
                """;

            AddParameter(
                command,
                "@doctorId",
                doctorId
            );

            AddParameter(
                command,
                "@date",
                date?.Date
            );

            return await ReadScheduleListAsync(
                command,
                cancellationToken
            );
        }
        finally
        {
            if (closeAfter)
            {
                await connection.CloseAsync();
            }
        }
    }

    // =====================================================
    // FIND ENTITY
    // =====================================================

    public async Task<WorkSchedule?> FindByIdAsync(
        long id,
        CancellationToken cancellationToken = default
    )
    {
        var connection =
            _dbContext.Database.GetDbConnection();

        var closeAfter =
            connection.State != ConnectionState.Open;

        if (closeAfter)
        {
            await connection.OpenAsync(
                cancellationToken
            );
        }

        try
        {
            await using var command =
                connection.CreateCommand();

            command.CommandText =
                """
                SELECT
                    LichID,
                    IDBacSi,
                    IDPhong,
                    Ngay,
                    Ca,
                    GioBatDau,
                    GioKetThuc,
                    NguonTao,
                    NguoiTaoUserID,
                    TrangThai,
                    GhiChu,
                    CreatedAt,
                    UpdatedAt

                FROM lichlamviec

                WHERE LichID =
                    @id

                LIMIT 1
                """;

            AddParameter(
                command,
                "@id",
                id
            );

            await using var reader =
                await command.ExecuteReaderAsync(
                    cancellationToken
                );

            if (
                !await reader.ReadAsync(
                    cancellationToken
                )
            )
            {
                return null;
            }

            return new WorkSchedule
            {
                Id =
                    Convert.ToInt64(
                        reader["LichID"]
                    ),

                DoctorId =
                    Convert.ToInt32(
                        reader["IDBacSi"]
                    ),

                RoomId =
                    ReadNullableString(
                        reader,
                        "IDPhong"
                    ),

                WorkDate =
                    Convert.ToDateTime(
                        reader["Ngay"]
                    ).Date,

                Shift =
                    Convert.ToString(
                        reader["Ca"]
                    )
                    ?? "TuyChinh",

                StartTime =
                    ReadTimeSpan(
                        reader["GioBatDau"]
                    ),

                EndTime =
                    ReadTimeSpan(
                        reader["GioKetThuc"]
                    ),

                Source =
                    Convert.ToString(
                        reader["NguonTao"]
                    )
                    ?? "admin",

                CreatedByUserId =
                    ReadNullableInt32(
                        reader,
                        "NguoiTaoUserID"
                    ),

                Status =
                    Convert.ToString(
                        reader["TrangThai"]
                    )
                    ?? "duoc_duyet",

                Note =
                    ReadNullableString(
                        reader,
                        "GhiChu"
                    ),

                CreatedAt =
                    ReadNullableDateTime(
                        reader,
                        "CreatedAt"
                    ),

                UpdatedAt =
                    ReadNullableDateTime(
                        reader,
                        "UpdatedAt"
                    )
            };
        }
        finally
        {
            if (closeAfter)
            {
                await connection.CloseAsync();
            }
        }
    }

    // =====================================================
    // RESPONSE BY ID
    // =====================================================

    public async Task<WorkScheduleResponse?>
        GetResponseByIdAsync(
            long id,
            CancellationToken cancellationToken = default
        )
    {
        var connection =
            _dbContext.Database.GetDbConnection();

        var closeAfter =
            connection.State != ConnectionState.Open;

        if (closeAfter)
        {
            await connection.OpenAsync(
                cancellationToken
            );
        }

        try
        {
            await using var command =
                connection.CreateCommand();

            command.CommandText =
                """
                SELECT
                    l.LichID,
                    l.IDBacSi,
                    bs.TenBacSi,
                    l.IDPhong,
                    p.TenPhong,
                    l.Ngay,
                    l.Ca,
                    l.GioBatDau,
                    l.GioKetThuc,
                    l.NguonTao,
                    l.NguoiTaoUserID,
                    l.TrangThai,
                    l.GhiChu,
                    l.CreatedAt

                FROM lichlamviec l

                INNER JOIN bacsi bs
                    ON l.IDBacSi =
                       bs.IDBacSi

                LEFT JOIN phong p
                    ON l.IDPhong =
                       p.IDPhong

                WHERE
                    l.LichID =
                        @id

                LIMIT 1
                """;

            AddParameter(
                command,
                "@id",
                id
            );

            var items =
                await ReadScheduleListAsync(
                    command,
                    cancellationToken
                );

            return items.FirstOrDefault();
        }
        finally
        {
            if (closeAfter)
            {
                await connection.CloseAsync();
            }
        }
    }

    // =====================================================
    // DOCTOR VALIDATION
    // =====================================================

    public async Task<bool> DoctorExistsAsync(
        int doctorId,
        CancellationToken cancellationToken = default
    )
    {
        return await ExecuteExistsAsync(
            """
            SELECT COUNT(*)

            FROM bacsi

            WHERE
                IDBacSi = @doctorId
                AND TrangThai = 'active'
            """,
            cancellationToken,
            ("@doctorId", doctorId)
        );
    }

    // =====================================================
    // ROOM VALIDATION
    // =====================================================

    public async Task<bool> RoomExistsAndActiveAsync(
        string roomId,
        CancellationToken cancellationToken = default
    )
    {
        return await ExecuteExistsAsync(
            """
            SELECT COUNT(*)

            FROM phong

            WHERE
                IDPhong = @roomId
                AND TrangThai = 'active'
            """,
            cancellationToken,
            ("@roomId", roomId)
        );
    }

    // =====================================================
    // DOCTOR OVERLAP
    // =====================================================

    public async Task<bool> HasDoctorOverlapAsync(
        int doctorId,
        DateTime date,
        TimeSpan start,
        TimeSpan end,
        long? excludeScheduleId,
        CancellationToken cancellationToken = default
    )
    {
        return await ExecuteExistsAsync(
            """
            SELECT COUNT(*)

            FROM lichlamviec

            WHERE
                IDBacSi =
                    @doctorId

                AND Ngay =
                    @date

                AND TrangThai =
                    'duoc_duyet'

                AND
                (
                    @excludeId IS NULL
                    OR LichID <> @excludeId
                )

                AND GioBatDau <
                    @end

                AND GioKetThuc >
                    @start
            """,
            cancellationToken,

            ("@doctorId", doctorId),
            ("@date", date.Date),
            ("@excludeId", excludeScheduleId),
            ("@start", start),
            ("@end", end)
        );
    }

    // =====================================================
    // ROOM OVERLAP
    // =====================================================

    public async Task<bool> HasRoomOverlapAsync(
        string roomId,
        DateTime date,
        TimeSpan start,
        TimeSpan end,
        long? excludeScheduleId,
        CancellationToken cancellationToken = default
    )
    {
        return await ExecuteExistsAsync(
            """
            SELECT COUNT(*)

            FROM lichlamviec

            WHERE
                IDPhong =
                    @roomId

                AND Ngay =
                    @date

                AND TrangThai =
                    'duoc_duyet'

                AND
                (
                    @excludeId IS NULL
                    OR LichID <> @excludeId
                )

                AND GioBatDau <
                    @end

                AND GioKetThuc >
                    @start
            """,
            cancellationToken,

            ("@roomId", roomId),
            ("@date", date.Date),
            ("@excludeId", excludeScheduleId),
            ("@start", start),
            ("@end", end)
        );
    }

    // =====================================================
    // CREATOR USER
    // =====================================================

    public async Task<int?> FindUserIdByLoginAsync(
        string login,
        CancellationToken cancellationToken = default
    )
    {
        if (
            string.IsNullOrWhiteSpace(login)
        )
        {
            return null;
        }

        var connection =
            _dbContext.Database.GetDbConnection();

        var closeAfter =
            connection.State != ConnectionState.Open;

        if (closeAfter)
        {
            await connection.OpenAsync(
                cancellationToken
            );
        }

        try
        {
            await using var command =
                connection.CreateCommand();

            command.CommandText =
                """
                SELECT UserID

                FROM users

                WHERE
                    Email = @login
                    OR Username = @login

                LIMIT 1
                """;

            AddParameter(
                command,
                "@login",
                login
            );

            var value =
                await command.ExecuteScalarAsync(
                    cancellationToken
                );

            if (
                value is null
                ||
                value == DBNull.Value
            )
            {
                return null;
            }

            return Convert.ToInt32(
                value
            );
        }
        finally
        {
            if (closeAfter)
            {
                await connection.CloseAsync();
            }
        }
    }

    // =====================================================
    // CREATE
    // =====================================================

    public async Task<long> CreateAsync(
        WorkSchedule schedule,
        CancellationToken cancellationToken = default
    )
    {
        var connection =
            _dbContext.Database.GetDbConnection();

        var closeAfter =
            connection.State != ConnectionState.Open;

        if (closeAfter)
        {
            await connection.OpenAsync(
                cancellationToken
            );
        }

        try
        {
            await using var command =
                connection.CreateCommand();

            command.CommandText =
                """
                INSERT INTO lichlamviec
                (
                    IDBacSi,
                    IDPhong,
                    Ngay,
                    Ca,
                    GioBatDau,
                    GioKetThuc,
                    NguonTao,
                    NguoiTaoUserID,
                    TrangThai,
                    GhiChu
                )
                VALUES
                (
                    @doctorId,
                    @roomId,
                    @date,
                    @shift,
                    @start,
                    @end,
                    @source,
                    @userId,
                    @status,
                    @note
                );

                SELECT LAST_INSERT_ID();
                """;

            AddParameter(
                command,
                "@doctorId",
                schedule.DoctorId
            );

            AddParameter(
                command,
                "@roomId",
                schedule.RoomId
            );

            AddParameter(
                command,
                "@date",
                schedule.WorkDate.Date
            );

            AddParameter(
                command,
                "@shift",
                schedule.Shift
            );

            AddParameter(
                command,
                "@start",
                schedule.StartTime
            );

            AddParameter(
                command,
                "@end",
                schedule.EndTime
            );

            AddParameter(
                command,
                "@source",
                schedule.Source
            );

            AddParameter(
                command,
                "@userId",
                schedule.CreatedByUserId
            );

            AddParameter(
                command,
                "@status",
                schedule.Status
            );

            AddParameter(
                command,
                "@note",
                schedule.Note
            );

            var value =
                await command.ExecuteScalarAsync(
                    cancellationToken
                );

            return Convert.ToInt64(
                value
            );
        }
        finally
        {
            if (closeAfter)
            {
                await connection.CloseAsync();
            }
        }
    }

    // =====================================================
    // UPDATE
    // =====================================================

    public async Task UpdateAsync(
        WorkSchedule schedule,
        CancellationToken cancellationToken = default
    )
    {
        var connection =
            _dbContext.Database.GetDbConnection();

        var closeAfter =
            connection.State != ConnectionState.Open;

        if (closeAfter)
        {
            await connection.OpenAsync(
                cancellationToken
            );
        }

        try
        {
            await using var command =
                connection.CreateCommand();

            command.CommandText =
                """
                UPDATE lichlamviec

                SET
                    IDBacSi =
                        @doctorId,

                    IDPhong =
                        @roomId,

                    Ngay =
                        @date,

                    Ca =
                        @shift,

                    GioBatDau =
                        @start,

                    GioKetThuc =
                        @end,

                    TrangThai =
                        @status,

                    GhiChu =
                        @note

                WHERE
                    LichID =
                        @id
                """;

            AddParameter(
                command,
                "@doctorId",
                schedule.DoctorId
            );

            AddParameter(
                command,
                "@roomId",
                schedule.RoomId
            );

            AddParameter(
                command,
                "@date",
                schedule.WorkDate.Date
            );

            AddParameter(
                command,
                "@shift",
                schedule.Shift
            );

            AddParameter(
                command,
                "@start",
                schedule.StartTime
            );

            AddParameter(
                command,
                "@end",
                schedule.EndTime
            );

            AddParameter(
                command,
                "@status",
                schedule.Status
            );

            AddParameter(
                command,
                "@note",
                schedule.Note
            );

            AddParameter(
                command,
                "@id",
                schedule.Id
            );

            await command.ExecuteNonQueryAsync(
                cancellationToken
            );
        }
        finally
        {
            if (closeAfter)
            {
                await connection.CloseAsync();
            }
        }
    }

    // =====================================================
    // COMMON EXISTS
    // =====================================================

    private async Task<bool> ExecuteExistsAsync(
        string sql,
        CancellationToken cancellationToken,
        params (string Name, object? Value)[] parameters
    )
    {
        var connection =
            _dbContext.Database.GetDbConnection();

        var closeAfter =
            connection.State != ConnectionState.Open;

        if (closeAfter)
        {
            await connection.OpenAsync(
                cancellationToken
            );
        }

        try
        {
            await using var command =
                connection.CreateCommand();

            command.CommandText =
                sql;

            foreach (
                var parameter in parameters
            )
            {
                AddParameter(
                    command,
                    parameter.Name,
                    parameter.Value
                );
            }

            var result =
                Convert.ToInt64(
                    await command.ExecuteScalarAsync(
                        cancellationToken
                    )
                    ?? 0
                );

            return result > 0;
        }
        finally
        {
            if (closeAfter)
            {
                await connection.CloseAsync();
            }
        }
    }

    // =====================================================
    // READER
    // =====================================================

    private static async Task<List<WorkScheduleResponse>>
        ReadScheduleListAsync(
            DbCommand command,
            CancellationToken cancellationToken
        )
    {
        var result =
            new List<WorkScheduleResponse>();

        await using var reader =
            await command.ExecuteReaderAsync(
                cancellationToken
            );

        while (
            await reader.ReadAsync(
                cancellationToken
            )
        )
        {
            var dbShift =
                Convert.ToString(
                    reader["Ca"]
                )
                ?? "TuyChinh";

            var dbStatus =
                Convert.ToString(
                    reader["TrangThai"]
                )
                ?? "duoc_duyet";

            result.Add(
                new WorkScheduleResponse
                {
                    Id =
                        Convert.ToInt64(
                            reader["LichID"]
                        ),

                    IdBacSi =
                        Convert.ToInt32(
                            reader["IDBacSi"]
                        ),

                    TenBacSi =
                        ReadNullableString(
                            reader,
                            "TenBacSi"
                        ),

                    IdPhong =
                        ReadNullableString(
                            reader,
                            "IDPhong"
                        ),

                    TenPhong =
                        ReadNullableString(
                            reader,
                            "TenPhong"
                        ),

                    NgayLamViec =
                        Convert.ToDateTime(
                            reader["Ngay"]
                        )
                        .ToString(
                            "yyyy-MM-dd"
                        ),

                    CaLamViec =
                        MapShiftToApi(
                            dbShift
                        ),

                    GioBatDau =
                        FormatTime(
                            ReadTimeSpan(
                                reader["GioBatDau"]
                            )
                        ),

                    GioKetThuc =
                        FormatTime(
                            ReadTimeSpan(
                                reader["GioKetThuc"]
                            )
                        ),

                    NguonTao =
                        Convert.ToString(
                            reader["NguonTao"]
                        )
                        ?? "admin",

                    NguoiTaoUserId =
                        ReadNullableInt32(
                            reader,
                            "NguoiTaoUserID"
                        ),

                    TrangThai =
                        dbStatus == "huy"
                            ? "CANCELLED"
                            : "ACTIVE",

                    DbTrangThai =
                        dbStatus,

                    GhiChu =
                        ReadNullableString(
                            reader,
                            "GhiChu"
                        ),

                    CreatedAt =
                        ReadNullableDateTime(
                            reader,
                            "CreatedAt"
                        )
                }
            );
        }

        return result;
    }

    // =====================================================
    // HELPERS
    // =====================================================

    private static string MapShiftToApi(
        string shift
    )
    {
        return shift switch
        {
            "Sang" =>
                "MORNING",

            "Chieu" =>
                "AFTERNOON",

            "Toi" =>
                "EVENING",

            _ =>
                "CUSTOM"
        };
    }

    private static string FormatTime(
        TimeSpan time
    )
    {
        return
            $"{(int)time.TotalHours:00}:{time.Minutes:00}";
    }

    private static TimeSpan ReadTimeSpan(
        object value
    )
    {
        if (value is TimeSpan timeSpan)
        {
            return timeSpan;
        }

        return TimeSpan.Parse(
            Convert.ToString(value)!
        );
    }

    private static string? ReadNullableString(
        DbDataReader reader,
        string column
    )
    {
        var value =
            reader[column];

        return value == DBNull.Value
            ? null
            : Convert.ToString(value);
    }

    private static int? ReadNullableInt32(
        DbDataReader reader,
        string column
    )
    {
        var value =
            reader[column];

        return value == DBNull.Value
            ? null
            : Convert.ToInt32(value);
    }

    private static DateTime? ReadNullableDateTime(
        DbDataReader reader,
        string column
    )
    {
        var value =
            reader[column];

        return value == DBNull.Value
            ? null
            : Convert.ToDateTime(value);
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

    private static void AddParameter(
        DbCommand command,
        string name,
        object? value
    )
    {
        var parameter =
            command.CreateParameter();

        parameter.ParameterName =
            name;

        parameter.Value =
            value ?? DBNull.Value;

        command.Parameters.Add(
            parameter
        );
    }
}