using System.Collections.Generic;
using System.Threading.Tasks;
using AttendanceSystem.Business.DTOs;

namespace AttendanceSystem.Business.Services
{
    public interface IClassService
    {
        Task<IEnumerable<ClassDto>> GetAllAsync();
        Task<ClassDto?> GetByIdAsync(int id);
        Task<ClassDto> CreateAsync(ClassDto dto);
        Task<ClassDto?> UpdateAsync(int id, ClassDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsByCodeAsync(string classCode, int? excludeId = null);
    }
}
