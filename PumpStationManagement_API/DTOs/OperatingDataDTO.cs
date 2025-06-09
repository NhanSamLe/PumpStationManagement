using System.ComponentModel.DataAnnotations;

namespace PumpStationManagement_API.DTOs
{
    public class OperatingDataDTO
    {
        [Required]
        public int PumpId { get; set; }

        [Required]
        public DateTime RecordTime { get; set; }

        public double? FlowRate { get; set; }

        public double? Pressure { get; set; }

        public double? PowerConsumption { get; set; }

        public double? Temperature { get; set; }

        public double? RunningHours { get; set; }

        public double? Efficiency { get; set; }

        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }

        [Required]
        public int Status { get; set; }
    }
}
