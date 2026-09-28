namespace KhamXetNghiem.Api.DTOs.Responses;

// ============================================================
// COMMON
// ============================================================

public sealed class DashboardUpcomingAppointment
{
    public string Id { get; init; } = string.Empty;

    public string Date { get; init; } = string.Empty;

    public string Time { get; init; } = string.Empty;

    public string PatientName { get; init; } = string.Empty;

    public string Type { get; init; } = string.Empty;

    public string ServiceName { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;
}

// ============================================================
// RECEPTIONIST
// ============================================================

public sealed class ReceptionistDashboardResponse
{
    public int TodayAppointments { get; init; }

    public int CheckedIn { get; init; }

    public int Waiting { get; init; }

    public int WalkIns { get; init; }

    public List<DashboardUpcomingAppointment>
        UpcomingAppointments { get; init; } = [];
}

// ============================================================
// DOCTOR
// ============================================================

public sealed class DoctorNextPatientResponse
{
    public long Id { get; init; }

    public int? QueueNumber { get; init; }

    public string PatientCode { get; init; } = string.Empty;

    public string PatientName { get; init; } = string.Empty;

    public string Time { get; init; } = string.Empty;

    public string Reason { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;
}

public sealed class DoctorPendingResultDashboardResponse
{
    public string Id { get; init; } = string.Empty;

    public string PatientName { get; init; } = string.Empty;

    public string TestName { get; init; } = string.Empty;

    public string TechnicianName { get; init; } = string.Empty;

    public DateTime? SubmittedAt { get; init; }

    public bool HasAbnormalIndicator { get; init; }
}

public sealed class DoctorDashboardResponse
{
    public bool Success { get; init; } = true;

    public int WaitingCount { get; init; }

    public int TodayVisits { get; init; }

    // Giữ tương thích frontend cũ.
    public int TodayExamsCount => TodayVisits;

    public int PendingApprovals { get; init; }

    public int PendingResultsCount => PendingApprovals;

    public int CompletedToday { get; init; }

    public int CompletedTodayCount => CompletedToday;

    public string CurrentRoom { get; init; } = "—";

    public List<DoctorNextPatientResponse>
        NextPatients { get; init; } = [];

    public List<DoctorPendingResultDashboardResponse>
        PendingResults { get; init; } = [];
}

// ============================================================
// TECHNICIAN
// ============================================================

public sealed class TechnicianWorklistDashboardResponse
{
    public long Id { get; init; }

    public string? SpecimenId { get; init; }

    public string SpecimenCode { get; init; } = string.Empty;

    public string PatientName { get; init; } = string.Empty;

    public string TestName { get; init; } = string.Empty;

    public string? TechnicianId { get; init; }

    public string Priority { get; init; } = "NORMAL";

    public string Status { get; init; } = string.Empty;

    public DateTime? ReceivedAt { get; init; }

    public DateTime? StartedAt { get; init; }

    public DateTime? FinishedAt { get; init; }
}

public sealed class TechnicianDashboardResponse
{
    public bool Success { get; init; } = true;

    public string TechnicianId { get; init; } = string.Empty;

    public int WaitingSpecimens { get; init; }

    public int ReceivedSpecimens { get; init; }

    public int InProgress { get; init; }

    public int PendingResultEntries { get; init; }

    public List<TechnicianWorklistDashboardResponse>
        Worklist { get; init; } = [];
}

// ============================================================
// ADMIN
// ============================================================

public sealed class AdminDashboardResponse
{
    public bool Success { get; init; } = true;

    public int TotalCustomers { get; init; }

    public int TotalDoctors { get; init; }

    public int TotalEmployees { get; init; }

    public int TotalTechnicians { get; init; }

    public int TodayAppointments { get; init; }

    public int CheckedInToday { get; init; }

    public int WaitingNow { get; init; }

    public int PendingResults { get; init; }

    public int ActiveWorklists { get; init; }

    public List<DashboardUpcomingAppointment>
        RecentAppointments { get; init; } = [];
}

// ============================================================
// CUSTOMER
// ============================================================

public sealed class CustomerDashboardResponse
{
    public bool Success { get; init; } = true;

    public int TodayAppointments { get; init; }

    public int UpcomingAppointmentsCount { get; init; }

    public int ActiveVisits { get; init; }

    public int ApprovedResults { get; init; }

    public List<DashboardUpcomingAppointment>
        UpcomingAppointments { get; init; } = [];
}