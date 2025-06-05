using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PumpStationManagement_API.Models
{
    [Index(nameof(MaintenanceId), IsUnique = true)]
    public class MaintenanceHistory
    {
        [Key]
        public int MaintenanceId { get; set; }

        public int? PumpId { get; set; }

        [Required]
        public int MaintenanceType { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public int Status { get; set; }

        public int? PerformedBy { get; set; }

        public bool IsDelete { get; set; } = false;

        public DateTime? CreatedOn { get; set; }

        // Navigation properties

        [ForeignKey(nameof(PerformedBy))]
        public virtual User? PerformedByNavigation { get; set; }

 
        [ForeignKey(nameof(PumpId))]
        public virtual Pump? Pump { get; set; }
    }
}
