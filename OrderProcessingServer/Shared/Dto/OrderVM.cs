namespace OrderProcessingServer.Shared.Dto;

public class OrderVM
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public CustomerVM? Customer { get; set; }
    public double Total { get; set; }
    public string? Status { get; set; }
    public int? SupplierId { get; set; }
    public SupplierVM? Supplier { get; set; }
}
