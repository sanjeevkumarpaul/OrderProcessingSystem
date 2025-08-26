using AutoMapper;
using OrderProcessingSystem.Contracts.Dto;

namespace OrderProcessingSystem.API.Mapping;

/// <summary>
/// AutoMapper profile for mapping between different DTO types
/// </summary>
public class GridColumnMappingProfile : Profile
{
    public GridColumnMappingProfile()
    {
        // Map from Contract DTO to UI DTO - optimized for client consumption
        CreateMap<GridColumnDto, UIGridColumnDto>()
            .ForMember(dest => dest.Header, opt => opt.MapFrom(src => src.Header))
            .ForMember(dest => dest.Field, opt => opt.MapFrom(src => src.Field))
            .ForMember(dest => dest.Sortable, opt => opt.MapFrom(src => src.Sortable))
            .ForMember(dest => dest.Filterable, opt => opt.MapFrom(src => src.Filterable))
            .ForMember(dest => dest.IsNumeric, opt => opt.MapFrom(src => src.IsNumeric))
            .ForMember(dest => dest.IsEnum, opt => opt.MapFrom(src => src.IsEnum));

        // Reverse mapping if needed
        CreateMap<UIGridColumnDto, GridColumnDto>();
    }
}
