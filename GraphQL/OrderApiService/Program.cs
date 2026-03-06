using OrderApiService.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient<GraphQLService>();

var app = builder.Build();
app.MapGet("/products", async (GraphQLService graphqlService, ILogger<Program> logger) =>
{
    try
    {
        var products = await graphqlService.GetAllProductsAsync();

        return Results.Ok(new
        {
            success = true,
            data = products,
            count = products.Count
        });
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Ürünler alınamadı");

        return Results.Problem(ex.Message, statusCode: 500);
    }
});

app.MapGet("/products/{id:int}", async (int id, GraphQLService graphqlService, ILogger<Program> logger) =>
{
    try
    {
        var product = await graphqlService.GetProductByIdAsync(id);

        if (product == null)
        {
            return Results.NotFound(new
            {
                success = false,
                message = $"Ürün bulunamadı: {id}"
            });
        }

        return Results.Ok(new
        {
            success = true,
            data = product
        });
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Ürün alınamadı: {Id}", id);

        return Results.Problem(ex.Message, statusCode: 500);
    }
});

app.MapPatch("/products/{id:int}/stock",
    async (int id, UpdateStockRequest request, GraphQLService graphqlService, ILogger<Program> logger) =>
{
    try
    {
        var product = await graphqlService.UpdateStockAsync(id, request.NewStock);

        if (product == null)
        {
            return Results.NotFound(new
            {
                success = false,
                message = $"Ürün bulunamadı: {id}"
            });
        }

        return Results.Ok(new
        {
            success = true,
            data = product,
            message = "Stok güncellendi"
        });
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Stok güncellenemedi: {Id}", id);

        return Results.Problem(ex.Message, statusCode: 500);
    }
});

app.Run();

