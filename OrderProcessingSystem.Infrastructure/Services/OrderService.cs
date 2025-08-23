using System.Collections.Generic;
using OrderProcessingSystem.Core.Entities;
using OrderProcessingSystem.Core.Interfaces;

namespace OrderProcessingSystem.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<Order> _orderRepository;
        public OrderService(IRepository<Order> orderRepository)
        {
            _orderRepository = orderRepository;
        }
        public IEnumerable<Order> GetOrders() => _orderRepository.GetAll();
    public Order? GetOrderById(int id) => _orderRepository.GetById(id);
        public void CreateOrder(Order order) => _orderRepository.Add(order);
        public void UpdateOrder(Order order) => _orderRepository.Update(order);
        public void DeleteOrder(int id) => _orderRepository.Delete(id);
    }
}
