# Nhiệm vụ của Dev A & AI Prompt (Vibe Coding)

Dựa trên tài liệu thiết kế hệ thống, đây là danh sách các tính năng mà **Dev A (The Admin & Foundation)** chịu trách nhiệm, cùng với một Prompt chi tiết để sử dụng cho các công cụ AI (như Cursor, GitHub Copilot, ChatGPT) nhằm generate code nhanh chóng.

## 📋 Danh sách các chức năng cần làm (Checklist)

### 1. Nền tảng (Foundation)
- [ ] Khởi tạo project ASP.NET Core MVC / Web API.
- [ ] Cấu hình Entity Framework Core kết nối với SQL Server sử dụng `AppDbContext` đã có.
- [ ] Thiết lập chuẩn API Response Format thống nhất (VD: `{ isSuccess, message, data }`).

### 2. Xác thực & Phân quyền (Authentication & Authorization)
- [ ] Chức năng Đăng nhập (Login).
- [ ] Phân quyền theo Role (Admin, Lecturer, Student).
- [ ] Middleware kiểm tra Role và xử lý Redirect khi không có quyền.

### 3. Quản trị Dữ liệu (Master Data Management)
- [ ] **Quản lý Học kỳ (Semester):** Trang CRUD và thiết lập học kỳ hiện tại.
- [ ] **Quản lý Môn học (Subject):** Trang CRUD thông tin môn học, số tín chỉ.
- [ ] **Quản lý Người dùng (Users):** Import danh sách tài khoản từ file Excel, tự động sinh AvatarUrl và ánh xạ dữ liệu vào bảng `Students`/`Lecturers`.

### 4. Thiết lập Đào tạo (Academic Setup)
- [ ] **Quản lý Lớp hành chính (Class):** Tạo lớp và thêm sinh viên vào lớp.
- [ ] **Phân công Lớp học phần (ClassSubject):** Gán Môn học, Lớp, Giảng viên vào Học kỳ.
- [ ] **Tạo Thời khóa biểu (Schedule):** Thiết lập lịch học cố định trong tuần.
- [ ] **Phân công dạy thay:** Giao diện cho Admin gán giảng viên dạy thay (`ClassSubstitute`).

---

## 🤖 Prompt AI chi tiết cho Dev A

*Lưu ý: Copy toàn bộ nội dung bên dưới dán vào Cursor Composer (Ctrl + I) hoặc ChatGPT/Claude để AI bắt đầu code cho bạn.*

```markdown
Bạn là một Senior .NET Fullstack Developer. Hãy giúp tôi xây dựng phần Admin Portal cho hệ thống Điểm danh (Attendance System) bằng ASP.NET Core MVC (sử dụng Razor Views, C# 11, .NET 8) và Entity Framework Core. Giao diện Frontend sử dụng template Bootstrap 5.

Dưới đây là các yêu cầu chi tiết. Hãy thực hiện theo từng bước, tôi sẽ phản hồi lại sau mỗi bước bạn hoàn thành.

### QUY TẮC CHUNG (RULES):
1. **Kiến trúc:** Cấu trúc theo mô hình N-Tier (Web, Business, Data, API).
2. **API Response:** Viết một Wrapper class chuẩn cho mọi API/Ajax response với cấu trúc:
   `{ "isSuccess": bool, "message": string, "data": object }`
3. **Datetime:** Mọi xử lý thời gian lưu xuống Database phải ở định dạng UTC (`DateTime.UtcNow`). Chỉ chuyển sang Local time (GMT+7) khi hiển thị lên View.
4. **Giao diện:** Tái sử dụng các class của Bootstrap 5 cho các bảng (Table) và Modal (Thêm/Sửa). Thiết kế Responsive.

### BƯỚC 1: FOUNDATION & AUTHENTICATION
- Cấu hình file `appsettings.json` kết nối SQL Server và setup `AppDbContext` trong `Program.cs`.
- Tạo chức năng Đăng nhập. Sử dụng Cookie Authentication cho MVC.
- Phân quyền (Role-based Authorization) cho 3 roles: `Admin`, `Lecturer`, `Student`. Yêu cầu có custom Attribute hoặc Policy cho phép khóa các Controller của Admin chỉ cho phép role `Admin` truy cập.

### BƯỚC 2: MASTER DATA CRUD (Semester & Subject)
- Viết tính năng CRUD cho thực thể `Semester` (Học kỳ: Id, Name, StartDate, EndDate, IsCurrent). Giao diện gồm 1 bảng danh sách và 1 Modal Bootstrap để Thêm/Sửa. Có nút để "Set as Current Semester".
- Viết tính năng CRUD cho thực thể `Subject` (Môn học: Id, Code, Name, Credits). Giao diện thiết kế y hệt như `Semester` để đảm bảo tính nhất quán (Vibe coding). 
- Đảm bảo có validation ở backend và frontend cho các trường bắt buộc.

### BƯỚC 3: USER MANAGEMENT & EXCEL IMPORT
- Tạo giao diện Quản lý Tài khoản (Users). 
- Viết chức năng Import User từ file Excel (sử dụng thư viện EPPlus hoặc ClosedXML). 
- Logic Import: Đọc file Excel (các cột: Username, FullName, Email, Role, StudentCode/LecturerCode). Khi tạo user, tự động sinh ra URL cho ảnh đại diện mặc định (AvatarUrl) lưu vào DB. Dựa vào cột Role, tự động insert record tương ứng vào bảng `Students` hoặc `Lecturers`.

### BƯỚC 4: ACADEMIC SETUP
- Tạo View quản lý Lớp hành chính (`Class`). Có tính năng chọn Sinh viên (từ danh sách Students) để add vào Lớp.
- Tạo màn hình Phân công Lớp học phần (`ClassSubject`). Cần giao diện cho phép chọn: Học kỳ, Môn học, Lớp hành chính và Giảng viên phụ trách.
- Tạo tính năng nhập Thời khóa biểu (`Schedule`) cho từng Lớp học phần (Ví dụ: Thứ 2 - Ca 1, Thứ 4 - Ca 3).

Hãy bắt đầu bằng BƯỚC 1. Hãy cho tôi biết file nào cần tạo mới, file nào cần sửa và cung cấp code cho chúng.
```
