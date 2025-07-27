using System.Text.Json;
using AutoMapper;
using EvacuationPlanning.Models;
using EvacuationPlanning.Models.Dtos.Evacuation;
using EvacuationPlanning.Models.Dtos.Vehicle;
using EvacuationPlanning.Repositories.Interfaces;
using EvacuationPlanning.Services.Interfaces;

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

        public async Task GeneratePlan()
        {
            var vehicles = await _vehicleRepository.GetAllAsync();
            var zones = await _evacuationZoneRepository.GetAllAsync();

            if (vehicles.Count == 0 || zones.Count == 0)
            {
                throw new InvalidOperationException("No vehicles or zones available for evacuation planning.");
            }

            List<EvacuationPlanDto> plan = new();
            List<VehicleDistanceDto> vehicleDistances = _mapper.Map<List<VehicleDistanceDto>>(vehicles);

            foreach (var zone in zones)
            {
                vehicleDistances = CalculateDistance(vehicleDistances, _reasonableDistance);
                AssignVehicle(vehicleDistances, zone, plan);
            }

            _logger.LogInformation("Evacuation plan generated successfully.");
            _logger.LogInformation("Plan details: {@Plan}", JsonSerializer.Serialize(plan));
        }

        private static List<VehicleDistanceDto> CalculateDistance(List<VehicleDistanceDto> vehicleDistances, int reasonableDistance)
        {
            return vehicleDistances = vehicleDistances
                    .Where(x => x.Distance <= reasonableDistance)
                    .Where(x => x.IsAvailable)
                    .Select(x => new VehicleDistanceDto()
                    {
                        VehicleId = x.VehicleId,
                        LocationCoordinates = x.LocationCoordinates,
                        Capacity = x.Capacity,
                        Type = x.Type,
                        // TODO : Calculate real distance and ETA
                        Distance = 10,
                        ETA = "10",
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
                AssignVehicle(vehicleDistances, zone, plan);
            }
        }
    }
}