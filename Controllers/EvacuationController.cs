using Microsoft.AspNetCore.Mvc;
using EvacuationPlanning.Services.Interfaces;
using AutoMapper;
using EvacuationPlanning.Models.Dtos.Evacuation;
using EvacuationPlanning.Infrastructures.Interfaces;
using EvacuationPlanning.Models.Dtos;

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
        private const string _cacheKey = "evacuation_plan";

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
        public async Task<ActionResult<List<EvacuationPlanResponseDto>>> GeneratePlan()
        {
            var plan = await _redisService.GetAsync<List<EvacuationStatusDto>>(_cacheKey);

            if (plan == null)
            {
                plan = await _evacuationService.GeneratePlan();
                await _redisService.SetAsync(_cacheKey, plan);
            }

            var response = _mapper.Map<List<EvacuationPlanResponseDto>>(plan);
            return Ok(response);
        }

        [HttpGet("status")]
        public async Task<ActionResult<List<EvacuationStatusResponseDto>>> GetStatus()
        {
            var plan = await _redisService.GetAsync<List<EvacuationStatusDto>>(_cacheKey);
            if (plan == null)
            {
                _logger.LogWarning("No evacuation plan found in cache.");
                return NotFound("No evacuation plan found.");
            }

            var response = plan.GroupBy(x => x.ZoneId)
                .Select(g => new EvacuationStatusResponseDto
                {
                    ZoneId = g.Key,
                    TotalEvacuated = g.Sum(x => x.Evacuated),
                    RemainingPeople = g.Sum(x => x.Remaining),
                    LastTravelBy = g.LastOrDefault().Logs?.LastOrDefault()?.VehicleId,
                })
                .ToList();

            return Ok(response);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateEvacuation([FromBody] UpdateEvacuationStatusDto updateDto)
        {
            if (updateDto == null || string.IsNullOrEmpty(updateDto.VehicleId) || updateDto.Evacuated < 0)
            {
                return BadRequest("Invalid update data.");
            }

            var plan = await _redisService.GetAsync<List<EvacuationStatusDto>>(_cacheKey);
            if (plan == null)
            {
                return NotFound("No evacuation plan found to update.");
            }

            var updatedPlan = await _evacuationService.UpdateStatus(plan, updateDto);
            await _redisService.SetAsync(_cacheKey, updatedPlan);

            return NoContent();
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearPlan()
        {
            var plan = await _redisService.GetAsync<List<EvacuationPlanDto>>(_cacheKey);
            if (plan == null)
            {
                return NotFound("No evacuation plan found to clear.");
            }

            await _redisService.RemoveAsync(_cacheKey);
            return NoContent();
        }
    }
}