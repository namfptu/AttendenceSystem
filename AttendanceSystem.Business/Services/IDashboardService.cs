using System.Threading.Tasks;
using AttendanceSystem.Business.DTOs;

namespace AttendanceSystem.Business.Services
{
    public interface IDashboardService
    {
        Task<StudentDashboardDto> GetStudentDashboardAsync(int studentId);
        Task<LecturerDashboardDto> GetLecturerDashboardAsync(int lecturerId);
        Task<AdminDashboardDto> GetAdminDashboardAsync();
    }
}
