 Hệ thống quản lý phòng khám xét nghiệm & truy vết bệnh nhân

Tài liệu mô tả quy trình truy vết bệnh nhân từ lúc đặt lịch, check-in, khám, lấy mẫu, xét nghiệm, duyệt kết quả đến khi hoàn tất; đồng thời đề xuất cấu trúc cơ sở dữ liệu phục vụ quản lý và truy vết.

📌 Mục lục

1. Tổng quan

2. Các vai trò trong hệ thống

3. Quy trình truy vết bệnh nhân đặt lịch khám

4. Quy trình bệnh nhân chỉ đăng ký lịch xét nghiệm

5. Chuỗi trạng thái hiển thị cho bệnh nhân

6. Sequence Diagram

7. Gợi ý màn hình truy vết

8. Thiết kế cơ sở dữ liệu

9. Bảng TRUYVET

10. Phân biệt TRUYVET và THONGBAO

1. Tổng quan

Hệ thống hỗ trợ theo dõi toàn bộ hành trình của bệnh nhân trong quá trình khám và xét nghiệm.

Mỗi thao tác quan trọng đều được ghi vào lịch sử truy vết, ví dụ:

Đặt lịch.

Check-in.

Tiếp nhận bệnh nhân.

Gọi bệnh nhân.

Bắt đầu khám.

Chỉ định xét nghiệm.

Lấy mẫu bệnh phẩm.

Bàn giao mẫu.

KTV tiếp nhận mẫu.

Thực hiện xét nghiệm.

Nhập kết quả.

Bác sĩ duyệt kết quả.

Mời bệnh nhân đọc kết quả.

Hoàn tất lượt khám/lượt xét nghiệm.

Thông tin truy vết có thể bao gồm:

Bệnh nhân.

Lượt khám hoặc lượt xét nghiệm.

Người thực hiện.

Hành động.

Thời gian thực hiện.

Trạng thái trước và sau thao tác.

Mô tả bổ sung.

2. Các vai trò trong hệ thống

Vai trò

Chức năng chính

KHACHHANG

Đặt lịch, check-in, theo dõi trạng thái, nhận thông báo và kết quả

BACSI

Khám, gọi bệnh nhân, chỉ định xét nghiệm, lấy mẫu, bàn giao mẫu, duyệt và đọc kết quả

TIEPTAN

Kiểm tra lịch và tiếp nhận bệnh nhân khi check-in

KTV

Tiếp nhận mẫu, thực hiện xét nghiệm, nhập kết quả

ADMIN

Quản lý hệ thống và dữ liệu liên quan

3. Quy trình truy vết bệnh nhân đặt lịch khám

Bước 1 — Đặt lịch khám

Bệnh nhân lựa chọn:

Chuyên khoa.

Bác sĩ.

Ngày khám.

Thời gian khám.

Hệ thống kiểm tra lịch làm việc của bác sĩ và tình trạng khung giờ.

Nếu hợp lệ:

Tạo lượt khám và lưu vào cơ sở dữ liệu.

Ghi sự kiện đặt lịch vào hệ thống truy vết.

Cập nhật trạng thái thành Đặt lịch thành công.

Gửi thông báo cho bệnh nhân.

Cung cấp mã đặt lịch hoặc mã QR để check-in.

Bước 2 — Check-in tại cơ sở

Khi đến cơ sở, bệnh nhân sử dụng mã đặt lịch cùng thông tin cá nhân để check-in.

Tiếp tân kiểm tra thông tin và xác nhận bệnh nhân đã đến.

Sau khi tiếp nhận thành công:

Trạng thái lượt khám → Chờ khám.

Ghi nhận thời gian check-in.

Ghi nhận người thực hiện.

Ghi nhận hành động tiếp nhận.

Màn hình bệnh nhân hiển thị:

Trạng thái.

Số thứ tự.

Phòng khám.

Bác sĩ phụ trách.

Thời gian dự kiến khám.

Bước 3 — Chờ đến lượt khám

Bệnh nhân được đưa vào danh sách chờ của bác sĩ.

