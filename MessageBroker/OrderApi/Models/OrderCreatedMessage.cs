using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderApi.Models
{
    public class OrderCreatedMessage
    {
         public int SiparisId { get; set; }
        public string UrunAdi { get; set; } = string.Empty;
        public int Miktar { get; set; }
        public decimal Fiyat { get; set; }
        public decimal ToplamTutar { get; set; }
        public DateTime OlusturulmaTarihi { get; set; }
    }
}