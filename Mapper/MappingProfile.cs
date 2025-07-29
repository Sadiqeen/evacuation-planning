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
            CreateMap<CreateVehicleDto, TableVehicle>()
                .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.LocationCoordinates.Latitude))
                .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.LocationCoordinates.Longitude))
                .ReverseMap();
            CreateMap<TableVehicle, VehicleDto>()
                .ForMember(dest => dest.LocationCoordinates, opt => opt.MapFrom(src => new LocationCoordinatesDto
                {
                    Latitude = src.Latitude,
                    Longitude = src.Longitude
                }))
                .ReverseMap();
                
            CreateMap<CreateEvacuationZoneDto, TableEvacuationZone>()
                .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.LocationCoordinates.Latitude))
                .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.LocationCoordinates.Longitude))
                .ReverseMap();
                
            CreateMap<TableEvacuationZone, EvacuationZoneDto>()
                .ForMember(dest => dest.LocationCoordinates, opt => opt.MapFrom(src => new LocationCoordinatesDto
                {
                    Latitude = src.Latitude,
                    Longitude = src.Longitude
                }))
                .ReverseMap();
            CreateMap<TableVehicle, VehicleDistanceDto>()
                .ForMember(dest => dest.LocationCoordinates, opt => opt.MapFrom(src => new LocationCoordinatesDto
                {
                    Latitude = src.Latitude,
                    Longitude = src.Longitude
                }))
                .ReverseMap();
        }
    }
}