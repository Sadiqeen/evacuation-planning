namespace EvacuationPlanning.Models.Dtos.Evacuation
{
    public class EvacuationPlanDto
    {
        public string ZoneId { get; set; }
        public string? VehicleId { get; set; }
        public string? ETA { get; set; }
        public int Remaining { get; set; }
        public int Evacuated { get; set; }
        public int TotalPeople { get; set; }
        public TableEvacuationZone? Zone { get; set; }
        public TableVehicle? Vehicle { get; set; }
        public List<EvacuationLogDto> Log { get; set; } = new ();
    }
    
    public class EvacuationLogDto
    {
        public string VehicleId { get; set; }
        public int EvacueesMoved { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
