# TÀI LIỆU THIẾT KẾ CƠ SỞ DỮ LIỆU - ATTENDANCE SYSTEM

Tài liệu này mô tả chi tiết kiến trúc các Models (Entities) của hệ thống điểm danh, tập trung vào cách tổ chức dữ liệu theo mô hình tín chỉ kết hợp lớp hành chính (như FPT University).

---

## 1. SƠ ĐỒ QUAN HỆ (ERD)

```mermaid
erDiagram
    USER ||--o| STUDENT : "1-1"
    USER ||--o| LECTURER : "1-1"
    
    CLASS ||--o{ CLASS_STUDENT : "1-N"
    STUDENT ||--o{ CLASS_STUDENT : "1-N"
    
    CLASS ||--o{ CLASS_SUBJECT : "1-N"
    SUBJECT ||--o{ CLASS_SUBJECT : "1-N"
    LECTURER ||--o{ CLASS_SUBJECT : "1-N"
    SEMESTER ||--o{ CLASS_SUBJECT : "1-N"
    
    CLASS_SUBJECT ||--o{ SCHEDULE : "1-N"
    CLASS_SUBJECT ||--o{ ATTENDANCE_SESSION : "1-N"
    
    SCHEDULE ||--o{ ATTENDANCE_SESSION : "1-N"
    LECTURER ||--o{ ATTENDANCE_SESSION : "1-N"
    
    ATTENDANCE_SESSION ||--o{ ATTENDANCE_RECORD : "1-N"
    STUDENT ||--o{ ATTENDANCE_RECORD : "1-N"
```

---

## 2. CHI TIẾT CÁC MODELS VÀ DỮ LIỆU MẪU

### 2.1. Nhóm Người dùng (Users & Roles)

#### Model: `User`
**Mô tả:** Nơi lưu trữ thông tin đăng nhập và thông tin chung cốt lõi của mọi người dùng (tránh lặp dữ liệu `Email`, `FullName` ở các bảng khác).

**Quan hệ:** 1-1 với `Student`, 1-1 với `Lecturer`.

| Id | Username | PasswordHash | FullName | Email | Role |
| :--- | :--- | :--- | :--- | :--- | :--- |
| 1 | `admin` | `***` | Quản trị viên | admin@fpt.edu.vn | Admin |
| 2 | `thaya` | `***` | Nguyễn Văn A | an@fpt.edu.vn | Lecturer |
| 3 | `svb` | `***` | Trần Văn B | btvse1801@fpt.edu.vn | Student |

#### Model: `Student` & `Lecturer`
**Mô tả:** Chứa các thông tin đặc thù của sinh viên hoặc giảng viên. Trỏ khóa ngoại `UserId` về bảng `User`.

**Dữ liệu mẫu `Student`:**

| Id | UserId | StudentCode | Faculty | Major |
| :--- | :--- | :--- | :--- | :--- |
| 10 | 3 | SE180011 | IT | Software Eng. |

---

### 2.2. Nhóm Khung chương trình (Subject & Semester)

#### Model: `Semester` (Học kỳ)
**Mô tả:** Quản lý các học kỳ theo thời gian thực.

**Dữ liệu mẫu:**

| Id | Name | StartDate | EndDate |
| :--- | :--- | :--- | :--- |
| 3 | Fall 2026 | 2026-09-01 | 2026-12-31 |

#### Model: `Subject` (Môn học)
**Mô tả:** Danh mục các môn học trong trường.

**Dữ liệu mẫu:**

| Id | SubjectCode | SubjectName | Credits |
| :--- | :--- | :--- | :--- |
| 50 | PRN231 | Build Cross-Platform Apps | 3 |
| 51 | PRJ301 | Java Web Development | 3 |

---

### 2.3. Nhóm Lớp học (Class & ClassSubject)

#### Model: `Class` (Lớp hành chính)
**Mô tả:** Đại diện cho lớp hành chính. Lớp này bao gồm 1 tập hợp sinh viên cố định (`ClassStudent`).

