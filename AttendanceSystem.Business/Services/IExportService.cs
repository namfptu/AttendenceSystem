using System.Threading.Tasks;

namespace AttendanceSystem.Business.Services
{
    public interface IExportService
    {
        Task<byte[]> ExportAttendanceByClassSubjectAsync(int classSubjectId);
    }
}
