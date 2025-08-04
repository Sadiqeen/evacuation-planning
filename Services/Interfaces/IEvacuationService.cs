using EvacuationPlanning.Models.Dtos.Evacuation;

namespace EvacuationPlanning.Services.Interfaces
{
    public interface IEvacuationService
    {
        Task<List<EvacuationStatusDto>> GeneratePlan();
        Task<List<EvacuationStatusDto>> UpdateStatus(List<EvacuationStatusDto> plan, UpdateEvacuationStatusDto updateDto);
    }
}