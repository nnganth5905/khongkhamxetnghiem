Hệ thống quản lý phòng khám xét nghiệm & truy vết bệnh nhân

1. Hệ thống hỗ trợ theo dõi toàn bộ hành trình của bệnh nhân trong quá trình khám và xét nghiệm.

Mỗi thao tác quan trọng đều được ghi vào lịch sử truy vết, ví dụ:
- Đặt lịch.
- Check-in.
- Tiếp nhận bệnh nhân.
- Gọi bệnh nhân.
- Bắt đầu khám.
- Chỉ định xét nghiệm.
- Lấy mẫu bệnh phẩm.
- Bàn giao mẫu.
- KTV tiếp nhận mẫu.
- Thực hiện xét nghiệm.
- Nhập kết quả.
- Bác sĩ duyệt kết quả.
- Mời bệnh nhân đọc kết quả.
- Hoàn tất lượt khám/lượt xét nghiệm.

Thông tin truy vết có thể bao gồm:
- Bệnh nhân.
- Lượt khám hoặc lượt xét nghiệm.
- Người thực hiện.
- Hành động.
- Thời gian thực hiện.
- Trạng thái trước và sau thao tác.
- Mô tả bổ sung.

2. Các vai trò trong hệ thống
Vai trò, vhức năng chính
- KHACHHANG: Đặt lịch, check-in, theo dõi trạng thái, nhận thông báo và kết quả
- BACSI: Khám, gọi bệnh nhân, chỉ định xét nghiệm, lấy mẫu, bàn giao mẫu, duyệt và đọc kết quả
- TIEPTAN: Kiểm tra lịch và tiếp nhận bệnh nhân khi check-in
- KTV: Tiếp nhận mẫu, thực hiện xét nghiệm, nhập kết quả
- ADMIN: Quản lý hệ thống và dữ liệu liên quan

3. Quy trình truy vết bệnh nhân đặt lịch khám
Bước 1 — Đặt lịch khám: Bệnh nhân lựa chọn:
- Chuyên khoa.
- Bác sĩ.
- Ngày khám.
- Thời gian khám.
Hệ thống kiểm tra lịch làm việc của bác sĩ và tình trạng khung giờ.
Nếu hợp lệ:
Tạo lượt khám và lưu vào cơ sở dữ liệu.
Ghi sự kiện đặt lịch vào hệ thống truy vết.
Cập nhật trạng thái thành Đặt lịch thành công.
Gửi thông báo cho bệnh nhân.
Cung cấp mã đặt lịch để check-in.

Bước 2 — Check-in tại cơ sở
Khi đến cơ sở, bệnh nhân sử dụng mã đặt lịch cùng thông tin cá nhân để check-in.
Tiếp tân kiểm tra thông tin và xác nhận bệnh nhân đã đến.
Sau khi tiếp nhận thành công: Trạng thái lượt khám → Chờ khám.
Ghi nhận thời gian check-in.
Ghi nhận người thực hiện.
Ghi nhận hành động tiếp nhận.
Màn hình bệnh nhân hiển thị:
+ Trạng thái.
+ Số thứ tự.
+ Phòng khám.
+ Bác sĩ phụ trách.

Bước 3 — Chờ đến lượt khám
Bệnh nhân được đưa vào danh sách chờ của bác sĩ.
Khi đến lượt, bác sĩ chọn Gọi bệnh nhân.
Hệ thống:
+ Ghi sự kiện gọi bệnh nhân.
+ Cập nhật trạng thái → Đã đến lượt khám.
+ Gửi thông báo cho bệnh nhân.

Ví dụ: Đã đến lượt khám, vui lòng vào phòng P.203.
Nếu bệnh nhân chưa xuất hiện, bác sĩ có thể: Chuyển sang Chờ gọi lại. Gọi lại, Bỏ lượt.
Tất cả thao tác đều được ghi vào lịch sử truy vết.

Bước 4 — Bác sĩ bắt đầu khám
Khi bệnh nhân vào phòng, bác sĩ xác nhận bắt đầu khám.
Trạng thái lượt khám → Đang khám.
Hệ thống ghi nhận:
+ Thời gian bắt đầu khám.
+ Bác sĩ thực hiện.
+ Lượt khám.
+ Hành động.

