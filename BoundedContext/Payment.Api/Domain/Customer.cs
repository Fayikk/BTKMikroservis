public class Customer
{
    public Guid Id { get; set; }
    public string CreditCardNumber { get; set; }
    public List<PaymentHistory> paymentHistories{ get; set; } = new List<PaymentHistory>();
}