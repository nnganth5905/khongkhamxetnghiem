using KhamXetNghiem.Api.DTOs.Requests;
using KhamXetNghiem.Api.DTOs.Responses;

namespace KhamXetNghiem.Api.Repositories.Interfaces;

public interface ITechnicianWorkflowRepository
{
    Task<TechnicianIdentity?> FindCurrentTechnicianAsync(
        string login,
        CancellationToken cancellationToken = default
    );

    Task<List<SpecimenListItemResponse>> GetSpecimensAsync(
        string? status,
        CancellationToken cancellationToken = default
    );

    Task<SpecimenDetailResponse?> GetSpecimenAsync(
        string specimenId,
        CancellationToken cancellationToken = default
    );

    Task ReceiveSpecimenAsync(
        string specimenId,
        ReceiveSpecimenRequest request,
        TechnicianIdentity technician,
        CancellationToken cancellationToken = default
    );

    Task RejectSpecimenAsync(
        string specimenId,
        RejectSpecimenRequest request,
        TechnicianIdentity technician,
        CancellationToken cancellationToken = default
    );

    Task<List<WorklistItemResponse>> GetWorklistAsync(
        string? status,
        TechnicianIdentity technician,
        CancellationToken cancellationToken = default
    );

    Task StartWorkAsync(
        long worklistId,
        TechnicianIdentity technician,
        CancellationToken cancellationToken = default
    );

    Task CompleteWorkAsync(
        long worklistId,
        TechnicianIdentity technician,
        CancellationToken cancellationToken = default
    );

    Task<ResultEntryResponse?> GetResultEntryAsync(
        long worklistId,
        TechnicianIdentity technician,
        CancellationToken cancellationToken = default
    );

    Task SaveResultAsync(
        long worklistId,
        ResultEntryRequest request,
        TechnicianIdentity technician,
        bool submit,
        CancellationToken cancellationToken = default
    );

    Task<long?> FindWorklistIdBySpecimenAsync(
        string specimenId,
        TechnicianIdentity technician,
        CancellationToken cancellationToken = default
    );
}