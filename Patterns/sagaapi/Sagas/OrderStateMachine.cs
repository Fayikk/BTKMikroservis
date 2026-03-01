using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using sagaapi.Contracts;

namespace sagaapi.Sagas
{
    public class OrderStateMachine : MassTransitStateMachine<OrderState>
{
    public State ProcessingPayment { get; private set; } = null!;
    public State UpdatingInventory { get; private set; } = null!;
    public State Faulted { get; private set; } = null!;
    public State Completed { get; private set; } = null!;

    public Event<OrderSubmittedEvent> OrderSubmitted { get; private set; } = null!;
    public Event<PaymentProcessedEvent> PaymentProcessed { get; private set; } = null!;
    public Event<PaymentFailedEvent> PaymentFailed { get; private set; } = null!;
    public Event<InventoryUpdatedEvent> InventoryUpdated { get; private set; } = null!;
    public Event<InventoryFailedEvent> InventoryFailed { get; private set; } = null!;

    public OrderStateMachine()
        {
            InstanceState(x => x.CurrentState);

        Event(() => OrderSubmitted, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => PaymentProcessed, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => PaymentFailed, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => InventoryUpdated, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => InventoryFailed, x => x.CorrelateById(context => context.Message.CorrelationId));

        Initially(
            When(OrderSubmitted)
                .Then(context =>
                {
                    context.Saga.OrderId = context.Message.OrderId;
                    context.Saga.CreatedAt = DateTime.UtcNow;
                    Console.WriteLine($"[SAGA] Sipariş alındı (OrderId: {context.Message.OrderId}). Ödeme sürecine geçiliyor...");
                })
                .PublishAsync(context => context.Init<ProcessPaymentCommand>(new
                {
                    context.Saga.CorrelationId,
                    context.Saga.OrderId
                }))
                .TransitionTo(ProcessingPayment)
        );

        During(ProcessingPayment,
            When(PaymentProcessed)
                .Then(context =>
                {
                    context.Saga.UpdatedAt = DateTime.UtcNow;
                    Console.WriteLine($"[SAGA] Ödeme başarılı. Stok güncelleme sürecine geçiliyor...");
                })
                .PublishAsync(context => context.Init<UpdateInventoryCommand>(new
                {
                    context.Saga.CorrelationId,
                    context.Saga.OrderId
                }))
                .TransitionTo(UpdatingInventory),

            When(PaymentFailed)
                .Then(context =>
                {
                    context.Saga.UpdatedAt = DateTime.UtcNow;
                    context.Saga.FaultReason = context.Message.Reason;
                    Console.WriteLine($"[SAGA] Ödeme HAKSIZ/BAŞARISIZ ({context.Message.Reason}). Süreç iptal edildi.");
                })
                .TransitionTo(Faulted)
        );

        During(UpdatingInventory,
            When(InventoryUpdated)
                .Then(context =>
                {
                    context.Saga.UpdatedAt = DateTime.UtcNow;
                    Console.WriteLine($"[SAGA] Stok güncellendi. Sipariş süreci BAŞARIYLA TAMAMLANDI!");
                })
                .TransitionTo(Completed)
                .Finalize(),

            When(InventoryFailed)
                .Then(context =>
                {
                    context.Saga.UpdatedAt = DateTime.UtcNow;
                    context.Saga.FaultReason = context.Message.Reason;
                    Console.WriteLine($"[SAGA] Stok güncellenemedi ({context.Message.Reason}). COMPENSATION (Telafi) BAŞLATILIYOR -> Ödemenin iade edilmesi talebi...");
                })
                .PublishAsync(context => context.Init<CancelPaymentCommand>(new
                {
                    context.Saga.CorrelationId,
                    context.Saga.OrderId
                }))
                .TransitionTo(Faulted) 
        );

        SetCompletedWhenFinalized();
        }


        
    }
}