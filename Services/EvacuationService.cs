using System.Text.Json;
using AutoMapper;
using EvacuationPlanning.Models;
using EvacuationPlanning.Models.Dtos;
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
        private static int _reasonableDistanceMetres { get { return 20 * 1000; } }

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
                vehicleDistances = CalculateDistance(zone, vehicleDistances);
                vehicleDistances = vehicleDistances.Where(x => x.Distance <= _reasonableDistanceMetres).ToList();
                _logger.LogInformation($"Zone {zone.ZoneId} has {vehicleDistances.Count} vehicles available for evacuation.");
                AssignVehicle(vehicleDistances, zone, plan);
            }

            var zoneWithNoVehicles = plan.Where(z => z.VehicleId == null).ToList();

            if (zoneWithNoVehicles.Count != 0)
            {
                throw new InvalidOperationException($"The following zones have no vehicles available for evacuation: {JsonSerializer.Serialize(zoneWithNoVehicles.Select(z => z.ZoneId))}");
            }

            return plan;
        }

        private static string CalEta(double distanceMetres, double speed)
        {
            var etaInSec = distanceMetres / UnitConvert.FromKmhToMps(speed);
            var eta = "";

            if (etaInSec <= TimeSpan.MaxValue.TotalMilliseconds)
            {
                eta = $"{TimeSpan.FromSeconds(etaInSec).Minutes} minutes";
            }
            else
            {
                eta = "N/A";
            }

            return eta;
        }

        private List<VehicleDistanceDto> CalculateDistance(TableEvacuationZone zone, List<VehicleDistanceDto> vehicleDistances)
        {
            return vehicleDistances = vehicleDistances
                    .Where(x => x.IsAvailable)
                    .Select(x =>
                    {
                        var distanceMetres = HaversineCalculate.Distance(
                            x.Vehicle.Latitude,
                            zone.Latitude,
                            x.Vehicle.Longitude,
                            zone.Longitude);

                        _logger.LogInformation($"Vehicle {x.VehicleId} is {distanceMetres} meters away from zone {zone.ZoneId}.");
                        var eta = CalEta(distanceMetres, x.Vehicle.Speed);
                        _logger.LogInformation($"Vehicle {x.VehicleId} has an ETA of {eta}.");

                        var vehicleDistance = new VehicleDistanceDto()
                        {
                            VehicleId = x.Vehicle.VehicleId,
                            Distance = distanceMetres,
                            ETA = eta,
                            Vehicle = x.Vehicle
                        };

                        return vehicleDistance;
                    })
                    .ToList();
        }

        private static void AssignVehicle(List<VehicleDistanceDto> vehicleDistances, TableEvacuationZone zone, List<EvacuationPlanDto> plan)
        {
            // Order by capacity
            var assignedVehicle = vehicleDistances.Where(x => x.Vehicle.Capacity >= zone.NumberOfPeople)
                .OrderBy(x => x.Vehicle.Capacity)
                .FirstOrDefault();

            // If no available capacity fine max capacity
            if (assignedVehicle == null)
            {
                assignedVehicle = vehicleDistances.OrderByDescending(x => x.Vehicle.Capacity).FirstOrDefault();
            }

            if (assignedVehicle == null)
            {
                // No vehicle available for this zone
                plan.Add(new EvacuationPlanDto()
                {
                    ZoneId = zone.ZoneId,
                    VehicleId = null,
                    ETA = null,
                    Remaining = zone.NumberOfPeople,
                    Evacuated = 0,
                    TotalPeople = zone.NumberOfPeople,
                    Zone = zone,
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
                Vehicle = assignedVehicle.Vehicle,
                TotalPeople = zone.NumberOfPeople,
                Remaining = zone.NumberOfPeople,
                Zone = zone,
            });

            int remain = zone.NumberOfPeople - assignedVehicle.Vehicle.Capacity;


            return;
        }

        public async Task<List<EvacuationPlanDto>> UpdateStatus(List<EvacuationPlanDto> plan, UpdateEvacuationStatusDto updateDto)
        {
            var evacuation = plan.FirstOrDefault(e => e.VehicleId == updateDto.VehicleId);
            if (evacuation == null)
            {
                throw new KeyNotFoundException($"Vehicle with ID {updateDto.VehicleId} not found in the evacuation plan.");
            }

            if (evacuation.Remaining < updateDto.Evacuated || evacuation.Vehicle.Capacity < updateDto.Evacuated)
            {
                throw new InvalidOperationException($"Cannot evacuate {updateDto.Evacuated} people from zone {evacuation.ZoneId}. Remaining people: {evacuation.Remaining}. Vehicle capacity: {evacuation.Vehicle.Capacity}.");
            }

            var evacuationLog = new EvacuationLogDto
            {
                VehicleId = updateDto.VehicleId,
                EvacueesMoved = updateDto.Evacuated,
                CreatedAt = DateTime.UtcNow
            };

            _logger.LogInformation($"Updating evacuation status for zone {evacuation.ZoneId} with vehicle {updateDto.VehicleId}.");

            var vehicle = new TableVehicle
            {
                VehicleId = evacuation.Vehicle.VehicleId,
                Latitude = evacuation.Vehicle.Latitude,
                Longitude = evacuation.Vehicle.Longitude,
                Speed = evacuation.Vehicle.Speed,
                Capacity = evacuation.Vehicle.Capacity
            };
            evacuation.VehicleId = null;
            evacuation.ETA = null;
            evacuation.Evacuated += updateDto.Evacuated;
            evacuation.Remaining = evacuation.TotalPeople - evacuation.Evacuated;
            evacuation.Log.Add(evacuationLog);

            _logger.LogInformation($"Vehicle {updateDto.VehicleId} has evacuated {updateDto.Evacuated} people. Remaining: {evacuation.Remaining}.");

            return await ShiftVehicle(plan, vehicle, updateDto);
        }

        private async Task<List<EvacuationPlanDto>> ShiftVehicle(List<EvacuationPlanDto> plan, TableVehicle vehicle, UpdateEvacuationStatusDto updateDto)
        {
            var evacuation = plan.FirstOrDefault(e => e.VehicleId == null && e.Remaining > 0);

            if (evacuation == null)
            {
                return plan;
            }

            evacuation.VehicleId = updateDto.VehicleId;
            evacuation.Vehicle = vehicle;

            var distance = HaversineCalculate.Distance(
                            vehicle.Latitude,
                            evacuation.Zone.Latitude,
                            vehicle.Longitude,
                            evacuation.Zone.Longitude);

            evacuation.ETA = CalEta(distance, vehicle.Speed);
            _logger.LogInformation($"Vehicle {updateDto.VehicleId} has been assigned to zone {evacuation.ZoneId} with an ETA of {evacuation.ETA}.");

            return plan;
        }
    }
}