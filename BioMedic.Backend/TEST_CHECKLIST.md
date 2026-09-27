# Backend smoke-test checklist

## ADMIN
- `POST /api/auth/login`
- `GET /api/auth/me`
- `GET /api/admin/customers`
- `GET /api/admin/employees`
- `GET /api/admin/doctors`
- `GET /api/admin/technicians`
- `GET /api/admin/tests`
- `GET /api/admin/specialties`
- `GET /api/admin/rooms`
- `GET /api/admin/work-schedules`

## DOCTOR
Login with a `bacsi` account, then authorize Swagger with the new token.

Preferred standardized routes:
- `GET /api/doctor/queue`
- `POST /api/doctor/queue/{appointmentCode}/call`
- `POST /api/doctor/queue/{appointmentCode}/hold`
- `POST /api/doctor/queue/{appointmentCode}/skip`

Legacy frontend-compatible routes remain available:
- `GET /api/appointments/waiting-queue`
- `POST /api/appointments/queue/{appointmentCode}/call`
- `POST /api/appointments/queue/{appointmentCode}/hold`
- `POST /api/appointments/queue/{appointmentCode}/skip`

Visit-id routes:
- `GET /api/doctor/visits/waiting`
- `GET /api/doctor/visits/queue`
- `POST /api/doctor/visits/{visitId}/call`
- `POST /api/doctor/visits/{visitId}/skip`
- `POST /api/doctor/visits/{visitId}/start`

Examination:
- `GET /api/doctor/examinations/visit/{visitId-or-appointmentCode}`
- `POST /api/doctor/examinations`
- `PUT /api/doctor/examinations/{examId}`
- `POST /api/doctor/examinations/{visitId-or-appointmentCode}/complete`

Results approval:
- `GET /api/doctor/results/pending`
- `GET /api/doctor/results/{id}`
- `POST /api/doctor/results/{id}/approve`

## RECEPTIONIST
- `GET /api/dashboard/receptionist`
- `GET /api/reception/visits/waiting`
- appointment/check-in endpoints under `/api/appointments`

## NOTIFICATIONS
- `GET /api/notifications`
- `PATCH /api/notifications/{id}/read`
- `POST /api/notifications/read-all`
