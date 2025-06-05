using System.ComponentModel.DataAnnotations;

namespace PumpStationManagemnet_BlazorApp.DTOs
{
    public class AlertDTO
    {
        public int? PumpId { get; set; }

        [Required(ErrorMessage = "Loại cảnh báo là bắt buộc")]
        public int AlertType { get; set; }

        [Required(ErrorMessage = "Thông điệp cảnh báo là bắt buộc")]
        [MaxLength(500, ErrorMessage = "Thông điệp không được vượt quá 500 ký tự")]
        public string AlertMessage { get; set; } = string.Empty;

        [Required(ErrorMessage = "Trạng thái là bắt buộc")]
        public int Status { get; set; }

        public int? ModifiedBy { get; set; }
    }
}
