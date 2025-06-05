using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace PumpStationManagement_API.Models
{
    [Index(nameof(Email), IsUnique = true)]
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string Password { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [Required]
        public int Role { get; set; }

        public bool? IsActive { get; set; } = true;

        public bool IsDelete { get; set; } = false;

        public DateTime? CreatedOn { get; set; }

        [Required]
        public int CreatedBy { get; set; }

        public DateTime? LastLogin { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        // Navigation properties

        [JsonIgnore]
        public virtual ICollection<Alert> AlertCreatedByNavigations { get; set; } = new List<Alert>();

        [JsonIgnore]
        public virtual ICollection<Alert> AlertModifiedByNavigations { get; set; } = new List<Alert>();

        [JsonIgnore]
        public virtual ICollection<MaintenanceHistory> MaintenanceHistories { get; set; } = new List<MaintenanceHistory>();

        public virtual ICollection<Pump> PumpCreatedByNavigations { get; set; } = new List<Pump>();

        public virtual ICollection<Pump> PumpModifiedByNavigations { get; set; } = new List<Pump>();

        public virtual ICollection<PumpStation> PumpStationCreatedByNavigations { get; set; } = new List<PumpStation>();

        public virtual ICollection<PumpStation> PumpStationModifiedByNavigations { get; set; } = new List<PumpStation>();
    }
}
