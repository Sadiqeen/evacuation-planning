using EvacuationPlanning.Models;

namespace EvacuationPlanning.Services.Interfaces
{
    public interface IVehicleService
    {
        Task<List<TableVehicle>> GetAllVehiclesAsync();
        Task<TableVehicle?> GetVehicleByIdAsync(string vehicleId);
        Task<TableVehicle> CreateVehicleAsync(TableVehicle vehicle);
    }
}
