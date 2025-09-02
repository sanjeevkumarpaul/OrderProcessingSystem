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

        // Map DTOs to ViewModels - Main entity mappings
        CreateMap<CustomerWithOrdersDto, CustomerWithOrdersVM>();
        CreateMap<SupplierWithOrdersDto, SupplierWithOrdersVM>();
        CreateMap<OrderDto, OrderVM>();
        CreateMap<TransExceptionDto, TransExceptionVM>();
        
        // Map DTOs to ViewModels - Supporting entity mappings
        CreateMap<CustomerDto, CustomerVM>()
            .ForMember(dest => dest.OrdersCount, opt => opt.Ignore()) // UI computed field
            .ForMember(dest => dest.TotalSales, opt => opt.Ignore()); // UI computed field
            
        CreateMap<SupplierDto, SupplierVM>()
            .ForMember(dest => dest.OrdersSupplied, opt => opt.Ignore()); // UI computed field
            
        CreateMap<SalesByCustomerDto, SalesByCustomerVM>();

        // Reverse mappings if needed
        CreateMap<GridColumnVM, GridColumnDto>();
        CreateMap<CustomerWithOrdersVM, CustomerWithOrdersDto>();
        CreateMap<SupplierWithOrdersVM, SupplierWithOrdersDto>();
        CreateMap<OrderVM, OrderDto>();
        CreateMap<TransExceptionVM, TransExceptionDto>();
        CreateMap<CustomerVM, CustomerDto>();
        CreateMap<SupplierVM, SupplierDto>();
        CreateMap<SalesByCustomerVM, SalesByCustomerDto>();
    }
}
