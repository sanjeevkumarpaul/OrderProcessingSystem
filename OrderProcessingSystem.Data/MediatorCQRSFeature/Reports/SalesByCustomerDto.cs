namespace OrderProcessingSystem.Data.MediatorCQRS.Reports;

public class SalesByCustomerDto
{
    public int CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public double TotalSales { get; set; }
    public int OrderCount { get; set; }
}
