using System.ComponentModel.DataAnnotations;

namespace EvacuationPlanning.Models.Dtos.Evacuation
{
    public class UpdateEvacuationStatusDto
    {
        [Required]
        public string VehicleId { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int Evacuated { get; set; }
    }
}