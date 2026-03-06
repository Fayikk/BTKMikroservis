var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient("ProductService",client =>
{
    client.BaseAddress = new Uri("http://localhost:5085");
});


var app = builder.Build();

app.MapGet("/products/{id}",(int id) =>
{
    var product = new
    {
        Id = id,
        Name = $"Product {id}"
    };
    return Results.Ok(product);
});


app.MapGet("/Order/RelationProduct/{productId}", async (int productId,IHttpClientFactory httpClientFactory) =>
{
    var httpClient = httpClientFactory.CreateClient("ProductService");

    var response = await httpClient.GetAsync($"/products/{productId}");

    var product = await response.Content.ReadFromJsonAsync<Product>();

    return Results.Ok(product);

}
);

app.Run();

record Product(int Id,string Name,decimal price,int userId);