**Quan hệ:** 1-N với `ClassStudent` (Danh sách SV), 1-N với `ClassSubject` (Danh sách môn học).

| Id | ClassCode | ClassName |
| :--- | :--- | :--- |
| 1 | SE1801 | Lớp Khối SE1801 |
| 2 | SE1802 | Lớp Khối SE1802 |

#### Model: `ClassStudent` (Sinh viên thuộc Lớp)
**Mô tả:** Bảng trung gian móc nối sinh viên vào lớp hành chính.

| Id | ClassId | StudentId | Status |
| :--- | :--- | :--- | :--- |
| 100 | 1 (SE1801) | 10 (Trần Văn B) | Active |

#### Model: `ClassSubject` (Lớp học phần / Phân công giảng dạy)
**Mô tả:** **TRÁI TIM CỦA HỆ THỐNG.** Bảng này móc nối: Lớp nào? Học môn gì? Ai dạy? Kỳ nào?

**Quan hệ:** Trỏ đến `Class`, `Subject`, `Lecturer`, `Semester`. Chứa danh sách `Schedule` và `AttendanceSession`.

| Id | ClassId | SubjectId | LecturerId | SemesterId | Status |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **800** | 1 (SE1801)| 50 (PRN231) | 2 (Thầy A) | 3 (Fall26)| Active |
| **801** | 1 (SE1801)| 51 (PRJ301) | 5 (Cô C) | 3 (Fall26)| Active |

*💡 Đọc dữ liệu:* Dòng `800` nghĩa là: Lớp SE1801 học môn PRN231, do thầy Nguyễn Văn A dạy vào kỳ Fall 2026.

---

### 2.4. Nhóm Điểm danh (Schedule, Session, Record)

#### Model: `Schedule` (Lịch học định kỳ)
**Mô tả:** Lịch thời khóa biểu cố định trong tuần của một Lớp Học Phần (`ClassSubject`).

| Id | ClassSubjectId | DayOfWeek | StartTime | EndTime | Room |
| :--- | :--- | :--- | :--- | :--- | :--- |
| 1001| 800 (SE1801-PRN231) | 1 (Monday) | 07:30:00 | 09:50:00 | AL-201 |
| 1002| 800 (SE1801-PRN231) | 4 (Thursday) | 10:00:00 | 12:20:00 | AL-201 |

#### Model: `AttendanceSession` (Phiên điểm danh thực tế)
**Mô tả:** Sinh ra thực tế mỗi khi giảng viên lên lớp mở app điểm danh.

**Quan hệ:** Trỏ về `ClassSubject` (Môn học) và `Schedule` (Buổi cố định nào).

| Id | ClassSubjectId | ScheduleId | SessionDate | Status | OpenedAt |
| :--- | :--- | :--- | :--- | :--- | :--- |
| 5000| 800 (SE1801-PRN231)| 1001 (Sáng T2) | 2026-09-07 | Closed | 07:30:05 |
| 5001| 800 (SE1801-PRN231)| 1002 (Sáng T5) | 2026-09-10 | Open | 10:01:00 |

#### Model: `AttendanceRecord` (Chi tiết điểm danh của từng SV)
**Mô tả:** Ghi nhận lịch sử check-in của sinh viên. Nếu trốn học, dòng sẽ do hệ thống tạo và báo Absent.

| Id | AttendanceSessionId | StudentId | Status | CheckInTime | CheckInMethod |
| :--- | :--- | :--- | :--- | :--- | :--- |
| 1 | 5000 (Sáng T2, 07/09)| 10 (Trần Văn B) | **Present** | 07:32:00 | QR |
| 2 | 5000 (Sáng T2, 07/09)| 11 (Lê Văn C) | **Absent** | NULL | NULL |
| 3 | 5001 (Sáng T5, 10/09)| 10 (Trần Văn B) | **Late** | 10:25:00 | QR |
| 4 | 5001 (Sáng T5, 10/09)| 11 (Lê Văn C) | **Present** | 10:02:00 | Manual |

