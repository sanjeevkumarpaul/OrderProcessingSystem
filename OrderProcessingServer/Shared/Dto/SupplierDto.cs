namespace OrderProcessingServer.Shared.Dto;

public class SupplierDto
{
    public int SupplierId { get; set; }
    public string? Name { get; set; }
    public string? Country { get; set; }
    // Number of orders supplied by this supplier (computed on the client)
    public int OrdersSupplied { get; set; }
}
