using OrderProcessingSystem.Core.Enums;
namespace OrderProcessingSystem.Core.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }      
    }
}
