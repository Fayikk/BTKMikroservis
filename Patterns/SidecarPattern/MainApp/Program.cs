using MainApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.WebHost.UseUrls("http://localhost:5500");
builder.Configuration["SharedLogPath"] = Path.Combine("..","shared","app.log");

builder.Services.AddSingleton<FileLogger>();




var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
