namespace PumpStationManagemnet_BlazorApp.DTOs
{
    public class StatusCountDto
    {
        public int Status { get; set; }
        public string StatusName { get; set; } = null!;
        public int Count { get; set; }
        public double Percentage { get; set; }
    }
}
