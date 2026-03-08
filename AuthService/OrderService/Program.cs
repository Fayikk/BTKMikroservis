
using OrderService.Middleware;
using OrderService.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<OrderRepository>();

var app = builder.Build();

app.UseMiddleware<GatewayAuthMiddleware>();
app.MapControllers();

app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    service = "OrderService",
    timestamp = DateTime.UtcNow
}));

app.Run();
