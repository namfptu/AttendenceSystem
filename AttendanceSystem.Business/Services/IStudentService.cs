using System.Collections.Generic;
using System.Threading.Tasks;
using AttendanceSystem.Business.DTOs;

namespace AttendanceSystem.Business.Services
{
    public interface IStudentService
    {
        Task<IEnumerable<StudentDto>> GetAllAsync();
        Task<StudentDto?> GetByIdAsync(int id);
        Task<StudentDto> CreateAsync(StudentDto dto);
        Task<StudentDto?> UpdateAsync(int id, StudentDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsByEmailAsync(string email, int? excludeId = null);
        Task<bool> ExistsByCodeAsync(string studentCode, int? excludeId = null);
        Task<ImportResultDto> ImportStudentsAsync(System.IO.Stream excelStream);
    }
}
