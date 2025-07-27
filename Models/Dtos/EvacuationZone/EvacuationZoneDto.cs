namespace EvacuationPlanning.Models.Dtos
{
    public class EvacuationZoneDto
    {
        public string ZoneId { get; set; } = string.Empty;
        public LocationCoordinatesDto LocationCoordinates { get; set; } = new();
        public int NumberOfPeople { get; set; }
        public int UrgencyLevel { get; set; }
    }
}
