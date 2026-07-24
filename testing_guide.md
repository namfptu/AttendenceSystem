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
  - **Ràng buộc:** 
    1. Không được Add trùng 1 sinh viên vào cùng 1 lớp hành chính nhiều lần.
    2. **Ràng buộc Trùng lịch học sinh (Mới):** Khi Admin gán thủ công hoặc Import Excel sinh viên vào Lớp hành chính, hệ thống tự động kiểm tra chéo lịch học của lớp này với tất cả các lớp sinh viên đó đang học trong học kỳ. Nếu có ca trùng giờ học, hệ thống chặn lại và báo lỗi chi tiết để tránh gán trùng lịch học cho sinh viên.
- **Schedules (`/Schedules`) - Xếp thời khóa biểu:** 
  - **Nút "Save / Add Schedule":** Khi Admin click tạo hoặc cập nhật thời khóa biểu, hệ thống sẽ trigger các ràng buộc cực kỳ nghiêm ngặt:
    - **Ràng buộc Trùng lịch Giảng viên:** Cùng 1 Giảng viên KHÔNG THỂ bị xếp 2 lớp học có khung giờ đè lên nhau. Sẽ báo lỗi `BadRequest`.
    - **Ràng buộc Trùng lịch Phòng học:** Cùng 1 Phòng học KHÔNG THỂ được xếp cho 2 lớp khác nhau vào cùng 1 thời điểm. Sẽ báo lỗi `BadRequest`.
    - **Ràng buộc Trùng lịch Lớp (Class Overlap):** Cùng 1 Lớp hành chính KHÔNG THỂ bị xếp học 2 môn khác nhau vào cùng một thời điểm. Sẽ báo lỗi `BadRequest`.
    - **Ràng buộc Trùng lịch Học sinh & Tự động xóa (Mới):** Khi xếp lịch cho lớp học phần, nếu phát hiện có sinh viên trong lớp bị trùng lịch chéo với môn học khác ở lớp hành chính khác, hệ thống sẽ **tự động xóa (Auto-remove)** sinh viên đó khỏi lớp hiện tại để giải quyết xung đột lịch học và tiếp tục lưu lịch.
  - **Bộ lọc nâng cao (Mới):** Giao diện quản lý lịch học được bổ sung thanh lọc đa tiêu chí (Lọc chéo theo Lớp học phần, Lớp hành chính, Môn học, Giảng viên, Thứ) giúp tìm kiếm lịch học nhanh chóng.
- **Class Substitutes (`/ClassSubstitutes` - Gán giảng viên dạy thay) (Mới):**
  - **Ràng buộc khi gán:**
    1. Giảng viên được gán dạy thay không được trùng với Giảng viên chính của lớp học phần.
    2. Ngày dạy thay phải nằm trong khoảng thời gian diễn ra học kỳ.
    3. Không cho phép gán trùng giảng viên dạy thay khác cho cùng một lớp học phần trong cùng một ngày.
    4. Ngày được gán dạy thay bắt buộc phải có lịch học hoặc phiên học đã thiết lập của môn này.
    5. Giảng viên dạy thay không bị trùng lịch dạy chính thức của chính mình trong khung giờ đó.
    6. Giảng viên dạy thay không bị trùng lịch dạy thế khác mà họ đã nhận trong khung giờ đó.
- **Sessions (`/AttendanceSessions`):** 
  - Admin có quyền xem danh sách toàn bộ các phiên điểm danh và xem chi tiết (Take Attendance) của bất kỳ lớp nào. 
  - **Hiển thị Tabular Day-Card (Mới):** Danh sách ca học được gom nhóm theo từng ngày và hiển thị dưới dạng Bảng (Table) bên trong các Thẻ ngày (Day-Card). Mỗi hàng hiển thị rõ ràng Mã Lớp, Môn học, Thời gian, Phòng học, Trạng thái và đặc biệt là **Tên Giảng viên (Mới)** phụ trách ca học.
  - **Ca học ảo (Virtual Sessions - Mới):** Tự động hiển thị các ca học ảo trạng thái `Pending` (ID = 0) tương ứng với các Lịch học của ngày hôm nay nhưng chưa được giảng viên mở. Giúp Admin giám sát được toàn bộ ca dạy của ngày hôm nay.
  - **Ràng buộc Thời gian mở ca (Mới):** Quy tắc thời gian mở ca học (chỉ cho phép mở từ trước 30 phút giờ bắt đầu đến trước giờ kết thúc ca học) được áp dụng đồng bộ cho **cả Giảng viên và Admin** (Admin không còn đặc quyền bypass giới hạn thời gian mở ca).
  - **Đặc quyền bypass:** Khi Giảng viên đã đóng phiên (Closed), giảng viên bị khóa quyền sửa đổi. Chỉ có Admin mới có quyền truy cập vào màn hình chi tiết điểm danh để cập nhật lại trạng thái điểm danh của sinh viên (Ví dụ: sinh viên khiếu nại điểm danh sai).
  - **Bộ lọc nâng cao (Mới):** Giao diện danh sách ca học được tích hợp bộ lọc nhanh theo Lớp hành chính, Môn học, Trạng thái, Ngày học, và Giảng viên dạy để quản lý hiệu quả.

---

## 2. Role: LECTURER (Giảng viên)
*Trọng tâm của Giảng viên là tính năng điểm danh tại lớp (Real-time). Các nút bấm được thiết kế để chống gian lận và sai sót.*

### Nhóm Main
- **Dashboard (`/Dashboard`):** 
  - **Hiển thị:** Các khung giờ học hôm nay (Today's Classes) phải được phân tách rõ ràng. Mỗi khung giờ (Schedule) là 1 thẻ riêng biệt, dù dạy cùng 1 lớp.
  - **Nút "⚡ Open Session" (Mở phiên nhanh):** Khi Giảng viên click vào nút này, hệ thống chạy các thao tác ngầm:
    1. Mapping chính xác `ScheduleId` của thẻ học đó để ghi nhận phiên.
    2. **Ràng buộc Thời gian:** Chỉ cho phép mở phiên điểm danh **sớm nhất 30 phút** trước giờ bắt đầu (`StartTime`) và **chậm nhất** là trước giờ kết thúc (`EndTime`). Nếu bấm mở quá sớm hoặc quá muộn, hệ thống chặn và hiện lỗi.
    3. Tự động chuyển hướng (Redirect) Giảng viên thẳng vào màn hình `Take Attendance` (Điểm danh sinh viên).
    4. **Hỗ trợ ca Pending sẵn có (Mới):** Hiển thị nút bấm để mở nhanh các ca học đã có trong cơ sở dữ liệu ở trạng thái `Pending` thay vì chỉ hiện Badge chữ tĩnh.
  - **Dạy thay (Class Substitute - Mới):** 
    - Nếu giảng viên được Admin phân công dạy thế vào ngày hôm nay, lớp dạy thế phải xuất hiện chính xác trên Dashboard của họ. 
    - Giảng viên dạy thế có toàn quyền mở ca học và điểm danh như giảng viên chính thức trong ngày được phân công. Quyền này sẽ tự động hết hạn khi qua ngày.
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
