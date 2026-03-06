var builder = WebApplication.CreateBuilder(args);



var app = builder.Build();

var products = new List<Product>()
{
    new Product(1,"Laptop1",15000,1),
    new Product(2,"Laptop2",15000,12),
    new Product(3,"Laptop3",15000,13),
    new Product(4,"Laptop4",15000,14)
};


app.MapGet("/products/{id}",(int id) =>
{
    var orderProduct = products.FirstOrDefault(p => p.Id == id);
    return Results.Ok(orderProduct);    
});





app.Run();

record Product(int Id,string Name,decimal price,int userId);