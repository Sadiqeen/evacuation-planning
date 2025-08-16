namespace EvacuationPlanning.Models.Dtos.Evacuation
{
    public class EvacuationStatusDto : EvacuationPlanDto
    {
        public int Evacuated { get; set; }
        public List<EvacuationLogDto> Logs { get; set; } = new ();
    }
}