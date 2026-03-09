using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrderService.Models;
using OrderService.Repository;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
         private readonly OrderRepository _repository;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(OrderRepository repository, ILogger<OrdersController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

     [HttpGet]
    public ActionResult<List<Order>> GetAllOrders()
    {
        _logger.LogInformation("Fetching all orders");
        
        var orders = _repository.GetAll();
        
        _logger.LogInformation("Found {OrderCount} orders", orders.Count);
        
        return Ok(orders);
    }

    [HttpGet("{id}")]
    public ActionResult<Order> GetOrder(int id)
    {
        _logger.LogInformation("Fetching order with ID: {OrderId}", id);
        
        var order = _repository.GetById(id);
        
        if (order == null)
        {
            _logger.LogWarning("Order with ID {OrderId} not found", id);
            return NotFound(new { message = $"Order with ID {id} not found" });
        }

        _logger.LogInformation("Order {OrderId} found: {ProductName}", id, order.ProductName);
        
        return Ok(order);
    }
 [HttpPost]
    public ActionResult<Order> CreateOrder([FromBody] CreateOrderDto dto)
    {
        _logger.LogInformation("Creating new order for product: {ProductName}, Quantity: {Quantity}", 
            dto.ProductName, dto.Quantity);

        try
        {
            var order = _repository.CreateOrder(dto);
            
            _logger.LogInformation("Order created successfully with ID: {OrderId}, Total: {TotalPrice:C}", 
                order.Id, order.TotalPrice);
            
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order for product: {ProductName}", dto.ProductName);
            return StatusCode(500, new { message = "Internal server error while creating order" });
        }
    }

    [HttpGet("simulate-error")]
    public ActionResult SimulateError()
    {
        _logger.LogWarning("Simulating an error for testing purposes");
        
        try
        {
            throw new InvalidOperationException("This is a simulated error for testing ELK Stack");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Simulated error occurred");
            return StatusCode(500, new { message = "Simulated error", error = ex.Message });
        }
    }
    }
}