# BioMedic - Java to ASP.NET Core migration progress

## Completed

- Backend targets .NET 10 and runs with Swagger/JWT/MySQL.
- Authentication: login/register/me/logout + forgot/reset password.
- Legacy plaintext passwords are upgraded to BCrypt after the first successful login.
- Admin CRUD: customers, employees, doctors, technicians, tests, specialties, rooms, work schedules.
- Notifications: list, mark one as read, mark all as read.
- Appointment/check-in flows and receptionist waiting list.
- Doctor queue is database-backed and now has two compatible API contracts:
  - legacy frontend route: `/api/appointments/waiting-queue` and `/api/appointments/queue/{id}/...`
  - standardized route: `/api/doctor/queue` and `/api/doctor/queue/{id}/...`
- Numeric visit operations remain under `/api/doctor/visits/...` to avoid route collisions.
- Examination workflow now enforces doctor ownership before reading/updating/completing a medical record.
- Completing an examination also completes the related appointment and writes tracking data.
- Public doctor/test dropdown endpoints were converted from raw SQL/silent fallback to EF Core async queries.
- Removed the unused exception warning in `DoctorController`.

## Important fixes in this pass

1. Resolved the conflicting meaning of `/api/doctor/queue`.
2. Added `cho_kham` and `cho_xet_nghiem` states to the doctor queue query.
3. Prevented one doctor from opening/updating another doctor's examination by guessing a visit id.
4. Added tracking for create/update/start/complete examination actions.
5. Replaced direct synchronous SQL in `TestController` and `DoctorController` with EF Core queries.

## Still to implement

- Full technician workflow: specimen receipt/collection/handover, worklist, result entry and submit-for-approval.
- Customer result lookup / print / PDF endpoints.
- Tracking query endpoints.
- Non-empty doctor/customer/technician dashboard endpoints.
- Final React integration verification against the frontend project.

## Verification on the user's machine

Run:

```bat
dotnet clean
dotnet build
dotnet run
```

Then open `http://localhost:5179/swagger`.

> This editing environment does not have the .NET SDK installed, so final compilation must be verified on the user's Windows machine.
