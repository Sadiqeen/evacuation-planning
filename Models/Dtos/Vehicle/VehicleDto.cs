using EvacuationPlanning.Enums;

namespace EvacuationPlanning.Models.Dtos
{
    public class VehicleDto
    {
        public string VehicleId { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public VehicleType Type { get; set; }
        public LocationCoordinatesDto LocationCoordinates { get; set; } = new();
        public decimal Speed { get; set; }
    }
}
