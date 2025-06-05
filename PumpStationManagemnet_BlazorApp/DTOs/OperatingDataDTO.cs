using System.ComponentModel.DataAnnotations;

namespace PumpStationManagemnet_BlazorApp.DTOs
{
    public class OperatingDataDTO
    {
        [Required(ErrorMessage = "Máy bơm là bắt buộc")]
        public int PumpId { get; set; }

        [Required(ErrorMessage = "Thời gian ghi nhận là bắt buộc")]
        public DateTime RecordTime { get; set; } = DateTime.Now;

        [Range(0, double.MaxValue, ErrorMessage = "Lưu lượng phải là số không âm")]
        public double? FlowRate { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Áp suất phải là số không âm")]
        public double? Pressure { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Công suất tiêu thụ phải là số không âm")]
        public double? PowerConsumption { get; set; }

        [Range(-50, 200, ErrorMessage = "Nhiệt độ phải nằm trong khoảng -50 đến 200°C")]
        public double? Temperature { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Số giờ vận hành phải là số không âm")]
        public double? RunningHours { get; set; }

        [Range(0, 100, ErrorMessage = "Hiệu suất phải từ 0 đến 100%")]
        public double? Efficiency { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc")]
        public int Status { get; set; }
    }
}
