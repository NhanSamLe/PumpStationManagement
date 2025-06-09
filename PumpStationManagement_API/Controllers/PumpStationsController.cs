using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PumpStationManagement_API.Models;
using PumpStationManagement_API.Services;
using PumpStationManagement_API.Enums;
using PumpStationManagement_API.DTOs;
using ClosedXML.Excel;
using System.Text.Json;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace PumpStationManagement_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PumpStationsController : ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly AuditLogService _auditLogService;

        public PumpStationsController(ApplicationDBContext context, AuditLogService auditLogService)
        {
            this.context = context;
            this._auditLogService = auditLogService;
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
        public async Task<ActionResult<PumpStation>> CreatePumpStation([FromBody] PumpStationDTO stationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var station = new PumpStation
                {
                    StationName = stationDto.StationName,
                    Location = stationDto.Location,
                    Description = stationDto.Description,
                    Status = stationDto.Status != null ? stationDto.Status : (int)StationStatus.Active,
                    IsDelete = false,
                    CreatedOn = DateTime.Now,
                    CreatedBy = stationDto.CreatedBy ?? 0 // Giả sử 0 là hệ thống hoặc người dùng tự tạo
                };

                context.PumpStations.Add(station);
                await context.SaveChangesAsync();
                //var contentAfter = JsonSerializer.Serialize(station);
                await _auditLogService.LogActionAsync(station.StationId, "PumpStation", "Create", "","", stationDto.CreatedBy ?? 0, "Tạo mới trạm bơm");
                return CreatedAtAction(nameof(GetPumpStation), new { id = station.StationId }, station);
            }
            catch (Exception ex)
            {
                var innerException = ex.InnerException;
                while (innerException != null)
                {
                    Console.WriteLine(innerException.Message);
                    innerException = innerException.InnerException;
                }
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi tạo trạm bơm", error = ex.Message });
            }
        }

        // PUT: api/PumpStations/5
        [HttpPut("{id}")]
        public async Task<ActionResult<PumpStation>> UpdatePumpStation(int id, [FromBody] PumpStationDTO stationDto)
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
              //  var contentBefore = JsonSerializer.Serialize(existingStation);
                existingStation.StationName = stationDto.StationName;
                existingStation.Location = stationDto.Location;
                existingStation.Description = stationDto.Description;
                if (Enum.IsDefined(typeof(StationStatus), stationDto.Status))
                {
                    existingStation.Status = stationDto.Status;
                }
                existingStation.ModifiedBy = stationDto.ModifiedBy;
                existingStation.ModifiedOn = DateTime.Now;

                await context.SaveChangesAsync();
               // var contentAfter = JsonSerializer.Serialize(existingStation);
                await _auditLogService.LogActionAsync(id, "PumpStation", "Update", "", "", stationDto.ModifiedBy ?? 0, "Cập nhật trạm bơm");
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
                //var contentBefore = JsonSerializer.Serialize(station);
                station.IsDelete = true;
                station.ModifiedBy = modifiedBy;
                station.ModifiedOn = DateTime.Now;

                await context.SaveChangesAsync();
                await _auditLogService.LogActionAsync(id, "PumpStation", "Delete", "", "", modifiedBy, "Xóa trạm bơm");
                return Ok(new { message = "Xóa trạm bơm thành công" });
            }
            catch (Exception ex)
            {
                
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi xóa trạm bơm", error = ex.Message });
            }
        }
        [HttpGet("export-excel")]
        public async Task<IActionResult> ExportPumpStationsToExcel([FromQuery] string? keyword = null)
        {
            try
            {
                Console.WriteLine("Bắt đầu xuất Excel, keyword: {0}", keyword);

                // Kiểm tra DbContext
                if (context == null)
                {
                    Console.WriteLine("DbContext là null");
                    return StatusCode(500, "DbContext không được khởi tạo");
                }

                Console.WriteLine("Truy vấn PumpStations...");
                IQueryable<PumpStation> query = context.PumpStations.Where(p => !p.IsDelete);

                if (!string.IsNullOrEmpty(keyword))
                {
                    keyword = keyword.Trim().ToLower();
                    Console.WriteLine("Áp dụng bộ lọc với keyword: {0}", keyword);
                    query = query.Where(p => (p.StationName != null && p.StationName.ToLower().Contains(keyword)) ||
                                            (p.Location != null && p.Location.ToLower().Contains(keyword)));
                }

                query = query.OrderByDescending(p => p.StationId);
                Console.WriteLine("Thực hiện truy vấn...");
                var stations = await query.ToListAsync();
                Console.WriteLine("Lấy được {0} trạm bơm", stations.Count);

                Console.WriteLine("Tạo file Excel với ClosedXML...");
                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("PumpStations");

                // Tiêu đề
                worksheet.Cell(1, 1).Value = "Mã";
                worksheet.Cell(1, 2).Value = "Tên Trạm Bơm";
                worksheet.Cell(1, 3).Value = "Vị Trí";
                worksheet.Cell(1, 4).Value = "Mô Tả";
                worksheet.Cell(1, 5).Value = "Trạng Thái";
                worksheet.Cell(1, 6).Value = "Ngày Tạo";
                worksheet.Cell(1, 7).Value = "Ngày Chỉnh Sửa";

                // Dữ liệu
                Console.WriteLine("Ghi dữ liệu vào worksheet...");
                int row = 2;
                foreach (var item in stations)
                {
                    Console.WriteLine("Ghi trạm bơm ID: {0}", item.StationId);
                    worksheet.Cell(row, 1).Value = item.StationId;
                    worksheet.Cell(row, 2).Value = item.StationName ?? "N/A";
                    worksheet.Cell(row, 3).Value = item.Location ?? "N/A";
                    worksheet.Cell(row, 4).Value = item.Description ?? "N/A";
                    worksheet.Cell(row, 5).Value = EnumHelper.GetDescription((StationStatus)item.Status);
                    worksheet.Cell(row, 6).Value = item.CreatedOn?.ToString("dd/MM/yyyy") ?? "N/A";
                    worksheet.Cell(row, 7).Value = item.ModifiedOn?.ToString("dd/MM/yyyy") ?? "N/A";
                    row++;
                }

                Console.WriteLine("Lưu file Excel vào stream...");
                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;

                Console.WriteLine("Trả về file Excel");
                return File(stream.ToArray(),
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            "PumpStations.xlsx");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi xuất Excel: {ex.Message}\nStackTrace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return StatusCode(500, $"Lỗi khi tạo file Excel: {ex.Message}");
            }
        }


    }
}
