using System.Collections.Generic;
using System.Linq;
using OrderProcessingSystem.Core.Entities;
using OrderProcessingSystem.Core.Interfaces;
using OrderProcessingSystem.Contracts.Interfaces;

namespace OrderProcessingSystem.Infrastructure.Repositories
{
    public class OrderRepository : IRepository<Order>
    {
        private readonly List<Order> _orders = new();
        public IEnumerable<Order> GetAll() => _orders;
    public Order? GetById(int id) => _orders.FirstOrDefault(o => o.Id == id);
        public void Add(Order entity) => _orders.Add(entity);
        public void Update(Order entity)
        {
            var index = _orders.FindIndex(o => o.Id == entity.Id);
            if (index >= 0) _orders[index] = entity;
        }
        public void Delete(int id) => _orders.RemoveAll(o => o.Id == id);
    }
}
