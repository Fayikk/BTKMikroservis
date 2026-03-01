public class Customer
{
    //Ödeme Servisleri Bilgisi
    public string CreditCardNumber { get; set; }
    public string Iban {get; set; }
    public string InvoiceAddress { get; set;}

    //Kargo Servisleri Bilgisi
    public string DeliveryAddress { get; set;   }
    public string DeliveryNote { get; set;   }

}