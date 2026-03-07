using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using ProductService.Configuration;
using ProductService.Models;

namespace ProductService.Services
{
    public class ProductRepository
    {
        
    private readonly IMongoCollection<Product> _products;
    private readonly ILogger<ProductRepository> _logger;

    public ProductRepository(MongoDbSettings settings, ILogger<ProductRepository> logger)
    {
        _logger = logger;
        var client = new MongoClient(settings.ConnectionString);
        var database = client.GetDatabase(settings.DatabaseName);
        _products = database.GetCollection<Product>(settings.CollectionName);
        SeedData();
    }

    
    private void SeedData()
    {
        try
        {
            if (_products.CountDocuments(FilterDefinition<Product>.Empty) == 0)
            {
                var seedProducts = new List<Product>
                {
                    new Product 
                    { 
                        Name = "Laptop", 
                        Description = "High performance laptop", 
                        Price = 15000, 
                        Stock = 10,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Product 
                    { 
                        Name = "Mouse", 
                        Description = "Wireless mouse", 
                        Price = 250, 
                        Stock = 50,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Product 
                    { 
                        Name = "Keyboard", 
                        Description = "Mechanical keyboard", 
                        Price = 800, 
                        Stock = 30,
                        CreatedAt = DateTime.UtcNow
                    }
                };

                _products.InsertMany(seedProducts);
                _logger.LogInformation("✅ MongoDB seeded with {Count} products", seedProducts.Count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding MongoDB data");
        }
    }



      public async Task<List<Product>> GetAllAsync()
    {
        _logger.LogInformation("Getting all products from MongoDB");
        return await _products.Find(_ => true).ToListAsync();
    }


    public async Task<Product> CreateAsync(Product product)
    {
        _logger.LogInformation("Creating new product: {ProductName}", product.Name);
        product.CreatedAt = DateTime.UtcNow;
        await _products.InsertOneAsync(product);
        _logger.LogInformation("Product created successfully with ID {ProductId}", product.Id);
        return product;
    }
    }
}