namespace EvacuationPlanning.Models.Dtos.Evacuation
{
    public class EvacuationPlanResponseDto
    {
        public string ZoneId { get; set; }
        public string VehicleId { get; set; }
        public string ETA { get; set; }
        public int NumberOfPeople { get; set; }
    }
}