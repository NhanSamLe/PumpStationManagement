using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PumpStationManagement_API.Models
{
    [Index(nameof(StationId), IsUnique = true)]
    public class PumpStation
    {
        [Key]
        public int StationId { get; set; }

        [Required]
        public string StationName { get; set; } = null!;

        [Required]
        public string Location { get; set; } = null!;

        public string? Description { get; set; }

        [Required]
        public int Status { get; set; }

        public bool IsDelete { get; set; } = false;

        public int? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        [ForeignKey(nameof(CreatedBy))]
        public virtual User? CreatedByNavigation { get; set; }

        [ForeignKey(nameof(ModifiedBy))]
        public virtual User? ModifiedByNavigation { get; set; }

        [InverseProperty(nameof(Pump.Station))]
        public virtual ICollection<Pump> Pumps { get; set; } = new List<Pump>();
    }
}
