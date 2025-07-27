using EvacuationPlanning.Models;

namespace EvacuationPlanning.Services.Interfaces
{
    public interface IEvacuationZoneService
    {
        Task<List<TableEvacuationZone>> GetAllEvacuationZonesAsync();
        Task<TableEvacuationZone?> GetEvacuationZoneByIdAsync(string zoneId);
        Task<TableEvacuationZone> CreateEvacuationZoneAsync(TableEvacuationZone evacuationZone);
    }
}
