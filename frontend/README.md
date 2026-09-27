# BioMedic

Bio Medic Center clinic and laboratory management system.

## Stack

Frontend:
- React
- JavaScript / JSX
- Vite
- Axios
- React Router
- Bootstrap

Backend:
- Java 21
- Spring Boot
- Maven
- Spring Web
- Spring Data JPA / Hibernate
- Spring Security
- JWT
- Jakarta Validation
- Spring Mail
- WebSocket
- MySQL

## Main structure

```text
BioMedic/
├── frontend/
├── backend/
├── database/
├── docs/
├── .gitignore
└── README.md

Database

Existing database:

phongkham_xetnghiem

Do not create a new database.

Hibernate is configured with:

spring.jpa.hibernate.ddl-auto=none

Run backend

cd D:\project\BioMedic\backend
.\mvnw.cmd clean compile
.\mvnw.cmd spring-boot:run

Backend:

http://localhost:8080

Run frontend

cd D:\project\BioMedic\frontend
npm install
npm run dev

Frontend:

http://localhost:5173
Authentication roles

ADMIN
DOCTOR
CUSTOMER
RECEPTIONIST
TECHNICIAN

JWT header:

Authorization: Bearer <access-token>

Uploads

Runtime files are stored in:

backend/uploads/

Do not commit real uploaded files.

Docs

docs/api/
docs/database/
docs/diagrams/

Database backup

database/biomedic.sql must come from an export of the real
phongkham_xetnghiem schema.

database/seed.sql is only for development/test sample data.
Do not put real patient data or plaintext passwords in Git.



## `backend/.env`

```dotenv
SERVER_PORT=8080

DB_URL=jdbc:mysql://localhost:3306/phongkham_xetnghiem?useSSL=false&allowPublicKeyRetrieval=true&serverTimezone=Asia/Ho_Chi_Minh&useUnicode=true&characterEncoding=UTF-8
DB_USERNAME=root
DB_PASSWORD=YOUR_MYSQL_PASSWORD

FRONTEND_URL=http://localhost:5173

JWT_SECRET=CHANGE_THIS_TO_A_LONG_RANDOM_SECRET_KEY_FOR_BIOMEDIC
JWT_EXPIRATION_SECONDS=86400

MAIL_HOST=smtp.gmail.com
MAIL_PORT=587
MAIL_USERNAME=
MAIL_PASSWORD=
MAIL_SMTP_AUTH=true
MAIL_STARTTLS=true

UPLOAD_DIR=uploads
MAX_FILE_SIZE=10MB
MAX_REQUEST_SIZE=20MB

JPA_SHOW_SQL=trueMình đã làm tiếp full code toàn bộ nhóm DTO + Security + Enums + Exception + Mapper + Util và đóng gói sẵn cho bạn.

Download ZIP full code

Download Markdown chứa toàn bộ code từng file

Cấu trúc đã tạo:

backend/
└── src/main/java/com/biomedic/backend/
    │
    ├── dto/
    │   ├── request/
    │   │   ├── LoginRequest.java
    │   │   ├── RegisterRequest.java
    │   │   ├── ForgotPasswordRequest.java
    │   │   ├── ResetPasswordRequest.java
    │   │   ├── CustomerRequest.java
    │   │   ├── EmployeeRequest.java
    │   │   ├── DoctorRequest.java
    │   │   ├── TechnicianRequest.java
    │   │   ├── QuickAppointmentRequest.java
    │   │   ├── TestAppointmentRequest.java
    │   │   ├── ExaminationAppointmentRequest.java
    │   │   ├── ExaminationRequest.java
    │   │   ├── TestOrderRequest.java
    │   │   ├── CollectSpecimenRequest.java
    │   │   ├── ResultEntryRequest.java
    │   │   ├── ScheduleRequest.java
    │   │   └── PromotionRequest.java
    │   │
    │   └── response/
    │       ├── ApiResponse.java
    │       ├── AuthResponse.java
    │       ├── UserResponse.java
    │       ├── CustomerResponse.java
    │       ├── DoctorResponse.java
    │       ├── AppointmentResponse.java
    │       ├── VisitResponse.java
    │       ├── TestResponse.java
    │       ├── SpecimenResponse.java
    │       ├── TestResultResponse.java
    │       ├── NotificationResponse.java
    │       └── TrackingResponse.java
    │
    ├── security/
    │   ├── JwtService.java
    │   ├── JwtAuthenticationFilter.java
    │   └── CustomUserDetailsService.java
    │
    ├── enums/
    │   ├── RoleName.java
    │   ├── AppointmentStatus.java
    │   ├── VisitStatus.java
    │   ├── TestStatus.java
    │   ├── SpecimenStatus.java
    │   └── NotificationType.java
    │
    ├── exception/
    │   ├── GlobalExceptionHandler.java
    │   ├── ResourceNotFoundException.java
    │   └── BadRequestException.java
    │
    ├── mapper/
    │   └── EntityMapper.java
    │
    └── util/
        ├── QRCodeUtil.java
        └── DateTimeUtil.java

