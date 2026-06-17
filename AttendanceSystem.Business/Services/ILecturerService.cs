using System.Collections.Generic;
using System.Threading.Tasks;
using AttendanceSystem.Business.DTOs;

namespace AttendanceSystem.Business.Services
{
    public interface ILecturerService
    {
        Task<IEnumerable<LecturerDto>> GetAllAsync();
        Task<LecturerDto?> GetByIdAsync(int id);
        Task<LecturerDto> CreateAsync(LecturerDto dto);
        Task<LecturerDto?> UpdateAsync(int id, LecturerDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsByCodeAsync(string code, int? excludeId = null);
        Task<bool> ExistsByEmailAsync(string email, int? excludeId = null);
    }
}
