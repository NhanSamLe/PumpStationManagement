using System.ComponentModel.DataAnnotations;

namespace PumpStationManagemnet_BlazorApp.DTOs
{
    public class PumpDTO
    {
        [Required(ErrorMessage = "Trạm bơm là bắt buộc")]
        public int StationId { get; set; }

        [Required(ErrorMessage = "Tên máy bơm là bắt buộc")]
        [MaxLength(100, ErrorMessage = "Tên máy bơm không được vượt quá 100 ký tự")]
        public string PumpName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Loại máy bơm là bắt buộc")]
        public int PumpType { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Công suất phải là số không âm")]
        public double? Capacity { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc")]
        public int Status { get; set; }

        [MaxLength(100, ErrorMessage = "Nhà sản xuất không được vượt quá 100 ký tự")]
        public string? Manufacturer { get; set; }

        [MaxLength(50, ErrorMessage = "Số serial không được vượt quá 50 ký tự")]
        public string? SerialNumber { get; set; }

        public DateTime? WarrantyExpireDate { get; set; }

        [MaxLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string? Description { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
    }
}
