namespace PumpStationManagemnet_BlazorApp.DTOs
{
    public class UserDTO
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public int Role { get; set; }
        public bool? IsActive { get; set; } = true;
        public int? ModifiedBy { get; set; }
    }
}
