using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sagaapi.Contracts
{
   public class OrderSubmittedEvent
{
    public Guid CorrelationId { get; set; }
    public Guid OrderId { get; set; }
}

public class PaymentProcessedEvent
{
    public Guid CorrelationId { get; set; }
}

public class PaymentFailedEvent
{
    public Guid CorrelationId { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class InventoryUpdatedEvent
{
    public Guid CorrelationId { get; set; }
}

public class InventoryFailedEvent
{
    public Guid CorrelationId { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class ProcessPaymentCommand
{
    public Guid CorrelationId { get; set; }
    public Guid OrderId { get; set; }
}

public class UpdateInventoryCommand
{
    public Guid CorrelationId { get; set; }
    public Guid OrderId { get; set; }
}

public class CancelPaymentCommand
{
    public Guid CorrelationId { get; set; }
    public Guid OrderId { get; set; }
}
}