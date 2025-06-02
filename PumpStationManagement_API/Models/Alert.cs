using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PumpStationManagement_API.Models
{
    [Index(nameof(AlertId), IsUnique = true)]
    public class Alert
    {
        [Key]
        public int AlertId { get; set; }

        public int? PumpId { get; set; }

        public int AlertType { get; set; }

        [Required]
        [MaxLength(500)]
        public string AlertMessage { get; set; } = null!;

        public int Status { get; set; }

        public bool IsDelete { get; set; }

        public int? CreatedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreatedOn { get; set; }

        public int? ModifiedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ModifiedOn { get; set; }

        [ForeignKey(nameof(CreatedBy))]
        public virtual User? CreatedByNavigation { get; set; }

        [ForeignKey(nameof(ModifiedBy))]
        public virtual User? ModifiedByNavigation { get; set; }

        [ForeignKey(nameof(PumpId))]
        public virtual Pump? Pump { get; set; }
    }
}
