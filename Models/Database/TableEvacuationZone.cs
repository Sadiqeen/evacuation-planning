using EvacuationPlanning.Models.Dtos;
using System.ComponentModel.DataAnnotations;

namespace EvacuationPlanning.Models
{
    public class TableEvacuationZone
    {
        [Key]
        public required string ZoneId { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public int NumberOfPeople { get; set; }
        public int UrgencyLevel { get; set; }
    }
}
