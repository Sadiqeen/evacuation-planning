using Microsoft.AspNetCore.Mvc;
using EvacuationPlanning.Services.Interfaces;
using AutoMapper;
using EvacuationPlanning.Models.Dtos.Evacuation;

namespace EvacuationPlanning.Controllers
{
    [ApiController]
    [Route("api/evacuations")]
    public class EvacuationController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<EvacuationController> _logger;
        private readonly IEvacuationService _evacuationService;

        public EvacuationController(
            IMapper mapper,
            ILogger<EvacuationController> logger,
            IEvacuationService evacuationService
        )
        {
            _mapper = mapper;
            _logger = logger;
            _evacuationService = evacuationService;
        }

        [HttpPost("plan")]
        public async Task<List<EvacuationPlanDto>> GeneratePlan()
        {
            return await _evacuationService.GeneratePlan();
        }

    }
}