Bước 5 — Kết thúc khám hoặc chỉ định xét nghiệm
- Trường hợp A — Không cần xét nghiệm
Bác sĩ nhập kết luận khám.
Hệ thống:
+ Lưu kết luận.
+ Ghi sự kiện truy vết.
+ Cập nhật trạng thái → Hoàn tất.
+ Quy trình kết thúc.
- Trường hợp B — Có chỉ định xét nghiệm: Bác sĩ tạo các chỉ định xét nghiệm.
Hệ thống:
+ Lưu từng chỉ định.
+ Liên kết chỉ định với đúng bệnh nhân và lượt khám.
+ Ghi sự kiện truy vết.
+ Cập nhật trạng thái → Đã chỉ định xét nghiệm.
+ Hiển thị danh sách xét nghiệm cho bệnh nhân.

Bước 6 — Bác sĩ lấy mẫu xét nghiệm: Bác sĩ kiểm tra bệnh nhân và chỉ định xét nghiệm, sau đó trực tiếp lấy mẫu bệnh phẩm.
Mẫu được liên kết với:
+ Bệnh nhân.
+ Lượt khám.
+ Chỉ định xét nghiệm.
+ Loại mẫu.
+ Bác sĩ thực hiện.

Sau khi lấy mẫu:
+ Tạo/ghi nhận mã mẫu bệnh phẩm.
+ Trạng thái xét nghiệm → Đã lấy mẫu.
+ Ghi thời gian lấy mẫu và người thực hiện.

Bước 7 — Bác sĩ bàn giao mẫu cho KTV: Bác sĩ sử dụng chức năng Bàn giao mẫu.
Hệ thống ghi nhận:
+ Bác sĩ bàn giao.
+ KTV tiếp nhận.
+ Mã mẫu.
+ Bệnh nhân.
+ Lượt khám.
+ Chỉ định xét nghiệm.
+ Thời gian bàn giao.
+ Loại mẫu.
+ Trạng thái mẫu.

Sau khi KTV xác nhận: Trạng thái → KTV đã tiếp nhận mẫu.
Chuỗi truy vết có dạng:
+ Bác sĩ → Bàn giao mẫu → KTV → Thời gian
+ Bệnh nhân có thể nhìn thấy: Mẫu đã được tiếp nhận – Đang chờ thực hiện xét nghiệm.

Bước 8 — KTV thực hiện xét nghiệm
KTV kiểm tra:
+ Mã mẫu.
+ Thông tin bệnh nhân.
+ Danh sách xét nghiệm.
+ Nếu hợp lệ, KTV chọn Bắt đầu xét nghiệm.
Hệ thống ghi nhận: KTV thực hiện.
+ Thời gian bắt đầu.
+ Mã mẫu.
+ Lượt khám.
+ Chỉ định xét nghiệm.
Trạng thái → Đang thực hiện xét nghiệm.

Bước 9 — KTV nhập kết quả: Sau khi hoàn thành xét nghiệm, KTV nhập kết quả vào hệ thống.
Kết quả được liên kết với: Bệnh nhân, Lượt khám, Phiếu/chỉ định xét nghiệm, Mã mẫu, KTV thực hiện.
Hệ thống ghi nhận thời gian hoàn thành, thời gian nhập kết quả và người thực hiện.
Trạng thái → Chờ bác sĩ duyệt kết quả.

Bước 10 — Bác sĩ duyệt kết quả
Bác sĩ kiểm tra kết quả và chọn Duyệt kết quả.
Hệ thống ghi nhận:
+ Bác sĩ duyệt.
+ Thời gian duyệt.
+ Kết quả được duyệt.
+ Lượt khám.
+ Chỉ định xét nghiệm.

Sau khi duyệt: Trạng thái → Đã có kết quả.
Bệnh nhân nhận thông báo.
Kết quả xét nghiệm đã có. Vui lòng chờ bác sĩ đọc kết quả.

Bước 11 — Bác sĩ đọc kết quả và kết luận
Kết quả không bất thường
Nếu không cần gặp trực tiếp bệnh nhân:
+ Bác sĩ nhập kết luận.
+ Hệ thống lưu kết luận và lịch sử truy vết.
+ Kết quả được gửi đến tài khoản bệnh nhân.
+ Trạng thái → Hoàn tất.
Kết quả bất thường
Nếu cần trao đổi trực tiếp, bác sĩ chọn Đọc kết quả cho bệnh nhân.
Hệ thống:
+ Ghi thao tác của bác sĩ.
+ Trạng thái → Mời vào đọc kết quả.
+ Gửi thông báo cho bệnh nhân.
+ Bác sĩ đã có kết quả, vui lòng vào phòng P.203 để đọc kết quả.

