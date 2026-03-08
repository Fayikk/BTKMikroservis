using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Models;
using OrderService.Repository;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
         private readonly OrderRepository _orderRepository;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(
            OrderRepository orderRepository,
            ILogger<OrdersController> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetOrders()
        {
            var userId = HttpContext.Request.Headers["X-User-Id"].FirstOrDefault();
            var userName = HttpContext.Request.Headers["X-User-Name"].FirstOrDefault();
            var userRole = HttpContext.Request.Headers["X-User-Role"].FirstOrDefault();

            _logger.LogInformation("Get orders for user: {UserId} ({UserName}), Role: {Role}",
                userId, userName, userRole);

            List<Order> orders;

            if (userRole == "Admin")
            {
                orders = _orderRepository.GetAll();
            }
            else  if (userRole == "Manager")
            {
                orders = _orderRepository.GetAll();
            }
            else
            {
                orders = _orderRepository.GetByUserId(userId ?? "");
            }

            return Ok(orders);
        }

    }
}