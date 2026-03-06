using ProductGraphQLService.Models;

public static class DataStore
{
    public static readonly List<Product> Products = new()
    {
        new Product { Id = 1, Name = "Laptop", Description = "High performance laptop", Price = 15000, Stock = 10, Category = "Electronics" },
        new Product { Id = 2, Name = "Mouse", Description = "Wireless mouse", Price = 250, Stock = 50, Category = "Electronics" },
        new Product { Id = 3, Name = "Keyboard", Description = "Mechanical keyboard", Price = 800, Stock = 30, Category = "Electronics" },
        new Product { Id = 4, Name = "Monitor", Description = "27 inch 4K monitor", Price = 5000, Stock = 15, Category = "Electronics" },
        new Product { Id = 5, Name = "Headphones", Description = "Noise cancelling", Price = 1200, Stock = 25, Category = "Electronics" }
    };

  
}
