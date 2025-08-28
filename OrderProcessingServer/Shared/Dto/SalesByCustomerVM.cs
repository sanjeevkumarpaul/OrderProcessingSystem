namespace OrderProcessingServer.Shared.Dto;

public class SalesByCustomerVM
{
    public int CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public double TotalSales { get; set; }
    public int OrderCount { get; set; }
}
