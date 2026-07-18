# HƯỚNG DẪN KIỂM THỬ (TESTING GUIDE) - ATTENDANCE SYSTEM

Tài liệu này hướng dẫn các bước kiểm thử (test) chi tiết dựa trên menu chức năng và mô tả rõ **từng ràng buộc (constraints)**, **logic khi bấm nút** cho 3 Role chính: **Admin**, **Lecturer** (Giảng viên), và **Student** (Sinh viên).

---

## 1. Role: ADMIN (Quản trị viên)
*Lưu ý: Admin có quyền truy cập vào việc tổ chức dữ liệu nền tảng và quản lý đào tạo. Cần kiểm tra kỹ các validation chống sai sót dữ liệu.*

### Nhóm Management (Dữ liệu nền tảng)
- **Lecturers (`/Lecturers`) & Students (`/Students`):**
  - **Nút "Create/Import":** Khi tạo mới hoặc Import từ Excel, hệ thống phải tự động tạo kèm một tài khoản User (để Login) ứng với mã Giảng viên/Sinh viên đó.
- **Classes (`/Classes`), Subjects (`/Subjects`), Semesters (`/Semesters`):**
  - **Thao tác:** Khởi tạo dữ liệu cơ bản. Kiểm tra validation bắt buộc nhập ở các text field.
  - **Ràng buộc Môn học (Subjects):** Khi tạo mới hoặc chỉnh sửa môn học, Admin có thể gán số lượng buổi học quy định (**Total Slots**, bắt buộc từ 1 đến 20). Khi chỉnh sửa số slot, hệ thống sẽ ngăn chặn nếu số lượng slot mới vượt quá giới hạn độ dài của các học kỳ đang phân công môn này.
- **Course Sections (`/ClassSubjects`):**
  - **Thao tác:** Map dữ liệu Lớp + Môn + Giảng viên + Học kỳ để tạo một lớp học phần cụ thể.
  - **Ràng buộc Phân công:** 
    1. Một Lớp không thể được phân công học cùng một Môn quá 1 lần trong cùng một Học kỳ. Hệ thống sẽ báo lỗi nếu cố tình tạo trùng.
    2. **Ràng buộc Độ dài Học kỳ:** Thời gian diễn ra học kỳ (tính theo số tuần) nhân với 4 (giới hạn tối đa 4 slot/tuần) phải lớn hơn hoặc bằng tổng số slot của môn học. Hệ thống sẽ báo lỗi nếu học kỳ quá ngắn để hoàn thành môn học đó (ví dụ: gán môn 20 slot vào học kỳ cực ngắn chỉ có 3 tuần).

### Nhóm Attendance (Tổ chức điểm danh)
- **Class Students (`/ClassStudents` - Danh sách sinh viên của lớp):**
  - **Ràng buộc:** Không được Add trùng 1 sinh viên vào cùng 1 lớp hành chính nhiều lần.
- **Schedules (`/Schedules`) - Xếp thời khóa biểu:** 
  - **Nút "Save / Add Schedule":** Khi Admin click tạo hoặc cập nhật thời khóa biểu, hệ thống sẽ trigger các ràng buộc cực kỳ nghiêm ngặt:
    - **Ràng buộc Trùng lịch Giảng viên:** Cùng 1 Giảng viên KHÔNG THỂ bị xếp 2 lớp học có khung giờ đè lên nhau. Sẽ báo lỗi `BadRequest`.
    - **Ràng buộc Trùng lịch Phòng học:** Cùng 1 Phòng học KHÔNG THỂ được xếp cho 2 lớp khác nhau vào cùng 1 thời điểm. Sẽ báo lỗi `BadRequest`.
    - **Ràng buộc Trùng lịch Lớp (Class Overlap):** Cùng 1 Lớp hành chính KHÔNG THỂ bị xếp học 2 môn khác nhau vào cùng một thời điểm. Sẽ báo lỗi `BadRequest`.
- **Sessions (`/AttendanceSessions`):** 
  - Admin có quyền xem danh sách toàn bộ các phiên điểm danh và xem chi tiết (Take Attendance) của bất kỳ lớp nào. 
  - **Đặc quyền bypass:** Khi Giảng viên đã đóng phiên (Closed), giảng viên bị khóa quyền sửa đổi. Chỉ có Admin mới có quyền truy cập vào màn hình chi tiết điểm danh để cập nhật lại trạng thái điểm danh của sinh viên (Ví dụ: sinh viên khiếu nại điểm danh sai).

---

## 2. Role: LECTURER (Giảng viên)
*Trọng tâm của Giảng viên là tính năng điểm danh tại lớp (Real-time). Các nút bấm được thiết kế để chống gian lận và sai sót.*

