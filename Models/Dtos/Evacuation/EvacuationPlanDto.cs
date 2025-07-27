namespace EvacuationPlanning.Models.Dtos.Evacuation
{
    public class EvacuationPlanDto
    {
        public string ZoneId { get; set; }
        public string VehicleId { get; set;}
        public string ETA { get; set;}
        public int NumberOfPeople  { get; set;}
    }
}