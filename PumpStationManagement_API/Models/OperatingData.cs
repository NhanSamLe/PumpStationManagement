using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PumpStationManagement_API.Models
{
    [Index(nameof(DataId), IsUnique = true)]
    public class OperatingData
    {
        [Key]
        public int DataId { get; set; }

        public int? PumpId { get; set; }

        [Required]
        public DateTime RecordTime { get; set; }

        public double? FlowRate { get; set; }

        public double? Pressure { get; set; }

        public double? PowerConsumption { get; set; }

        public double? Temperature { get; set; }

        public double? RunningHours { get; set; }

        public double? Efficiency { get; set; }

        [Required]
        public int Status { get; set; }

        public bool IsDelete { get; set; } = false;

        public DateTime? CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

      
        [ForeignKey(nameof(PumpId))]
        public virtual Pump? Pump { get; set; }
    }
}
