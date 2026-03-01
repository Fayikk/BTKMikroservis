var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.WebHost.UseUrls("http://localhost:5100");
var app = builder.Build();

// Configure the HTTP request pipeline.
       
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
