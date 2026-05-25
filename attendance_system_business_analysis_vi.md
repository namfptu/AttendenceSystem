# PHÂN TÍCH NGHIỆP VỤ HỆ THỐNG QUẢN LÍ ĐIỂM DANH

## 1. Giới thiệu dự án

### 1.1 Tên dự án
Attendance Management System

### 1.2 Mục tiêu dự án
Xây dựng hệ thống quản lí điểm danh dành cho môi trường trường học/đại học.

Hệ thống hỗ trợ:

- Quản lí sinh viên
- Quản lí giảng viên
- Quản lí môn học
- Quản lí lớp học phần
- Quản lí lịch học
- Điểm danh bằng QR code
- Theo dõi trạng thái đi học
- Báo cáo thống kê
- Dashboard quản trị

### 1.3 Công nghệ sử dụng

#### Backend
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- JWT Authentication
- SignalR (optional)

#### Frontend
- ASP.NET Core MVC (Razor)
- Bootstrap
- JavaScript / jQuery

#### Khác
- ClosedXML (Export Excel)
- AutoMapper
- FluentValidation (optional)

---

# 2. Kiến trúc hệ thống

## 2.1 Kiến trúc tổng thể

```text
Browser
   ↓
AttendanceSystem.Web
   ↓
MVC Controller
   ↓
ApiClient / HttpClient Service
   ↓ HTTP Request
AttendanceSystem.API
   ↓
API Controller
   ↓
Business Service
   ↓
Repository
   ↓
AppDbContext
   ↓
SQL Server
```

## 2.2 Mô tả các layer

### AttendanceSystem.Web
Frontend MVC Razor.

Chứa:

- Controllers
- Views
- ViewModels
- ApiClients
- wwwroot

Nhiệm vụ:

- Hiển thị giao diện
- Gửi request tới API
- Xử lý session/token
- Render dữ liệu cho người dùng

### AttendanceSystem.API
Backend RESTful API.

Chứa:

- Controllers
- Authentication
- Authorization
- Middleware
- Swagger

Nhiệm vụ:

- Nhận request từ frontend
- Validate request
- Kiểm tra quyền
- Gọi business layer

### AttendanceSystem.Business
Business layer.

Chứa:

- Services
- DTOs
- Interfaces
- Validators
- Mappings

Nhiệm vụ:

- Xử lý nghiệp vụ hệ thống
- Logic điểm danh
- Logic check-in QR
- Logic báo cáo

### AttendanceSystem.Data
Data access layer.

Chứa:

- Entities
- DbContext
- Repositories
- Configurations
- Migrations

Nhiệm vụ:

- Làm việc với EF Core
- Truy vấn dữ liệu
- Lưu dữ liệu SQL Server

---

# 3. Actors hệ thống

## 3.1 Admin

Admin là người quản trị hệ thống.

### Chức năng

- Quản lí tài khoản
- Quản lí sinh viên
- Quản lí giảng viên
- Quản lí môn học
- Quản lí lớp học phần
- Quản lí lịch học
- Gán sinh viên vào lớp
- Gán giảng viên vào lớp
- Xem báo cáo tổng
- Khóa/mở tài khoản

---

## 3.2 Giảng viên

Giảng viên là người trực tiếp quản lí điểm danh.

### Chức năng

- Xem lớp đang dạy
- Xem danh sách sinh viên
- Tạo phiên điểm danh
- Mở điểm danh QR
- Theo dõi check-in realtime
- Chỉnh sửa trạng thái điểm danh
- Đóng phiên điểm danh
- Export Excel báo cáo

---

## 3.3 Sinh viên

Sinh viên là người thực hiện check-in.

### Chức năng

- Đăng nhập
- Xem lịch học
- Xem lớp học phần
- Check-in bằng QR
- Xem lịch sử điểm danh
- Xem số buổi vắng/muộn

---

# 4. Chức năng hệ thống

## 4.1 Authentication & Authorization

### Chức năng

- Đăng nhập
- Đăng xuất
- JWT Authentication
- Role-based Authorization
- Đổi mật khẩu
- Khóa tài khoản

### Roles

- Admin
- Lecturer
- Student

---

## 4.2 Quản lí sinh viên

### Chức năng

- Thêm sinh viên
- Cập nhật sinh viên
- Xóa mềm sinh viên
- Tìm kiếm sinh viên
- Import Excel
- Export Excel
- Xem lịch sử điểm danh

### Thông tin sinh viên

- StudentCode
- Faculty
- Major
*(Ghi chú: FullName, Email, Phone... được lưu chung tại bảng Users)*

---

## 4.3 Quản lí giảng viên

### Chức năng

- Thêm giảng viên
- Cập nhật giảng viên
- Xóa mềm giảng viên
- Gán giảng viên vào lớp
- Xem lớp đang dạy

