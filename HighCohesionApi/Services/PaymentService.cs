
//High Cohesion
public class PaymentService
    {
        public async Task<bool> CreatePayment(object payment)
        {
            return true;
        }        

        public async Task<bool> UpdatPaymentStatus(object status)
        {
            return true;
        }
    }