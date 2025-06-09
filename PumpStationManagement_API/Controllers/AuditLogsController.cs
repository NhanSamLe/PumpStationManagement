using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PumpStationManagement_API.DTOs;
using PumpStationManagement_API.Enums;
using PumpStationManagement_API.Models;
using PumpStationManagement_API.Services;
namespace PumpStationManagement_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditLogsController : Controller
    {

        private readonly ApplicationDBContext _context;
        private readonly AuditLogService _auditLogService;

        public AuditLogsController(ApplicationDBContext context, AuditLogService auditLogService)
        {
            _context = context;
            _auditLogService = auditLogService;
        }

        [HttpGet]
        public async Task<ActionResult<List<AuditLog>>> GetAuditLogs([FromQuery] string? entityType = null, [FromQuery] int? entityId = null, [FromQuery] string? actionType = null)
        {
            try
            {
                IQueryable<AuditLog> query = _context.AuditLogs
                    .Include(a => a.PerformedByNavigation)
                    .OrderByDescending(a => a.ActionDate);

                if (!string.IsNullOrEmpty(entityType))
                {
                    query = query.Where(a => a.EntityType.ToLower() == entityType.ToLower());
                }

                if (entityId.HasValue)
                {
                    query = query.Where(a => a.EntityId == entityId);
                }

                if (!string.IsNullOrEmpty(actionType))
                {
                    query = query.Where(a => a.ActionType.ToLower() == actionType.ToLower());
                }

                var auditLogs = await query.ToListAsync();
                return Ok(auditLogs);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi lấy danh sách lịch sử thay đổi", error = ex.Message });
            }
        }
    }
}
