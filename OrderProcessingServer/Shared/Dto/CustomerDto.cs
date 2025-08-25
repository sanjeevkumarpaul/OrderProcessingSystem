namespace OrderProcessingServer.Shared.Dto;

public class CustomerDto
{
    public int CustomerId { get; set; }
    public string? Name { get; set; }
    // computed on the UI side
    public int OrdersCount { get; set; }
    public double TotalSales { get; set; }
}
