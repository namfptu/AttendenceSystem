using System.Collections.Generic;
using System.Threading.Tasks;
using AttendanceSystem.Business.DTOs;

namespace AttendanceSystem.Business.Services
{
    public interface IClassStudentService
    {
        Task<IEnumerable<ClassStudentDto>> GetStudentsByClassAsync(int classId);
        Task<ClassStudentDto> AddStudentToClassAsync(int classId, int studentId);
        Task<bool> RemoveStudentFromClassAsync(int id);
        Task<bool> ExistsAsync(int classId, int studentId);
        Task<ImportResultDto> ImportStudentsAsync(int classId, System.IO.Stream excelStream);
    }
}
