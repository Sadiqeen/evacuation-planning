using Microsoft.AspNetCore.Mvc;
using EvacuationPlanning.Models;
using EvacuationPlanning.Models.Dtos;
using EvacuationPlanning.Services.Interfaces;
using AutoMapper;

namespace EvacuationPlanning.Controllers
{
    [ApiController]
    [Route("api/evacuation-zone")]
    public class EvacuationZoneController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IEvacuationZoneService _evacuationZoneService;

        public EvacuationZoneController(
            IMapper mapper,
            IEvacuationZoneService evacuationZoneService
            )
        {
            _mapper = mapper;
            _evacuationZoneService = evacuationZoneService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EvacuationZoneDto>>> GetAllEvacuationZones()
        {
            var zones = await _evacuationZoneService.GetAllEvacuationZonesAsync();
            var response = _mapper.Map<List<EvacuationZoneDto>>(zones);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EvacuationZoneDto>> GetEvacuationZone(string id)
        {
            var zone = await _evacuationZoneService.GetEvacuationZoneByIdAsync(id);
            if (zone == null)
            {
                return NotFound($"Evacuation zone with ID '{id}' not found.");
            }

            var response = _mapper.Map<EvacuationZoneDto>(zone);
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<EvacuationZoneDto>> CreateEvacuationZone(CreateEvacuationZoneDto createZoneDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingZone = await _evacuationZoneService.GetEvacuationZoneByIdAsync(createZoneDto.ZoneId);
            if (existingZone != null)
            {
                return Conflict($"Evacuation zone with ID '{createZoneDto.ZoneId}' already exists.");
            }

            var zone = _mapper.Map<TableEvacuationZone>(createZoneDto);
            var createdZone = await _evacuationZoneService.CreateEvacuationZoneAsync(zone);

            return CreatedAtAction(
                nameof(GetEvacuationZone),
                new { id = createdZone.ZoneId },
                _mapper.Map<EvacuationZoneDto>(createdZone));
        }
    }
}