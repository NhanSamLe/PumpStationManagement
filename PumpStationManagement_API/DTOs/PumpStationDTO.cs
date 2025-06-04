using System.ComponentModel.DataAnnotations;

namespace PumpStationManagement_API.DTOs
{
    public class PumpStationDTO
    {
        [Required]
        [MaxLength(100)]
        public string StationName { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        public string Location { get; set; } = null!;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public int Status { get; set; }

        public int? ModifiedBy { get; set; }
    }
}
