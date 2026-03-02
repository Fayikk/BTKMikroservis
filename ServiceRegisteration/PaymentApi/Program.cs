using Consul;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
{
    consulConfig.Address = new Uri("http://localhost:8500");
}));



builder.WebHost.UseUrls("http://localhost:5100");
var app = builder.Build();

var serviceName = "payment-api";
var serviceId = $"{serviceName}-{Guid.NewGuid()}";
var consulClient = app.Services.GetRequiredService<IConsulClient>();

var registration = new AgentServiceRegistration()
{
    ID = serviceId,
    Name = serviceName,
    Address = "host.docker.internal", 
    Port = 5100,
    Check = new AgentServiceCheck()
    {
        HTTP = "http://host.docker.internal:5100/health",
        Interval = TimeSpan.FromSeconds(10),
        Timeout = TimeSpan.FromSeconds(5)
    }
};
await consulClient.Agent.ServiceRegister(registration);
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
lifetime.ApplicationStopping.Register(async () =>
{
    await consulClient.Agent.ServiceDeregister(serviceId);
    app.Logger.LogInformation("❌ PaymentApi deregistered from Consul");
});

app.MapGet("/health", () => Results.Ok(new { service = "payment-api", status = "healthy" }));

// Configure the HTTP request pipeline.
       
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
