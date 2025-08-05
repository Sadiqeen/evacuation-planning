namespace EvacuationPlanning.Models.Dtos.Evacuation
{
    public class EvacuationPlanDto : EvacuationZoneDto
    {
        public string ZoneId { get; set; }
        public string? VehicleId { get; set; }
        public string? ETA { get; set; }
        public int Remaining { get; set; }
    }
}