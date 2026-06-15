# Quy trình xây dựng Code-First & Hướng dẫn Migration (Dành cho Team)

Tài liệu này chuẩn hóa quy trình làm việc với Database theo mô hình **Entity Framework Core Code-First** để đảm bảo code của Dev A và Dev B không bị conflict và luôn đồng bộ.

## 1. Tổng quan Quy trình Code-First
Trong mô hình Code-First, CSDL được sinh ra tự động từ code C#. Quy trình tiêu chuẩn gồm 3 bước:
1. **Viết Code:** Định nghĩa cấu trúc bảng bằng các class (Entities) trong thư mục `AttendanceSystem.Data/Entities` và đăng ký (DbSet) vào `AppDbContext`.
2. **Tạo Migration:** Chạy lệnh để EF Core ghi nhận sự thay đổi cấu trúc và sinh ra các file kịch bản C# (trong thư mục `Migrations`).
3. **Cập nhật Database:** Chạy lệnh để thực thi các file kịch bản đó thành mã SQL, tác động trực tiếp lên SQL Server / LocalDB.

---

## 2. Hướng dẫn dành cho người thay đổi DB (Dev A)
Bất cứ khi nào Dev A cần Thêm bảng, Thêm cột, Sửa kiểu dữ liệu, hoặc Đổi tên khóa ngoại:

**Bước 1:** Thay đổi code các file trong thư mục `Entities` và `AppDbContext.cs`.
**Bước 2:** Mở Terminal ở thư mục gốc (`h:\diemdanh\AttendanceSystem`) và tạo bản Migration mới:
```bash
dotnet ef migrations add <MoTaNganGon> --project AttendanceSystem.Data --startup-project AttendanceSystem.Web
```
*Lưu ý: `<MoTaNganGon>` viết liền không dấu, VD: `AddAvatarToUser`.*

**Bước 3:** Cập nhật thay đổi đó xuống LocalDB của máy Dev A:
```bash
dotnet ef database update --project AttendanceSystem.Data --startup-project AttendanceSystem.Web
```

**Bước 4:** Commit toàn bộ code (Bao gồm cả các file sinh ra trong thư mục `Migrations`) và Push lên Github/Gitlab.

---

## 3. Hướng dẫn dành cho người lấy code về (Dev B)
Khi Dev A thông báo đã thay đổi Database và push code lên nhánh chung, máy của Dev B sẽ bị lỗi do Database cục bộ của Dev B chưa có các cột mới.
Lúc này, **Dev B KHÔNG ĐƯỢC chạy lệnh `add migration`**. 

**Các bước xử lý cho Dev B:**
**Bước 1:** Kéo code mới nhất từ kho lưu trữ về máy (`git pull`).
**Bước 2:** Mở Terminal ở thư mục gốc (`h:\diemdanh\AttendanceSystem`).
**Bước 3:** Chạy **duy nhất lệnh này** để EF tự động đọc các file Migration mới nhất từ thư mục `Migrations` và áp dụng vào DB của Dev B:
```bash
dotnet ef database update --project AttendanceSystem.Data --startup-project AttendanceSystem.Web
```
*Hệ thống sẽ chạy một loạt mã SQL "Applying migration..." và Database của Dev B sẽ tự động được cập nhật giống y hệt máy Dev A.*

---

## ⚠️ Quy tắc Sống còn (Team Rules)
1. **Chỉ Một nguồn Chân lý (Single Source of Truth):** Mọi sự thay đổi về Cấu trúc (Schema) của DB bắt buộc phải code trong C# (Entities). **Tuyệt đối KHÔNG** mở SSMS để sửa cột, thêm bảng bằng tay.
2. **Luôn Pull trước khi Migrate:** Nếu anh định thêm tính năng và chạy lệnh `add migration`, hãy luôn đảm bảo mình đã `git pull` code mới nhất của người kia về để tránh bị rẽ nhánh Migration.
3. Lỗi gõ lệnh: Hãy gõ chính xác tham số `--project AttendanceSystem.Data` và `--startup-project AttendanceSystem.Web`. Nếu thiếu tham số sẽ bị văng lỗi.
