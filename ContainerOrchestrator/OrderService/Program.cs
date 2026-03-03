using Prometheus;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.UseHttpMetrics();
app.MapGet("/", () => "Hello World!");
app.MapGet("/api/orders", () =>
{
    var orders = new[]
    {
        new { Id = 1, ProductId = 1, Quantity = 2, TotalPrice = 2400.00m, Status = "Completed" },
        new { Id = 2, ProductId = 2, Quantity = 5, TotalPrice = 127.50m, Status = "Pending" },
        new { Id = 3, ProductId = 3, Quantity = 1, TotalPrice = 75.00m, Status = "Shipped" }
    };
    
    return Results.Ok(new { 
        service = "OrderService", 
        hostname = Environment.MachineName,
        orders 
    });
})
.WithName("GetOrders");
app.MapMetrics();
app.Run();