Khi đến lượt, bác sĩ chọn Gọi bệnh nhân.

Hệ thống:

Ghi sự kiện gọi bệnh nhân.

Cập nhật trạng thái → Đã đến lượt khám.

Gửi thông báo cho bệnh nhân.

Ví dụ:

Đã đến lượt khám, vui lòng vào phòng P.203.

Nếu bệnh nhân chưa xuất hiện, bác sĩ có thể:

Chuyển sang Chờ gọi lại.

Gọi lại.

Bỏ lượt.

Tất cả thao tác đều được ghi vào lịch sử truy vết.

Bước 4 — Bác sĩ bắt đầu khám

Khi bệnh nhân vào phòng, bác sĩ xác nhận bắt đầu khám.

Trạng thái lượt khám → Đang khám.

Hệ thống ghi nhận:

Thời gian bắt đầu khám.

Bác sĩ thực hiện.

Lượt khám.

Hành động.

Bước 5 — Kết thúc khám hoặc chỉ định xét nghiệm

Trường hợp A — Không cần xét nghiệm

Bác sĩ nhập kết luận khám.

Hệ thống:

Lưu kết luận.

Ghi sự kiện truy vết.

Cập nhật trạng thái → Hoàn tất.

Quy trình kết thúc.

Trường hợp B — Có chỉ định xét nghiệm

Bác sĩ tạo các chỉ định xét nghiệm.

Hệ thống:

Lưu từng chỉ định.

Liên kết chỉ định với đúng bệnh nhân và lượt khám.

Ghi sự kiện truy vết.

Cập nhật trạng thái → Đã chỉ định xét nghiệm.

Hiển thị danh sách xét nghiệm cho bệnh nhân.

Bước 6 — Bác sĩ lấy mẫu xét nghiệm

Bác sĩ kiểm tra bệnh nhân và chỉ định xét nghiệm, sau đó trực tiếp lấy mẫu bệnh phẩm.

Mẫu được liên kết với:

Bệnh nhân.

Lượt khám.

Chỉ định xét nghiệm.

Loại mẫu.

Bác sĩ thực hiện.

Sau khi lấy mẫu:

Tạo/ghi nhận mã mẫu bệnh phẩm.

Trạng thái xét nghiệm → Đã lấy mẫu.

Ghi thời gian lấy mẫu và người thực hiện.

Bước 7 — Bác sĩ bàn giao mẫu cho KTV

Bác sĩ sử dụng chức năng Bàn giao mẫu.

Hệ thống ghi nhận:

Bác sĩ bàn giao.

KTV tiếp nhận.

Mã mẫu.

Bệnh nhân.

Lượt khám.

Chỉ định xét nghiệm.

Thời gian bàn giao.

Loại mẫu.

Trạng thái mẫu.

Sau khi KTV xác nhận:

Trạng thái → KTV đã tiếp nhận mẫu.

Chuỗi truy vết có dạng:

Bác sĩ → Bàn giao mẫu → KTV → Thời gian

Bệnh nhân có thể nhìn thấy:

Mẫu đã được tiếp nhận – Đang chờ thực hiện xét nghiệm.

Bước 8 — KTV thực hiện xét nghiệm

KTV kiểm tra:

Mã mẫu.

Thông tin bệnh nhân.

Danh sách xét nghiệm.

Nếu hợp lệ, KTV chọn Bắt đầu xét nghiệm.

Hệ thống ghi nhận:

KTV thực hiện.

Thời gian bắt đầu.

Mã mẫu.

Lượt khám.

Chỉ định xét nghiệm.

Trạng thái → Đang thực hiện xét nghiệm.

Bước 9 — KTV nhập kết quả

Sau khi hoàn thành xét nghiệm, KTV nhập kết quả vào hệ thống.

Kết quả được liên kết với:

Bệnh nhân.

Lượt khám.

Phiếu/chỉ định xét nghiệm.

Mã mẫu.

KTV thực hiện.

Hệ thống ghi nhận thời gian hoàn thành, thời gian nhập kết quả và người thực hiện.

Trạng thái → Chờ bác sĩ duyệt kết quả.

