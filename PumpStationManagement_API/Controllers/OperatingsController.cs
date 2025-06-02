using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PumpStationManagement_API.Models;
using PumpStationManagement_API.Services;

namespace PumpStationManagement_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperatingsController : ControllerBase
    {
        private readonly ApplicationDBContext context;
        public OperatingsController(ApplicationDBContext context)
        {
            context = context;
        }
        [HttpGet]
        public async Task<ActionResult<List<OperatingData>>> GetOperatingData([FromQuery] string? keyword = null, [FromQuery] int? stationId = null)
        {
            try
            {
                IQueryable<OperatingData> query = context.OperatingDatas
                    .Include(p => p.Pump)
                    .Where(p => !p.IsDelete);

                if (stationId.HasValue && stationId > 0)
                {
                    query = query.Where(p => p.Pump.StationId == stationId);
                }

                if (!string.IsNullOrEmpty(keyword))
                {
                    keyword = keyword.Trim().ToLower();
                    query = query.Where(p => p.Pump.PumpName.ToLower().Contains(keyword));
                }

                query = query.OrderByDescending(p => p.DataId);

                var operatingData = await query.ToListAsync();
                return Ok(operatingData);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi lấy danh sách dữ liệu vận hành", error = ex.Message });
            }
        }

        // GET: api/Operatings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OperatingData>> GetOperatingDatum(int id)
        {
            try
            {
                var operatingDatum = await context.OperatingDatas
                    .Where(p => p.DataId == id && !p.IsDelete)
                    .FirstOrDefaultAsync();

                if (operatingDatum == null)
                {
                    return NotFound(new { message = "Không tìm thấy dữ liệu vận hành" });
                }

                return Ok(operatingDatum);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi lấy thông tin dữ liệu vận hành", error = ex.Message });
            }
        }

        // POST: api/Operatings
        [HttpPost]
        public async Task<ActionResult<OperatingData>> CreateOperatingDatum([FromBody] OperatingData operatingData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Kiểm tra PumpId hợp lệ
                var pumpExists = await context.Pumps
                    .AnyAsync(p => p.PumpId == operatingData.PumpId && !p.IsDelete);
                if (!pumpExists)
                {
                    return BadRequest(new { message = "Máy bơm không tồn tại hoặc đã bị xóa" });
                }

                operatingData.IsDelete = false;
                operatingData.CreatedOn = DateTime.Now;

                context.OperatingDatas.Add(operatingData);
                await context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetOperatingDatum), new { id = operatingData.DataId }, operatingData);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi tạo dữ liệu vận hành", error = ex.Message });
            }
        }

        // PUT: api/Operatings/5
        [HttpPut("{id}")]
        public async Task<ActionResult<OperatingData>> UpdateOperatingDatum(int id, [FromBody] OperatingData operatingData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existingOperatingData = await context.OperatingDatas
                    .FirstOrDefaultAsync(p => p.DataId == id && !p.IsDelete);

                if (existingOperatingData == null)
                {
                    return NotFound(new { message = "Không tìm thấy dữ liệu vận hành" });
                }

                // Kiểm tra PumpId hợp lệ
                var pumpExists = await context.Pumps
                    .AnyAsync(p => p.PumpId == operatingData.PumpId && !p.IsDelete);
                if (!pumpExists)
                {
                    return BadRequest(new { message = "Máy bơm không tồn tại hoặc đã bị xóa" });
                }

                existingOperatingData.PumpId = operatingData.PumpId;
                existingOperatingData.FlowRate = operatingData.FlowRate;
                existingOperatingData.Pressure = operatingData.Pressure;
                existingOperatingData.ModifiedOn = DateTime.Now;

                await context.SaveChangesAsync();
                return Ok(existingOperatingData);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi cập nhật dữ liệu vận hành", error = ex.Message });
            }
        }

        // DELETE: api/Operatings/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteOperatingDatum(int id, [FromQuery] int modifiedBy)
        {
            try
            {
                var operatingDatum = await context.OperatingDatas
                    .FirstOrDefaultAsync(p => p.DataId == id && !p.IsDelete);
                if (operatingDatum == null)
                {
                    return NotFound(new { message = "Không tìm thấy dữ liệu vận hành" });
                }

                operatingDatum.IsDelete = true;
                operatingDatum.ModifiedOn = DateTime.Now;
                await context.SaveChangesAsync();
                return Ok(new { message = "Xóa dữ liệu vận hành thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi xóa dữ liệu vận hành", error = ex.Message });
            }
        }
    }
}
