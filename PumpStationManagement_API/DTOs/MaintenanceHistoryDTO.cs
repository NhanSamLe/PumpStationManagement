using System.ComponentModel.DataAnnotations;

namespace PumpStationManagement_API.DTOs
{
    public class MaintenanceHistoryDTO
    {
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
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
    }
}
