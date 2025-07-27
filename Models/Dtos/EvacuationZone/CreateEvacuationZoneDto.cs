using System.ComponentModel.DataAnnotations;

namespace EvacuationPlanning.Models.Dtos
{
    public class CreateEvacuationZoneDto
    {
        [Required]
        public string ZoneId { get; set; } = string.Empty;
        
        [Required]
        public LocationCoordinatesDto LocationCoordinates { get; set; } = new();
        
        [Required]
        [Range(0, int.MaxValue)]
        public int NumberOfPeople { get; set; }
        
        [Required]
        [Range(1, 5)]
        public int UrgencyLevel { get; set; }
    }
}
