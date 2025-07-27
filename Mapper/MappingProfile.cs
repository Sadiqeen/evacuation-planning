using AutoMapper;
using EvacuationPlanning.Models;
using EvacuationPlanning.Models.Dtos;
using EvacuationPlanning.Models.Dtos.Vehicle;

namespace EvacuationPlanning.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TableVehicle, CreateVehicleDto>().ReverseMap();
            CreateMap<TableVehicle, VehicleDto>().ReverseMap();
            CreateMap<TableEvacuationZone, CreateEvacuationZoneDto>().ReverseMap();
            CreateMap<TableEvacuationZone, EvacuationZoneDto>().ReverseMap();

            CreateMap<VehicleDto, VehicleDistanceDto>().ReverseMap();
            
        }
    }
}