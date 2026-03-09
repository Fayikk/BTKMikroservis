using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderService.Models;

namespace OrderService.Repository
{
public class OrderRepository
{
    private readonly List<Order> _orders = new();
    private int _nextId = 1;

    public OrderRepository()
    {
        CreateOrder(new CreateOrderDto { ProductName = "Laptop", Quantity = 2, TotalPrice = 2499.99m });
        CreateOrder(new CreateOrderDto { ProductName = "Mouse", Quantity = 5, TotalPrice = 149.95m });
    }

    public List<Order> GetAll() => _orders;

    public Order? GetById(int id) => _orders.FirstOrDefault(o => o.Id == id);

    public Order CreateOrder(CreateOrderDto dto)
    {
        var order = new Order
        {
            Id = _nextId++,
            ProductName = dto.ProductName,
            Quantity = dto.Quantity,
            TotalPrice = dto.TotalPrice,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        _orders.Add(order);
        return order;
    }

    public bool UpdateStatus(int id, string status)
    {
        var order = GetById(id);
        if (order == null) return false;

        order.Status = status;
        order.UpdatedAt = DateTime.UtcNow;
        return true;
    }

    public bool DeleteOrder(int id)
    {
        var order = GetById(id);
        if (order == null) return false;

        _orders.Remove(order);
        return true;
    }
}

}