using AutoMapper;
using OrderProcessingSystem.Data.Entities;
using OrderProcessingSystem.Core.Entities;
using OrderProcessingSystem.Infrastructure.Dto;
using OrderProcessingSystem.Contracts.Dto;

namespace OrderProcessingSystem.API.Mapping;

public class ApiMappingProfile : Profile
{
    public ApiMappingProfile()
    {
    CreateMap<OrderProcessingSystem.Data.Entities.Supplier, SupplierDto>();
    CreateMap<OrderProcessingSystem.Data.Entities.Customer, CustomerDto>();
    CreateMap<OrderProcessingSystem.Data.Entities.Order, OrderDto>();
    CreateMap<OrderProcessingSystem.Core.Entities.Order, OrderDto>();
    CreateMap<OrderProcessingSystem.Data.Features.Reports.SalesByCustomerDto, SalesByCustomerDto>();
    CreateMap<OrderProcessingSystem.Infrastructure.Dto.SalesByCustomerReportDto, SalesByCustomerDto>();
    }
}
