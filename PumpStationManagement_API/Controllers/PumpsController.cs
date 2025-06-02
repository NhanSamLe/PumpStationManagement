using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PumpStationManagement_API.Models;
using PumpStationManagement_API.Services;
using PumpStationManagement_API.Enums;

namespace PumpStationManagement_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PumpsController : ControllerBase
    {
        private ApplicationDBContext context;
        public PumpsController(ApplicationDBContext context)
        {
            this.context = context;
        }
        [HttpGet]
        public async Task<ActionResult<List<Pump>>> GetPumps([FromQuery] string? keyword = null, [FromQuery] int? stationId = null)
        {
            try
            {
                IQueryable<Pump> query = context.Pumps.Where(p => !p.IsDelete);

                if (stationId.HasValue && stationId > 0)
                {
                    query = query.Where(p => p.StationId == stationId);
                }

                if (!string.IsNullOrEmpty(keyword))
                {
                    keyword = keyword.Trim().ToLower();
                    query = query.Where(p => p.PumpName.ToLower().Contains(keyword) ||
                                            p.SerialNumber.ToLower().Contains(keyword));
                }

                query = query.OrderByDescending(p => p.PumpId);

                var pumps = await query.ToListAsync();
                return Ok(pumps);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi lấy danh sách máy bơm", error = ex.Message });
            }
        }

        // GET: api/Pumps/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Pump>> GetPump(int id)
        {
            try
            {
                var pump = await context.Pumps
                    .Where(p => p.PumpId == id && !p.IsDelete)
                    .FirstOrDefaultAsync();

                if (pump == null)
                {
                    return NotFound(new { message = "Không tìm thấy máy bơm" });
                }

                return Ok(pump);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi lấy thông tin máy bơm", error = ex.Message });
            }
        }

        // POST: api/Pumps
        [HttpPost]
        public async Task<ActionResult<Pump>> CreatePump([FromBody] Pump pump)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Kiểm tra StationId hợp lệ
                var stationExists = await context.PumpStations
                    .AnyAsync(s => s.StationId == pump.StationId && !s.IsDelete);
                if (!stationExists)
                {
                    return BadRequest(new { message = "Trạm bơm không tồn tại hoặc đã bị xóa" });
                }

                // Mặc định Status là Active nếu không được cung cấp
                pump.Status = (int)PumpStatus.Active;
                pump.IsDelete = false;
                pump.CreatedOn = DateTime.Now;

                context.Pumps.Add(pump);
                await context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetPump), new { id = pump.PumpId }, pump);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi tạo máy bơm", error = ex.Message });
            }
        }

        // PUT: api/Pumps/5
        [HttpPut("{id}")]
        public async Task<ActionResult<Pump>> UpdatePump(int id, [FromBody] Pump pump)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existingPump = await context.Pumps
                    .FirstOrDefaultAsync(p => p.PumpId == id && !p.IsDelete);

                if (existingPump == null)
                {
                    return NotFound(new { message = "Không tìm thấy máy bơm" });
                }

                // Kiểm tra StationId hợp lệ
                var stationExists = await context.PumpStations
                    .AnyAsync(s => s.StationId == pump.StationId && !s.IsDelete);
                if (!stationExists)
                {
                    return BadRequest(new { message = "Trạm bơm không tồn tại hoặc đã bị xóa" });
                }

                existingPump.PumpName = pump.PumpName;
                existingPump.SerialNumber = pump.SerialNumber;
                existingPump.StationId = pump.StationId;
                existingPump.Status = pump.Status;
                existingPump.ModifiedBy = pump.ModifiedBy;
                existingPump.ModifiedOn = DateTime.Now;

                await context.SaveChangesAsync();
                return Ok(existingPump);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi cập nhật máy bơm", error = ex.Message });
            }
        }

        // DELETE: api/Pumps/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePump(int id, [FromQuery] int modifiedBy)
        {
            try
            {
                var pump = await context.Pumps
                    .FirstOrDefaultAsync(p => p.PumpId == id && !p.IsDelete);

                if (pump == null)
                {
                    return NotFound(new { message = "Không tìm thấy máy bơm" });
                }

                pump.IsDelete = true;
                pump.ModifiedBy = modifiedBy;
                pump.ModifiedOn = DateTime.Now;

                await context.SaveChangesAsync();
                return Ok(new { message = "Xóa máy bơm thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi xóa máy bơm", error = ex.Message });
            }
        }
    }
}
