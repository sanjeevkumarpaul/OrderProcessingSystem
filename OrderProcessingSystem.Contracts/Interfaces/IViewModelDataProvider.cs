using OrderProcessingSystem.Contracts.Dto;

namespace OrderProcessingSystem.Contracts.Interfaces;

/// <summary>
/// Service for fetching data from API and mapping DTOs to ViewModels
/// Eliminates repetitive mapping code across UI pages
/// </summary>
public interface IViewModelDataProvider
{
    /// <summary>
    /// Get customers with orders data mapped to specified ViewModel type
    /// </summary>
    Task<List<TViewModel>?> GetCustomersWithOrdersAsync<TViewModel>() where TViewModel : class;

    /// <summary>
    /// Get suppliers with orders data mapped to specified ViewModel type
    /// </summary>
    Task<List<TViewModel>?> GetSuppliersWithOrdersAsync<TViewModel>() where TViewModel : class;

    /// <summary>
    /// Get orders data mapped to specified ViewModel type
    /// </summary>
    Task<List<TViewModel>?> GetOrdersAsync<TViewModel>() where TViewModel : class;

    /// <summary>
    /// Get transaction exceptions data mapped to specified ViewModel type
    /// </summary>
    Task<List<TViewModel>?> GetTransExceptionsAsync<TViewModel>() where TViewModel : class;

    /// <summary>
    /// Map GridColumnDto to GridColumnVM (for grid metadata)
    /// </summary>
    //List<GridColumnVM> MapGridColumns(List<GridColumnDto> gridColumnDtos);
    List<TViewModel> MapGridColumns<TViewModel>(List<GridColumnDto> gridColumnDtos);

    /// <summary>
    /// Generic method for custom data fetching and mapping
    /// </summary>
    Task<List<TViewModel>?> GetMappedDataAsync<TDto, TViewModel>(
        Func<Task<List<TDto>?>> dataFetcher)
        where TViewModel : class
        where TDto : class;
}
