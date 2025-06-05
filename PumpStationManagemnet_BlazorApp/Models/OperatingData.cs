namespace PumpStationManagemnet_BlazorApp.Models
{
    public class OperatingData
    {
        public int DataId { get; set; }
        public int? PumpId { get; set; }
        public DateTime RecordTime { get; set; }
        public double? FlowRate { get; set; }
        public double? Pressure { get; set; }
        public double? PowerConsumption { get; set; }
        public double? Temperature { get; set; }
        public double? RunningHours { get; set; }
        public double? Efficiency { get; set; }
        public int Status { get; set; }
        public bool IsDelete { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public Pump? Pump { get; set; }
    }
}
