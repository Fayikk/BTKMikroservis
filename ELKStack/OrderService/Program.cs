using OrderService.Repository;
using Serilog;
using Serilog.Sinks.Elasticsearch;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
ConfigureLogging();
builder.Host.UseSerilog();
builder.Services.AddControllers();
builder.Services.AddSingleton<OrderRepository>();
var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.UseSerilogRequestLogging();
app.UseAuthorization();

app.MapControllers();

app.Run();

void ConfigureLogging()
{
    var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
    var elasticsearchUrl = Environment.GetEnvironmentVariable("ELASTICSEARCH_URL") ?? "http://elasticsearch:9200";

    Log.Logger = new LoggerConfiguration()
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithEnvironmentName()
        .Enrich.WithThreadId()
        .Enrich.WithProperty("Application", "OrderService")
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
        .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticsearchUrl))
        {
            AutoRegisterTemplate = true,
            AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
            IndexFormat = $"orderservice-logs-{environment.ToLower()}-{DateTime.UtcNow:yyyy-MM}",
            NumberOfReplicas = 1,
            NumberOfShards = 2,
            ModifyConnectionSettings = x => x.BasicAuthentication("elastic", "changeme")
        })
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
        .CreateLogger();

    Log.Information("Application Starting - ELK Stack Monitoring Demo");
}
