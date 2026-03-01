using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataCentralizeManagement.Api.Services
{
    public class PaymentService
    {
        private readonly PaymentDbContext paymentDbContext;

        private readonly ApplicationDbContext _applicationDbContext;

        public void Add()
        {
            //Payment db ye
            //DbContext kullanılacak.

            ///Order db ye git 
            /// Publisher
            /// _message.Publish(new OrderDto{Id= id,OrderName=orderName})
        }
    }


    public class OrderService
    {
        private readonly OrderDbContext orderDbContext;
        ///Consumer
        /// async Task Handle(OrderEvent event){var order = new Order{Id=event.Id,....}; orderDbContext.Add(order)}
        ///
    }

    public class ApplicationDbContext 
    {
            
    }

    public class OrderDbContext
    {
       //Order 
    }

    public class PaymentDbContext
    {
        //Payment
    }

    public class Order
    {
        
    }

    public class Payment
    {
        
    }
}