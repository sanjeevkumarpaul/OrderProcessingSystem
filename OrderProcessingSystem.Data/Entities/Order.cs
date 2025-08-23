namespace OrderProcessingSystem.Data.Entities;

public class Order
{
    // Database uses OrderId as the primary key
    public int OrderId { get; set; }

    // FK to Customers table in existing DB
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public double Total { get; set; }
    public string? Status { get; set; }
    public int? SupplierId { get; set; }
    public Supplier? Supplier { get; set; }
}