Bước 10 — Bác sĩ duyệt kết quả

Bác sĩ kiểm tra kết quả và chọn Duyệt kết quả.

Hệ thống ghi nhận:

Bác sĩ duyệt.

Thời gian duyệt.

Kết quả được duyệt.

Lượt khám.

Chỉ định xét nghiệm.

Sau khi duyệt:

Trạng thái → Đã có kết quả.

Bệnh nhân nhận thông báo.

Kết quả xét nghiệm đã có. Vui lòng chờ bác sĩ đọc kết quả.

Bước 11 — Bác sĩ đọc kết quả và kết luận

Kết quả không bất thường

Nếu không cần gặp trực tiếp bệnh nhân:

Bác sĩ nhập kết luận.

Hệ thống lưu kết luận và lịch sử truy vết.

Kết quả được gửi đến tài khoản bệnh nhân.

Trạng thái → Hoàn tất.

Kết quả bất thường

Nếu cần trao đổi trực tiếp, bác sĩ chọn Đọc kết quả cho bệnh nhân.

Hệ thống:

Ghi thao tác của bác sĩ.

Trạng thái → Mời vào đọc kết quả.

Gửi thông báo cho bệnh nhân.

Bác sĩ đã có kết quả, vui lòng vào phòng P.203 để đọc kết quả.

Bước 12 — Bệnh nhân vào phòng đọc kết quả

Bác sĩ xác nhận bệnh nhân đã vào phòng.

Trạng thái → Đang tư vấn kết quả.

Bác sĩ:

Giải thích kết quả.

Đưa ra kết luận.

Hướng dẫn bước tiếp theo nếu cần.

Sau khi hoàn tất:

Trạng thái lượt khám → Hoàn tất.

Ghi sự kiện cuối cùng vào hệ thống truy vết.

4. Quy trình bệnh nhân chỉ đăng ký lịch xét nghiệm

Đây là trường hợp bệnh nhân không đặt lịch khám, chỉ đăng ký lịch xét nghiệm.

Bước 1 — Đặt lịch xét nghiệm

Bệnh nhân lựa chọn:

Loại xét nghiệm.

Bác sĩ thực hiện.

Ngày xét nghiệm.

Thời gian xét nghiệm.

Nếu lịch hợp lệ:

Tạo lượt xét nghiệm.

Liên kết bệnh nhân với danh sách xét nghiệm đã đăng ký.

Ghi sự kiện truy vết.

Trạng thái → Đặt lịch xét nghiệm thành công.

Gửi mã đặt lịch cho bệnh nhân.

Bước 2 — Check-in

Tiếp tân kiểm tra:

Mã đặt lịch.

Bệnh nhân.

Loại xét nghiệm.

Bác sĩ.

Ngày và giờ thực hiện.

Sau khi tiếp nhận:

Trạng thái → Chờ thực hiện xét nghiệm.

Ghi thời gian check-in, người thực hiện và trạng thái trước/sau.

Bước 3 — Gọi bệnh nhân và lấy mẫu

Khi đến lượt, bác sĩ chọn Gọi bệnh nhân.

Trạng thái → Đã đến lượt xét nghiệm.

Khi bệnh nhân vào phòng:

Trạng thái → Đang thực hiện lấy mẫu.

Bác sĩ thực hiện lấy mẫu.

Tạo mã mẫu bệnh phẩm.

Trạng thái → Đã lấy mẫu.

Bước 4 — Bàn giao mẫu cho KTV

Bác sĩ bàn giao mẫu trực tiếp cho KTV.

Sau khi KTV xác nhận tiếp nhận:

Trạng thái → KTV đã tiếp nhận mẫu.

Ghi lại bác sĩ bàn giao, KTV nhận mẫu, mã mẫu và thời gian.

Bước 5 — KTV bắt đầu xét nghiệm

KTV kiểm tra mẫu và chọn Bắt đầu xét nghiệm.

Trạng thái → Đang thực hiện xét nghiệm.

Bước 6 — KTV nhập kết quả

Sau khi hoàn tất xét nghiệm:

