using EvacuationPlanning.Enums;
using EvacuationPlanning.Models.Dtos;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EvacuationPlanning.Models
{
  public class TableVehicle
  {
    [Key]
    public required string VehicleId { get; set; }
    public int Capacity { get; set; }
    public VehicleType Type { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int Speed { get; set; }
  }
}