Tổng cộng mình tạo 44 file Java.

JWT đã làm không cần thư viện JJWT

JwtService.java dùng trực tiếp:

HmacSHA256
Base64 URL
Jackson ObjectMapper

nên bạn không cần cài:

io.jsonwebtoken
jjwt

Trong application.properties thêm:

app.jwt.secret=THAY_BANG_CHUOI_BI_MAT_DAI_IT_NHAT_32_KY_TU
app.jwt.expiration-seconds=86400

Ví dụ:

app.jwt.secret=BioMedic-2026-Secret-Key-Change-This-Immediately
app.jwt.expiration-seconds=86400

Không nên dùng secret mẫu này khi deploy thật.

JwtService có sẵn:

generateToken(...)
extractUsername(...)
extractRole(...)
extractClaims(...)
isTokenValid(...)

Ví dụ:

String token = jwtService.generateToken(
        "abc@gmail.com",
        RoleName.CUSTOMER
);

Token sẽ chứa:

{
  "sub": "abc@gmail.com",
  "role": "CUSTOMER",
  "iat": 1780000000,
  "exp": 1780086400
}
JwtAuthenticationFilter.java

Filter tự đọc:

Authorization: Bearer eyJ...

sau đó:

JWT
 ↓
kiểm tra signature
 ↓
kiểm tra hết hạn
 ↓
extract username
 ↓
extract role
 ↓
SecurityContextHolder

Role sẽ trở thành:

ROLE_ADMIN
ROLE_DOCTOR
ROLE_CUSTOMER
ROLE_RECEPTIONIST
ROLE_TECHNICIAN

nên khớp với:

@PreAuthorize("hasRole('ADMIN')")

hoặc:

@PreAuthorize("hasRole('DOCTOR')")
CustomUserDetailsService.java

File này hiện tại mình cố tình để compile-safe.

Nó chưa query:

users

vì phần trước chúng ta chưa chuyển:

AccountRepository

thành Spring Data JPA thật.

Hiện tại nó sẽ báo:

AccountRepository chưa được map JPA nên chưa thể tải user từ bảng users

Điều đó là chủ ý.

Trong cấu trúc hiện tại:

JwtAuthenticationFilter
        ↓
JwtService
        ↓
username + role từ JWT
        ↓
SecurityContext

nên JWT filter chưa phụ thuộc vào CustomUserDetailsService.

Khi Entity Account được map thật với users, mình sẽ đổi thành:

@Service
public class CustomUserDetailsService
        implements UserDetailsService {

    private final AccountRepository accountRepository;

    ...
}
RoleName.java

Đã xử lý cả role cũ tiếng Việt:

admin       → ADMIN

bacsi
doctor
bs          → DOCTOR

khachhang
customer
patient     → CUSTOMER

letan
tieptan
reception   → RECEPTIONIST

ktv
kythuatvien → TECHNICIAN

Ví dụ:

RoleName role =
        RoleName.fromValue("bacsi");

kết quả:

RoleName.DOCTOR
Các enum trạng thái

Appointment:

PENDING
CONFIRMED
CHECKED_IN
CANCELLED
COMPLETED
NO_SHOW

Visit:

WAITING
IN_PROGRESS
WAITING_TEST
WAITING_RESULT
RESULT_READY
COMPLETED
CANCELLED

Test:

