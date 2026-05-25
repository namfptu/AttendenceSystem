using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AttendanceSystem.Data.Entities;

namespace AttendanceSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // KHÔNG CÓ DbSet<BaseEntity> ở đây!
        // Đăng ký các thực thể (Entities) thực sự sẽ trở thành Bảng trong CSDL
        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Lecturer> Lecturers { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Semester> Semesters { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<ClassSubject> ClassSubjects { get; set; }
        public DbSet<ClassStudent> ClassStudents { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<AttendanceSession> AttendanceSessions { get; set; }
        public DbSet<AttendanceRecord> AttendanceRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ==========================================
            // 1. CẤU HÌNH UNIQUE CONSTRAINTS
            // ==========================================
            
            // Các mã định danh phải là duy nhất
            builder.Entity<User>().HasIndex(u => u.Username).IsUnique();
            builder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            builder.Entity<Student>().HasIndex(s => s.StudentCode).IsUnique();
            builder.Entity<Lecturer>().HasIndex(l => l.LecturerCode).IsUnique();
            builder.Entity<Subject>().HasIndex(s => s.SubjectCode).IsUnique();
            builder.Entity<Class>().HasIndex(c => c.ClassCode).IsUnique();

            // Chống trùng lặp quan hệ N-N
            // Một sinh viên không thể nằm 2 lần trong cùng 1 lớp hành chính
            builder.Entity<ClassStudent>()
                .HasIndex(cs => new { cs.ClassId, cs.StudentId })
                .IsUnique();

            // Một lớp không thể học cùng 1 môn 2 lần trong 1 kỳ
            builder.Entity<ClassSubject>()
                .HasIndex(cs => new { cs.ClassId, cs.SubjectId, cs.SemesterId })
                .IsUnique();

            // Một sinh viên không thể điểm danh 2 lần trong cùng 1 buổi học
            builder.Entity<AttendanceRecord>()
                .HasIndex(ar => new { ar.AttendanceSessionId, ar.StudentId })
                .IsUnique();

            // ==========================================
            // 2. CẤU HÌNH CASCADE DELETE (RESTRICT)
            // ==========================================
            // Tránh việc xóa 1 dòng làm bay luôn toàn bộ dữ liệu (Multiple cascade paths)
            
            builder.Entity<ClassSubject>()
                .HasOne(cs => cs.Lecturer)
                .WithMany(l => l.ClassSubjects)
                .HasForeignKey(cs => cs.LecturerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ClassStudent>()
                .HasOne(cs => cs.Student)
                .WithMany(s => s.ClassStudents)
                .HasForeignKey(cs => cs.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AttendanceRecord>()
                .HasOne(ar => ar.Student)
                .WithMany(s => s.AttendanceRecords)
                .HasForeignKey(ar => ar.StudentId)
                .OnDelete(DeleteBehavior.Restrict);
                
            builder.Entity<AttendanceRecord>()
                .HasOne(ar => ar.EditedByLecturer)
                .WithMany()
                .HasForeignKey(ar => ar.EditedByLecturerId)
                .OnDelete(DeleteBehavior.Restrict);
                
            builder.Entity<AttendanceSession>()
                .HasOne(s => s.CreatedByLecturer)
                .WithMany()
                .HasForeignKey(s => s.CreatedByLecturerId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        // ==========================================
        // 3. TỰ ĐỘNG HÓA BASE ENTITY
        // ==========================================
        public override int SaveChanges()
        {
            ApplyAuditInformation();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditInformation();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void ApplyAuditInformation()
        {
            // Tìm tất cả các Model kế thừa từ BaseEntity đang bị chỉnh sửa
            var entries = ChangeTracker.Entries<BaseEntity>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow; // Tự gán ngày tạo
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow; // Tự gán ngày sửa
                }
            }
        }
    }
}
