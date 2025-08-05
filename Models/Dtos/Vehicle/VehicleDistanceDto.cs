using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvacuationPlanning.Models.Dtos.Vehicle
{
    public class VehicleDistanceDto : VehicleDto
    {
        public double Distance { get; set; }
        public string ETA { get; set; }
        public bool IsAvailable { get; set; } = true;
    }
}