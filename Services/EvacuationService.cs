using System.Text.Json;
using AutoMapper;
using EvacuationPlanning.Models;
using EvacuationPlanning.Models.Dtos.Evacuation;
using EvacuationPlanning.Models.Dtos.Vehicle;
using EvacuationPlanning.Repositories.Interfaces;
using EvacuationPlanning.Services.Interfaces;
using EvacuationPlanning.Utils;

namespace EvacuationPlanning.Services
{
    public class EvacuationService : IEvacuationService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<EvacuationService> _logger;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IEvacuationZoneRepository _evacuationZoneRepository;
        private int _reasonableDistance { get { return 100; } }

        public EvacuationService(
            IMapper mapper,
            ILogger<EvacuationService> logger,
            IVehicleRepository vehicleRepository,
            IEvacuationZoneRepository evacuationZoneRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _vehicleRepository = vehicleRepository;
            _evacuationZoneRepository = evacuationZoneRepository;
        }

        public async Task<List<EvacuationPlanDto>> GeneratePlan()
        {
            var vehicles = await _vehicleRepository.GetAllAsync();
            var zones = await _evacuationZoneRepository.GetAllAsync();
            zones = zones.OrderByDescending(x => x.UrgencyLevel).ToList();

            if (vehicles.Count == 0 || zones.Count == 0)
            {
                throw new InvalidOperationException("No vehicles or zones available for evacuation planning.");
            }

            List<EvacuationPlanDto> plan = new();
            List<VehicleDistanceDto> vehicleDistances = _mapper.Map<List<VehicleDistanceDto>>(vehicles);

            foreach (var zone in zones)
            {
                vehicleDistances = CalculateDistance(zone, vehicleDistances, _reasonableDistance);
                AssignVehicle(vehicleDistances, zone, plan);
            }

            return plan;
        }

        private static List<VehicleDistanceDto> CalculateDistance(TableEvacuationZone zone, List<VehicleDistanceDto> vehicleDistances, int reasonableDistance)
        {
            return vehicleDistances = vehicleDistances
                    .Where(x => x.IsAvailable)
                    .Select(x =>
                    {
                        // Calculate distance and ETA
                        var distance = HaversineCalculate.Distance(
                            x.LocationCoordinates.Latitude,
                            zone.Latitude,
                            x.LocationCoordinates.Longitude,
                            zone.Longitude);

                        var etaInSec = distance / UnitConvert.FromKmhToMps(x.Speed);
                        var eta = "";
                        
                        if (etaInSec <= TimeSpan.MaxValue.TotalMilliseconds)
                        {
                            eta = $"{TimeSpan.FromSeconds(etaInSec).Minutes} minutes";
                        }
                        else
                        {
                            eta = "N/A";
                        }

                        var test = new VehicleDistanceDto()
                        {
                            VehicleId = x.VehicleId,
                            LocationCoordinates = x.LocationCoordinates,
                            Capacity = x.Capacity,
                            Type = x.Type,
                            Distance = distance,
                            Speed = x.Speed,
                            ETA = eta,
                        };

                        return test;
                    })
                    .ToList();
        }

        private static void AssignVehicle(List<VehicleDistanceDto> vehicleDistances, TableEvacuationZone zone, List<EvacuationPlanDto> plan)
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
                // No vehicle available for this zone
                plan.Add(new EvacuationPlanDto()
                {
                    ZoneId = zone.ZoneId,
                    VehicleId = null,
                    ETA = null,
                    NumberOfPeople = 0,
                });

                return;
            }

            // Assign vehicle to zone
            assignedVehicle.IsAvailable = false;
            plan.Add(new EvacuationPlanDto()
            {
                ZoneId = zone.ZoneId,
                VehicleId = assignedVehicle.VehicleId,
                ETA = assignedVehicle.ETA,
                NumberOfPeople =
                    zone.NumberOfPeople >= assignedVehicle.Capacity
                        ? assignedVehicle.Capacity
                        : assignedVehicle.Capacity - zone.NumberOfPeople,
            });

            int remain = zone.NumberOfPeople - assignedVehicle.Capacity;

            if (remain > 0)
            {
                zone.NumberOfPeople = remain;
                vehicleDistances = vehicleDistances.Where(x => x.VehicleId != assignedVehicle.VehicleId).ToList();
                AssignVehicle(vehicleDistances, zone, plan);
            }

            return;
        }
    }
}