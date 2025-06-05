using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PumpStationManagement_API.Models
{
    [Index(nameof(PumpId), IsUnique = true)]
    public class Pump
    {
        [Key]
        public int PumpId { get; set; }

        public int? StationId { get; set; }

        [Required]
        public string PumpName { get; set; } = null!;

        [Required]
        public int PumpType { get; set; }

        public double? Capacity { get; set; }

        [Required]
        public int Status { get; set; }

        public bool IsDelete { get; set; } = false;

        public string? Manufacturer { get; set; }

        public string? SerialNumber { get; set; }

        public DateTime? WarrantyExpireDate { get; set; }

        public string? Description { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        [JsonIgnore]

        [InverseProperty(nameof(Alert.Pump))]
        public virtual ICollection<Alert> Alerts { get; set; } = new List<Alert>();

        [ForeignKey(nameof(CreatedBy))]
        public virtual User? CreatedByNavigation { get; set; }

        [JsonIgnore]
        [InverseProperty(nameof(MaintenanceHistory.Pump))]
        public virtual ICollection<MaintenanceHistory> MaintenanceHistories { get; set; } = new List<MaintenanceHistory>();

        [ForeignKey(nameof(ModifiedBy))]
        public virtual User? ModifiedByNavigation { get; set; }

        [JsonIgnore]
        [InverseProperty(nameof(OperatingData.Pump))]
        public virtual ICollection<OperatingData> OperatingDatas { get; set; } = new List<OperatingData>();

        [ForeignKey(nameof(StationId))]
        public virtual PumpStation? Station { get; set; }
    }
}
