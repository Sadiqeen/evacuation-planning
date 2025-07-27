using AutoMapper;
using EvacuationPlanning.Models;
using EvacuationPlanning.Models.Dtos;

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
        }
    }
}