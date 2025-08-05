namespace EvacuationPlanning.Models.Dtos.Evacuation
{
    public class EvacuationStatusResponseDto
    {
        public string ZoneId { get; set; }
        public int TotalEvacuated { get; set; }
        public int RemainingPeople { get; set; }
        public string? LastTravelBy { get; set; }
    }
}