using Microsoft.AspNetCore.Mvc;
using EvacuationPlanning.Services.Interfaces;
using AutoMapper;
using EvacuationPlanning.Models.Dtos.Evacuation;
using EvacuationPlanning.Infrastructures.Interfaces;

namespace EvacuationPlanning.Controllers
{
    [ApiController]
    [Route("api/evacuations")]
    public class EvacuationController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<EvacuationController> _logger;
        private readonly IEvacuationService _evacuationService;
        private readonly IRedisService _redisService;

        public EvacuationController(
            IMapper mapper,
            ILogger<EvacuationController> logger,
            IEvacuationService evacuationService,
            IRedisService redisService
        )
        {
            _mapper = mapper;
            _logger = logger;
            _evacuationService = evacuationService;
            _redisService = redisService;
        }

        [HttpPost("plan")]
        public async Task<List<EvacuationPlanDto>> GeneratePlan()
        {
            var cacheKey = "evacuation_plan";
            var plan = await _redisService.GetAsync<List<EvacuationPlanDto>>(cacheKey);

            if (plan == null)
            {
                plan = await _evacuationService.GeneratePlan();
                await _redisService.SetAsync(cacheKey, plan);
            }

            return plan;
        }


        [HttpPost("clear")]
        public async Task<IActionResult> ClearPlan()
        {
            var cacheKey = "evacuation_plan";
            await _redisService.RemoveAsync(cacheKey);
            return NoContent();
        }
    }
}