# Git Workflow Option 2 (Feature-Centric Workflow)

## Mục tiêu

Workflow này dành cho:
- Team nhỏ (2–5 người)
- ASP.NET / Web API / MVC project
- Muốn workflow gọn
- Không dùng local develop thường xuyên

Ý tưởng chính:

```text
Chỉ code trên feature branch
+
Sync trực tiếp từ origin/develop
```

---

# Các branch sử dụng

## Local

```text
feature/student-management
feature/attendance
feature/auth
...
```

## Remote (GitHub)

```text
origin/develop
origin/feature/student-management
origin/feature/attendance
...
```

---

# Tư duy workflow

```text
origin/develop
      ↓
local feature
      ↓ code
      ↓ commit
origin/feature
      ↓ PR
origin/develop
      ↺ repeat
```

---

# FLOW CHUẨN

# 1. Clone project

```bash
git clone https://github.com/username/AttendanceSystem.git
```

Đi vào project:

```bash
cd AttendanceSystem
```

---

# 2. Tạo feature branch mới từ origin/develop

## Luôn fetch trước

```bash
git fetch origin
```

## Tạo feature branch

Ví dụ:

```bash
git checkout -b feature/student-management origin/develop
```

Ý nghĩa:

```text
Tạo local feature branch
từ trạng thái mới nhất của origin/develop
```

---

# 3. Push feature branch lần đầu

```bash
git push -u origin feature/student-management
```

Sau lần đầu:
- local feature branch sẽ tracking remote feature branch
- lần sau chỉ cần git push

---

# 4. Mỗi lần bắt đầu code

## Đứng ở feature branch

```bash
git checkout feature/student-management
```

## Sync develop mới nhất NGAY ĐẦU BUỔI

```bash
git fetch origin
git merge origin/develop
```

Ý nghĩa:

```text
Merge code mới nhất của team
vào feature branch hiện tại
```

Làm bước này thường xuyên để:
- tránh conflict lớn
- lấy DTO/API mới
- lấy migration mới
- lấy Program.cs mới

---

# 5. Code feature

Ví dụ:
- Controller
- Service
- Repository
- DTO
- Razor View
- API Client

---

# 6. Commit code

## Check status

```bash
git status
```

## Add code

```bash
git add .
```

## Commit

```bash
git commit -m "Add student management feature"
```

---

# 7. Push feature branch

```bash
git push
```

Code giờ đã được:
- backup trên GitHub
- teammate nhìn thấy
- có thể tạo PR

---

# 8. Tạo Pull Request (PR)

```text
origin/feature/student-management
→
origin/develop
```

Mục đích:
- merge feature vào develop
- review code
- quản lý thay đổi

---

# 9. Sau khi PR merge xong

Feature đã được merge vào:

```text
origin/develop
```

---

# 10. Bắt đầu feature mới

Ví dụ:

```bash
git fetch origin

git checkout -b feature/attendance origin/develop

git push -u origin feature/attendance
```

Loop workflow tiếp tục.

---

# FLOW HOÀN CHỈNH

```text
START CODING
↓
git checkout feature/xxx

↓
git fetch origin
git merge origin/develop

↓
CODE

↓
git add .
git commit -m "..."

↓
git push

↓
Pull Request:
origin/feature/xxx
→
origin/develop

↓
repeat
```

---

# Ý nghĩa các branch

## origin/develop

```text
Branch trung tâm của team
```

Chứa:
- code mới nhất
- code đã merge
- nơi mọi người sync

---

## feature/*

```text
Workspace cá nhân
```

Dùng để:
- code riêng
- test riêng
- tránh phá code team

---

# Các lệnh quan trọng nhất

## Tạo feature mới

```bash
git checkout -b feature/xxx origin/develop
```

---

## Sync develop mới nhất

```bash
git fetch origin
git merge origin/develop
```

---

## Commit

```bash
git add .
git commit -m "..."
```

---

## Push

```bash
git push
```

---

# Ví dụ thực tế

## Anh làm:

```text
feature/student-management
```

## Người còn lại làm:

```text
feature/attendance
```

Cả 2:
- sync từ origin/develop
- code riêng
- push riêng
- PR về origin/develop

---

# Quy tắc recommend

## Luôn sync develop đầu buổi

```bash
git fetch origin
git merge origin/develop
```

---

## Không code trực tiếp trên main

---

## Mỗi feature → 1 branch riêng

---

## Commit nhỏ, rõ ràng

Ví dụ tốt:

```text
Add attendance API
Add pagination
Fix login validation
Refactor repository layer
```

Ví dụ xấu:

```text
update project
fix bug
final
```

---

# Ưu điểm của Option 2

- Workflow gọn
- Ít branch local
- Ít checkout qua lại
- Dễ backup
- Phù hợp team nhỏ
- Gần workflow công ty thật

---

# Nhược điểm

- Người mới dễ lú hơn Option 1
- Cần hiểu origin/develop là remote branch
- Cần cẩn thận khi merge conflict

---

# Kết luận

Workflow Option 2 thực chất là:

```text
Feature-centric workflow
```

Tức:
- mọi người làm việc trên feature branch
- sync trực tiếp từ remote develop
- PR về remote develop
