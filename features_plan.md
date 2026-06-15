# TÀI LIỆU PHÂN CHIA TÍNH NĂNG (FEATURE BREAKDOWN & TEAM PLAN)
Dự án: **Attendance System**
Quy mô team: **2 Lập trình viên (Dev A & Dev B)**

Tài liệu này chia nhỏ hệ thống thành 5 Module cốt lõi. Do team có 2 người, chiến lược tốt nhất là chia theo nhóm **Nghiệp vụ (Domain-based)** thay vì chia Frontend/Backend. Mỗi người sẽ làm Fullstack (cả API lẫn UI) cho module mình phụ trách để hiểu sâu nghiệp vụ và không bị block nhau.

---

## PHẦN 1: CHI TIẾT CÁC MODULE TÍNH NĂNG

### Module 1: Authentication & User Profile (Xác thực & Hồ sơ)
*Chịu trách nhiệm:* **Dev A**
1. **Tính năng Login:** Đăng nhập bằng `Username` / `Password`. Trả về JWT Token.
2. **Tính năng Phân quyền (RBAC):** Middleware kiểm tra Role (`Admin`, `Lecturer`, `Student`) cho từng API.
3. **Tính năng Đổi mật khẩu:** Yêu cầu User đổi mật khẩu lần đầu tiên đăng nhập.
4. **Quản lý Hồ sơ cá nhân (Profile):** Sinh viên/Giảng viên có thể xem thông tin cá nhân của mình.

### Module 2: Master Data Management (Quản trị Dữ liệu - Role Admin)
*Chịu trách nhiệm:* **Dev A**
1. **Quản lý Học kỳ (Semester):** CRUD Học kỳ, thiết lập Học kỳ hiện tại (Current Semester).
2. **Quản lý Môn học (Subject):** CRUD môn học, số tín chỉ.
3. **Quản lý Users (Tài khoản):** Tạo tài khoản Admin, tạo tài khoản Hàng loạt (Import Excel) cho Giảng viên và Sinh viên. Đồng thời sinh data map sang bảng `Students` và `Lecturers`.

### Module 3: Academic Setup (Tổ chức Đào tạo - Role Admin)
*Chịu trách nhiệm:* **Dev A**
1. **Quản lý Lớp hành chính (Class):** Khởi tạo lớp (VD: SE1801) và Add sinh viên vào lớp.
2. **Phân công Lớp học phần (ClassSubject):** Gán Môn học, Lớp hành chính, và Giảng viên phụ trách vào Học kỳ tương ứng.
3. **Tạo Thời khóa biểu (Schedule):** Thiết lập lịch học cố định trong tuần cho các Lớp học phần (VD: Thứ 2, 7h-9h).

### Module 4: Core Attendance Operations (Nghiệp vụ Điểm danh cốt lõi)
*Chịu trách nhiệm:* **Dev B** (Đây là module trái tim, phức tạp nhất)
1. **Quản lý Phiên điểm danh (Session) cho Giảng viên:** 
   - Lấy danh sách lịch học hôm nay (Today's Schedule). Có tính năng chọn "Học bù" nếu khác lịch.
   - Nút "Mở lớp" -> Tạo bản ghi `AttendanceSession` trạng thái `Open`.
   - Nút "Đóng lớp" -> Chuyển trạng thái `Closed`.
2. **Điểm danh thủ công (Manual Check-in):**
   - Màn hình hiển thị danh sách lớp, bắt buộc **có Ảnh Thẻ (Avatar)** của sinh viên bên cạnh tên để giảng viên đối chiếu mặt.
   - Giảng viên tick chọn vắng (Absent), có mặt (Present), đi muộn (Late).
   - API lưu trạng thái điểm danh cho toàn lớp.
3. **Sửa điểm danh / Ghi chú (Audit Trail):**
   - Giảng viên có thể vào điểm danh lại nếu sinh viên có lý do chính đáng.
   - API lưu vết `IsManualEdited = true`, `EditedByLecturerId` và `Note` để Ban đào tạo kiểm tra sau này.

### Module 5: Reports & Dashboards (Báo cáo Thống kê)
*Chịu trách nhiệm:* **Dev B**
1. **Dashboard Sinh viên:** 
   - Hiển thị danh sách các môn đang học.
   - Hiển thị thanh Tiến độ (Progress bar) % số buổi vắng mặt.
   - Cảnh báo màu Đỏ nếu số buổi vắng vượt mức 20% (Cấm thi - FE - Fatal Error).
2. **Dashboard Giảng viên:**
   - Báo cáo sĩ số các lớp mình dạy trong ngày.
   - Xem lịch sử điểm danh của một sinh viên cụ thể.
   - Chốt sổ (Export ra Excel) kết quả điểm danh cuối kỳ nộp cho Đào tạo.
3. **Dashboard Admin:**
   - Xem tổng quan toàn trường hôm nay có bao nhiêu lớp đang mở.

---

## PHẦN 2: KẾ HOẠCH TRIỂN KHAI (SPRINT PLAN)

Nên chia làm **3 Giai đoạn (Sprints)** để tránh ngợp:

### Sprint 1: Foundation (Tuần 1-2)
- **Mục tiêu:** Xây móng và nhập được dữ liệu khung.
- **Dev A:** Dựng khung project, kết nối DB bằng `AppDbContext` vừa tạo, làm màn hình Login, và toàn bộ CRUD của Module 2 (Môn học, Học kỳ, Users).
- **Dev B:** Chuẩn bị bộ UI/UX Components (Tailwind/Bootstrap), làm màn hình Profile, thiết kế DB Seed data (Dữ liệu giả để test).

### Sprint 2: Core Academic & Basic Attendance (Tuần 3-4)
- **Mục tiêu:** Phân công được lịch học, hỗ trợ học bù, dạy thay và Điểm danh bằng tay được.
- **Dev A:** Làm màn hình tạo Lớp, Phân công Giảng viên (Module 3). Bổ sung tính năng phân công Giảng viên dạy thay.
- **Dev B:** Làm tính năng Giảng viên mở lớp (Module 4), hiển thị danh sách SV kèm ảnh thẻ, cho phép Giảng viên tick chọn điểm danh thủ công.

### Sprint 3: Advanced Features & Polish (Tuần 5-6)
- **Mục tiêu:** Thống kê và Báo cáo.
- **Dev A:** Tập trung làm các màn hình Report/Thống kê cho Admin.
- **Dev B:** Làm Dashboard cho sinh viên và Giảng viên, tính năng export file Excel nộp cho trường. Test luồng sửa điểm danh (Audit log).

---
*Lưu ý cho team: Với 2 người, hãy thống nhất chuẩn API Response (VD: `{ statusCode: 200, message: "", data: {} }`) và format Ngày giờ (luôn dùng UTC ở Backend) ngay từ ngày đầu tiên để lúc ráp code không bị "đánh nhau".*
