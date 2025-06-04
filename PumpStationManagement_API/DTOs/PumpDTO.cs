using System.ComponentModel.DataAnnotations;

namespace PumpStationManagement_API.DTOs
{
    public class PumpDTO
    {
        [Required]
        public int StationId { get; set; }

        [Required]
        [MaxLength(100)]
        public string PumpName { get; set; } = null!;

        [Required]
        public int PumpType { get; set; }

        public double? Capacity { get; set; }

        [Required]
        public int Status { get; set; }

        [MaxLength(100)]
        public string? Manufacturer { get; set; }

        [MaxLength(50)]
        public string? SerialNumber { get; set; }

        public DateTime? WarrantyExpireDate { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public int? ModifiedBy { get; set; }
    }
}
