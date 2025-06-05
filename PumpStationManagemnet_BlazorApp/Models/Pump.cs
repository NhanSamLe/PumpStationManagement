namespace PumpStationManagemnet_BlazorApp.Models
{
    public class Pump
    {
        public int PumpId { get; set; }
        public int? StationId { get; set; }
        public string PumpName { get; set; } = string.Empty;
        public int PumpType { get; set; }
        public double? Capacity { get; set; }
        public int Status { get; set; }
        public string? Manufacturer { get; set; }
        public string? SerialNumber { get; set; }
        public DateTime? WarrantyExpireDate { get; set; }
        public string? Description { get; set; }
        public bool IsDelete { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
