using System.Collections.Generic;
using System.Threading.Tasks;
using AttendanceSystem.Business.DTOs;

namespace AttendanceSystem.Business.Services
{
    public interface IClassSubstituteService
    {
        Task<IEnumerable<ClassSubstituteDto>> GetAllAsync(int? classSubjectId = null);
        Task<ClassSubstituteDto?> GetByIdAsync(int id);
        Task<ClassSubstituteDto> CreateAsync(ClassSubstituteDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