KTV nhập kết quả.

Hệ thống lưu thời gian hoàn thành và thời gian nhập.

Trạng thái → Chờ bác sĩ duyệt kết quả.

Bước 7 — Bác sĩ duyệt kết quả

Bác sĩ kiểm tra và duyệt kết quả.

Trạng thái → Đã có kết quả.

Bệnh nhân nhận thông báo:

Kết quả xét nghiệm đã có. Vui lòng chờ bác sĩ đọc kết quả.

Bước 8 — Bác sĩ kết luận

Không bất thường

Bác sĩ nhập kết luận.

Kết quả và kết luận được gửi cho bệnh nhân.

Trạng thái → Hoàn tất.

Có bất thường

Bác sĩ chọn Mời bệnh nhân vào đọc kết quả.

Trạng thái → Mời vào đọc kết quả.

Hệ thống gửi thông báo cho bệnh nhân.

Bước 9 — Tư vấn kết quả

Bệnh nhân vào phòng theo thông báo.

Bác sĩ giải thích kết quả và đưa ra hướng dẫn tiếp theo.

Sau khi hoàn tất:

Trạng thái → Hoàn tất.

Hệ thống ghi sự kiện cuối cùng vào lịch sử truy vết.

5. Chuỗi trạng thái hiển thị cho bệnh nhân

Luồng khám có thể kèm xét nghiệm

Đặt lịch thành công
        ↓
Đã tiếp nhận / Chờ khám
        ↓
Đã đến lượt khám
        ↓
Đang khám
        ↓
Đã chỉ định xét nghiệm (nếu có)
        ↓
Đã lấy mẫu
        ↓
KTV đã tiếp nhận mẫu
        ↓
Đang thực hiện xét nghiệm
        ↓
Chờ bác sĩ duyệt kết quả
        ↓
Đã có kết quả
        ↓
Mời vào đọc kết quả (nếu cần)
        ↓
Đang tư vấn kết quả
        ↓
Hoàn tất

Flowchart

flowchart TD
    A[Đặt lịch thành công] --> B[Check-in / Chờ khám]
    B --> C[Đã đến lượt khám]
    C --> D[Đang khám]
    D --> E{Có chỉ định xét nghiệm?}
    E -- Không --> N[Hoàn tất]
    E -- Có --> F[Đã chỉ định xét nghiệm]
    F --> G[Đã lấy mẫu]
    G --> H[KTV đã tiếp nhận mẫu]
    H --> I[Đang thực hiện xét nghiệm]
    I --> J[Chờ bác sĩ duyệt kết quả]
    J --> K[Đã có kết quả]
    K --> L{Cần gặp bệnh nhân?}
    L -- Không --> N
    L -- Có --> M[Mời vào đọc kết quả]
    M --> O[Đang tư vấn kết quả]
    O --> N

6. Sequence Diagram

sequenceDiagram
    actor BN as Bệnh nhân
    participant HT as Hệ thống
    participant TT as Tiếp tân
    participant BS as Bác sĩ
    participant KTV as Kỹ thuật viên

    BN->>HT: Đặt lịch khám
    HT-->>BN: Xác nhận + mã đặt lịch/QR

    BN->>HT: Check-in
    HT->>TT: Gửi yêu cầu tiếp nhận
    TT->>HT: Xác nhận bệnh nhân đã đến
    HT-->>BN: Trạng thái Chờ khám

    BS->>HT: Gọi bệnh nhân
    HT-->>BN: Đã đến lượt khám

    BS->>HT: Bắt đầu khám
    HT-->>BN: Đang khám

    alt Không cần xét nghiệm
        BS->>HT: Nhập kết luận
        HT-->>BN: Hoàn tất
    else Có chỉ định xét nghiệm
        BS->>HT: Tạo chỉ định xét nghiệm
        HT-->>BN: Đã chỉ định xét nghiệm

        BS->>HT: Xác nhận lấy mẫu
        HT-->>BN: Đã lấy mẫu

        BS->>KTV: Bàn giao mẫu
        KTV->>HT: Xác nhận tiếp nhận mẫu
        HT-->>BN: Mẫu đã được tiếp nhận

        KTV->>HT: Bắt đầu xét nghiệm
        HT-->>BN: Đang thực hiện xét nghiệm

        KTV->>HT: Nhập kết quả
        HT->>BS: Chờ duyệt kết quả

        BS->>HT: Duyệt kết quả
        HT-->>BN: Đã có kết quả

        alt Kết quả không bất thường
            BS->>HT: Nhập kết luận
            HT-->>BN: Hoàn tất
        else Kết quả bất thường
            BS->>HT: Mời bệnh nhân đọc kết quả
            HT-->>BN: Mời vào phòng đọc kết quả
            BN->>BS: Vào phòng
            BS->>HT: Hoàn tất tư vấn
            HT-->>BN: Hoàn tất
        end
    end

