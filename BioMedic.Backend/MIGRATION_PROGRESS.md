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

## Refactor 2 - Laboratory workflow

Completed in this refactor:

- Added doctor specimen collection endpoint compatible with the existing React frontend:
  - `POST /api/doctor/specimens`
  - `POST /api/doctor/specimens/{id}/handover`
  - `GET /api/technicians`
- Added technician profile and specimen workflow:
  - `GET /api/technician/me`
  - `GET /api/technician/specimens`
  - `GET /api/technician/specimens/{id}`
  - `POST /api/technician/specimens/{id}/receive`
  - `POST /api/technician/specimens/{id}/reject`
  - `PUT /api/technician/specimens/{id}`
- Added worklist workflow:
  - `GET /api/technician/worklist`
  - `GET /api/technician/worklist/{id}`
  - `POST /api/technician/worklist/{id}/start`
  - `POST /api/technician/worklist/{id}/complete`
- Added result-entry workflow:
  - `GET /api/technician/results/{worklistId}`
  - `PUT /api/technician/results/{worklistId}`
  - `POST /api/technician/results/{worklistId}/submit`
- Result entry uses the real `chisoxetnghiem` records and preserves saved draft values.
- Technician identity is taken from the authenticated JWT account instead of a hard-coded KTV id.
- Doctor approval now creates a customer notification and tracking/audit record.
- Added customer result APIs:
  - `GET /api/results/mine`
  - `GET /api/results/{id}`

The endpoint shapes intentionally match the current React `technicianService.js` so frontend integration requires fewer changes.
- Replaced the empty technician dashboard response with live specimen/worklist counts and worklist rows (`GET /api/dashboard/technician`).
- Allowed authenticated doctors to read appointment detail required by the sample-collection screen.
