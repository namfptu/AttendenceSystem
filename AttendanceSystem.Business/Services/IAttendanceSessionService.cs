using System.Collections.Generic;
using System.Threading.Tasks;
using AttendanceSystem.Business.DTOs;

namespace AttendanceSystem.Business.Services
{
    public interface IAttendanceSessionService
    {
        Task<IEnumerable<AttendanceSessionDto>> GetAllAsync();
        Task<IEnumerable<AttendanceSessionDto>> GetByLecturerIdAsync(int lecturerId);
        Task<IEnumerable<AttendanceSessionDto>> GetTodaySessionsByLecturerAsync(int lecturerId);
        Task<AttendanceSessionDto> GetByIdAsync(int id);
        Task<AttendanceSessionDto> CreateAsync(AttendanceSessionDto dto);
        Task<bool> OpenSessionAsync(int id);
        Task<bool> CloseSessionAsync(int id);
    }
}
