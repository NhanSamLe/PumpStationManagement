using System.ComponentModel.DataAnnotations;

namespace PumpStationManagemnet_BlazorApp.DTOs
{
    public class PumpStationDTO
    {
        [Required(ErrorMessage = "Tên trạm bơm là bắt buộc")]
        [MaxLength(100)]
        public string StationName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vị trí là bắt buộc")]
        [MaxLength(200)]
        public string Location { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc")]
        public int Status { get; set; }

        public int? ModifiedBy { get; set; }
    }
}
