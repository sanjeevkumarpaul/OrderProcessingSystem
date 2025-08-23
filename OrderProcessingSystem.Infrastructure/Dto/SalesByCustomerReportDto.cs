namespace OrderProcessingSystem.Infrastructure.Dto
{
    public class SalesByCustomerReportDto
    {
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public double TotalSales { get; set; }
        public int OrderCount { get; set; }
    }
}
