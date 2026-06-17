using System.Collections.Generic;
using System.Threading.Tasks;
using AttendanceSystem.Business.DTOs;

namespace AttendanceSystem.Business.Services
{
    public interface ISubjectService
    {
        Task<IEnumerable<SubjectDto>> GetAllAsync();
        Task<SubjectDto?> GetByIdAsync(int id);
        Task<SubjectDto> CreateAsync(SubjectDto dto);
        Task<SubjectDto?> UpdateAsync(int id, SubjectDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsByCodeAsync(string subjectCode, int? excludeId = null);
    }
}
