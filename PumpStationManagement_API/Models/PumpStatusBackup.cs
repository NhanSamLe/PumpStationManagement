using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PumpStationManagement_API.Models
{
    public class PumpStatusBackup
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Pump))]
        public int PumpId { get; set; }

        public int OriginalStatus { get; set; }

        public DateTime BackupTime { get; set; } = DateTime.Now;

        public virtual Pump Pump { get; set; } = null!;
    }
}