7. Gợi ý màn hình truy vết

Khi bệnh nhân đang chờ khám

┌─────────────────────────────────────┐
│             ĐANG CHỜ KHÁM           │
│                                     │
│  Số thứ tự: 05                     │
│  Đang khám: 04                     │
│                                     │
│  Phòng: P.203                      │
│  Bác sĩ: Nguyễn Văn A              │
│                                     │
│  Vui lòng chờ đến lượt             │
└─────────────────────────────────────┘

Khi bác sĩ gọi bệnh nhân

┌─────────────────────────────────────┐
│          ĐÃ ĐẾN LƯỢT KHÁM          │
│                                     │
│          🔔 MỜI BỆNH NHÂN           │
│                                     │
│  Phòng: P.203                      │
│  Bác sĩ: Nguyễn Văn A              │
│                                     │
│  Vui lòng vào phòng khám           │
└─────────────────────────────────────┘

Timeline khi đang khám

✓ Đặt lịch thành công
✓ Đã tiếp nhận
✓ Đã đến lượt khám
● Đang khám
○ Chỉ định xét nghiệm
○ Đang thực hiện xét nghiệm
○ Đang chờ kết quả
○ Đã có kết quả
○ Mời vào đọc kết quả
○ Hoàn tất

Timeline khi có xét nghiệm

✓ Đặt lịch thành công
✓ Đã tiếp nhận
✓ Đã đến lượt khám
✓ Đang khám
✓ Đã chỉ định xét nghiệm
● Đang thực hiện xét nghiệm
○ Đang chờ kết quả
○ Đã có kết quả
○ Mời vào đọc kết quả
○ Hoàn tất

Sau khi KTV nhập kết quả và bác sĩ duyệt:

✓ Đã chỉ định xét nghiệm
✓ Đang thực hiện xét nghiệm
✓ Đang chờ kết quả
✓ Đã có kết quả
● Mời vào đọc kết quả
○ Hoàn tất

8. Thiết kế cơ sở dữ liệu

Danh sách hiện tại gồm 19 bảng chính, chia thành 5 nhóm.

Nhóm 1 — Tài khoản và người dùng

TAIKHOAN

KHACHHANG

NHANVIEN

VAITRO

Nhóm 2 — Chuyên môn

CHUYENKHOA

BACSI

KYTHUATVIEN

PHONGKHAM

Nhóm 3 — Đặt lịch

LICHLAMVIEC

DATLICHKHAM

DATLICHXETNGHIEM

Nhóm 4 — Khám và xét nghiệm

LUOTKHAM

KHAM

CHIDINHXETNGHIEM

XETNGHIEM

MAUBENHPHAM

KETQUAXETNGHIEM

Nhóm 5 — Truy vết và thông báo

TRUYVET

THONGBAO

8.1. TAIKHOAN

TAIKHOAN
--------------------------------
IDTaiKhoan       PK
Username         UNIQUE
Email            UNIQUE
PasswordHash
IDVaiTro         FK
TrangThai
NgayTao
LanDangNhapCuoi

Ví dụ:

IDTaiKhoan

Username

Email

Vai trò

TK001

nguyenvana

a@gmail.com

KHACHHANG

TK002

bsnguyen

bs@gmail.com

