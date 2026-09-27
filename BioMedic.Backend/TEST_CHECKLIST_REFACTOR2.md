# Refactor 2 smoke-test checklist

Run backend and open Swagger. Test in this order so a failure is easy to isolate.

## 1. Doctor
1. Login as a DOCTOR and Authorize Swagger.
2. `GET /api/technicians` -> expect 200.
3. Use an already checked-in TEST appointment.
4. `POST /api/doctor/specimens` -> expect 201.
5. `POST /api/doctor/specimens/{barcode-or-id}/handover` -> expect 200.

## 2. Technician specimen receipt
1. Login as TECHNICIAN and Authorize Swagger.
2. `GET /api/technician/me` -> expect 200.
3. `GET /api/technician/specimens?status=HANDED_OVER` -> expect 200.
4. `GET /api/technician/specimens/{id}` -> expect 200.
5. `POST /api/technician/specimens/{id}/receive` -> expect 200 and a `worklistId`.

## 3. Worklist
1. `GET /api/technician/worklist` -> expect received sample in `PENDING` state.
2. `POST /api/technician/worklist/{id}/start` -> expect 200 / `IN_PROGRESS`.
3. `POST /api/technician/worklist/{id}/complete` -> expect 200 / `COMPLETED`.

## 4. Result entry
1. `GET /api/technician/results/{worklistId}` -> expect patient/test info and indicator list.
2. `PUT /api/technician/results/{worklistId}` with values -> expect draft save success.
3. Repeat GET and verify values are preserved.
4. `POST /api/technician/results/{worklistId}/submit` -> expect `SUBMITTED`.

Example result payload:
```json
{
  "indicators": [
    { "indicatorId": "CS001", "value": "6.5", "abnormal": false }
  ],
  "notes": "Kết quả demo"
}
```
Use the indicator IDs returned by the GET endpoint; do not invent IDs.

## 5. Doctor approval
1. Login as the doctor responsible for the order.
2. `GET /api/doctor/results/pending` -> submitted result should appear.
3. `GET /api/doctor/results/{id}` -> expect detail.
4. `POST /api/doctor/results/{id}/approve`:
```json
{ "conclusion": "Kết quả phù hợp, tiếp tục theo dõi." }
```
5. Expect 200 and customer notification to be created.

## 6. Customer
1. Login as the customer who owns the order.
2. `GET /api/results/mine` -> approved result should appear.
3. `GET /api/results/{id}` -> detail should be visible.
4. `GET /api/notifications` -> result-ready notification should appear.

## Expected workflow states
`da_lay_mau -> da_ban_giao -> ktv_tiep_nhan -> dang_xu_ly -> cho_duyet -> da_duyet/hoan_tat`
