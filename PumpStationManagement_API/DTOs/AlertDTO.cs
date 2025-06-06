using System.ComponentModel.DataAnnotations;

namespace PumpStationManagement_API.DTOs
{
    public class AlertDTO
    {
        public int? PumpId { get; set; }

        [Required]
        public int AlertType { get; set; }

        [Required]
        [MaxLength(500)]
        public string AlertMessage { get; set; } = null!;

        [Required]
        public int Status { get; set; }

        public int? CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }
    }
}
