# 🎧 VIBE CODING TASK BẢNG PHÂN CÔNG (2 NHÂN SỰ)

**"Vibe Coding"** nghĩa là chúng ta không cặm cụi gõ từng dòng code CRUD nhàm chán nữa. Tinh thần ở đây là: **Dùng AI (Cursor, Copilot, ChatGPT) để generate 80-90% code**, 2 anh em chỉ đóng vai trò "Nhạc trưởng" (Architect & Reviewer) để ghép nối các module lại với nhau, tập trung vào prompt chuẩn và fix bug logic.

Vì là Vibe Coding, hệ thống được chia làm 2 "Vùng không gian" (Domains) để mỗi người tự do prompt AI tạo ra Fullstack (cả API + UI) cho mảng của mình mà không sợ conflict code (Git merge conflict).

---

## 👨‍💻 VIBE 1: DEV A - THE ADMIN & FOUNDATION (Người xây móng)
**Nhiệm vụ:** Lo toàn bộ phần Quản trị hệ thống (Admin Portal), thiết lập dữ liệu để Dev B có data làm điểm danh.

### 🎯 Check-list Prompt & Code:
- [ ] **Setup Foundation:** Prompt AI dựng khung project Web API và Web MVC/React/Vue (tùy stack). Cấu hình kết nối SQL Server với `AppDbContext` đã có sẵn.
- [ ] **Authentication:** Prompt AI làm chức năng Login (JWT) và Role-based Authorization (Admin, Lecturer, Student).
- [ ] **Master Data CRUD (Prompt 1 phát ăn ngay):** 
  - Quản lý `Semester` (Học kỳ)
  - Quản lý `Subject` (Môn học)
  - Quản lý `User` (Import danh sách tài khoản từ file Excel, nhớ prompt AI viết hàm đọc Excel sinh ra cột `AvatarUrl`).
- [ ] **Academic Management:**
  - Tạo trang Quản lý Lớp hành chính (`Class`) & thêm sinh viên vào lớp (`ClassStudent`).
  - Phân công Giảng dạy (`ClassSubject`) và tạo Thời khóa biểu (`Schedule`).
- [ ] **Tính năng dạy thay:** Giao diện cho Admin gán `ClassSubstitute` (Giảng viên dạy thay).

💡 *Mẹo Vibe cho Dev A:* Hãy tạo 1 component UI CRUD chuẩn (Table + Modal Thêm/Sửa), sau đó bảo AI: *"Dựa vào giao diện quản lý Semester này, hãy gen ra giao diện quản lý Subject và Class y hệt"*.

---

## 👨‍💻 VIBE 2: DEV B - THE CORE OPERATIONS (Người làm nghiệp vụ)
**Nhiệm vụ:** Làm luồng điểm danh cho Giảng viên và xem thống kê cho Sinh viên (Giao diện người dùng cuối).

### 🎯 Check-list Prompt & Code:
- [ ] **Lecturer Dashboard:** 
  - Prompt AI gọi API lấy danh sách Lịch học của Giảng viên (dựa theo user đang đăng nhập và `ClassSubstitute` nếu có đi dạy thay).
- [ ] **Luồng Điểm danh (Trái tim hệ thống):**
  - Màn hình điểm danh: Prompt AI vẽ 1 cái bảng danh sách sinh viên, **đặc biệt yêu cầu AI render kèm `AvatarUrl`** kế bên tên.
  - Các nút trạng thái (Radio buttons): Present (Có mặt) / Absent (Vắng) / Late (Muộn).
  - Nút "Submit": Gửi toàn bộ data xuống API để insert vào `AttendanceRecord`.
  - Luồng Sửa điểm danh: Lưu vết `IsManualEdited` và nhập `Note` (lý do sửa).
- [ ] **Student Dashboard:**
  - Prompt AI vẽ màn hình để sinh viên Login vào xem mình đang học môn gì, số buổi vắng là bao nhiêu %.
  - Báo động đỏ (Red Flag): Yêu cầu AI đổi màu dòng thành Đỏ nếu số buổi vắng vượt quá 20% (Cấm thi).
- [ ] **Export Báo cáo:** Prompt AI viết chức năng xuất bảng điểm danh môn học ra file Excel nộp cho phòng Đào tạo.

💡 *Mẹo Vibe cho Dev B:* Chỗ UI Điểm danh rất quan trọng trải nghiệm. Hãy prompt AI: *"Make the attendance table mobile-responsive, use big toggle buttons for Present/Absent so lecturers can easily tap on their tablets"*.

---

## 🤝 QUY TẮC "VIBE CHUNG" (Rules of Engagement)
Để 2 người gen code mà ráp vào nhau chạy được luôn, cần chốt nhanh 3 thứ này trước khi bắt đầu:
1. **UI Framework:** Chốt dùng TailwindCSS, Bootstrap hay Ant Design? (Để prompt AI cho giống nhau, giao diện không bị lệch pha).
2. **API Response Format:** Chốt một format chung cho tất cả API trả về. Ví dụ:
   ```json
   {
     "isSuccess": true,
     "message": "Thao tác thành công",
     "data": { ... }
   }
   ```
   *(Bảo AI viết 1 cái Wrapper để bọc tất cả Response lại).*
3. **Date/Time:** Tuyệt đối dặn AI luôn lưu thời gian ở dạng `UTC` xuống Database, và khi hiển thị lên UI mới convert sang giờ Việt Nam (GMT+7) để tránh lỗi lệch giờ điểm danh.