### Nhóm Main
- **Dashboard (`/Dashboard`):** 
  - **Hiển thị:** Các khung giờ học hôm nay (Today's Classes) phải được phân tách rõ ràng. Mỗi khung giờ (Schedule) là 1 thẻ riêng biệt, dù dạy cùng 1 lớp.
  - **Nút "⚡ Open Session" (Mở phiên nhanh):** Khi Giảng viên click vào nút này, hệ thống chạy 3 thao tác ngầm:
    1. Mapping chính xác `ScheduleId` của thẻ học đó để ghi nhận phiên.
    2. **Ràng buộc Thời gian:** Chỉ cho phép mở phiên điểm danh **sớm nhất 30 phút** trước giờ bắt đầu (`StartTime`) và **chậm nhất** là trước giờ kết thúc (`EndTime`). Nếu bấm mở quá sớm hoặc quá muộn, hệ thống chặn và hiện lỗi.
    3. Tự động chuyển hướng (Redirect) Giảng viên thẳng vào màn hình `Take Attendance` (Điểm danh sinh viên).
- **Tạo Phiên Điểm Danh Thủ Công (Học bù):**
  - **Ràng buộc:** 
    - `StartTime` phải nhỏ hơn `EndTime`.
    - Ngày diễn ra phiên (`SessionDate`) phải nằm trong khoảng thời gian diễn ra Học kỳ (`StartDate - EndDate`).
    - Khung giờ không được trùng lặp đè lên nhau của cùng một ClassSubject trong cùng một ngày.
    - Không được tạo phiên mới vượt quá số lượng buổi quy định (**Total Slots**) của môn học đó.

### Nhóm Attendance
- **Take Attendance (Điểm danh):** 
  - **Thao tác tick điểm danh:** Chỉ hỗ trợ 2 trạng thái: `Present` (P - Có mặt) hoặc `Absent` (A - Vắng). 
  - **Nút "Submit Attendance":** Khi bấm Submit, dữ liệu lưu tạm xuống Database nhưng phiên học vẫn đang `Open`.
- **Đóng phiên (Close Session):**
  - **Ràng buộc Auto-Absent:** Khi click "Close Session", tất cả những sinh viên **chưa được Giảng viên tick chọn trạng thái nào** sẽ mặc định bị hệ thống **tự động đánh vắng (Absent)**. Lúc này phiên chuyển sang trạng thái `Closed`.
- **Sửa điểm danh sau khi Closed:** 
  - **Quy tắc:** Giảng viên **bị khóa hoàn toàn quyền sửa đổi** sau khi đã bấm Close Session (nếu cố tình gửi request chỉnh sửa sẽ bị API chặn lại). Muốn thay đổi trạng thái điểm danh, Giảng viên phải báo cáo để Admin xử lý qua đặc quyền bypass.
- **Export Excel:**
  - **Nút "Export":** Khi click xuất file Excel, hệ thống xuất thông tin điểm danh của các phiên đã diễn ra để Giảng viên xem báo cáo tiến độ.

---

## 3. Role: STUDENT (Sinh viên)
*Sinh viên chỉ xem được tiến độ cá nhân, tuyệt đối không được ghi đè quyền.*

### Nhóm Main
- **Dashboard (`/Dashboard`):** 
  - **Tiến độ (Progress):** Trạng thái điểm danh (Present/Absent) do Giảng viên vừa thao tác hiển thị đồng bộ ngay lập tức bên màn hình Sinh viên.
  - **Cách tính phần trăm vắng mặt:** Được tính theo công thức: `(Số buổi vắng) / (Tổng số slot quy định - Total Slots)`.
  - **Text Cảnh báo (Red Flag):** Nếu tổng số buổi vắng (Absent) **vượt mức 20%** tổng số slot quy định, thanh tiến độ và chữ phải tự động chuyển sang **Màu đỏ (Banned)** cảnh báo Sinh viên bị cấm thi.

---

## 4. Ràng buộc Bảo mật chung (Auth & Route Guards)
1. **Sidebar Rendering:** Khi đăng nhập bằng Sinh viên/Giảng viên, hệ thống tuyệt đối KHÔNG ĐƯỢC gen ra mã HTML của menu Admin (như `/Students`, `/Classes`) ẩn trong DOM.
2. **URL Bypassing:** Kể cả khi Sinh viên cố tình click link lạ hoặc tự gõ URL nội bộ của Admin (ví dụ `https://domain/Classes`) lên thanh địa chỉ trình duyệt, hệ thống phải chặn lại và trả về lỗi **403 Forbidden** vì role Student không có Policy để truy cập.