BACSI

TK003

ktv001

ktv@gmail.com

KTV

TK004

tieptan01

tt@gmail.com

TIEPTAN

TK005

admin

admin@gmail.com

ADMIN

Không nên lưu mật khẩu dạng plain text. Nên lưu PasswordHash thay cho Password.

8.2. VAITRO

VAITRO
------------------
IDVaiTro         PK
TenVaiTro
MoTa

Dữ liệu gợi ý:

1 - KHACHHANG
2 - BACSI
3 - TIEPTAN
4 - KTV
5 - ADMIN

8.3. KHACHHANG

KHACHHANG
--------------------------------
IDKhachHang      PK
IDTaiKhoan       FK
HoTen
NgaySinh
GioiTinh
SoDienThoai
CCCD
DiaChi
Email
TrangThai
NgayTao

Quan hệ:

TAIKHOAN 1 ───── 1 KHACHHANG

8.4. NHANVIEN

NHANVIEN
--------------------------------
IDNhanVien       PK
IDTaiKhoan       FK
HoTen
NgaySinh
GioiTinh
SoDienThoai
Email
IDChuyenKhoa     FK
TrangThai
NgayTao

Có thể phân biệt nhân viên bằng vai trò:

VAITRO
├── BACSI
├── TIEPTAN
├── KTV
└── ADMIN

8.5. CHUYENKHOA

CHUYENKHOA
----------------------------
IDChuyenKhoa     PK
TenChuyenKhoa
MoTa
TrangThai

Ví dụ:

CK001 - Nội tổng quát
CK002 - Tim mạch
CK003 - Tai mũi họng
CK004 - Nhi

8.6. PHONGKHAM

PHONGKHAM
----------------------------
IDPhong          PK
TenPhong
LoaiPhong
IDChuyenKhoa     FK
Tang
TrangThai

Ví dụ:

P001 | Phòng khám 203 | Khám bệnh | Nội tổng quát
P002 | Phòng xét nghiệm 01

8.7. LICHLAMVIEC

LICHLAMVIEC
--------------------------------
IDLichLamViec    PK
IDNhanVien       FK
NgayLam
CaLam
GioBatDau
GioKetThuc
IDPhong          FK
TrangThai

8.8. DATLICHKHAM

DATLICHKHAM
--------------------------------
IDDatLichKham    PK
IDKhachHang      FK
IDChuyenKhoa     FK
IDBacSi          FK
IDPhong          FK
NgayKham
GioKham
SoThuTu
TrangThai
MaQR
NgayDat
GhiChu

Luồng sử dụng QR:

DATLICHKHAM
     ↓
Sinh MaQR
     ↓
Bệnh nhân sử dụng QR để check-in

8.9. LUOTKHAM

Lịch đặt ≠ lượt khám thực tế. Một lịch có thể bị hủy, đổi lịch hoặc bệnh nhân không đến.

LUOTKHAM
--------------------------------
IDLuotKham       PK
IDDatLichKham    FK
IDKhachHang      FK
IDBacSi          FK
IDPhong          FK
ThoiGianTiepNhan
ThoiGianBatDau
ThoiGianKetThuc
TrangThai

Ví dụ:

LK001
  ↓
Check-in
  ↓
LTK001

8.10. KHAM

KHAM
--------------------------------
IDKham           PK
IDLuotKham       FK
IDBacSi          FK
TrieuChung
TienSuBenh
ChanDoan
KetLuan
HuongDieuTri
ThoiGianKham

8.11. XETNGHIEM

XETNGHIEM
--------------------------------
IDXetNghiem      PK
TenXetNghiem
MoTa
LoaiXetNghiem
DonVi
GiaTriThamChieu
TrangThai

Ví dụ:

XN001 | Công thức máu
XN002 | Đường huyết
XN003 | Cholesterol
XN004 | Men gan

8.12. CHIDINHXETNGHIEM

CHIDINHXETNGHIEM
--------------------------------
IDChiDinh        PK
IDKham           FK
IDXetNghiem      FK
IDBacSi          FK
ThoiGianChiDinh
TrangThai
GhiChu

