using AutoMapper;
using OrderProcessingSystem.Data.Entities;
using OrderProcessingSystem.Core.Entities;
using OrderProcessingSystem.Infrastructure.Dto;
using OrderProcessingSystem.Contracts.Dto;
using OrderProcessingSystem.Data.MediatorCQRS.Customers;
using OrderProcessingSystem.Data.MediatorCQRS.Suppliers;

namespace OrderProcessingSystem.API.Mapping;

public class ApiMappingProfile : Profile
{
    public ApiMappingProfile()
    {
        CreateMap<OrderProcessingSystem.Data.Entities.Supplier, SupplierDto>();
        CreateMap<OrderProcessingSystem.Data.Entities.Customer, CustomerDto>();
        CreateMap<OrderProcessingSystem.Data.Entities.Order, OrderDto>();
        CreateMap<OrderProcessingSystem.Core.Entities.Order, OrderDto>();
        CreateMap<OrderProcessingSystem.Data.MediatorCQRS.Reports.SalesByCustomerDto, SalesByCustomerDto>();
        CreateMap<OrderProcessingSystem.Infrastructure.Dto.SalesByCustomerReportDto, SalesByCustomerDto>();
        
        // Add mappings between Data layer DTOs and Contract layer DTOs
        CreateMap<OrderProcessingSystem.Data.MediatorCQRS.Customers.CustomerWithOrdersDto, OrderProcessingSystem.Contracts.Dto.CustomerWithOrdersDto>();
        CreateMap<OrderProcessingSystem.Data.MediatorCQRS.Suppliers.SupplierWithOrdersDto, OrderProcessingSystem.Contracts.Dto.SupplierWithOrdersDto>();
    }
}
