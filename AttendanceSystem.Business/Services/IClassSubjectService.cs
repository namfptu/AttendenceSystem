using System.Collections.Generic;
using System.Threading.Tasks;
using AttendanceSystem.Business.DTOs;

namespace AttendanceSystem.Business.Services
{
    public interface IClassSubjectService
    {
        Task<IEnumerable<ClassSubjectDto>> GetAllAsync(int? semesterId = null);
        Task<ClassSubjectDto?> GetByIdAsync(int id);
        Task<ClassSubjectDto> CreateAsync(ClassSubjectDto dto);
        Task<ClassSubjectDto?> UpdateAsync(int id, ClassSubjectDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int classId, int subjectId, int semesterId, int? excludeId = null);
    }
}
