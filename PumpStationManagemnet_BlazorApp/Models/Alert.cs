namespace PumpStationManagemnet_BlazorApp.Models
{
    public class Alert
    {
        public int AlertId { get; set; }
        public int? PumpId { get; set; }
        public int AlertType { get; set; }
        public string AlertMessage { get; set; } = string.Empty;
        public int Status { get; set; }
        public bool IsDelete { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public User? CreatedByNavigation { get; set; }
        public User? ModifiedByNavigation { get; set; }
        public Pump? Pump { get; set; }
    }
}
