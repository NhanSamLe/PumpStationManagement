using ClosedXML.Excel;
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
    public class OperatingsController : ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly AuditLogService _auditLogService;

        public OperatingsController(ApplicationDBContext context, AuditLogService auditLogService)
        {
            this.context = context;
            this._auditLogService = auditLogService;
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
                    .Include(p => p.Pump)
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
        public async Task<ActionResult<OperatingData>> CreateOperatingDatum([FromBody] OperatingDataDTO operatingDataDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Kiểm tra PumpId hợp lệ
                var pumpExists = await context.Pumps
                    .AnyAsync(p => p.PumpId == operatingDataDto.PumpId && !p.IsDelete);
                if (!pumpExists)
                {
                    return BadRequest(new { message = "Máy bơm không tồn tại hoặc đã bị xóa" });
                }

                var operatingData = new OperatingData
                {
                    PumpId = operatingDataDto.PumpId,
                    RecordTime = operatingDataDto.RecordTime,
                    FlowRate = operatingDataDto.FlowRate,
                    Pressure = operatingDataDto.Pressure,
                    PowerConsumption = operatingDataDto.PowerConsumption,
                    Temperature = operatingDataDto.Temperature,
                    RunningHours = operatingDataDto.RunningHours,
                    Efficiency = operatingDataDto.Efficiency,
                    Status = operatingDataDto.Status,
                    IsDelete = false,
                    CreatedOn = DateTime.Now,
                    CreatedBy =  operatingDataDto.CreatedBy
                };

                context.OperatingDatas.Add(operatingData);
                await context.SaveChangesAsync();
                await _auditLogService.LogActionAsync(operatingData.DataId, "Vận Hành", "Tạo Mới", "", "", operatingDataDto.CreatedBy??1, "Tạo mới dữ liệu vận hành");
                return CreatedAtAction(nameof(GetOperatingDatum), new { id = operatingData.DataId }, operatingData);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi tạo dữ liệu vận hành", error = ex.Message });
            }
        }

        // PUT: api/Operatings/5
        [HttpPut("{id}")]
        public async Task<ActionResult<OperatingData>> UpdateOperatingDatum(int id, [FromBody] OperatingDataDTO operatingDataDto)
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
                    .AnyAsync(p => p.PumpId == operatingDataDto.PumpId && !p.IsDelete);
                if (!pumpExists)
                {
                    return BadRequest(new { message = "Máy bơm không tồn tại hoặc đã bị xóa" });
                }

                existingOperatingData.PumpId = operatingDataDto.PumpId;
                existingOperatingData.RecordTime = operatingDataDto.RecordTime;
                existingOperatingData.FlowRate = operatingDataDto.FlowRate;
                existingOperatingData.Pressure = operatingDataDto.Pressure;
                existingOperatingData.PowerConsumption = operatingDataDto.PowerConsumption;
                existingOperatingData.Temperature = operatingDataDto.Temperature;
                existingOperatingData.RunningHours = operatingDataDto.RunningHours;
                existingOperatingData.Efficiency = operatingDataDto.Efficiency;
                existingOperatingData.Status = operatingDataDto.Status;
                existingOperatingData.ModifiedOn = DateTime.Now;
                existingOperatingData.ModifiedBy = operatingDataDto.ModifiedBy ?? 1;
                await context.SaveChangesAsync();
                await _auditLogService.LogActionAsync(id, "Vận Hành", "Cập Nhập","", "", operatingDataDto.ModifiedBy??1, "Cập nhật dữ liệu vận hành");
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
                await _auditLogService.LogActionAsync(id, "Vận Hành", "Xóa", "", "",modifiedBy, "Xóa dữ liệu vận hành");
                return Ok(new { message = "Xóa dữ liệu vận hành thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi xóa dữ liệu vận hành", error = ex.Message });
            }
        }

        [HttpGet("export-excel")]
        public async Task<IActionResult> ExportOperatingDataToExcel([FromQuery] string? keyword = null, [FromQuery] int? stationId = null)
        {
            try
            {
                Console.WriteLine("Bắt đầu xuất Excel dữ liệu vận hành, keyword: {0}, stationId: {1}", keyword, stationId);

                // Kiểm tra DbContext
                if (context == null)
                {
                    Console.WriteLine("DbContext là null");
                    return StatusCode(StatusCodes.Status500InternalServerError, "DbContext không được khởi tạo");
                }

                Console.WriteLine("Truy vấn OperatingDatas...");
                IQueryable<OperatingData> query = context.OperatingDatas
                    .Include(p => p.Pump)
                    .ThenInclude(p => p.Station)
                    .Where(p => !p.IsDelete);

                if (stationId.HasValue && stationId > 0)
                {
                    Console.WriteLine("Áp dụng bộ lọc stationId: {0}", stationId);
                    query = query.Where(p => p.Pump.StationId == stationId);
                }

                if (!string.IsNullOrEmpty(keyword))
                {
                    keyword = keyword.Trim().ToLower();
                    Console.WriteLine("Áp dụng bộ lọc keyword: {0}", keyword);
                    query = query.Where(p => p.Pump.PumpName != null && p.Pump.PumpName.ToLower().Contains(keyword));
                }

                query = query.OrderByDescending(p => p.DataId);
               
                var operatingData = await query.ToListAsync();
            

               
                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("OperatingData");

                // Tiêu đề
                worksheet.Cell(1, 1).Value = "Mã Dữ Liệu";
                worksheet.Cell(1, 2).Value = "Tên Máy Bơm";
                worksheet.Cell(1, 3).Value = "Tên Trạm Bơm";
                worksheet.Cell(1, 4).Value = "Thời Gian Ghi Nhận";
                worksheet.Cell(1, 5).Value = "Lưu Lượng (m³/h)";
                worksheet.Cell(1, 6).Value = "Áp Suất (bar)";
                worksheet.Cell(1, 7).Value = "Công Suất Tiêu Thụ (kW)";
                worksheet.Cell(1, 8).Value = "Nhiệt Độ (°C)";
                worksheet.Cell(1, 9).Value = "Giờ Chạy (h)";
                worksheet.Cell(1, 10).Value = "Hiệu Suất (%)";
                worksheet.Cell(1, 11).Value = "Trạng Thái";
                worksheet.Cell(1, 12).Value = "Ngày Tạo";
                worksheet.Cell(1, 13).Value = "Ngày Chỉnh Sửa";

                // Dữ liệu
                
                int row = 2;
                foreach (var item in operatingData)
                {
                   
                    worksheet.Cell(row, 1).Value = item.DataId;
                    worksheet.Cell(row, 2).Value = item.Pump?.PumpName ?? "N/A";
                    worksheet.Cell(row, 3).Value = item.Pump?.Station?.StationName ?? "N/A";
                    worksheet.Cell(row, 4).Value = item.RecordTime.ToString("dd/MM/yyyy HH:mm:ss");
                    worksheet.Cell(row, 5).Value = item.FlowRate?.ToString("F2") ?? "N/A";
                    worksheet.Cell(row, 6).Value = item.Pressure?.ToString("F2") ?? "N/A";
                    worksheet.Cell(row, 7).Value = item.PowerConsumption?.ToString("F2") ?? "N/A";
                    worksheet.Cell(row, 8).Value = item.Temperature?.ToString("F2") ?? "N/A";
                    worksheet.Cell(row, 9).Value = item.RunningHours?.ToString("F2") ?? "N/A";
                    worksheet.Cell(row, 10).Value = item.Efficiency?.ToString("F2") ?? "N/A";
                    worksheet.Cell(row, 11).Value = Enum.IsDefined(typeof(OperatingStatus), item.Status)
                        ? EnumHelper.GetDescription((OperatingStatus)item.Status)
                        : "Trạng thái không xác định";
                    worksheet.Cell(row, 12).Value = item.CreatedOn?.ToString("dd/MM/yyyy HH:mm:ss") ?? "N/A";
                    worksheet.Cell(row, 13).Value = item.ModifiedOn?.ToString("dd/MM/yyyy HH:mm:ss") ?? "N/A";
                    row++;
                }

                
                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;

                
                return File(stream.ToArray(),
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            "OperatingData.xlsx");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi xuất Excel: {ex.Message}\nStackTrace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return StatusCode(StatusCodes.Status500InternalServerError, $"Lỗi khi tạo file Excel: {ex.Message}");
            }
        }
    }
}
