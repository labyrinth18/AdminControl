using AdminControl.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdminControl.DAL
{
    public interface IAdminActionLogRepository
    {
        Task<IEnumerable<AdminActionLogDto>> GetAllLogsAsync();
        Task<IEnumerable<AdminActionLogDto>> GetLogsByAdminIdAsync(int adminUserId);
        Task<IEnumerable<AdminActionLogDto>> GetLogsByTargetUserIdAsync(int targetUserId);
        Task<AdminActionLogDto> AddLogAsync(AdminActionLogCreateDto logDto);
    }
}
