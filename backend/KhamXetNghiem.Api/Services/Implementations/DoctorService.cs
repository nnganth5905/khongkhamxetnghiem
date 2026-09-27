using KhamXetNghiem.Api.DTOs.Requests;
using KhamXetNghiem.Api.DTOs.Responses;
using KhamXetNghiem.Api.Exceptions;
using KhamXetNghiem.Api.Repositories.Interfaces;
using KhamXetNghiem.Api.Services.Interfaces;

namespace KhamXetNghiem.Api.Services.Implementations;

public sealed class DoctorService
    : IDoctorService
{
    private readonly IDoctorRepository
        _doctorRepository;

    private readonly IDoctorResultRepository
        _doctorResultRepository;

    public DoctorService(
        IDoctorRepository doctorRepository,
        IDoctorResultRepository doctorResultRepository
    )
    {
        _doctorRepository =
            doctorRepository;

        _doctorResultRepository =
            doctorResultRepository;
    }

    // =====================================================
    // DOCTOR CRUD
    // =====================================================

    public Task<List<DoctorResponse>> GetDoctorsAsync(
        string? specialtyId,
        string? query,
        decimal? minimumStars,
        bool activeOnly,
        CancellationToken cancellationToken = default
    )
    {
        return _doctorRepository
            .GetAllAsync(
                specialtyId,
                query,
                minimumStars,
                activeOnly,
                cancellationToken
            );
    }

    public async Task<DoctorResponse> GetDoctorByIdAsync(
        int id,
        CancellationToken cancellationToken = default
    )
    {
        var doctor =
            await _doctorRepository
                .GetByIdAsync(
                    id,
                    cancellationToken
                );

        if (doctor is null)
        {
            throw new ResourceNotFoundException(
                $"Không tìm thấy bác sĩ với mã: {id}"
            );
        }

        return doctor;
    }

    public Task<DoctorResponse> CreateDoctorAsync(
        DoctorRequest request,
        CancellationToken cancellationToken = default
    )
    {
        return _doctorRepository
            .CreateAsync(
                request,
                cancellationToken
            );
    }

    public Task<DoctorResponse> UpdateDoctorAsync(
        int id,
        DoctorRequest request,
        CancellationToken cancellationToken = default
    )
    {
        return _doctorRepository
            .UpdateAsync(
                id,
                request,
                cancellationToken
            );
    }

    public Task DeleteDoctorAsync(
        int id,
        CancellationToken cancellationToken = default
    )
    {
        return _doctorRepository
            .SoftDeleteAsync(
                id,
                cancellationToken
            );
    }

    // =====================================================
    // RESULT WORKFLOW
    // =====================================================

    public async Task<List<PendingDoctorResultResponse>>
        GetPendingResultsAsync(
            string login,
            CancellationToken cancellationToken = default
        )
    {
        var doctor =
            await RequireDoctorAsync(
                login,
                cancellationToken
            );

        return await _doctorResultRepository
            .GetPendingAsync(
                doctor.DoctorId,
                cancellationToken
            );
    }

    public async Task<DoctorResultDetailResponse>
        GetResultDetailAsync(
            string resultId,
            string login,
            CancellationToken cancellationToken = default
        )
    {
        var doctor =
            await RequireDoctorAsync(
                login,
                cancellationToken
            );

        var result =
            await _doctorResultRepository
                .GetDetailAsync(
                    resultId,
                    doctor.DoctorId,
                    cancellationToken
                );

        if (result is null)
        {
            throw new ResourceNotFoundException(
                "Không tìm thấy kết quả hoặc bạn không được phân công kết quả này."
            );
        }

        return result;
    }

    public async Task<ApproveResultResponse>
        ApproveResultAsync(
            string resultId,
            string conclusion,
            string login,
            CancellationToken cancellationToken = default
        )
    {
        var doctor =
            await RequireDoctorAsync(
                login,
                cancellationToken
            );

        return await _doctorResultRepository
            .ApproveAsync(
                resultId,
                conclusion?.Trim()
                    ?? string.Empty,
                doctor,
                cancellationToken
            );
    }

    // =====================================================
    // REQUIRE DOCTOR
    // =====================================================

    private async Task<DoctorIdentity>
        RequireDoctorAsync(
            string login,
            CancellationToken cancellationToken
        )
    {
        if (
            string.IsNullOrWhiteSpace(
                login
            )
        )
        {
            throw new ForbiddenException(
                "Không xác định được tài khoản bác sĩ."
            );
        }

        var doctor =
            await _doctorResultRepository
                .FindDoctorIdentityAsync(
                    login,
                    cancellationToken
                );

        /*
         * FIX Java cũ:
         * tuyệt đối không fallback IDBacSi = 1.
         */
        if (doctor is null)
        {
            throw new ForbiddenException(
                "Tài khoản này chưa được liên kết với hồ sơ bác sĩ."
            );
        }

        return doctor;
    }
}