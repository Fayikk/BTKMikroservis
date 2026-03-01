using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExamplePaymentApi
{
    public class PaymentService
    {
          public decimal Pay(decimal amount)
        {
            decimal tax = 0.2;
            amount = tax * amount;
            return amount;
        }
    }
}