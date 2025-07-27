using Microsoft.AspNetCore.Mvc;
using EvacuationPlanning.Models;
using EvacuationPlanning.Models.Dtos;
using EvacuationPlanning.Services.Interfaces;
using CourseService.Models.Database;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace EvacuationPlanning.Controllers
{
    [ApiController]
    [Route("api/vehicle")]
    public class VehicleController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IVehicleService _vehicleService;

        public VehicleController(
            IMapper mapper,
            IVehicleService vehicleService
            )
        {
            _mapper = mapper;
            _vehicleService = vehicleService;
        }

        [HttpGet]
        public async Task<ActionResult<List<VehicleDto>>> GetAllVehicles()
        {
            var vehicles = await _vehicleService.GetAllVehiclesAsync();
            var response = _mapper.Map<List<VehicleDto>>(vehicles);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VehicleDto>> GetVehicle(string id)
        {
            var vehicle = await _vehicleService.GetVehicleByIdAsync(id);
            if (vehicle == null)
            {
                return NotFound($"Vehicle with ID '{id}' not found.");
            }

            var response = _mapper.Map<VehicleDto>(vehicle);
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<VehicleDto>> CreateVehicle(CreateVehicleDto createVehicleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingVehicle = await _vehicleService.GetVehicleByIdAsync(createVehicleDto.VehicleId);
            if (existingVehicle != null)
            {
                return Conflict($"Vehicle with ID '{createVehicleDto.VehicleId}' already exists.");
            }

            var vehicle = _mapper.Map<TableVehicle>(createVehicleDto);
            var createdVehicle = await _vehicleService.CreateVehicleAsync(vehicle);

            return CreatedAtAction(
                nameof(GetVehicle),
                new { id = createdVehicle.VehicleId },
                _mapper.Map<VehicleDto>(createdVehicle));
        }
    }

}