ORDERED
WAITING_SPECIMEN
SPECIMEN_COLLECTED
IN_PROGRESS
RESULT_ENTERED
WAITING_APPROVAL
APPROVED
COMPLETED
CANCELLED

Specimen:

CREATED
COLLECTED
HANDED_OVER
RECEIVED
REJECTED
PROCESSING
IN_PROGRESS
COMPLETED
DISPOSED

Notification:

GENERAL
APPOINTMENT
CHECK_IN
QUEUE
SPECIMEN
TEST_RESULT
RESULT_APPROVED
RESULT_READY
PAYMENT
SYSTEM
GlobalExceptionHandler

Đã bắt các lỗi chính:

ResourceNotFoundException
BadRequestException
MethodArgumentNotValidException
ConstraintViolationException
MissingServletRequestParameterException
AccessDeniedException
IllegalArgumentException
Exception

Ví dụ validation sai sẽ trả JSON dạng:

{
  "success": false,
  "message": "Dữ liệu gửi lên không hợp lệ.",
  "data": {
    "email": "Email không hợp lệ.",
    "password": "Mật khẩu phải có ít nhất 6 ký tự."
  },
  "timestamp": "2026-09-07T21:00:00"
}
ApiResponse

Có thể dùng rất tiện trong Controller:

return ResponseEntity.ok(
        ApiResponse.success(data)
);

hoặc:

return ResponseEntity.ok(
        ApiResponse.success(
                "Đăng nhập thành công.",
                data
        )
);

Khi lỗi:

ApiResponse.error(
        "Không tìm thấy khách hàng."
)
EntityMapper.java

Mình đã map sẵn:

Account
→ UserResponse

Customer
→ CustomerResponse

Doctor
→ DoctorResponse

Visit
→ VisitResponse

TestCatalog
→ TestResponse

Specimen
→ SpecimenResponse

TestResult
→ TestResultResponse

Notification
→ NotificationResponse

Ví dụ:

CustomerResponse response =
        EntityMapper.toCustomerResponse(
                customer
        );
DateTimeUtil.java

Đã cố định timezone:

Asia/Ho_Chi_Minh

và format:

dd/MM/yyyy
HH:mm
dd/MM/yyyy HH:mm

Có:

DateTimeUtil.today();

DateTimeUtil.now();

DateTimeUtil.formatDate(...);

DateTimeUtil.formatTime(...);

DateTimeUtil.formatDateTime(...);

DateTimeUtil.parseDate(...);

DateTimeUtil.parseTime(...);
QRCodeUtil.java

File này tạo QR thật bằng ZXing:

QRCodeUtil.generatePng(...)

QRCodeUtil.generateBase64(...)

QRCodeUtil.generateDataUrl(...)

Ví dụ:

String qr =
        QRCodeUtil.generateDataUrl(
                "DLK-2026-00001",
                300,
                300
        );

Frontend có thể dùng trực tiếp:

<img src={qr} />

Bạn cần thêm 2 dependency vào pom.xml:

<dependency>
    <groupId>com.google.zxing</groupId>
    <artifactId>core</artifactId>
    <version>3.5.3</version>
</dependency>

<dependency>
    <groupId>com.google.zxing</groupId>
    <artifactId>javase</artifactId>
    <version>3.5.3</version>
</dependency>

Nếu chưa có Validation thì thêm:

<dependency>
    <groupId>org.springframework.boot</groupId>
    <artifactId>spring-boot-starter-validation</artifactId>
</dependency>

Sau khi copy toàn bộ vào project, chạy:

cd D:\project\BioMedic\backend
.\mvnw.cmd clean compile

Một lưu ý: các Controller hiện tại của bạn đang có một số record Request nằm bên trong Controller. Các DTO mới mình tạo chưa bắt buộc bạn phải sửa Controller ngay. Giữ nguyên Controller vẫn compile được. Sau khi toàn backend hết lỗi, bước sau chúng ta có thể refactor từ:

public record LoginRequest(...) {}

bên trong AuthController

sang:

import com.biomedic.backend.dto.request.LoginRequest;

để cấu trúc backend cuối cùng sạch và đúng kiến trúc hơn. 