---

## 4.4 Quản lí môn học

### Chức năng

- CRUD môn học
- Tìm kiếm môn học
- Kích hoạt/vô hiệu hóa môn học

### Thông tin

- SubjectCode
- SubjectName
- Credits
- Description

---

## 4.5 Quản lí Lớp hành chính & Phân công giảng dạy

### Lớp hành chính (Class)
- Quản lí mã lớp (VD: SE1801)
- Gán sinh viên vào lớp cố định

### Lớp học phần / Phân công giảng dạy (ClassSubject)
- Gán môn học cho lớp
- Gán giảng viên phụ trách
- Quản lí trạng thái môn học (Active, Closed)
- Là cơ sở để sinh lịch học và điểm danh

---

## 4.6 Quản lí lịch học

### Chức năng

- Tạo lịch học
- Cập nhật lịch học
- Xóa lịch học
- Xem lịch học

### Thông tin

- DayOfWeek
- StartTime
- EndTime
- Room
- EffectiveFrom
- EffectiveTo

---

## 4.7 Điểm danh

### Chức năng

- Tạo phiên điểm danh
- Sinh QR code
- Check-in QR
- Xử lý Present/Late/Absent
- Chỉnh sửa điểm danh thủ công
- Đóng phiên điểm danh

### Attendance Status

- Present
- Late
- Absent
- Excused

---

## 4.8 Báo cáo & Dashboard

### Chức năng

- Thống kê tỉ lệ điểm danh
- Sinh viên vắng nhiều
- Sinh viên đi muộn nhiều
- Dashboard tổng quan
- Export Excel

---

# 5. Workflow nghiệp vụ

## 5.1 Workflow Admin

1. Admin tạo tài khoản.
2. Admin tạo môn học.
3. Admin tạo lớp học phần.
4. Admin gán giảng viên.
5. Admin thêm sinh viên vào lớp.
6. Admin tạo lịch học.

---

## 5.2 Workflow giảng viên tạo phiên điểm danh

1. Giảng viên đăng nhập.
2. Chọn lớp học phần.
3. Nhấn tạo phiên điểm danh.
4. Hệ thống tạo Attendance Session.
5. Hệ thống sinh QR token.
6. Phiên chuyển sang trạng thái Open.

---

## 5.3 Workflow sinh viên check-in

1. Sinh viên đăng nhập.
2. Quét QR code.
3. Frontend gửi token lên API.
4. API kiểm tra:
   - Sinh viên thuộc lớp?
   - Phiên còn mở?
   - Token hợp lệ?
   - Đã check-in chưa?
5. Hệ thống ghi Attendance Record.
6. Trả kết quả check-in.

---

## 5.4 Workflow xử lý đi muộn

Ví dụ:

- Bắt đầu lớp: 7:30
- LateAfterMinutes: 15

Logic:

- Check-in trước 7:45 => Present
- Check-in sau 7:45 => Late
- Không check-in => Absent

---

## 5.5 Workflow đóng phiên điểm danh

1. Giảng viên nhấn đóng phiên.
2. Hệ thống lấy danh sách sinh viên trong lớp.
3. Sinh viên chưa check-in sẽ bị đánh dấu Absent.
4. Session chuyển sang Closed.

---

# 6. Database Design

## 6.1 Entities chính

### Users

```text
Users
- Id
- Username
- Email
- PasswordHash
- FullName
- Role
- IsActive
- CreatedAt
- UpdatedAt
- IsDeleted
```

---

### Students

```text
Students
- Id
- UserId
- StudentCode
- Faculty
- Major
- CreatedAt
- UpdatedAt
- IsDeleted
```

---

### Lecturers

```text
Lecturers
- Id
- UserId
- LecturerCode
- Department
- CreatedAt
- UpdatedAt
- IsDeleted
```

---

### Subjects

```text
Subjects
- Id
- SubjectCode
- SubjectName
- Credits
- Description
- CreatedAt
- UpdatedAt
- IsDeleted
```

---

### Semesters

```text
Semesters
- Id
- Name
- StartDate
- EndDate
- CreatedAt
- UpdatedAt
- IsDeleted
```

---

### Classes (Lớp hành chính)

```text
Classes
- Id
- ClassCode
- ClassName
- CreatedAt
- UpdatedAt
- IsDeleted
```

---

### ClassSubjects (Lớp học phần)

```text
ClassSubjects
- Id
- ClassId
- SubjectId
- LecturerId
- SemesterId
- Status
- CreatedAt
- UpdatedAt
- IsDeleted
```

---

### ClassStudents

```text
ClassStudents
- Id
- ClassId
- StudentId
- EnrolledAt
- Status
- CreatedAt
- UpdatedAt
- IsDeleted
```

---

### Schedules

