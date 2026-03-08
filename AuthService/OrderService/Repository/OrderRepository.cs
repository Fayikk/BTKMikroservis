using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderService.Models;

namespace OrderService.Repository
{
    public class OrderRepository
    {
         private readonly List<Order> _orders;
        private int _nextId = 1;

        public OrderRepository()
        {
            _orders = new List<Order>
            {
                new Order
                {
                    Id = _nextId++,
                    UserId = "1",
                    UserName = "admin",
                    ProductId = 1,
                    ProductName = "Laptop",
                    Quantity = 1,
                    TotalPrice = 15000,
                    Status = OrderStatus.Delivered,
                    CreatedAt = DateTime.UtcNow.AddDays(-5)
                },
                new Order
                {
                    Id = _nextId++,
                    UserId = "2",
                    UserName = "user",
                    ProductId = 2,
                    ProductName = "Mouse",
                    Quantity = 2,
                    TotalPrice = 500,
                    Status = OrderStatus.Shipped,
                    CreatedAt = DateTime.UtcNow.AddDays(-2)
                }
            };
        }
    
         public List<Order> GetAll() => _orders;
        public List<Order> GetByUserId(string userId) =>
            _orders.Where(o => o.UserId == userId).ToList();
    }
}