---

## 3. WORKFLOW TRUY VẤN DỮ LIỆU (VÍ DỤ)

1. **Hiển thị Thời khóa biểu cho Sinh viên B (Id=10)**
   - Query bảng `ClassStudent` -> Tìm xem B thuộc `ClassId` nào (VD: 1 - SE1801).
   - Query bảng `ClassSubject` lấy tất cả dòng có `ClassId = 1` -> Ra các môn B học.
   - Nối (JOIN) vào bảng `Schedule` để lấy lịch thứ mấy, phòng mấy.

2. **Giảng viên A mở phiên điểm danh lớp SE1801 môn PRN231**
   - Từ `ClassSubjectId = 800`, tạo mới 1 `AttendanceSession` nối với `ScheduleId = 1001`.
   - Sinh viên B quét mã QR -> Insert vào `AttendanceRecord` (SessionId = Session vừa tạo, StudentId = 10).

3. **Tính phần trăm vắng mặt môn PRN231 của sinh viên B**
   - Lấy tổng số `AttendanceSession` của `ClassSubjectId = 800` (đã `Closed`).
   - Query `AttendanceRecord` đếm số buổi có `Status = Absent` của `StudentId = 10`.
   - Tính %.

---

## 4. QUYẾT ĐỊNH KIẾN TRÚC (ARCHITECTURE DECISIONS)

### 4.1. Tại sao `Student` trỏ Khóa ngoại đến `User` thay vì Kế thừa (`Student : User`)?
Mặc dù về mặt logic OOP, sinh viên là một loại người dùng, nhưng trong thiết kế CSDL với Entity Framework Core, chúng ta sử dụng **Quan hệ 1-1 (Composition)** thay vì **Kế thừa (Inheritance)** vì:
- **Ngăn chặn Table-Per-Hierarchy (TPH):** Nếu `Student` kế thừa `User` (một Entity có thật), EF Core mặc định sẽ gom tất cả (User, Student, Lecturer) vào ĐÚNG 1 bảng `Users` khổng lồ. Bảng này sẽ chứa vô số các cột rỗng (NULL) do sự chênh lệch thuộc tính giữa sinh viên và giảng viên, gây phình to và chậm Database.
- **Tách biệt Auth và Business:** Bảng `User` chỉ chuyên dùng để xử lý Đăng nhập/Xác thực (Auth). Bảng `Student` chứa nghiệp vụ học tập. Tách riêng giúp hệ thống chuẩn hóa (Normalize) và dễ bảo trì.
- **Một tài khoản nhiều vai trò:** Bằng cách tách bảng, một tài khoản đăng nhập `User` có thể vừa trỏ khóa ngoại từ bảng `Student`, vừa trỏ khóa ngoại từ bảng `Lecturer` (Ví dụ: sinh viên cao học kiêm trợ giảng).

### 4.2. Tại sao các Entity kế thừa `BaseEntity` mà EF Core lại không gom chung vào 1 bảng?
- `BaseEntity` được khai báo là lớp trừu tượng (`public abstract class BaseEntity`) và **KHÔNG** được đăng ký làm một bảng thực sự (`DbSet`) trong `AppDbContext`.
- Do không có `DbSet`, EF Core không coi `BaseEntity` là một thực thể CSDL. Nó chỉ đóng vai trò là một "bản mẫu" (Code Template).
- Khi `Student` kế thừa `BaseEntity`, EF Core đơn giản là **copy** 3 cột (`CreatedAt`, `UpdatedAt`, `IsDeleted`) và dán trực tiếp vào bảng `Students`. (Đây là cơ chế ánh xạ thuộc tính cơ bản của EF Core đối với lớp trừu tượng không có DbSet).
- Nhờ vậy, chúng ta có được sự đồng nhất của OOP (viết các hàm Generic dùng chung, tự động điền ngày tháng cho mọi bảng khi lưu data) mà không làm hỏng cấu trúc chuẩn hóa của SQL.
