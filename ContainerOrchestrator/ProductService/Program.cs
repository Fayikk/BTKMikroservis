using Prometheus;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.UseHttpMetrics();
app.MapGet("/", () => "Hello World!");
app.MapGet("/api/products", () =>
{
    var products = new[]
    {
        new { Id = 1, Name = "Laptop", Price = 1200.00m, Stock = 15 },
        new { Id = 2, Name = "Mouse", Price = 25.50m, Stock = 150 },
        new { Id = 3, Name = "Keyboard", Price = 75.00m, Stock = 80 },
        new { Id = 4, Name = "Monitor", Price = 350.00m, Stock = 25 }
    };
    
    return Results.Ok(new { 
        service = "ProductService", 
        hostname = Environment.MachineName,
        products 
    });
})
.WithName("GetProducts");
app.MapMetrics();
app.Run();
