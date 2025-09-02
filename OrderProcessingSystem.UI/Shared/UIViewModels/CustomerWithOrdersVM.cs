namespace OrderProcessingSystem.UI.Shared.UIViewModels;

/// <summary>
/// DTO for customer data with pre-calculated statistics from database
/// </summary>
public class CustomerWithOrdersVM
{
    public int CustomerId { get; set; }
    public string? Name { get; set; }
    public string? Country { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    
    // Pre-calculated fields from database joins
    public int OrdersCount { get; set; }
    public decimal TotalSales { get; set; }
}
