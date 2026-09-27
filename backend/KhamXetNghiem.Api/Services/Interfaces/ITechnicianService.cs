using System.Security.Claims;
using KhamXetNghiem.Api.DTOs.Requests;
using KhamXetNghiem.Api.DTOs.Responses;

namespace KhamXetNghiem.Api.Services.Interfaces;

public interface ITechnicianService
{
    Task<CurrentTechnicianResponse> GetCurrentTechnicianAsync(
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    );

    Task<List<TechnicianResponse>> GetTechniciansAsync(
        string? query,
        CancellationToken cancellationToken = default
    );

    Task<TechnicianResponse> GetTechnicianByIdAsync(
        string id,
        CancellationToken cancellationToken = default
    );

    Task<TechnicianResponse> CreateTechnicianAsync(
        TechnicianRequest request,
        CancellationToken cancellationToken = default
    );

    Task<TechnicianResponse> UpdateTechnicianAsync(
        string id,
        TechnicianRequest request,
        CancellationToken cancellationToken = default
    );

    Task DeleteTechnicianAsync(
        string id,
        CancellationToken cancellationToken = default
    );

    Task<List<SpecimenListItemResponse>> GetSpecimensAsync(
        string? status,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    );

    Task<SpecimenDetailResponse> GetSpecimenAsync(
        string specimenId,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    );

    Task ReceiveSpecimenAsync(
        string specimenId,
        ReceiveSpecimenRequest request,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    );

    Task RejectSpecimenAsync(
        string specimenId,
        RejectSpecimenRequest request,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    );

    Task<List<WorklistItemResponse>> GetWorklistAsync(
        string? status,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    );

    Task StartWorkAsync(
        long id,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    );

    Task CompleteWorkAsync(
        long id,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    );

    Task<ResultEntryResponse> GetResultEntryAsync(
        long id,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    );

    Task SaveResultAsync(
        long id,
        ResultEntryRequest request,
        ClaimsPrincipal user,
        bool submit,
        CancellationToken cancellationToken = default
    );

    Task SubmitResultBySpecimenAsync(
        string specimenId,
        ResultEntryRequest request,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default
    );
}