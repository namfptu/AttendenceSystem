using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AttendanceSystem.Business.DTOs;
using AttendanceSystem.Data;
using AttendanceSystem.Data.Entities;
using AttendanceSystem.Data.Entities.Enums;

namespace AttendanceSystem.Business.Services
{
    public class AttendanceRecordService : IAttendanceRecordService
    {
        private readonly AppDbContext _context;

        public AttendanceRecordService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AttendanceRecordDto>> GetBySessionIdAsync(int sessionId)
        {
            var session = await _context.AttendanceSessions
                .Include(s => s.ClassSubject)
                .FirstOrDefaultAsync(s => s.Id == sessionId && !s.IsDeleted);
            if (session == null) return new List<AttendanceRecordDto>();

            var classStudents = await _context.ClassStudents
                .Include(cs => cs.Student).ThenInclude(s => s.User)
                .Where(cs => cs.ClassId == session.ClassSubject.ClassId && !cs.IsDeleted && cs.Status == ClassStudentStatus.Active)
                .ToListAsync();

            var existingRecords = await _context.AttendanceRecords
                .Where(ar => ar.AttendanceSessionId == sessionId && !ar.IsDeleted)
                .ToListAsync();

            var result = new List<AttendanceRecordDto>();
            foreach (var cs in classStudents)
            {
                var existing = existingRecords.FirstOrDefault(r => r.StudentId == cs.StudentId);
                result.Add(new AttendanceRecordDto
                {
                    Id = existing?.Id ?? 0,
                    AttendanceSessionId = sessionId,
                    StudentId = cs.StudentId,
                    Status = existing?.Status.ToString() ?? AttendanceStatus.Present.ToString(),
                    CheckInTime = existing?.CheckInTime,
                    IsManualEdited = existing?.IsManualEdited ?? false,
                    EditedByLecturerId = existing?.EditedByLecturerId,
                    EditedAt = existing?.EditedAt,
                    Note = existing?.Note,
                    StudentCode = cs.Student.StudentCode,
                    StudentName = cs.Student.User.FullName,
                    AvatarUrl = cs.Student.User.AvatarUrl
                });
            }
            return result.OrderBy(r => r.StudentCode).ToList();
        }

        public async Task<bool> SaveAttendanceAsync(TakeAttendanceDto dto, int lecturerId)
        {
            var session = await _context.AttendanceSessions
                .Include(s => s.ClassSubject)
                .FirstOrDefaultAsync(s => s.Id == dto.AttendanceSessionId && !s.IsDeleted);
            if (session == null) return false;

            if (session.Status == SessionStatus.Closed && lecturerId > 0)
                return false; // Lecturer cannot edit closed sessions

            if (lecturerId > 0 && session.ClassSubject.LecturerId != lecturerId)
            {
                // Check if they are a valid substitute
                var isSubstitute = await _context.ClassSubstitutes
                    .AnyAsync(cs => cs.ClassSubjectId == session.ClassSubjectId 
                                 && cs.LecturerId == lecturerId 
                                 && cs.SubstituteDate.Date == session.SessionDate.Date);
                if (!isSubstitute) return false;
            }

            var existingRecords = await _context.AttendanceRecords
                .Where(ar => ar.AttendanceSessionId == dto.AttendanceSessionId && !ar.IsDeleted)
                .ToListAsync();

            foreach (var recordDto in dto.Records)
            {
                Enum.TryParse<AttendanceStatus>(recordDto.Status, out var status);
                var existing = existingRecords.FirstOrDefault(r => r.StudentId == recordDto.StudentId);
                var checkInTime = status == AttendanceStatus.Present ? DateTime.UtcNow : (DateTime?)null;

                if (existing != null)
                {
                    existing.Status = status;
                    existing.CheckInTime = checkInTime;
                    existing.Note = recordDto.Note ?? string.Empty;
                }
                else
                {
                    _context.AttendanceRecords.Add(new AttendanceRecord
                    {
                        AttendanceSessionId = dto.AttendanceSessionId,
                        StudentId = recordDto.StudentId,
                        Status = status,
                        CheckInTime = checkInTime,
                        IsManualEdited = false,
                        Note = recordDto.Note ?? string.Empty
                    });
                }
            }
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateRecordAsync(int recordId, AttendanceRecordDto dto, int lecturerId)
        {
            var record = await _context.AttendanceRecords
                .Include(r => r.AttendanceSession).ThenInclude(s => s.ClassSubject)
                .FirstOrDefaultAsync(r => r.Id == recordId && !r.IsDeleted);
            if (record == null) return false;

            if (record.AttendanceSession.Status == SessionStatus.Closed && lecturerId > 0)
                return false; // Lecturer cannot edit closed sessions

            if (lecturerId > 0 && record.AttendanceSession.ClassSubject.LecturerId != lecturerId) return false;

            if (!Enum.TryParse<AttendanceStatus>(dto.Status, out var status)) return false;

            record.Status = status;
            if (status == AttendanceStatus.Absent)
            {
                record.CheckInTime = null;
            }
            record.Note = dto.Note ?? string.Empty;
            record.IsManualEdited = true;
            record.EditedByLecturerId = lecturerId;
            record.EditedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<AttendanceRecordDto>> GetStudentHistoryAsync(int studentId, int classSubjectId)
        {
            return await _context.AttendanceRecords
                .Include(ar => ar.AttendanceSession)
                .Include(ar => ar.Student).ThenInclude(s => s.User)
                .Where(ar => ar.StudentId == studentId && ar.AttendanceSession.ClassSubjectId == classSubjectId && !ar.IsDeleted && !ar.AttendanceSession.IsDeleted)
                .OrderBy(ar => ar.AttendanceSession.SessionDate)
                .Select(ar => new AttendanceRecordDto
                {
                    Id = ar.Id, AttendanceSessionId = ar.AttendanceSessionId, StudentId = ar.StudentId,
                    Status = ar.Status.ToString(), CheckInTime = ar.CheckInTime, IsManualEdited = ar.IsManualEdited,
                    EditedByLecturerId = ar.EditedByLecturerId, EditedAt = ar.EditedAt, Note = ar.Note,
                    StudentCode = ar.Student.StudentCode, StudentName = ar.Student.User.FullName, AvatarUrl = ar.Student.User.AvatarUrl
                }).ToListAsync();
        }
    }
}
