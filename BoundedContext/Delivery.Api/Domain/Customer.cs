using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delivery.Api.Domain
{
    public class Customer
    {
        public Guid Id { get; set; }
        public List<Delivery> Deliveries { get; set; } = new List<Delivery>();
    }
}