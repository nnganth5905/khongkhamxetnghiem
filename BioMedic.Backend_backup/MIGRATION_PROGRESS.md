# BioMedic - Java to ASP.NET Core migration progress

## Completed in this pass

- Retargeted backend to .NET 10 while keeping EF Core 8 / Pomelo 8 for MySQL provider compatibility.
- Kept React API contract under `/api`.
- Completed authentication routes for forgot/reset password and email confirmation compatibility.
- Added persistent password reset tokens using the existing `password_resets` table.
- Added notification list/read/read-all endpoints.
- Replaced doctor queue mock flow with database-backed queue actions:
  - queue listing
  - call patient
  - skip patient
  - start examination
  - tracking + patient notification when called
- Added examination workflow:
  - load examination by visit/appointment code
  - create/update medical record
  - complete examination
- Added admin CRUD endpoints used by React for:
  - customers
  - employees
  - doctors
  - technicians
  - tests
  - specialties
  - rooms
  - work schedules
- Added development configuration template for local MySQL + JWT.

## Still to migrate/finish

- Full specimen/worklist/result-entry technician workflow.
- Tracking endpoints (`/tracking/...`).
- Public result lookup and PDF/print endpoints.
- News/promotions/home/contact endpoints (Java currently contains partly mocked content).
- Remaining appointment aliases such as quick/walk-in/taken-times/time-slots where not already covered.
- Final integration test against a running MySQL instance and React frontend.

## Local run

1. Import `phongkham_xetnghiem.sql` into MySQL.
2. Edit `appsettings.Development.json` if your MySQL password differs from `new_password`.
3. Install .NET 10 SDK.
4. From `BioMedic.Backend` run `dotnet restore` then `dotnet run`.
5. Start the React frontend and point `VITE_API_URL` to the backend `/api` URL if needed.

> Never use the development JWT secret in production.
