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

        public async Task<List<EvacuationStatusDto>> GeneratePlan()
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
                vehicleDistances = vehicleDistances.Where(x => x.Distance <= _reasonableDistance).ToList();
                _logger.LogInformation($"Zone {zone.ZoneId} has {vehicleDistances.Count} vehicles available for evacuation.");
                AssignVehicle(vehicleDistances, zone, plan);
            }

            return _mapper.Map<List<EvacuationStatusDto>>(plan);
        }

        private static string CalEta(double distance, double speed)
        {
            var etaInSec = distance / UnitConvert.FromKmhToMps(speed);
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
                        var distance = HaversineCalculate.Distance(
                            x.LocationCoordinates.Latitude,
                            zone.Latitude,
                            x.LocationCoordinates.Longitude,
                            zone.Longitude);

                        _logger.LogInformation($"Vehicle {x.VehicleId} is {distance} meters away from zone {zone.ZoneId}.");
                        var eta = CalEta(distance, x.Speed);
                        _logger.LogInformation($"Vehicle {x.VehicleId} has an ETA of {eta}.");

                        var vehicleDistance = new VehicleDistanceDto()
                        {
                            VehicleId = x.VehicleId,
                            LocationCoordinates = x.LocationCoordinates,
                            Capacity = x.Capacity,
                            Type = x.Type,
                            Distance = distance,
                            Speed = x.Speed,
                            ETA = eta,
                        };

                        return vehicleDistance;
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
                    NumberOfPeople = zone.NumberOfPeople,
                    UrgencyLevel = zone.UrgencyLevel,
                    LocationCoordinates = new LocationCoordinatesDto
                    {
                        Latitude = zone.Latitude,
                        Longitude = zone.Longitude
                    }
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
                UrgencyLevel = zone.UrgencyLevel,
                LocationCoordinates = new LocationCoordinatesDto
                {
                    Latitude = zone.Latitude,
                    Longitude = zone.Longitude
                },
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

        public async Task<List<EvacuationStatusDto>> UpdateStatus(List<EvacuationStatusDto> plan, UpdateEvacuationStatusDto updateDto)
        {
            var evacuation = plan.FirstOrDefault(e => e.VehicleId == updateDto.VehicleId);
            if (evacuation == null)
            {
                throw new KeyNotFoundException($"Vehicle with ID {updateDto.VehicleId} not found in the evacuation plan.");
            }

            var evacuationLog = new EvacuationLogDto
            {
                VehicleId = updateDto.VehicleId,
                EvacueesMoved = updateDto.Evacuated,
                CreatedAt = DateTime.UtcNow
            };

            evacuation.VehicleId = null;
            evacuation.ETA = null;
            evacuation.Evacuated += updateDto.Evacuated;
            evacuation.Remaining -= updateDto.Evacuated;
            evacuation.Logs.Add(evacuationLog);

            var mappedZones = plan.Select(e => new EvacuationZoneDto
            {
                ZoneId = e.ZoneId,
                UrgencyLevel = e.UrgencyLevel,
                NumberOfPeople = e.Remaining,
                LocationCoordinates = e.LocationCoordinates
            }).ToList();

            return await ShiftVehicle(plan, updateDto);
        }

        private async Task<List<EvacuationStatusDto>> ShiftVehicle(List<EvacuationStatusDto> plan, UpdateEvacuationStatusDto updateDto)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(updateDto.VehicleId);
            if (vehicle == null)
            {
                throw new KeyNotFoundException($"Vehicle with ID {updateDto.VehicleId} not found.");
            }

            var evacuation = plan.FirstOrDefault(e => e.VehicleId == null && e.Remaining > 0);

            if (evacuation == null)
            {
                return plan;
            }

            evacuation.VehicleId = updateDto.VehicleId;

            var distance = HaversineCalculate.Distance(
                            vehicle.Latitude,
                            evacuation.LocationCoordinates.Latitude,
                            vehicle.Longitude,
                            evacuation.LocationCoordinates.Longitude);

            evacuation.ETA = CalEta(distance, vehicle.Speed);

            return plan;
        }
    }
}