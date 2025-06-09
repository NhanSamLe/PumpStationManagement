using System.ComponentModel.DataAnnotations;

namespace PumpStationManagemnet_BlazorApp.DTOs
{
    public class MaintenanceHistoryDTO
    {
        public int? PumpId { get; set; }

        [Required(ErrorMessage = "Loại bảo trì là bắt buộc")]
        public int MaintenanceType { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc")]
        public DateTime StartDate { get; set; } = DateTime.Now;

        public DateTime? EndDate { get; set; }

        [MaxLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc")]
        public int Status { get; set; }

        public int? PerformedBy { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
    }
}
