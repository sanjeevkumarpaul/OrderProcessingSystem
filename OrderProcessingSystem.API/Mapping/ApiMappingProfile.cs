using AutoMapper;
using OrderProcessingSystem.Data.Entities;
using OrderProcessingSystem.Core.Entities;
using OrderProcessingSystem.Infrastructure.Dto;
using OrderProcessingSystem.Contracts.Dto;
using OrderProcessingSystem.Data.Features.Customers;
using OrderProcessingSystem.Data.Features.Suppliers;

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
        
        // Add mappings between Data layer DTOs and Contract layer DTOs
        CreateMap<OrderProcessingSystem.Data.Features.Customers.CustomerWithOrdersDto, OrderProcessingSystem.Contracts.Dto.CustomerWithOrdersDto>();
        CreateMap<OrderProcessingSystem.Data.Features.Suppliers.SupplierWithOrdersDto, OrderProcessingSystem.Contracts.Dto.SupplierWithOrdersDto>();
    }
}
