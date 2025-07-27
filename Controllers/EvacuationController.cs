using Microsoft.AspNetCore.Mvc;
using EvacuationPlanning.Models;
using EvacuationPlanning.Models.Dtos;
using EvacuationPlanning.Services.Interfaces;
using AutoMapper;
using EvacuationPlanning.Enums;
using EvacuationPlanning.Models.Dtos.Vehicle;
using System.Globalization;
using EvacuationPlanning.Models.Dtos.Evacuation;

namespace EvacuationPlanning.Controllers
{
    [ApiController]
    [Route("api/evacuations")]
    public class EvacuationController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<EvacuationController> _logger;
        private int _reasonableDistance { get { return 100; } }

        public EvacuationController(
            IMapper mapper,
            ILogger<EvacuationController> logger
        )
        {
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost("plan")]
        public async Task generatePlan()
        {
            List<VehicleDto> vehicles = GetVehicles();
            List<EvacuationZoneDto> zones = GetZones();

            // Order zone
            List<EvacuationZoneDto> zoneUrgentOrder = zones.OrderBy(x => x.UrgencyLevel).ToList();

            var res = assignedVehicle(vehicles, zones);
            _logger.LogInformation("response => ", res);

        }

        private void assignV(List<VehicleDistanceDto> vehicleDistances, EvacuationZoneDto zone, List<EvacuationPlanDto> plan)
        {
            // Order by capacity
            var assignedVehicle = vehicleDistances.Where(x => x.Capacity >= zone.NumberOfPeople)
                .OrderBy(x => x.Capacity)
                .FirstOrDefault();

            // If no available capacity fine max capacity
            if (assignedVehicle == null)
            {
                assignedVehicle = vehicleDistances.OrderByDescending(x => x.Capacity).FirstOrDefault();
            }

            if (assignedVehicle == null)
            {
                return;
            }

            assignedVehicle.IsAvailable = false;
            plan.Add(new EvacuationPlanDto()
            {
                ZoneId = zone.ZoneId,
                VehicleId = assignedVehicle.VehicleId,
                ETA = assignedVehicle.ETA,
                NumberOfPeople = assignedVehicle.Capacity,
            });

            int remain = zone.NumberOfPeople - assignedVehicle.Capacity;

            if (remain > 0)
            {
                zone.NumberOfPeople = remain;
                vehicleDistances = vehicleDistances.Where(x => x.VehicleId != assignedVehicle.VehicleId).ToList();
                assignV(vehicleDistances, zone, plan);
            }
        }

        private List<EvacuationPlanDto> assignedVehicle(List<VehicleDto> vehicles, List<EvacuationZoneDto> zones)
        {
            List<EvacuationPlanDto> plan = new();
            List<VehicleDistanceDto> vehicleDistances = _mapper.Map<List<VehicleDistanceDto>>(vehicles);

            foreach (var zone in zones)
            {
                vehicleDistances = vehicleDistances
                    .Where(x => x.Distance <= _reasonableDistance)
                    .Where(x => x.IsAvailable)
                    .Select(x => new VehicleDistanceDto()
                    {
                        VehicleId = x.VehicleId,
                        LocationCoordinates = x.LocationCoordinates,
                        Capacity = x.Capacity,
                        Type = x.Type,
                        Distance = 10,
                        ETA = "10",
                    })
                    .ToList();

                assignV(vehicleDistances, zone, plan);
            }

            return plan;
        }

        private List<VehicleDto> GetVehicles()
        {
            List<VehicleDto> vehicles = new List<VehicleDto>() { };
            vehicles.Add(new VehicleDto()
            {
                VehicleId = "001",
                Capacity = 10,
                Type = VehicleType.BUS,
                Speed = 80,
                LocationCoordinates = new LocationCoordinatesDto(),
            });

            vehicles.Add(new VehicleDto()
            {
                VehicleId = "002",
                Capacity = 50,
                Type = VehicleType.BUS,
                Speed = 40,
                LocationCoordinates = new LocationCoordinatesDto(),
            });

            vehicles.Add(new VehicleDto()
            {
                VehicleId = "003",
                Capacity = 50,
                Type = VehicleType.BUS,
                Speed = 40,
                LocationCoordinates = new LocationCoordinatesDto(),
            });

            return vehicles;
        }

        private List<EvacuationZoneDto> GetZones()
        {
            List<EvacuationZoneDto> zones = new List<EvacuationZoneDto>() { };
            zones.Add(new EvacuationZoneDto()
            {
                ZoneId = "001",
                NumberOfPeople = 100,
                LocationCoordinates = new LocationCoordinatesDto(),
                UrgencyLevel = 1,
            });

            zones.Add(new EvacuationZoneDto()
            {
                ZoneId = "002",
                NumberOfPeople = 20,
                LocationCoordinates = new LocationCoordinatesDto(),
                UrgencyLevel = 2,
            });

            zones.Add(new EvacuationZoneDto()
            {
                ZoneId = "003",
                NumberOfPeople = 30,
                LocationCoordinates = new LocationCoordinatesDto(),
                UrgencyLevel = 3,
            });

            zones.Add(new EvacuationZoneDto()
            {
                ZoneId = "004",
                NumberOfPeople = 10,
                LocationCoordinates = new LocationCoordinatesDto(),
                UrgencyLevel = 1,
            });

            return zones;
        }
    }
}