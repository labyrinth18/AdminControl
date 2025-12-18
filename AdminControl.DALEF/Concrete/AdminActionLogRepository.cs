using AdminControl.DAL;
using AdminControl.DALEF.Models;
using AdminControl.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminControl.DALEF.Concrete
{
    public class AdminActionLogRepository : IAdminActionLogRepository
    {
        private readonly AdminControlContext _context;

        public AdminActionLogRepository(AdminControlContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AdminActionLogDto>> GetAllLogsAsync()
        {
            return await _context.AdminActionLogs
                .Include(l => l.AdminUser)
                .Include(l => l.TargetUser)
                .Include(l => l.ActionType)
                .OrderByDescending(l => l.ActionDate)
                .Select(l => new AdminActionLogDto
                {
                    LogID = l.LogID,
                    AdminUserID = l.AdminUserID,
                    AdminUserName = l.AdminUser != null 
                        ? $"{l.AdminUser.FirstName} {l.AdminUser.LastName}" 
                        : "Невідомо",
                    TargetUserID = l.TargetUserID,
                    TargetUserName = l.TargetUser != null 
                        ? $"{l.TargetUser.FirstName} {l.TargetUser.LastName}" 
                        : null,
                    ActionTypeID = l.ActionTypeID,
                    ActionName = l.ActionType != null ? l.ActionType.ActionName : string.Empty,
                    ActionDescription = l.ActionType != null 
                        ? l.ActionType.ActionDescription ?? string.Empty 
                        : string.Empty,
                    Reason = l.Reason,
                    CreatedAt = l.ActionDate
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<AdminActionLogDto>> GetLogsByAdminIdAsync(int adminUserId)
        {
            return await _context.AdminActionLogs
                .Include(l => l.AdminUser)
                .Include(l => l.TargetUser)
                .Include(l => l.ActionType)
                .Where(l => l.AdminUserID == adminUserId)
                .OrderByDescending(l => l.ActionDate)
                .Select(l => new AdminActionLogDto
                {
                    LogID = l.LogID,
                    AdminUserID = l.AdminUserID,
                    AdminUserName = l.AdminUser != null 
                        ? $"{l.AdminUser.FirstName} {l.AdminUser.LastName}" 
                        : "Невідомо",
                    TargetUserID = l.TargetUserID,
                    TargetUserName = l.TargetUser != null 
                        ? $"{l.TargetUser.FirstName} {l.TargetUser.LastName}" 
                        : null,
                    ActionTypeID = l.ActionTypeID,
                    ActionName = l.ActionType != null ? l.ActionType.ActionName : string.Empty,
                    ActionDescription = l.ActionType != null 
                        ? l.ActionType.ActionDescription ?? string.Empty 
                        : string.Empty,
                    Reason = l.Reason,
                    CreatedAt = l.ActionDate
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<AdminActionLogDto>> GetLogsByTargetUserIdAsync(int targetUserId)
        {
            return await _context.AdminActionLogs
                .Include(l => l.AdminUser)
                .Include(l => l.TargetUser)
                .Include(l => l.ActionType)
                .Where(l => l.TargetUserID == targetUserId)
                .OrderByDescending(l => l.ActionDate)
                .Select(l => new AdminActionLogDto
                {
                    LogID = l.LogID,
                    AdminUserID = l.AdminUserID,
                    AdminUserName = l.AdminUser != null 
                        ? $"{l.AdminUser.FirstName} {l.AdminUser.LastName}" 
                        : "Невідомо",
                    TargetUserID = l.TargetUserID,
                    TargetUserName = l.TargetUser != null 
                        ? $"{l.TargetUser.FirstName} {l.TargetUser.LastName}" 
                        : null,
                    ActionTypeID = l.ActionTypeID,
                    ActionName = l.ActionType != null ? l.ActionType.ActionName : string.Empty,
                    ActionDescription = l.ActionType != null 
                        ? l.ActionType.ActionDescription ?? string.Empty 
                        : string.Empty,
                    Reason = l.Reason,
                    CreatedAt = l.ActionDate
                })
                .ToListAsync();
        }

        public async Task<AdminActionLogDto> AddLogAsync(AdminActionLogCreateDto logDto)
        {
            var log = new AdminActionLog
            {
                AdminUserID = logDto.AdminUserID,
                TargetUserID = logDto.TargetUserID,
                ActionTypeID = logDto.ActionTypeID,
                Reason = logDto.Reason,
                ActionDate = DateTime.UtcNow
            };

            _context.AdminActionLogs.Add(log);
            await _context.SaveChangesAsync();

            await _context.Entry(log).Reference(l => l.AdminUser).LoadAsync();
            await _context.Entry(log).Reference(l => l.TargetUser).LoadAsync();
            await _context.Entry(log).Reference(l => l.ActionType).LoadAsync();

            return new AdminActionLogDto
            {
                LogID = log.LogID,
                AdminUserID = log.AdminUserID,
                AdminUserName = log.AdminUser != null 
                    ? $"{log.AdminUser.FirstName} {log.AdminUser.LastName}" 
                    : "Невідомо",
                TargetUserID = log.TargetUserID,
                TargetUserName = log.TargetUser != null 
                    ? $"{log.TargetUser.FirstName} {log.TargetUser.LastName}" 
                    : null,
                ActionTypeID = log.ActionTypeID,
                ActionName = log.ActionType?.ActionName ?? string.Empty,
                ActionDescription = log.ActionType?.ActionDescription ?? string.Empty,
                Reason = log.Reason,
                CreatedAt = log.ActionDate
            };
        }
    }
}
