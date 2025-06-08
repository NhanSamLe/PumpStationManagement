namespace PumpStationManagemnet_BlazorApp.DTOs
{
    public class StationStatusDto
    {
        public int StationId { get; set; }
        public string StationName { get; set; } = null!;
        public List<StatusCountDto> StatusCounts { get; set; } = new();
    }
}
