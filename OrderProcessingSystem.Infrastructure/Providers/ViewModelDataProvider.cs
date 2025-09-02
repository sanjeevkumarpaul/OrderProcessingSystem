using AutoMapper;
using OrderProcessingSystem.Contracts.Interfaces;
using OrderProcessingSystem.Contracts.Dto;

namespace OrderProcessingSystem.Infrastructure.Providers;

/// <summary>
/// Implementation of IViewModelDataProvider that fetches data from API and maps DTOs to ViewModels
/// Centralizes the repetitive pattern of: Fetch DTOs -> Map to VMs -> Return VMs
/// </summary>
public class ViewModelDataProvider : IViewModelDataProvider
{
    private readonly IApiDataService _apiDataService;
    private readonly IMapper _mapper;

    public ViewModelDataProvider(IApiDataService apiDataService, IMapper mapper)
    {
        _apiDataService = apiDataService;
        _mapper = mapper;
    }

    /// <summary>
    /// Get customers with orders data mapped to specified ViewModel type
    /// </summary>
    public async Task<List<TViewModel>?> GetCustomersWithOrdersAsync<TViewModel>() where TViewModel : class
    {
        var customerDtos = await _apiDataService.GetCustomersWithOrdersAsync();
        return customerDtos != null ? _mapper.Map<List<TViewModel>>(customerDtos) : null;
    }

    /// <summary>
    /// Get suppliers with orders data mapped to specified ViewModel type
    /// </summary>
    public async Task<List<TViewModel>?> GetSuppliersWithOrdersAsync<TViewModel>() where TViewModel : class
    {
        var supplierDtos = await _apiDataService.GetSuppliersWithOrdersAsync();
        return supplierDtos != null ? _mapper.Map<List<TViewModel>>(supplierDtos) : null;
    }

    /// <summary>
    /// Get orders data mapped to specified ViewModel type
    /// </summary>
    public async Task<List<TViewModel>?> GetOrdersAsync<TViewModel>() where TViewModel : class
    {
        var orderDtos = await _apiDataService.GetOrdersAsync();
        return orderDtos != null ? _mapper.Map<List<TViewModel>>(orderDtos) : null;
    }

    /// <summary>
    /// Get transaction exceptions data mapped to specified ViewModel type
    /// </summary>
    public async Task<List<TViewModel>?> GetTransExceptionsAsync<TViewModel>() where TViewModel : class
    {
        var transExceptionDtos = await _apiDataService.GetTransExceptionsAsync();
        return transExceptionDtos != null ? _mapper.Map<List<TViewModel>>(transExceptionDtos) : null;
    }

    /// <summary>
    /// Map GridColumnDto to GridColumnVM (for grid metadata)
    /// </summary>
    //public List<GridColumnVM> MapGridColumns(List<GridColumnDto> gridColumnDtos)
    public List<TViewModel> MapGridColumns<TViewModel>(List<GridColumnDto> gridColumnDtos)
    {
        return _mapper.Map<List<TViewModel>>(gridColumnDtos);
    }

    /// <summary>
    /// Generic method for custom data fetching and mapping
    /// Allows for any custom data fetching logic while still handling the mapping
    /// </summary>
    public async Task<List<TViewModel>?> GetMappedDataAsync<TDto, TViewModel>(
        Func<Task<List<TDto>?>> dataFetcher) 
        where TViewModel : class 
        where TDto : class
    {
        var dtos = await dataFetcher();
        return dtos != null ? _mapper.Map<List<TViewModel>>(dtos) : null;
    }
}