```text
Schedules
- Id
- ClassSubjectId
- DayOfWeek
- StartTime
- EndTime
- Room
- CreatedAt
- UpdatedAt
- IsDeleted
```

---

### AttendanceSessions

```text
AttendanceSessions
- Id
- ClassSubjectId
- ScheduleId
- SessionDate
- Title
- StartTime
- EndTime
- LateAfterMinutes
- Status
- OpenedAt
- ClosedAt
- CreatedByLecturerId
- QrToken
- QrExpiredAt
- AllowedLatitude
- AllowedLongitude
- AllowedRadiusMeters
- CreatedAt
- UpdatedAt
- IsDeleted
```

---

### AttendanceRecords

```text
AttendanceRecords
- Id
- AttendanceSessionId
- StudentId
- Status
- CheckInTime
- CheckInMethod
- IsManualEdited
- EditedByLecturerId
- EditedAt
- Note
- CheckInLatitude
- CheckInLongitude
- CheckInIpAddress
- UserAgent
- CreatedAt
- UpdatedAt
- IsDeleted
```

---

# 7. Cardinality

```text
User 1 - 1 Student
User 1 - 1 Lecturer

Class 1 - N ClassStudent
Student 1 - N ClassStudent

Class 1 - N ClassSubject
Subject 1 - N ClassSubject
Lecturer 1 - N ClassSubject
Semester 1 - N ClassSubject

ClassSubject 1 - N Schedule
ClassSubject 1 - N AttendanceSession

Schedule 1 - N AttendanceSession

AttendanceSession 1 - N AttendanceRecord
Student 1 - N AttendanceRecord
```

---

# 8. REST API Design

## Authentication

```http
POST /api/auth/login
POST /api/auth/logout
GET  /api/auth/me
```

---

## Students

```http
GET    /api/students
GET    /api/students/{id}
POST   /api/students
PUT    /api/students/{id}
DELETE /api/students/{id}
```

---

## Course Classes

```http
GET    /api/classes
POST   /api/classes
PUT    /api/classes/{id}
DELETE /api/classes/{id}
```

---

## Attendance Sessions

```http
POST /api/classes/{classId}/attendance-sessions
POST /api/attendance-sessions/{id}/open
POST /api/attendance-sessions/{id}/close
```

---

## QR Check-in

```http
POST /api/attendance/check-in
```

---

# 9. Best Practices

## Layered Architecture

Không để MVC hoặc API Controller xử lý business logic trực tiếp.

Luồng đúng:

```text
Controller -> Service -> Repository -> DbContext
```

---

## DTO Pattern

Không return Entity trực tiếp.

Nên dùng:

- StudentDto
- AttendanceRecordDto
- AttendanceSessionDto

---

## AsNoTracking

Dùng cho query chỉ đọc.

Ví dụ:

```csharp
_context.Students
    .AsNoTracking()
    .ToListAsync();
```

---

## Pagination

Ví dụ:

```http
GET /api/students?pageNumber=1&pageSize=10
```

---

## Unique Constraints

Nên có:

- StudentCode unique
- LecturerCode unique
- SubjectCode unique
- AttendanceSessionId + StudentId unique

---

# 10. Feature nâng cao

## QR Attendance

- Sinh QR token
- Sinh viên quét QR để check-in
- Token có thời hạn

---

## SignalR Realtime

Hiển thị realtime:

- Sinh viên vừa check-in
- Tổng số đã điểm danh

---

## Dashboard

### Admin Dashboard

- Tổng sinh viên
- Tổng lớp
- Tổng phiên điểm danh
- Tỉ lệ chuyên cần

### Lecturer Dashboard

- Lớp đang dạy
- Sinh viên vắng nhiều
- Buổi học gần nhất

---

## Export Excel

Sử dụng ClosedXML.

Export:

- Danh sách điểm danh
- Báo cáo lớp
- Báo cáo sinh viên

---

# 11. Scope đề xuất cho đồ án

## Must-have

- Login phân quyền
- CRUD dữ liệu nền
- Quản lí lớp học phần
- Tạo phiên điểm danh
- Check-in QR
- Xử lý Present/Late/Absent
- Báo cáo cơ bản

---

## Should-have

- Dashboard
- Export Excel
- Pagination
- Search/filter

---

## Nice-to-have

- SignalR
- Import Excel
- Audit log
- Refresh token

---

# 12. Kết luận

Hệ thống quản lí điểm danh được thiết kế theo kiến trúc nhiều lớp nhằm tách biệt frontend, API, business logic và data access.

Hệ thống tập trung vào:

- Quản lí dữ liệu học tập
- Điểm danh bằng QR
- Tự động xử lý trạng thái chuyên cần
- Theo dõi và báo cáo thống kê

Dự án phù hợp cho:

- Đồ án môn học
- Đồ án tốt nghiệp
- Portfolio cá nhân
- CV backend .NET

