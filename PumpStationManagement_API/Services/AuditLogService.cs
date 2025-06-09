using PumpStationManagement_API.Models;

namespace PumpStationManagement_API.Services
{
    public class AuditLogService
    {
        private readonly ApplicationDBContext _context;

        public AuditLogService(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task LogActionAsync(int entityId, string entityType, string actionType, string contentBefore, string contentAfter, int performedBy, string description = null)
        {
            var auditLog = new AuditLog
            {
                EntityId = entityId,
                EntityType = entityType,
                ActionType = actionType,
                ContentBefore = contentBefore,
                ContentAfter = contentAfter,
                ActionDate = DateTime.Now,
                PerformedBy = performedBy,
                Description = description
            };

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        }
    }
}
