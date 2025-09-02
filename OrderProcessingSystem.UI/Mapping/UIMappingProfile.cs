using AutoMapper;
using OrderProcessingSystem.Contracts.Dto;
using OrderProcessingSystem.UI.Shared.UIViewModels;

namespace OrderProcessingSystem.UI.Mapping;

/// <summary>
/// AutoMapper profile for mapping between DTOs and ViewModels in the UI project
/// </summary>
public class UIMappingProfile : Profile
{
    public UIMappingProfile()
    {
        // Map from GridColumnDto to GridColumnVM
        CreateMap<GridColumnDto, GridColumnVM>()
            .ForMember(dest => dest.Header, opt => opt.MapFrom(src => src.Header))
            .ForMember(dest => dest.Field, opt => opt.MapFrom(src => src.Field))
            .ForMember(dest => dest.Sortable, opt => opt.MapFrom(src => src.Sortable))
            .ForMember(dest => dest.Filterable, opt => opt.MapFrom(src => src.Filterable))
            .ForMember(dest => dest.IsNumeric, opt => opt.MapFrom(src => src.IsNumeric))
            .ForMember(dest => dest.IsEnum, opt => opt.MapFrom(src => src.IsEnum))
            .ForMember(dest => dest.EnumValues, opt => opt.MapFrom(src => src.EnumValues));

        // Reverse mapping if needed
        CreateMap<GridColumnVM, GridColumnDto>();
    }
}