Bước 12 — Bệnh nhân vào phòng đọc kết quả
Bác sĩ xác nhận bệnh nhân đã vào phòng.
Trạng thái → Đang tư vấn kết quả.
Bác sĩ:
+ Giải thích kết quả.
+ Đưa ra kết luận.
+ Hướng dẫn bước tiếp theo nếu cần.
Sau khi hoàn tất: Trạng thái lượt khám → Hoàn tất.
Ghi sự kiện cuối cùng vào hệ thống truy vết.

4. Quy trình bệnh nhân chỉ đăng ký lịch xét nghiệm
Đây là trường hợp bệnh nhân không đặt lịch khám, chỉ đăng ký lịch xét nghiệm.
Bước 1 — Đặt lịch xét nghiệm
Bệnh nhân lựa chọn:
+ Loại xét nghiệm.
+ Bác sĩ thực hiện.
+ Ngày xét nghiệm.
+ Thời gian xét nghiệm.
Nếu lịch hợp lệ: Tạo lượt xét nghiệm.
Liên kết bệnh nhân với danh sách xét nghiệm đã đăng ký.
Ghi sự kiện truy vết.
Trạng thái → Đặt lịch xét nghiệm thành công.
Gửi mã đặt lịch cho bệnh nhân.

Bước 2 — Check-in: Tiếp tân kiểm tra:
+ Mã đặt lịch.
+ Bệnh nhân.
+ Loại xét nghiệm.
+ Bác sĩ.
+ Ngày và giờ thực hiện.
Sau khi tiếp nhận:
+ Trạng thái → Chờ thực hiện xét nghiệm.
+ Ghi thời gian check-in, người thực hiện và trạng thái trước/sau.

Bước 3 — Gọi bệnh nhân và lấy mẫu
Khi đến lượt, bác sĩ chọn Gọi bệnh nhân.
Trạng thái → Đã đến lượt xét nghiệm.
Khi bệnh nhân vào phòng:
+ Trạng thái → Đang thực hiện lấy mẫu.
+ Bác sĩ thực hiện lấy mẫu.
+ Tạo mã mẫu bệnh phẩm.
+ Trạng thái → Đã lấy mẫu.

Bước 4 — Bàn giao mẫu cho KTV
Bác sĩ bàn giao mẫu trực tiếp cho KTV.
Sau khi KTV xác nhận tiếp nhận:
+ Trạng thái → KTV đã tiếp nhận mẫu.
+ Ghi lại bác sĩ bàn giao, KTV nhận mẫu, mã mẫu và thời gian.

Bước 5 — KTV bắt đầu xét nghiệm
KTV kiểm tra mẫu và chọn Bắt đầu xét nghiệm.
Trạng thái → Đang thực hiện xét nghiệm.

Bước 6 — KTV nhập kết quả
Sau khi hoàn tất xét nghiệm:
+ KTV nhập kết quả.
+ Hệ thống lưu thời gian hoàn thành và thời gian nhập.
+ Trạng thái → Chờ bác sĩ duyệt kết quả.

Bước 7 — Bác sĩ duyệt kết quả
+ Bác sĩ kiểm tra và duyệt kết quả.
+ Trạng thái → Đã có kết quả.
Bệnh nhân nhận thông báo:
+ Kết quả xét nghiệm đã có. Vui lòng chờ bác sĩ đọc kết quả.

Bước 8 — Bác sĩ kết luận
- Không bất thường
+ Bác sĩ nhập kết luận.
+ Kết quả và kết luận được gửi cho bệnh nhân.
+ Trạng thái → Hoàn tất.
- Có bất thường
+ Bác sĩ chọn Mời bệnh nhân vào đọc kết quả.
+ Trạng thái → Mời vào đọc kết quả.
+ Hệ thống gửi thông báo cho bệnh nhân.

Bước 9 — Tư vấn kết quả
Bệnh nhân vào phòng theo thông báo.
Bác sĩ giải thích kết quả và đưa ra hướng dẫn tiếp theo.
Sau khi hoàn tất:
+ Trạng thái → Hoàn tất.
+ Hệ thống ghi sự kiện cuối cùng vào lịch sử truy vết.

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


Gợi ý màn hình truy vết

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
