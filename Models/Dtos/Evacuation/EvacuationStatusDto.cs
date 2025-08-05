namespace EvacuationPlanning.Models.Dtos.Evacuation
{
    public class EvacuationStatusDto : EvacuationPlanDto
    {
        public int Evacuated { get; set; }
        public List<EvacuationLogDto> Logs { get; set; } = new ();
    }

    public class EvacuationLogDto
    {
        public string VehicleId { get; set; }
        public int EvacueesMoved { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}