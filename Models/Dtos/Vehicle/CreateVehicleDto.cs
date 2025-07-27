using System.ComponentModel.DataAnnotations;
using EvacuationPlanning.Enums;

namespace EvacuationPlanning.Models.Dtos
{
    public class CreateVehicleDto
    {
        [Required]
        public string VehicleId { get; set; } = string.Empty;
        
        [Required]
        [Range(1, int.MaxValue)]
        public int Capacity { get; set; }
        
        [Required]
        public VehicleType Type { get; set; }
        
        [Required]
        public LocationCoordinatesDto LocationCoordinates { get; set; } = new();
        
        [Required]
        [Range(0, double.MaxValue)]
        public int Speed { get; set; }
    }
}
