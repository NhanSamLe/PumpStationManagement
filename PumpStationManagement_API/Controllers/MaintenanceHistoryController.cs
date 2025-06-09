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
    public class MaintenanceHistoryController : ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly AuditLogService _auditLogService;

        public MaintenanceHistoryController(ApplicationDBContext context, AuditLogService auditLogService)
        {
            this.context = context;
            this._auditLogService = auditLogService;
        }
        // GET: api/MaintenanceHistory
        [HttpGet]
        public async Task<ActionResult<List<MaintenanceHistory>>> GetMaintenanceHistories([FromQuery] string? keyword = null, [FromQuery] int? stationId = null)
        {
            try
            {
                IQueryable<MaintenanceHistory> query = context.MaintenanceHistories
                    .Include(m => m.Pump)
                    .Include(m => m.PerformedByNavigation)
                    .Where(m => !m.IsDelete);

                if (stationId.HasValue && stationId > 0)
                {
                    query = query.Where(m => m.Pump != null && m.Pump.StationId == stationId);
                }

                if (!string.IsNullOrEmpty(keyword))
                {
                    keyword = keyword.Trim().ToLower();
                    query = query.Where(m => (m.Pump != null && m.Pump.PumpName.ToLower().Contains(keyword)) ||
                                            (m.Description != null && m.Description.ToLower().Contains(keyword)));
                }

                query = query.OrderByDescending(m => m.MaintenanceId);

                var maintenanceHistories = await query.ToListAsync();
                return Ok(maintenanceHistories);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi lấy danh sách lịch sử bảo trì", error = ex.Message });
            }
        }

        // GET: api/MaintenanceHistory/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaintenanceHistory>> GetMaintenanceHistory(int id)
        {
            try
            {
                var maintenanceHistory = await context.MaintenanceHistories
                    .Include(m => m.Pump)
                    .Include(m => m.PerformedByNavigation)
                    .FirstOrDefaultAsync(m => m.MaintenanceId == id && !m.IsDelete);

                if (maintenanceHistory == null)
                {
                    return NotFound(new { message = "Không tìm thấy lịch sử bảo trì" });
                }

                return Ok(maintenanceHistory);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi lấy thông tin lịch sử bảo trì", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<MaintenanceHistory>> CreateMaintenanceHistory([FromBody] MaintenanceHistoryDTO maintenanceHistoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Kiểm tra PumpId hợp lệ (nếu có)
                if (maintenanceHistoryDto.PumpId.HasValue)
                {
                    var pumpExists = await context.Pumps
                        .AnyAsync(p => p.PumpId == maintenanceHistoryDto.PumpId && !p.IsDelete);
                    if (!pumpExists)
                    {
                        return BadRequest(new { message = "Máy bơm không tồn tại hoặc đã bị xóa" });
                    }
                }

                // Kiểm tra PerformedBy hợp lệ (nếu có)
                if (maintenanceHistoryDto.PerformedBy.HasValue)
                {
                    var userExists = await context.Users
                        .AnyAsync(u => u.UserId == maintenanceHistoryDto.PerformedBy);
                    if (!userExists)
                    {
                        return BadRequest(new { message = "Người thực hiện không tồn tại" });
                    }
                }

                var maintenanceHistory = new MaintenanceHistory
                {
                    PumpId = maintenanceHistoryDto.PumpId,
                    MaintenanceType = maintenanceHistoryDto.MaintenanceType,
                    StartDate = maintenanceHistoryDto.StartDate,
                    EndDate = maintenanceHistoryDto.EndDate,
                    Description = maintenanceHistoryDto.Description,
                    Status = maintenanceHistoryDto.Status != default ? maintenanceHistoryDto.Status : 0, // 0 = Pending
                    PerformedBy = maintenanceHistoryDto.PerformedBy,
                    IsDelete = false,
                    CreatedOn = DateTime.Now
                };

                context.MaintenanceHistories.Add(maintenanceHistory);
                await context.SaveChangesAsync();
                await _auditLogService.LogActionAsync(maintenanceHistory.MaintenanceId, "Bảo Trì", "Tạo Mới", "","", maintenanceHistoryDto.PerformedBy ?? 0, "Tạo mới lịch sử bảo trì");
                return CreatedAtAction(nameof(GetMaintenanceHistory), new { id = maintenanceHistory.MaintenanceId }, maintenanceHistory);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi tạo lịch sử bảo trì", error = ex.Message });
            }
        }

        // PUT: api/MaintenanceHistory/5
        [HttpPut("{id}")]
        public async Task<ActionResult<MaintenanceHistory>> UpdateMaintenanceHistory(int id, [FromBody] MaintenanceHistoryDTO maintenanceHistoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existingMaintenanceHistory = await context.MaintenanceHistories
                    .FirstOrDefaultAsync(m => m.MaintenanceId == id && !m.IsDelete);

                if (existingMaintenanceHistory == null)
                {
                    return NotFound(new { message = "Không tìm thấy lịch sử bảo trì" });
                }

                // Kiểm tra PumpId hợp lệ (nếu có)
                if (maintenanceHistoryDto.PumpId.HasValue)
                {
                    var pumpExists = await context.Pumps
                        .AnyAsync(p => p.PumpId == maintenanceHistoryDto.PumpId && !p.IsDelete);
                    if (!pumpExists)
                    {
                        return BadRequest(new { message = "Máy bơm không tồn tại hoặc đã bị xóa" });
                    }
                }

                // Kiểm tra PerformedBy hợp lệ (nếu có)
                if (maintenanceHistoryDto.PerformedBy.HasValue)
                {
                    var userExists = await context.Users
                        .AnyAsync(u => u.UserId == maintenanceHistoryDto.PerformedBy);
                    if (!userExists)
                    {
                        return BadRequest(new { message = "Người thực hiện không tồn tại" });
                    }
                }

                existingMaintenanceHistory.PumpId = maintenanceHistoryDto.PumpId;
                existingMaintenanceHistory.MaintenanceType = maintenanceHistoryDto.MaintenanceType;
                existingMaintenanceHistory.StartDate = maintenanceHistoryDto.StartDate;
                existingMaintenanceHistory.EndDate = maintenanceHistoryDto.EndDate;
                existingMaintenanceHistory.Description = maintenanceHistoryDto.Description;
                existingMaintenanceHistory.Status = maintenanceHistoryDto.Status;
                existingMaintenanceHistory.PerformedBy = maintenanceHistoryDto.PerformedBy;

                await context.SaveChangesAsync();
                await _auditLogService.LogActionAsync(id, "Bảo Trì", "Cập Nhập", "", "", maintenanceHistoryDto.PerformedBy ?? 0, "Cập nhật lịch sử bảo trì");
                return Ok(existingMaintenanceHistory);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi cập nhật lịch sử bảo trì", error = ex.Message });
            }
        }

        // DELETE: api/MaintenanceHistory/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMaintenanceHistory(int id, [FromQuery] int modifiedBy)
        {
            try
            {
                var maintenanceHistory = await context.MaintenanceHistories
                    .FirstOrDefaultAsync(m => m.MaintenanceId == id && !m.IsDelete);

                if (maintenanceHistory == null)
                {
                    return NotFound(new { message = "Không tìm thấy lịch sử bảo trì" });
                }

                maintenanceHistory.IsDelete = true;
                maintenanceHistory.PerformedBy = modifiedBy;
                await context.SaveChangesAsync();
                await _auditLogService.LogActionAsync(id, "Bảo Trì", "Xóa", "", "", modifiedBy, "Xóa lịch sử bảo trì");
                return Ok(new { message = "Xóa lịch sử bảo trì thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi xóa lịch sử bảo trì", error = ex.Message });
            }
        }

        // PATCH: api/MaintenanceHistory/5/complete
        [HttpPatch("{id}/complete")]
        public async Task<ActionResult> CompleteMaintenanceHistory(int id, [FromQuery] int modifiedBy)
        {
            try
            {
                var maintenanceHistory = await context.MaintenanceHistories
                    .FirstOrDefaultAsync(m => m.MaintenanceId == id && !m.IsDelete);

                if (maintenanceHistory == null)
                {
                    return NotFound(new { message = "Không tìm thấy lịch sử bảo trì" });
                }

                if (maintenanceHistory.Status == (int)MaintainStatus.Completed) // 2 = Completed
                {
                    return BadRequest(new { message = "Lịch sử bảo trì đã hoàn thành" });
                }

                maintenanceHistory.Status = (int)MaintainStatus.Completed; // 2 = Completed
                maintenanceHistory.EndDate = DateTime.Now;
                maintenanceHistory.PerformedBy = modifiedBy;
                await context.SaveChangesAsync();
                await _auditLogService.LogActionAsync(id, "Bảo Trì", " Hoàn Thành", "", "", modifiedBy, "Hoàn Thành Bảo Trì");
                return Ok(new { message = "Cập nhật trạng thái bảo trì thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi cập nhật trạng thái bảo trì", error = ex.Message });
            }
        }
        [HttpPatch("{id}/active")]
        public async Task<ActionResult> ActiveMaintenanceHistory(int id, [FromQuery] int modifiedBy)
        {
            try
            {
                var maintenanceHistory = await context.MaintenanceHistories
                    .FirstOrDefaultAsync(m => m.MaintenanceId == id && !m.IsDelete);

                if (maintenanceHistory == null)
                {
                    return NotFound(new { message = "Không tìm thấy lịch sử bảo trì" });
                }

                if (maintenanceHistory.Status == (int)MaintainStatus.InProgress) // 2 = Completed
                {
                    return BadRequest(new { message = "Lịch sử bảo trì đang được xử lý rồi" });
                }

                maintenanceHistory.Status = (int)MaintainStatus.InProgress; // 2 = Completed
                maintenanceHistory.PerformedBy = modifiedBy;
                await context.SaveChangesAsync();
                await _auditLogService.LogActionAsync(id, "Bảo Trì", " Kích Hoạt", "", "", modifiedBy, "Bắt Đầu Tiến Hành Bảo Trì");
                return Ok(new { message = "Cập nhật trạng thái bảo trì thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi cập nhật trạng thái bảo trì", error = ex.Message });
            }
        }
    }
}
