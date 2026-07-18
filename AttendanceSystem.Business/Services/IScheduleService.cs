using System.Collections.Generic;
using System.Threading.Tasks;
using AttendanceSystem.Business.DTOs;

namespace AttendanceSystem.Business.Services
{
    public interface IScheduleService
    {
        Task<IEnumerable<ScheduleDto>> GetAllAsync();
        Task<IEnumerable<ScheduleDto>> GetByClassSubjectIdAsync(int classSubjectId);
        Task<ScheduleDto> GetByIdAsync(int id);
        Task<ScheduleDto> CreateAsync(ScheduleDto dto);
        Task<ScheduleDto> UpdateAsync(int id, ScheduleDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
