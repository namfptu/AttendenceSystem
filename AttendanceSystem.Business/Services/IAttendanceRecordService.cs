using System.Collections.Generic;
using System.Threading.Tasks;
using AttendanceSystem.Business.DTOs;

namespace AttendanceSystem.Business.Services
{
    public interface IAttendanceRecordService
    {
        Task<IEnumerable<AttendanceRecordDto>> GetBySessionIdAsync(int sessionId);
        Task<bool> SaveAttendanceAsync(TakeAttendanceDto dto, int lecturerId);
        Task<bool> UpdateRecordAsync(int recordId, AttendanceRecordDto dto, int lecturerId);
        Task<IEnumerable<AttendanceRecordDto>> GetStudentHistoryAsync(int studentId, int classSubjectId);
    }
}
