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
    public class StatisticsController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public StatisticsController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet("station-status")]
        public async Task<ActionResult<List<StationStatusDto>>> GetStationStatus([FromQuery] int? stationId = null)
        {
            try
            {
                var query = _context.OperatingDatas
                    .Include(p => p.Pump)
                    .ThenInclude(p => p.Station)
                    .Where(p => !p.IsDelete);

                if (stationId.HasValue && stationId > 0)
                {
                    query = query.Where(p => p.Pump.StationId == stationId);
                }

                var groupedData = await query
                    .GroupBy(p => new { p.Pump.StationId, p.Pump.Station.StationName, p.Status })
                    .Select(g => new
                    {
                        StationId = g.Key.StationId,
                        StationName = g.Key.StationName,
                        Status = g.Key.Status,
                        Count = g.Count()
                    })
                    .ToListAsync();

                var stationGroups = groupedData
                    .GroupBy(g => new { g.StationId, g.StationName })
                    .Select(g => new StationStatusDto
                    {
                        StationId = (int)g.Key.StationId,
                        StationName = g.Key.StationName,
                        StatusCounts = g.Select(s => new StatusCountDto
                        {
                            Status = s.Status,
                            StatusName = EnumHelper.GetDescription((OperatingStatus)s.Status),
                            Count = s.Count,
                            Percentage = 0 // Sẽ tính sau
                        }).ToList()
                    })
                    .ToList();

                // Tính phần trăm cho mỗi trạng thái
                foreach (var station in stationGroups)
                {
                    var total = station.StatusCounts.Sum(s => s.Count);
                    if (total > 0)
                    {
                        foreach (var status in station.StatusCounts)
                        {
                            status.Percentage = (double)status.Count / total * 100;
                        }
                    }
                }

                if (!stationGroups.Any())
                {
                    return Ok(new List<StationStatusDto>());
                }

                return Ok(stationGroups);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy dữ liệu thống kê", error = ex.Message });
            }
        }
    }
}