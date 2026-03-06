var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ProductGrpcClient>();

var app = builder.Build();

app.MapGet("/api/minimal/products",async (ProductGrpcClient productClient) =>
{
    var product = await productClient.ListProductsAsync();
    return Results.Ok(product);
});


app.UseHttpsRedirection();


app.Run();

