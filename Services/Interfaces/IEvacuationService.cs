using EvacuationPlanning.Models.Dtos.Evacuation;

namespace EvacuationPlanning.Services.Interfaces
{
    public interface IEvacuationService
    {
        Task<List<EvacuationPlanDto>> GeneratePlan();
        Task<List<EvacuationPlanDto>> UpdateStatus(List<EvacuationPlanDto> plan, UpdateEvacuationStatusDto updateDto);
    }
}