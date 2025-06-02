using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PumpStationManagement_API.Enums;
using PumpStationManagement_API.Models;
using PumpStationManagement_API.Services;

namespace PumpStationManagement_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlertsController : ControllerBase
    {
        private readonly ApplicationDBContext context;

        public AlertsController(ApplicationDBContext context)
        {
            context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Alert>>> GetAlerts([FromQuery] string? keyword = null, [FromQuery] int? stationId = null)
        {
            try
            {
                IQueryable<Alert> query = context.Alerts
                    .Include(a => a.Pump)
                    .Include(a => a.ModifiedByNavigation)
                    .Where(a => !a.IsDelete);

                if (stationId.HasValue && stationId > 0)
                {
                    query = query.Where(a => a.Pump != null && a.Pump.StationId == stationId);
                }

                if (!string.IsNullOrEmpty(keyword))
                {
                    keyword = keyword.Trim().ToLower();
                    query = query.Where(a => (a.Pump != null && a.Pump.PumpName.ToLower().Contains(keyword)) ||
                                            a.AlertMessage.ToLower().Contains(keyword));
                }

                query = query.OrderByDescending(a => a.AlertId);

                var alerts = await query.ToListAsync();
                return Ok(alerts);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi lấy danh sách cảnh báo", error = ex.Message });
            }
        }

        // GET: api/Alerts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Alert>> GetAlert(int id)
        {
            try
            {
                var alert = await context.Alerts
                    .Include(a => a.Pump)
                    .Include(a => a.ModifiedByNavigation)
                    .FirstOrDefaultAsync(a => a.AlertId == id && !a.IsDelete);

                if (alert == null)
                {
                    return NotFound(new { message = "Không tìm thấy cảnh báo" });
                }

                return Ok(alert);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi lấy thông tin cảnh báo", error = ex.Message });
            }
        }

        // POST: api/Alerts
        [HttpPost]
        public async Task<ActionResult<Alert>> CreateAlert([FromBody] Alert alert)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Kiểm tra PumpId hợp lệ (nếu có)
                if (alert.PumpId.HasValue)
                {
                    var pumpExists = await context.Pumps
                        .AnyAsync(p => p.PumpId == alert.PumpId && !p.IsDelete);
                    if (!pumpExists)
                    {
                        return BadRequest(new { message = "Máy bơm không tồn tại hoặc đã bị xóa" });
                    }
                }

                // Kiểm tra CreatedBy hợp lệ (nếu có)
                if (alert.CreatedBy.HasValue)
                {
                    var userExists = await context.Users
                        .AnyAsync(u => u.UserId == alert.CreatedBy);
                    if (!userExists)
                    {
                        return BadRequest(new { message = "Người tạo không tồn tại" });
                    }
                }
                // Mặc định Status là Pending nếu không được cung cấp
                alert.Status = (int)AlertStatus.Active;
                alert.IsDelete = false;
                alert.CreatedOn = DateTime.Now;

                context.Alerts.Add(alert);
                await context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetAlert), new { id = alert.AlertId }, alert);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi tạo cảnh báo", error = ex.Message });
            }
        }

        // PUT: api/Alerts/5
        [HttpPut("{id}")]
        public async Task<ActionResult<Alert>> UpdateAlert(int id, [FromBody] Alert alert)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existingAlert = await context.Alerts
                    .FirstOrDefaultAsync(a => a.AlertId == id && !a.IsDelete);

                if (existingAlert == null)
                {
                    return NotFound(new { message = "Không tìm thấy cảnh báo" });
                }

                // Kiểm tra PumpId hợp lệ (nếu có)
                if (alert.PumpId.HasValue)
                {
                    var pumpExists = await context.Pumps
                        .AnyAsync(p => p.PumpId == alert.PumpId && !p.IsDelete);
                    if (!pumpExists)
                    {
                        return BadRequest(new { message = "Máy bơm không tồn tại hoặc đã bị xóa" });
                    }
                }

                // Kiểm tra ModifiedBy hợp lệ (nếu có)
                if (alert.ModifiedBy.HasValue)
                {
                    var userExists = await context.Users
                        .AnyAsync(u => u.UserId == alert.ModifiedBy);
                    if (!userExists)
                    {
                        return BadRequest(new { message = "Người chỉnh sửa không tồn tại" });
                    }
                }

                existingAlert.PumpId = alert.PumpId;
                existingAlert.AlertType = alert.AlertType;
                existingAlert.AlertMessage = alert.AlertMessage;
                existingAlert.Status = alert.Status;
                existingAlert.ModifiedBy = alert.ModifiedBy;
                existingAlert.ModifiedOn = DateTime.Now;

                await context.SaveChangesAsync();
                return Ok(existingAlert);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi cập nhật cảnh báo", error = ex.Message });
            }
        }

        // DELETE: api/Alerts/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAlert(int id, [FromQuery] int modifiedBy)
        {
            try
            {
                var alert = await context.Alerts
                    .FirstOrDefaultAsync(a => a.AlertId == id && !a.IsDelete);

                if (alert == null)
                {
                    return NotFound(new { message = "Không tìm thấy cảnh báo" });
                }

                alert.IsDelete = true;
                alert.ModifiedBy = modifiedBy;
                alert.ModifiedOn = DateTime.Now;

                await context.SaveChangesAsync();
                return Ok(new { message = "Xóa cảnh báo thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi xóa cảnh báo", error = ex.Message });
            }
        }

        // PATCH: api/Alerts/5/resolve
        [HttpPatch("{id}/resolve")]
        public async Task<ActionResult> ResolveAlert(int id, [FromQuery] int modifiedBy)
        {
            try
            {
                var alert = await context.Alerts
                    .FirstOrDefaultAsync(a => a.AlertId == id && !a.IsDelete);

                if (alert == null)
                {
                    return NotFound(new { message = "Không tìm thấy cảnh báo" });
                }

                if (alert.Status == (int)AlertStatus.Resolved) // 1 = Resolved
                {
                    return BadRequest(new { message = "Cảnh báo đã được xử lý" });
                }

                alert.Status = (int)AlertStatus.Resolved;
                alert.ModifiedBy = modifiedBy;
                alert.ModifiedOn = DateTime.Now;

                await context.SaveChangesAsync();
                return Ok(new { message = "Xử lý cảnh báo thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi xử lý cảnh báo", error = ex.Message });
            }
        }

        // PATCH: api/Alerts/5/ignore
        [HttpPatch("{id}/ignore")]
        public async Task<ActionResult> IgnoreAlert(int id, [FromQuery] int modifiedBy)
        {
            try
            {
                var alert = await context.Alerts
                    .FirstOrDefaultAsync(a => a.AlertId == id && !a.IsDelete);

                if (alert == null)
                {
                    return NotFound(new { message = "Không tìm thấy cảnh báo" });
                }

                if (alert.Status == (int)AlertStatus.Ignored) 
                {
                    return BadRequest(new { message = "Cảnh báo đã bị bỏ qua" });
                }

                alert.Status = (int)AlertStatus.Ignored; // 2 = Ignored
                alert.ModifiedBy = modifiedBy;
                alert.ModifiedOn = DateTime.Now;

                await context.SaveChangesAsync();
                return Ok(new { message = "Bỏ qua cảnh báo thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi bỏ qua cảnh báo", error = ex.Message });
            }
        }

    }
}
