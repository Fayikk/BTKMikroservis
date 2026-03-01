using OrderApi.Policies;
using OrderApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.WebHost.UseUrls("http://localhost:5200");
builder.Services.AddHttpClient<IOrderService, OrderService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5100");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
})
// .AddPolicyHandler(PollyPolicies.CircuitBreakerPolicy())
// .AddPolicyHandler(PollyPolicies.RetryPolicy()) 
.AddPolicyHandler(PollyPolicies.TimeoutPolicy());  
var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
