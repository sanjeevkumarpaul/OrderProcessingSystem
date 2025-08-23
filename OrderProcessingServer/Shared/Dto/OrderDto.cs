namespace OrderProcessingServer.Shared.Dto;

public class OrderDto
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public CustomerDto? Customer { get; set; }
    public double Total { get; set; }
    public string? Status { get; set; }
    public int? SupplierId { get; set; }
    public SupplierDto? Supplier { get; set; }
}
