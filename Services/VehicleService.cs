using EvacuationPlanning.Models;
using EvacuationPlanning.Repositories.Interfaces;
using EvacuationPlanning.Services.Interfaces;
using System.Collections.Concurrent;

namespace EvacuationPlanning.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;

        public VehicleService(IVehicleRepository vehicleRepository)
        {
            _vehicleRepository = vehicleRepository;
        }

        public async Task<List<TableVehicle>> GetAllVehiclesAsync()
        {
            return await _vehicleRepository.GetAllAsync();
        }

        public async Task<TableVehicle?> GetVehicleByIdAsync(string vehicleId)
        {
           return await _vehicleRepository.GetByIdAsync(vehicleId);
        }

        public async Task<TableVehicle> CreateVehicleAsync(TableVehicle vehicle)
        {
           return await _vehicleRepository.InsertAsync(vehicle);
        }
    }
}