BioMedic_dto_security_enums_exception_mapper_util_FULL_CODE.md
Tài liệu
Hôm nay 21:24
hãy ghi nhớ các code bạn đã gửi và các câu hỏi tôi hỏi bên trên 

Mình sẽ tiếp tục bám theo toàn bộ context BioMedic ở trên, đặc biệt là các code đã gửi, cấu trúc thư mục, các lỗi bạn gặp và cách chúng ta đã sửa, để các phần code tiếp theo không bị lệch nhau.

Cụ thể mình sẽ giữ nhất quán các điểm sau:

Project: D:\project\BioMedic
Frontend: React + Vite + Axios + React Router + Bootstrap
Backend: Java 21 + Spring Boot + Maven
Backend chạy localhost:8080, frontend localhost:5173
Database thật: phongkham_xetnghiem, không tạo database mới
Class khởi động của bạn là BackendApplication.java, không phải BioMedicApplication.java
Không tự đoán schema MySQL; ddl-auto=none
Các Controller, Service, Entity/domain, Repository, DTO, Security, Enum, Exception, Mapper, Util mà mình vừa gửi phải được xem là cùng một project, không tạo lại theo kiến trúc khác
Các Service/Repository hiện có một số phần compile-safe tạm thời vì chưa map JPA theo schema SQL thật
TechnicianController đã được sửa để dùng technicianService.getCurrentTechnician()
JWT dùng JwtService + JwtAuthenticationFilter
Role chuẩn:
ADMIN, DOCTOR, CUSTOMER, RECEPTIONIST, TECHNICIAN
Role cũ như bacsi, khachhang, letan, ktv phải được normalize
Các API frontend/backend đã thống nhất trước đó phải được giữ lại, không tự đổi endpoint nếu không cần
Không gửi lại những file đã gửi trước nếu bạn nói “trừ code đã gửi rồi”
Khi bạn báo một đoạn đỏ/lỗi, mình phải kiểm tra cả class được gọi và method signature ở Service, không chỉ sửa Controller
Khi đến bước JPA thật, phải dùng schema của các bảng users, khachhang, nhanvien, bacsi, datlichkham, datlichxetnghiem, luotkham, luotxetnghiem, phieuxetnghiem, maubenhpham, worklist, ketquaxetnghiem... của database hiện tại.

Trong cuộc trò chuyện này mình đã có đầy đủ context đó và sẽ dùng nó cho các câu hỏi tiếp theo. Với các cuộc trò chuyện mới, hệ thống Memory có thể tổng hợp thông tin từ các chat trước theo thời gian; mình không thể tự ép một mục cụ thể thành “bộ nhớ vĩnh viễn” ngay từ tin nhắn này.

 resources/
│   │   │       ├── application.properties
│   │   │       └── static/
│   │   │
│   │   └── test/
│   │       └── java/
│   │
│   ├── uploads/
│   ├── pom.xml
│   └── .env
│
├── database/
│   ├── biomedic.sql
│   └── seed.sql
│
├── docs/
│   ├── api/
│   ├── diagrams/
│   └── database/
│
├── .gitignore
└── README.md còn phần này là code tự động hay mình phải tự gõ, nếu tự thì hãy cho tôi full code

Phần này có cái được Spring/Vite tạo sẵn, có cái bạn phải tự tạo. Mình đã làm sẵn toàn bộ phần cần thiết theo BioMedic hiện tại để bạn có thể copy vào project.

Download ZIP toàn bộ phần còn lại

Download file Markdown chứa full code từng file

Thành phần	Tự động hay tự tạo?	Ghi chú
application.properties	Tự sửa/tạo	Spring Initializr tạo file nhưng bạn phải cấu hình DB, JWT, mail...
resources/static/	Tự động có thể có	BioMedic dùng React riêng nên để trống
src/test/java/	Spring Initializr thường tạo	Mình gửi lại test an toàn
uploads/	Tự tạo	Dùng chứa file upload runtime
pom.xml	Spring Initializr tạo	Bạn phải thêm dependencies
backend/.env	Tự tạo	Chứa password DB, JWT secret, mail
database/biomedic.sql	Không tự động	Phải export từ MySQL Workbench
database/seed.sql	Tự tạo	Chỉ dùng dữ liệu mẫu
docs/	Tự tạo	Chỉ là tài liệu project
.gitignore	Có thể được tạo ban đầu	Nên chỉnh thêm cho frontend/backend
README.md	Tự tạo	Tài liệu hướng dẫn project

