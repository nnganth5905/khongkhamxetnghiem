# Frontend fix notes

Các lỗi đã sửa để nối trang Admin với backend ASP.NET Core hiện tại:

- `AdminCrudPage` nay nhận đúng prop `loadItems`, nên các trang Admin thực sự gọi API khi mount.
- Hỗ trợ `column.value(row)` để các cột dùng hàm mapping hiển thị đúng dữ liệu.
- Hỗ trợ `itemId(row)` khi Update/Delete, tránh gửi `undefined` làm ID.
- Hỗ trợ `colClass` trong cấu hình form.
- `adminService.js` chuẩn hóa response backend (`id`, `name`, `phone`, ...) sang model đang dùng ở UI (`idKhachHang`, `tenKhachHang`, ...).
- Chuẩn hóa payload Create/Update từ form React sang DTO .NET hiện tại cho Customer, Employee, Doctor, Technician, Test, Specialty, Room, Work Schedule.
- `.env` giữ `VITE_API_URL=http://localhost:5179/api`.

Sau khi chép source mới, chạy lại frontend bằng `npm run dev` và refresh trình duyệt.
