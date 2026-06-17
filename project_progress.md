# Tiến độ Dự án FPT Attendance System

Tài liệu này theo dõi tiến trình phát triển tổng thể của Dự án Điểm danh, được chia làm 2 Phase chính do **Dev A** và **Dev B** đảm nhiệm.

---

## 🟢 PHASE 1: Dev A - Nền tảng & Dữ liệu lõi (ĐÃ HOÀN THÀNH 100%)

Nhiệm vụ của Dev A là xây dựng toàn bộ phần móng kiến trúc, xử lý bảo mật và quản lý toàn bộ các danh mục gốc (Master Data). Hệ thống không thể vận hành nếu thiếu dữ liệu đầu vào chuẩn xác từ Phase này.

**Thành quả của Dev A:**
- Hệ thống đã có thể thêm/sửa/xóa và Import hàng nghìn sinh viên/giảng viên chỉ bằng 1 cú click.
- Xếp lớp (Enrollment) và ghép Môn học (Course Sections) hoạt động mượt mà.
- Kiến trúc API siêu gọn nhẹ, dễ dàng scale.

- `[x]` **1. Thiết lập Kiến trúc (Architecture)**
  - Chia 4 tầng: `Data` (EF Core), `Business` (DTOs, Services), `API` (Endpoints), `Web` (MVC).
  - Tích hợp giao diện Dashboard cực kỳ hiện đại từ template Vite (Inapp).
- `[x]` **2. Bảo mật & Phân quyền (Auth)**
  - Cơ chế Cookie Authentication.
  - Đăng nhập trực tiếp bằng **Email**.
  - Phân quyền (Role-based): Admin, Lecturer, Student (Tự động ẩn hiện menu tương ứng).
- `[x]` **3. Quản lý Danh mục (Master Data CRUD)**
  - Giảng viên (`Lecturers`).
  - Sinh viên (`Students`).
  - Lớp học hành chính (`Classes`).
  - Môn học (`Subjects`).
  - Học kỳ (`Semesters`).
- `[x]` **4. Quản lý Mối quan hệ (Relationship/Enrollment)**
  - Gán Sinh viên vào Lớp học (`ClassStudents`).
  - Tạo Lớp học phần (Ghép Lớp + Môn + Giảng viên + Học kỳ) (`ClassSubjects`).
- `[x]` **5. Tính năng Import Hàng loạt (Bulk Actions)**
  - Tích hợp `ClosedXML`.
  - Import Master Data Sinh viên từ Excel (Tự động tạo Account User, bắt lỗi trùng lặp/rỗng).
  - Import danh sách Sinh viên vào Lớp học cụ thể từ Excel.

---

## 🟡 PHASE 2: Dev B - Nghiệp vụ Điểm danh (ĐANG THỰC HIỆN)

Sau khi Dev A đã nạp đầy đủ Data, Dev B sẽ vào cuộc để giải quyết **Core Business** (Nghiệp vụ lõi) của phần mềm: Lên lịch dạy và Điểm danh.

**Trọng tâm của Dev B:**
Trải nghiệm thao tác của Giảng viên phải là ưu tiên số 1. Màn hình điểm danh phải thân thiện, phản hồi cực nhanh trên cả PC lẫn màn hình cảm ứng (Tablet/Điện thoại) khi thầy cô cầm lên bục giảng.

- `[ ]` **1. Quản lý Lịch dạy (Schedules)**
  - Giao diện cho Admin tạo lịch học cố định cho các Lớp học phần (VD: Thứ 2 & Thứ 5, Slot 1, Phòng A101).
- `[ ]` **2. Hệ thống Phiên Điểm danh (Attendance Sessions)**
  - Tool/Background job để tự động sinh ra các `Attendance Session` dựa trên `Schedule` đã lập.
  - Giao diện quản lý các Phiên điểm danh theo ngày cho Admin & Giảng viên.
- `[ ]` **3. Giao diện Take Attendance (Take Attendance UI)**
  - Màn hình dành riêng cho Giảng viên. Hiển thị danh sách Sinh viên trong lớp của ca học đó.
  - Nút bấm trực quan: `Present (Có mặt)`, `Absent (Vắng)`, `Late (Muộn)`, `Excused (Có phép)`.
  - Logic tự động tính toán số buổi vắng và đưa ra cảnh báo (Warning).
- `[ ]` **4. Báo cáo & Thống kê (Reports & Analytics)**
  - **Giảng viên**: Xem tổng quan % vắng mặt của toàn lớp mình dạy. Ai vắng >20% sẽ bị Highlight cấm thi đỏ chót.
  - **Sinh viên**: Có màn hình riêng để theo dõi bản thân (Tự xem được mình đã cúp học bao nhiêu buổi, còn lại bao nhiêu "quota").
  - **Admin**: Thống kê mức độ chuyên cần toàn trường.
- `[ ]` **5. Export Data**
  - Giảng viên/Admin có thể bấm Export xuất file Excel báo cáo điểm danh gửi cho Phòng Đào tạo cuối kỳ.
