using AutoMapper;
using EvacuationPlanning.Models;
using EvacuationPlanning.Models.Dtos;
using EvacuationPlanning.Models.Dtos.Evacuation;
using EvacuationPlanning.Models.Dtos.Vehicle;

namespace EvacuationPlanning.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateVehicleDto, TableVehicle>()
                .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.LocationCoordinates.Latitude))
                .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.LocationCoordinates.Longitude));
            CreateMap<TableVehicle, VehicleDto>()
                .ForMember(dest => dest.LocationCoordinates, opt => opt.MapFrom(src => new LocationCoordinatesDto
                {
                    Latitude = src.Latitude,
                    Longitude = src.Longitude
                }));

            CreateMap<CreateEvacuationZoneDto, TableEvacuationZone>()
                .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.LocationCoordinates.Latitude))
                .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.LocationCoordinates.Longitude));

            CreateMap<TableEvacuationZone, EvacuationZoneDto>()
                .ForMember(dest => dest.LocationCoordinates, opt => opt.MapFrom(src => new LocationCoordinatesDto
                {
                    Latitude = src.Latitude,
                    Longitude = src.Longitude
                }));
            CreateMap<TableVehicle, VehicleDistanceDto>()
                .ForMember(dest => dest.Vehicle, opt => opt.MapFrom(src => src));

            CreateMap<EvacuationPlanDto, EvacuationStatusDto>()
                .ForMember(dest => dest.Remaining, opt => opt.MapFrom(src => src.Vehicle.Capacity));

            CreateMap<EvacuationStatusDto, EvacuationPlanDto>();

            CreateMap<EvacuationPlanDto, EvacuationPlanResponseDto>()
                .ForMember(dest => dest.NumberOfPeople, opt => opt.MapFrom(src => src.Remaining < src.Vehicle.Capacity ? src.Remaining : src.Vehicle.Capacity));
        }
    }
}