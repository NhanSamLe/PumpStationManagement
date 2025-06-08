using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PumpStationManagement_API.Models;
using PumpStationManagement_API.Services;
using PumpStationManagement_API.Enums;
using PumpStationManagement_API.DTOs;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;

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

        [HttpPost]
        public async Task<ActionResult<Pump>> CreatePump([FromBody] PumpDTO pumpDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Kiểm tra StationId hợp lệ
                var stationExists = await context.PumpStations
                    .AnyAsync(s => s.StationId == pumpDto.StationId && !s.IsDelete);
                if (!stationExists)
                {
                    return BadRequest(new { message = "Trạm bơm không tồn tại hoặc đã bị xóa" });
                }

                var pump = new Pump
                {
                    StationId = pumpDto.StationId,
                    PumpName = pumpDto.PumpName,
                    PumpType = pumpDto.PumpType,
                    Capacity = pumpDto.Capacity,
                    Status = pumpDto.Status != null ? pumpDto.Status : (int)PumpStatus.Active,
                    Manufacturer = pumpDto.Manufacturer,
                    SerialNumber = pumpDto.SerialNumber,
                    WarrantyExpireDate = pumpDto.WarrantyExpireDate,
                    Description = pumpDto.Description,
                    IsDelete = false,
                    CreatedOn = DateTime.Now,
                    CreatedBy = pumpDto.ModifiedBy ?? 0 // Giả sử 0 là hệ thống hoặc người dùng tự tạo
                };

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
        public async Task<ActionResult<Pump>> UpdatePump(int id, [FromBody] PumpDTO pumpDto)
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
                    .AnyAsync(s => s.StationId == pumpDto.StationId && !s.IsDelete);
                if (!stationExists)
                {
                    return BadRequest(new { message = "Trạm bơm không tồn tại hoặc đã bị xóa" });
                }

                existingPump.StationId = pumpDto.StationId;
                existingPump.PumpName = pumpDto.PumpName;
                existingPump.PumpType = pumpDto.PumpType;
                existingPump.Capacity = pumpDto.Capacity;
                existingPump.Status = pumpDto.Status;
                existingPump.Manufacturer = pumpDto.Manufacturer;
                existingPump.SerialNumber = pumpDto.SerialNumber;
                existingPump.WarrantyExpireDate = pumpDto.WarrantyExpireDate;
                existingPump.Description = pumpDto.Description;
                existingPump.ModifiedBy = pumpDto.ModifiedBy;
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

        [HttpGet("export-excel")]
        public async Task<IActionResult> ExportToExcel([FromQuery] string? keyword = null, [FromQuery] int? stationId = null)
        {
            try
            {
            

                // Kiểm tra DbContext
                if (context == null)
                {
                    Console.WriteLine("DbContext là null");
                    return StatusCode(500, "DbContext không được khởi tạo");
                }


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
                Console.WriteLine("Thực hiện truy vấn...");
                var pumps = await query.ToListAsync();
                Console.WriteLine("Lấy được {0} trạm bơm", pumps.Count);

                Console.WriteLine("Tạo file Excel với ClosedXML...");
                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("PumpStations");

                // Tiêu đề
                worksheet.Cell(1, 1).Value = "Mã";
                worksheet.Cell(1, 2).Value = "Tên Máy Bơm";
                worksheet.Cell(1, 3).Value = "Loại Máy Bơm";
                worksheet.Cell(1, 4).Value = "Trạm Bơm";
                worksheet.Cell(1, 5).Value = "Công Suất";
                worksheet.Cell(1, 6).Value = "Trạng Thái";
                worksheet.Cell(1, 7).Value = "Nhà Sản Xuất";
                worksheet.Cell(1, 7).Value = "Số serial";
                worksheet.Cell(1, 7).Value = "Ngày hết bảo hành";
                worksheet.Cell(1, 7).Value = "Mô tả";
                worksheet.Cell(1, 7).Value = "Ngày tạo";
                worksheet.Cell(1, 7).Value = "Ngày chỉnh sửa";

                // Dữ liệu
                Console.WriteLine("Ghi dữ liệu vào worksheet...");
                int row = 2;
                foreach (var item in pumps)
                {
                    
                    worksheet.Cell(row, 1).Value = item.PumpId;
                    worksheet.Cell(row, 2).Value = item.PumpName ?? "N/A";
                    worksheet.Cell(row, 3).Value = EnumHelper.GetDescription((PumpType)item.PumpType);
                    worksheet.Cell(row, 4).Value = item.Station?.StationName ?? "N/A";
                    worksheet.Cell(row, 5).Value =item.Capacity;
                    worksheet.Cell(row, 6).Value = EnumHelper.GetDescription((PumpStatus)item.Status);
                    worksheet.Cell(row, 7).Value = item.Manufacturer;
                    worksheet.Cell(row, 8).Value = item.SerialNumber;
                    worksheet.Cell(row, 9).Value = item.WarrantyExpireDate?.ToString("dd/MM/yyyy") ?? "N/A";
                    worksheet.Cell(row, 10).Value = item.Description;
                    worksheet.Cell(row, 11).Value = item.CreatedOn?.ToString("dd/MM/yyyy") ?? "N/A";
                    worksheet.Cell(row, 12).Value = item.ModifiedOn?.ToString("dd/MM/yyyy") ?? "N/A";
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

        [HttpPut("set-active-all")]
        public async Task<ActionResult> SetAllPumpsActive([FromQuery] int modifiedBy)
        {
            try
            {
                var pumps = await context.Pumps.Where(p => !p.IsDelete).ToListAsync();

                foreach (var pump in pumps)
                {
                    pump.Status = (int)PumpStatus.Active;
                    pump.ModifiedBy = modifiedBy;
                    pump.ModifiedOn = DateTime.Now;
                }

                await context.SaveChangesAsync();
                return Ok(new { message = "Đã chuyển trạng thái tất cả máy bơm thành Đang hoạt động" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi cập nhật trạng thái máy bơm", error = ex.Message });
            }
        }

        [HttpPut("set-inactive-all")]
        public async Task<ActionResult> SetAllPumpsInactive([FromQuery] int modifiedBy)
        {
            try
            {
                var pumps = await context.Pumps.Where(p => !p.IsDelete).ToListAsync();

                foreach (var pump in pumps)
                {
                    pump.Status = (int)PumpStatus.Inactive;
                    pump.ModifiedBy = modifiedBy;
                    pump.ModifiedOn = DateTime.Now;
                }

                await context.SaveChangesAsync();
                return Ok(new { message = "Đã chuyển trạng thái tất cả máy bơm thành Ngừng hoạt động" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi cập nhật trạng thái máy bơm", error = ex.Message });
            }
        }

    }
}
