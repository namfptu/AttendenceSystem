-- ==============================================================
-- MYSQL DATABASE CREATION SCRIPT FOR ATTENDANCE SYSTEM
-- ==============================================================

CREATE DATABASE IF NOT EXISTS attendance_system_db
CHARACTER SET utf8mb4
COLLATE utf8mb4_unicode_ci;

USE attendance_system_db;

-- 1. Users Table
CREATE TABLE IF NOT EXISTS Users (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Username VARCHAR(255) NOT NULL,
    Email VARCHAR(255) NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    FullName VARCHAR(255) NOT NULL,
    AvatarUrl TEXT NULL,
    Phone VARCHAR(50) NULL,
    Role INT NOT NULL, -- Enum: Admin = 0, Lecturer = 1, Student = 2
    IsActive TINYINT(1) NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NULL,
    IsDeleted TINYINT(1) NOT NULL DEFAULT 0,
    UNIQUE KEY UQ_Users_Username (Username),
    UNIQUE KEY UQ_Users_Email (Email)
) ENGINE=InnoDB;

-- 2. Students Table
CREATE TABLE IF NOT EXISTS Students (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    StudentCode VARCHAR(50) NOT NULL,
    Faculty VARCHAR(255) NOT NULL,
    Major VARCHAR(255) NOT NULL,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NULL,
    IsDeleted TINYINT(1) NOT NULL DEFAULT 0,
    UNIQUE KEY UQ_Students_StudentCode (StudentCode),
    UNIQUE KEY UQ_Students_UserId (UserId),
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- 3. Lecturers Table
CREATE TABLE IF NOT EXISTS Lecturers (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    LecturerCode VARCHAR(50) NOT NULL,
    Department VARCHAR(255) NOT NULL,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NULL,
    IsDeleted TINYINT(1) NOT NULL DEFAULT 0,
    UNIQUE KEY UQ_Lecturers_LecturerCode (LecturerCode),
    UNIQUE KEY UQ_Lecturers_UserId (UserId),
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- 4. Classes Table
CREATE TABLE IF NOT EXISTS Classes (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ClassCode VARCHAR(50) NOT NULL,
    ClassName VARCHAR(255) NOT NULL,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NULL,
    IsDeleted TINYINT(1) NOT NULL DEFAULT 0,
    UNIQUE KEY UQ_Classes_ClassCode (ClassCode)
) ENGINE=InnoDB;

-- 5. Semesters Table
CREATE TABLE IF NOT EXISTS Semesters (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NOT NULL,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NULL,
    IsDeleted TINYINT(1) NOT NULL DEFAULT 0
) ENGINE=InnoDB;

-- 6. Subjects Table
CREATE TABLE IF NOT EXISTS Subjects (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    SubjectCode VARCHAR(50) NOT NULL,
    SubjectName VARCHAR(255) NOT NULL,
    Credits INT NOT NULL,
    TotalSlots INT NOT NULL DEFAULT 20, -- Default FPT style (max 20 slots)
    Description TEXT NULL,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NULL,
    IsDeleted TINYINT(1) NOT NULL DEFAULT 0,
    UNIQUE KEY UQ_Subjects_SubjectCode (SubjectCode)
) ENGINE=InnoDB;

-- 7. ClassStudents Table (Many-to-Many Bridge)
CREATE TABLE IF NOT EXISTS ClassStudents (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ClassId INT NOT NULL,
    StudentId INT NOT NULL,
    EnrolledAt DATETIME NOT NULL,
    Status INT NOT NULL, -- Enum: Active = 0, Inactive = 1
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NULL,
    IsDeleted TINYINT(1) NOT NULL DEFAULT 0,
    UNIQUE KEY UQ_ClassStudents_Class_Student (ClassId, StudentId),
    FOREIGN KEY (ClassId) REFERENCES Classes(Id) ON DELETE CASCADE,
    FOREIGN KEY (StudentId) REFERENCES Students(Id) ON DELETE RESTRICT
) ENGINE=InnoDB;

-- 8. ClassSubjects Table (Course Section Bridge)
CREATE TABLE IF NOT EXISTS ClassSubjects (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ClassId INT NOT NULL,
    SubjectId INT NOT NULL,
    LecturerId INT NOT NULL,
    SemesterId INT NOT NULL,
    Status INT NOT NULL, -- Enum: Active = 0, Closed = 1, Cancelled = 2
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NULL,
    IsDeleted TINYINT(1) NOT NULL DEFAULT 0,
    UNIQUE KEY UQ_ClassSubjects_Class_Subject_Semester (ClassId, SubjectId, SemesterId),
    FOREIGN KEY (ClassId) REFERENCES Classes(Id) ON DELETE CASCADE,
    FOREIGN KEY (SubjectId) REFERENCES Subjects(Id) ON DELETE CASCADE,
    FOREIGN KEY (LecturerId) REFERENCES Lecturers(Id) ON DELETE RESTRICT,
    FOREIGN KEY (SemesterId) REFERENCES Semesters(Id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- 9. ClassSubstitutes Table (Substitution)
CREATE TABLE IF NOT EXISTS ClassSubstitutes (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ClassSubjectId INT NOT NULL,
    LecturerId INT NOT NULL,
    SubstituteDate DATETIME NOT NULL,
    Note TEXT NULL,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NULL,
    IsDeleted TINYINT(1) NOT NULL DEFAULT 0,
    FOREIGN KEY (ClassSubjectId) REFERENCES ClassSubjects(Id) ON DELETE CASCADE,
    FOREIGN KEY (LecturerId) REFERENCES Lecturers(Id) ON DELETE RESTRICT
) ENGINE=InnoDB;

-- 10. Schedules Table (Weekly Slots)
CREATE TABLE IF NOT EXISTS Schedules (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ClassSubjectId INT NOT NULL,
    DayOfWeek INT NOT NULL, -- Enum: Monday = 1, etc.
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,
    Room VARCHAR(100) NOT NULL,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NULL,
    IsDeleted TINYINT(1) NOT NULL DEFAULT 0,
    FOREIGN KEY (ClassSubjectId) REFERENCES ClassSubjects(Id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- 11. AttendanceSessions Table (Actual sessions held)
CREATE TABLE IF NOT EXISTS AttendanceSessions (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ClassSubjectId INT NOT NULL,
    ScheduleId INT NULL,
    SessionDate DATETIME NOT NULL,
    Title VARCHAR(255) NOT NULL,
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,
    Status INT NOT NULL, -- Enum: Pending = 0, Open = 1, Closed = 2
    LateAfterMinutes INT NOT NULL DEFAULT 0,
    OpenedAt DATETIME NULL,
    ClosedAt DATETIME NULL,
    CreatedByLecturerId INT NOT NULL,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NULL,
    IsDeleted TINYINT(1) NOT NULL DEFAULT 0,
    FOREIGN KEY (ClassSubjectId) REFERENCES ClassSubjects(Id) ON DELETE CASCADE,
    FOREIGN KEY (ScheduleId) REFERENCES Schedules(Id) ON DELETE SET NULL,
    FOREIGN KEY (CreatedByLecturerId) REFERENCES Lecturers(Id) ON DELETE RESTRICT
) ENGINE=InnoDB;

-- 12. AttendanceRecords Table (Student-level checkin)
CREATE TABLE IF NOT EXISTS AttendanceRecords (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    AttendanceSessionId INT NOT NULL,
    StudentId INT NOT NULL,
    Status INT NOT NULL, -- Enum: Present = 0, Absent = 1
    CheckInTime DATETIME NULL,
    IsManualEdited TINYINT(1) NOT NULL DEFAULT 0,
    EditedByLecturerId INT NULL,
    EditedAt DATETIME NULL,
    Note TEXT NULL,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NULL,
    IsDeleted TINYINT(1) NOT NULL DEFAULT 0,
    UNIQUE KEY UQ_AttendanceRecords_Session_Student (AttendanceSessionId, StudentId),
    FOREIGN KEY (AttendanceSessionId) REFERENCES AttendanceSessions(Id) ON DELETE CASCADE,
    FOREIGN KEY (StudentId) REFERENCES Students(Id) ON DELETE RESTRICT,
    FOREIGN KEY (EditedByLecturerId) REFERENCES Lecturers(Id) ON DELETE RESTRICT
) ENGINE=InnoDB;

-- ==============================================================
-- SEED DATA FOR TESTING (FPT UNIVERSITY ATTENDANCE CONTEXT)
-- ==============================================================

-- Seed Users
INSERT INTO Users (Id, Username, Email, PasswordHash, FullName, Role, IsActive, CreatedAt, IsDeleted) VALUES
(1, 'admin', 'admin@fpt.edu.vn', 'AQAAAAIAAYagAAAAEJrO...', 'System Administrator', 0, 1, NOW(), 0),
(2, 'thaya', 'anv@fpt.edu.vn', 'AQAAAAIAAYagAAAAEJrO...', 'Nguyên Văn A', 1, 1, NOW(), 0),
(3, 'svb', 'btvse1501@fpt.edu.vn', 'AQAAAAIAAYagAAAAEJrO...', 'Trần Văn B', 2, 1, NOW(), 0),
(4, 'svc', 'clvse1501@fpt.edu.vn', 'AQAAAAIAAYagAAAAEJrO...', 'Lê Văn C', 2, 1, NOW(), 0);

-- Seed Lecturers
INSERT INTO Lecturers (Id, UserId, LecturerCode, Department, CreatedAt, IsDeleted) VALUES
(1, 2, 'LECT001', 'Software Engineering', NOW(), 0);

-- Seed Students
INSERT INTO Students (Id, UserId, StudentCode, Faculty, Major, CreatedAt, IsDeleted) VALUES
(1, 3, 'SE150123', 'Information Technology', 'Software Engineering', NOW(), 0),
(2, 4, 'SE150124', 'Information Technology', 'Software Engineering', NOW(), 0);

-- Seed Classes
INSERT INTO Classes (Id, ClassCode, ClassName, CreatedAt, IsDeleted) VALUES
(1, 'SE1501', 'Software Engineering Class 1501', NOW(), 0);

-- Seed ClassStudents
INSERT INTO ClassStudents (ClassId, StudentId, EnrolledAt, Status, CreatedAt, IsDeleted) VALUES
(1, 1, NOW(), 0, NOW(), 0),
(1, 2, NOW(), 0, NOW(), 0);

-- Seed Semesters
INSERT INTO Semesters (Id, Name, StartDate, EndDate, CreatedAt, IsDeleted) VALUES
(1, 'Fall2026', '2026-09-01 00:00:00', '2026-12-31 23:59:59', NOW(), 0);

-- Seed Subjects
INSERT INTO Subjects (Id, SubjectCode, SubjectName, Credits, TotalSlots, Description, CreatedAt, IsDeleted) VALUES
(1, 'PRN232', 'C# programming', 3, 20, 'Advanced C# and .NET development', NOW(), 0);

-- Seed ClassSubjects (Lecturer Nguyễn Văn A teaches PRN232 to Class SE1501 in Fall2026)
INSERT INTO ClassSubjects (Id, ClassId, SubjectId, LecturerId, SemesterId, Status, CreatedAt, IsDeleted) VALUES
(1, 1, 1, 1, 1, 0, NOW(), 0);

-- Seed Schedules (Every Monday 07:30:00 - 09:30:00 at Room LA101)
INSERT INTO Schedules (Id, ClassSubjectId, DayOfWeek, StartTime, EndTime, Room, CreatedAt, IsDeleted) VALUES
(1, 1, 1, '07:30:00', '09:30:00', 'LA101', NOW(), 0);
