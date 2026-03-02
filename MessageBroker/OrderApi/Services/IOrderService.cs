using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderApi.Services
{
    public interface IOrderService
    {
         Task<PaymentResult> Payment(PaymentRequest request);
        Task<bool>  CancelPayment(Guid odemeId);
    }
}