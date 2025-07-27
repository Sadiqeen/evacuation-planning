using EvacuationPlanning.Models;
using EvacuationPlanning.Repositories.Interfaces;
using EvacuationPlanning.Services.Interfaces;

namespace EvacuationPlanning.Services
{
    public class EvacuationZoneService : IEvacuationZoneService
    {
        private readonly IEvacuationZoneRepository _evacuationZoneRepository;

        public EvacuationZoneService(
            IEvacuationZoneRepository evacuationZoneRepository
            )
        {
            _evacuationZoneRepository = evacuationZoneRepository;
        }

        public async Task<List<TableEvacuationZone>> GetAllEvacuationZonesAsync()
        {
            return await _evacuationZoneRepository.GetAllAsync();
        }

        public async Task<TableEvacuationZone?> GetEvacuationZoneByIdAsync(string zoneId)
        {
            return await _evacuationZoneRepository.GetByIdAsync(zoneId);
        }

        public async Task<TableEvacuationZone> CreateEvacuationZoneAsync(TableEvacuationZone evacuationZone)
        {
            
            return await _evacuationZoneRepository.InsertAsync(evacuationZone);
        }
    }
}
