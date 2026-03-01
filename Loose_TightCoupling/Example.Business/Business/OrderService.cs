using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Example.Business.Business
{
    public class OrderService
    {
        private PaymentService _paymentService = new PaymentService();
        public void CreateOrder(decimal orderAmount)
        {
            _paymentService.Pay(orderAmount);
        }
    }


    public class PaymentService
    {
        public PaymentService()
        {
        }

        public decimal Pay(decimal amount)
        {
            return amount;
        }
    }
}