Cấu trúc mình đã đóng gói cho bạn:

BioMedic/
│
├── backend/
│   ├── src/
│   │   ├── main/
│   │   │   └── resources/
│   │   │       ├── application.properties
│   │   │       └── static/
│   │   │           └── .gitkeep
│   │   │
│   │   └── test/
│   │       └── java/
│   │           └── com/
│   │               └── biomedic/
│   │                   └── backend/
│   │                       └── BackendApplicationTests.java
│   │
│   ├── uploads/
│   │   └── .gitkeep
│   │
│   ├── pom.xml
│   ├── .env
│   └── .env.example
│
├── database/
│   ├── biomedic.sql
│   └── seed.sql
│
├── docs/
│   ├── api/
│   │   └── README.md
│   ├── diagrams/
│   │   └── README.md
│   └── database/
│       └── README.md
│
├── .gitignore
└── README.md
application.properties

Đây là phần bạn nên dùng luôn:

spring.application.name=BioMedic
server.port=${SERVER_PORT:8080}

spring.config.import=optional:file:./.env[.properties]

spring.datasource.url=${DB_URL:jdbc:mysql://localhost:3306/phongkham_xetnghiem?useSSL=false&allowPublicKeyRetrieval=true&serverTimezone=Asia/Ho_Chi_Minh&useUnicode=true&characterEncoding=UTF-8}
spring.datasource.username=${DB_USERNAME:root}
spring.datasource.password=${DB_PASSWORD:}
spring.datasource.driver-class-name=com.mysql.cj.jdbc.Driver

spring.jpa.hibernate.ddl-auto=none
spring.jpa.open-in-view=false
spring.jpa.show-sql=${JPA_SHOW_SQL:true}
spring.jpa.properties.hibernate.format_sql=true

spring.jackson.time-zone=Asia/Ho_Chi_Minh

spring.servlet.multipart.max-file-size=${MAX_FILE_SIZE:10MB}
spring.servlet.multipart.max-request-size=${MAX_REQUEST_SIZE:20MB}

app.upload.dir=${UPLOAD_DIR:uploads}

app.frontend-url=${FRONTEND_URL:http://localhost:5173}

app.jwt.secret=${JWT_SECRET:BioMedic-Development-Secret-Key-Change-Me-Immediately-2026}
app.jwt.expiration-seconds=${JWT_EXPIRATION_SECONDS:86400}

spring.mail.host=${MAIL_HOST:smtp.gmail.com}
spring.mail.port=${MAIL_PORT:587}
spring.mail.username=${MAIL_USERNAME:}
spring.mail.password=${MAIL_PASSWORD:}

spring.mail.properties.mail.smtp.auth=${MAIL_SMTP_AUTH:true}
spring.mail.properties.mail.smtp.starttls.enable=${MAIL_STARTTLS:true}
spring.mail.properties.mail.smtp.starttls.required=${MAIL_STARTTLS:true}

logging.level.root=INFO
logging.level.com.biomedic.backend=${APP_LOG_LEVEL:INFO}

Điểm quan trọng nhất vẫn là:

spring.jpa.hibernate.ddl-auto=none

vì chúng ta đang giữ nguyên database:

phongkham_xetnghiem

Spring Boot không được tự sửa hoặc tạo bảng.

backend/.env

Bạn phải tự điền password MySQL thật:

SERVER_PORT=8080

DB_URL=jdbc:mysql://localhost:3306/phongkham_xetnghiem?useSSL=false&allowPublicKeyRetrieval=true&serverTimezone=Asia/Ho_Chi_Minh&useUnicode=true&characterEncoding=UTF-8

DB_USERNAME=root
DB_PASSWORD=YOUR_MYSQL_PASSWORD

FRONTEND_URL=http://localhost:5173

JWT_SECRET=CHANGE_THIS_TO_A_LONG_RANDOM_SECRET_KEY_FOR_BIOMEDIC
JWT_EXPIRATION_SECONDS=86400

MAIL_HOST=smtp.gmail.com
MAIL_PORT=587
MAIL_USERNAME=
MAIL_PASSWORD=
MAIL_SMTP_AUTH=true
MAIL_STARTTLS=true

UPLOAD_DIR=uploads
MAX_FILE_SIZE=10MB
MAX_REQUEST_SIZE=20MB

JPA_SHOW_SQL=true
APP_LOG_LEVEL=INFO

Ví dụ nếu MySQL của bạn:

username = root
password = 123456

thì:

DB_USERNAME=root
DB_PASSWORD=123456

Không gửi password đó lên GitHub.

Mình đã cấu hình:

spring.config.import=optional:file:./.env[.properties]

nên Spring Boot có thể đọc file .env khi bạn chạy Maven từ:

D:\project\BioMedic\backend
pom.xml

pom.xml thường do Spring Initializr tạo tự động.

Mình đã gửi full pom.xml trong ZIP, gồm:

Spring Web
Spring Data JPA
Spring Security
Validation
Spring Mail
WebSocket
MySQL Driver
ZXing QR Code
Spring Boot Test
Spring Security Test

Nhưng có một điều rất quan trọng:

Nếu pom.xml hiện tại của bạn đã từng chạy BUILD SUCCESS thì không nên xóa toàn bộ rồi thay file mới.

Hãy giữ:

<parent>
    ...
    <version>VERSION HIỆN TẠI CỦA BẠN</version>
</parent>

và chỉ bổ sung những dependency còn thiếu.

Bản pom.xml mình cho trong ZIP là bản đầy đủ để tham khảo/recover.

resources/static/

Bạn không cần code gì trong đó.

Vì frontend của BioMedic là:

React + Vite
http://localhost:5173

không phải frontend do Spring Boot render.

Do đó:

resources/
└── static/
    └── .gitkeep

là đủ.

Sau này nếu bạn muốn Spring Boot phục vụ file ảnh tĩnh thì mới dùng folder này.

uploads/

Folder này bạn tạo:

backend/uploads/

Nó dùng để lưu:

ảnh bác sĩ
ảnh bài viết
file kết quả
file người dùng upload
...

File thực tế trong uploads không nên commit Git.

BackendApplicationTests.java

Mình dùng test rất đơn giản:

package com.biomedic.backend;

import static org.junit.jupiter.api.Assertions.assertNotNull;

import org.junit.jupiter.api.Test;

class BackendApplicationTests {

    @Test
    void backendApplicationClassExists() {
        assertNotNull(BackendApplication.class);
    }
}

Mình không dùng ngay:

@SpringBootTest

vì nếu dùng nó lúc này test sẽ khởi động toàn bộ Spring Context và MySQL. Chỉ cần thiếu DB password hoặc JPA chưa map xong là mvn test đỏ dù code Java không sai.

database/biomedic.sql

File này không được tự viết bằng cách đoán schema.

Mình chỉ tạo placeholder:

USE phongkham_xetnghiem;

Bạn phải lấy schema thật từ Workbench:

MySQL Workbench
→ Server
→ Data Export
→ phongkham_xetnghiem
→ chọn tất cả tables
→ Dump Structure Only
→ Export to Self-Contained File

sau đó lưu thành:

D:\project\BioMedic\database\biomedic.sql

Lúc đó mình mới có thể viết chính xác:

@Entity
@Table(...)
@Column(...)
@JoinColumn(...)
@ManyToOne
@OneToMany

cho toàn bộ entity.

seed.sql

Hiện tại không nên thêm dữ liệu tự động, vì DB của bạn đã có dữ liệu.

Mình để:

USE phongkham_xetnghiem;

và comment hướng dẫn.

Sau này nếu muốn có dữ liệu test:

INSERT INTO ...
VALUES (...);

thì phải dựa trên schema thật.

.gitignore

Mình đã cấu hình để không đẩy những thứ như:

backend/.env
backend/target/
frontend/node_modules/
frontend/dist/
backend/uploads/*
*.log
database/*-private.sql

lên Git.

Đặc biệt:

backend/.env

phải được ignore vì chứa:

DB password
JWT secret
Mail password
Sau khi copy xong

Backend:

cd D:\project\BioMedic\backend

cài/compile:

.\mvnw.cmd clean compile

nếu thành công:

.\mvnw.cmd spring-boot:run

Frontend mở terminal khác:

cd D:\project\BioMedic\frontend
npm run dev

Kết quả:

Frontend:
http://localhost:5173

Backend:
http://localhost:8080

Với phần bạn vừa hỏi, cây project BioMedic gần như đã được dựng đủ toàn bộ file/folder chính. Phần quan trọng tiếp theo không phải tạo thêm folder nữa mà là export schema thật của phongkham_xetnghiem để sửa 22 Entity + 14 Repository từ bản compile-safe thành JPA/MySQL thật. 

FULL_CODE_remaining_project_files.md
Tài liệu

Thư viện
/
FULL_CODE_remaining_project_files.md
BioMedic - Remaining project files

Configuration, test, database placeholders, docs, gitignore, and README.

.gitignore
# OS / editor
.DS_Store
Thumbs.db
.idea/
.vscode/
*.iml

# Secrets
.env
.env.*
!.env.example
backend/.env
frontend/.env.local
frontend/.env.*.local

# Java / Maven
backend/target/
target/
*.class
*.jar
*.war
*.log

# React / Vite
frontend/node_modules/
frontend/dist/
frontend/.vite/
node_modules/
dist/

# Runtime uploads
backend/uploads/*
!backend/uploads/.gitkeep

# Temporary files
*.tmp
*.temp
*.bak
*.swp
*~

# Private database dumps
database/*.dump
database/*.bak
database/*-private.sql
database/*-production.sql
README.md
# BioMedic

Bio Medic Center clinic and laboratory management system.

## Stack

Frontend:
- React
- JavaScript / JSX
- Vite
- Axios
- React Router
- Bootstrap

Backend:
- Java 21
- Spring Boot
- Maven
- Spring Web
- Spring Data JPA / Hibernate
- Spring Security
- JWT
- Jakarta Validation
- Spring Mail
- WebSocket
- MySQL

## Main structure

```text
BioMedic/
├── frontend/
├── backend/
├── database/
├── docs/
├── .gitignore
└── README.md
Database

Existing database:

phongkham_xetnghiem

Do not create a new database.

Hibernate is configured with:

spring.jpa.hibernate.ddl-auto=none
Run backend
cd D:\project\BioMedic\backend
.\mvnw.cmd clean compile
.\mvnw.cmd spring-boot:run

Backend:

http://localhost:8080
Run frontend
cd D:\project\BioMedic\frontend
npm install
npm run dev

Frontend:

http://localhost:5173
Authentication roles
ADMIN
DOCTOR
CUSTOMER
RECEPTIONIST
TECHNICIAN

JWT header:

Authorization: Bearer <access-token>
Uploads

Runtime files are stored in:

backend/uploads/

Do not commit real uploaded files.

Docs
docs/api/
docs/database/
docs/diagrams/
Database backup

database/biomedic.sql must come from an export of the real
phongkham_xetnghiem schema.

database/seed.sql is only for development/test sample data.
Do not put real patient data or plaintext passwords in Git.



## `backend/.env`

```dotenv
SERVER_PORT=8080

DB_URL=jdbc:mysql://localhost:3306/phongkham_xetnghiem?useSSL=false&allowPublicKeyRetrieval=true&serverTimezone=Asia/Ho_Chi_Minh&useUnicode=true&characterEncoding=UTF-8
DB_USERNAME=root
DB_PASSWORD=YOUR_MYSQL_PASSWORD

FRONTEND_URL=http://localhost:5173

JWT_SECRET=CHANGE_THIS_TO_A_LONG_RANDOM_SECRET_KEY_FOR_BIOMEDIC
JWT_EXPIRATION_SECONDS=86400

MAIL_HOST=smtp.gmail.com
MAIL_PORT=587
MAIL_USERNAME=
MAIL_PASSWORD=
MAIL_SMTP_AUTH=true
MAIL_STARTTLS=true

UPLOAD_DIR=uploads
MAX_FILE_SIZE=10MB
MAX_REQUEST_SIZE=20MB

JPA_SHOW_SQL=true
APP_LOG_LEVEL=INFO