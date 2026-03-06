using Asp.Versioning;
using MassTransit;
using OrderService.Outbox;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
var redisConnection = builder.Configuration["Redis:ConnectionString"] ?? "localhost:6379";
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = ConfigurationOptions.Parse(redisConnection);
    configuration.AbortOnConnectFail = false;
    return ConnectionMultiplexer.Connect(configuration);
});


builder.Services.AddSingleton<RedisOutboxService>();
builder.Services.AddHostedService<OutboxProcessorService>();

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"] ?? "localhost", "/", h =>
        {
            h.Username(builder.Configuration["RabbitMQ:Username"] ?? "guest");
            h.Password(builder.Configuration["RabbitMQ:Password"] ?? "guest");
        });

        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddApiVersioning(options =>
{

    options.ApiVersionReader = new QueryStringApiVersionReader("api-version");

    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1,0);
    options.AssumeDefaultVersionWhenUnspecified = true;

    options.ReportApiVersions = true;
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddApiVersioning(options =>
{

    options.ApiVersionReader = new HeaderApiVersionReader("X-Api-Version");

    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1,0);
    options.AssumeDefaultVersionWhenUnspecified = true;

    options.ReportApiVersions = true;
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = false;
});



builder.Services.AddApiVersioning(options =>
{

    options.ApiVersionReader = new MediaTypeApiVersionReader("v");

    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1,0);
    options.AssumeDefaultVersionWhenUnspecified = true;

    options.ReportApiVersions = true;
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = false;
});

var app = builder.Build();



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
