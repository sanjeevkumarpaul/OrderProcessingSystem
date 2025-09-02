namespace OrderProcessingSystem.UI.Shared.UIViewModels;

/// <summary>
/// DTO for supplier data with pre-calculated statistics from database
/// </summary>
public class SupplierWithOrdersVM
{
    public int SupplierId { get; set; }
    public string? Name { get; set; }
    public string? Country { get; set; }
    
    // Pre-calculated fields from database joins
    public int OrdersSupplied { get; set; }
}