Luồng:

Bác sĩ khám
    ↓
CHIDINHXETNGHIEM
    ↓
Xét nghiệm được chỉ định
    ↓
Thực hiện xét nghiệm

8.13. MAUBENHPHAM

MAUBENHPHAM
--------------------------------
IDMau            PK
IDChiDinh        FK
LoaiMau
MaMau
ThoiGianLayMau
IDNguoiLayMau    FK
ThoiGianTiepNhan
IDKTVTiepNhan    FK
TrangThai

Ví dụ:

Mẫu: M001
Loại: Máu
Mã mẫu: MB-20260821-001
Người lấy: NV005
KTV tiếp nhận: KTV002

8.14. KETQUAXETNGHIEM

KETQUAXETNGHIEM
--------------------------------
IDKetQua         PK
IDChiDinh        FK
IDMau            FK
IDKTV            FK
KetQua
DonVi
GiaTriThamChieu
NhanXet
ThoiGianThucHien
ThoiGianNhap
IDBacSiDuyet     FK
ThoiGianDuyet
TrangThai

Luồng kết quả:

KTV nhập kết quả
      ↓
Chờ bác sĩ duyệt
      ↓
Bác sĩ duyệt
      ↓
ĐÃ CÓ KẾT QUẢ
      ↓
Bệnh nhân được thông báo

8.15. THONGBAO

THONGBAO
--------------------------------
IDThongBao       PK
IDKhachHang      FK
LoaiThongBao
TieuDe
NoiDung
IDDoiTuong
LoaiDoiTuong
ThoiGianTao
DaDoc

Ví dụ thông báo:

Đặt lịch thành công.

Lấy mẫu máu thành công.

Đã đến lượt khám.

Đã có kết quả.

Mời vào phòng đọc kết quả.

9. Bảng TRUYVET

Đây là bảng trung tâm để lưu lịch sử hoạt động của hệ thống.

TRUYVET
----------------------------------------
IDTruyVet          PK
IDKhachHang        FK
LoaiDoiTuong
IDDoiTuong
HanhDong
TrangThaiCu
TrangThaiMoi
IDNguoiThucHien
ThoiGian
MoTa
IPAddress

Ví dụ dữ liệu:

Thời gian

Hành động

Người thực hiện

Trạng thái

08:30

Đặt lịch

KH001

→ ĐÃ_ĐẶT

08:45

Check-in QR

LT001

→ CHỜ_KHÁM

09:10

Gọi bệnh nhân

BS001

CHỜ_KHÁM → ĐẾN_LƯỢT

09:12

Bắt đầu khám

BS001

ĐẾN_LƯỢT → ĐANG_KHÁM

09:30

Chỉ định XN

BS001

ĐANG_KHÁM → ĐÃ_CHỈ_ĐỊNH

09:35

Lấy mẫu máu

KTV001

→ ĐANG_XN

10:10

Nhập kết quả

KTV001

→ CHỜ_DUYỆT

10:15

Duyệt kết quả

BS001

→ ĐÃ_CÓ_KQ

10:20

Mời đọc KQ

BS001

→ MỜI_ĐỌC

10:30

Hoàn tất

BS001

→ HOÀN_TẤT

10. Phân biệt TRUYVET và THONGBAO

Thành phần

Mục đích

TRUYVET

Ghi lại lịch sử thao tác và thay đổi trạng thái trong hệ thống

THONGBAO

Gửi thông tin cần thiết đến bệnh nhân

TRUYVET
→ Lưu lịch sử hệ thống

THONGBAO
→ Gửi thông tin cho bệnh nhân

✅ Kết luận

Thiết kế truy vết giúp hệ thống theo dõi được ai thực hiện hành động gì, trên đối tượng nào, vào thời điểm nào và trạng thái đã thay đổi ra sao.

Đối với bệnh nhân, hệ thống cung cấp một timeline rõ ràng từ lúc đặt lịch đến khi hoàn tất, giúp người dùng biết mình đang ở bước nào trong quá trình khám hoặc xét nghiệm.
