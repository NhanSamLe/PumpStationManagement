
using System.ComponentModel.DataAnnotations;

namespace PumpStationManagement_API.DTOs
{
    public class UserDTO
    {
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

        public int? ModifiedBy { get; set; }
    }
}
