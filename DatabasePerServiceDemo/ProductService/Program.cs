using ProductService.Configuration;
using ProductService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var mongoSettings = new MongoDbSettings
{
    ConnectionString = builder.Configuration.GetConnectionString("MongoDb") ?? "mongodb://localhost:27017",
    DatabaseName = builder.Configuration["MongoDb:DatabaseName"] ?? "ProductDb",
    CollectionName = builder.Configuration["MongoDb:CollectionName"] ?? "Products"
};

builder.Services.AddSingleton(mongoSettings);
builder.Services.AddSingleton<ProductRepository>();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var repo = scope.ServiceProvider.GetRequiredService<ProductRepository>();
        Console.WriteLine("✅ ProductService: MongoDB connected successfully");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ ProductService: MongoDB connection failed: {ex.Message}");
    }
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
