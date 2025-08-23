using System.Collections.Generic;
using OrderProcessingSystem.Core.Entities;

namespace OrderProcessingSystem.Core.Interfaces
{
    public interface IOrderService
    {
        IEnumerable<Order> GetOrders();
    Order? GetOrderById(int id);
        void CreateOrder(Order order);
        void UpdateOrder(Order order);
        void DeleteOrder(int id);
    }
}
