using System.Collections.Generic;
using System.Threading.Tasks;
using AttendanceSystem.Business.DTOs;

namespace AttendanceSystem.Business.Services
{
    public interface ISemesterService
    {
        Task<IEnumerable<SemesterDto>> GetAllAsync();
        Task<SemesterDto?> GetByIdAsync(int id);
        Task<SemesterDto> CreateAsync(SemesterDto dto);
        Task<SemesterDto?> UpdateAsync(int id, SemesterDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
    }
}
