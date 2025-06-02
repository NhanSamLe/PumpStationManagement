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
    public class PumpStationsController : ControllerBase
    {
        private ApplicationDBContext context;
        public PumpStationsController(ApplicationDBContext context)
        {
            this.context = context;
        }
        // GET: api/PumpStations
        [HttpGet]
        public async Task<ActionResult<List<PumpStation>>> GetPumpStations([FromQuery] string? keyword = null)
        {
            try
            {
                IQueryable<PumpStation> query = context.PumpStations.Where(p => !p.IsDelete);

                if (!string.IsNullOrEmpty(keyword))
                {
                    keyword = keyword.Trim().ToLower();
                    query = query.Where(p => p.StationName.ToLower().Contains(keyword) ||
                                            p.Location.ToLower().Contains(keyword));
                }

                query = query.OrderByDescending(p => p.StationId);

                var stations = await query.ToListAsync();
                return Ok(stations);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi lấy danh sách trạm bơm", error = ex.Message });
            }
        }

        // GET: api/PumpStations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PumpStation>> GetPumpStation(int id)
        {
            try
            {
                var station = await context.PumpStations
                    .Where(p => p.StationId == id && !p.IsDelete)
                    .FirstOrDefaultAsync();

                if (station == null)
                {
                    return NotFound(new { message = "Không tìm thấy trạm bơm" });
                }

                return Ok(station);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi lấy thông tin trạm bơm", error = ex.Message });
            }
        }

        // POST: api/PumpStations
        [HttpPost]
        public async Task<ActionResult<PumpStation>> CreatePumpStation([FromBody] PumpStation station)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                station.Status = station.Status != null ? station.Status : (int)StationStatus.Active;
                station.IsDelete = false;
                station.CreatedOn = DateTime.Now;

                context.PumpStations.Add(station);
                await context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetPumpStation), new { id = station.StationId }, station);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi tạo trạm bơm", error = ex.Message });
            }
        }

        // PUT: api/PumpStations/5
        [HttpPut("{id}")]
        public async Task<ActionResult<PumpStation>> UpdatePumpStation(int id, [FromBody] PumpStation station)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existingStation = await context.PumpStations
                    .FirstOrDefaultAsync(p => p.StationId == id && !p.IsDelete);

                if (existingStation == null)
                {
                    return NotFound(new { message = "Không tìm thấy trạm bơm" });
                }

                existingStation.StationName = station.StationName;
                existingStation.Location = station.Location;
                if (Enum.IsDefined(typeof(StationStatus), station.Status))
                {
                    existingStation.Status = station.Status;
                }
                existingStation.ModifiedBy = station.ModifiedBy;
                existingStation.ModifiedOn = DateTime.Now;

                await context.SaveChangesAsync();
                return Ok(existingStation);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi cập nhật trạm bơm", error = ex.Message });
            }
        }

        // DELETE: api/PumpStations/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePumpStation(int id, [FromQuery] int modifiedBy)
        {
            try
            {
                var station = await context.PumpStations
                    .FirstOrDefaultAsync(p => p.StationId == id && !p.IsDelete);

                if (station == null)
                {
                    return NotFound(new { message = "Không tìm thấy trạm bơm" });
                }

                station.IsDelete = true;
                station.ModifiedBy = modifiedBy;
                station.ModifiedOn = DateTime.Now;

                await context.SaveChangesAsync();
                return Ok(new { message = "Xóa trạm bơm thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi xóa trạm bơm", error = ex.Message });
            }
        }
    }
}
