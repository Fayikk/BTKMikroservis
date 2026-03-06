using Grpc.Core;
using ProductService.Protos;

public class ProductGrpcService : ProductGrpc.ProductGrpcBase
{
     private readonly ILogger<ProductGrpcService> _logger;
    
    private static readonly List<ProductResponse> Products = new()
    {
        new ProductResponse 
        { 
            ProductId = 1, 
            Name = "Laptop", 
            Description = "High performance laptop",
            Price = 1299.99,
            Stock = 50,
            Category = "Electronics"
        },
        new ProductResponse 
        { 
            ProductId = 2, 
            Name = "Mouse", 
            Description = "Wireless optical mouse",
            Price = 29.99,
            Stock = 200,
            Category = "Electronics"
        },
        new ProductResponse 
        { 
            ProductId = 3, 
            Name = "Keyboard", 
            Description = "Mechanical gaming keyboard",
            Price = 89.99,
            Stock = 100,
            Category = "Electronics"
        },
        new ProductResponse 
        { 
            ProductId = 4, 
            Name = "Monitor", 
            Description = "27-inch 4K monitor",
            Price = 449.99,
            Stock = 30,
            Category = "Electronics"
        },
        new ProductResponse 
        { 
            ProductId = 5, 
            Name = "Headphones", 
            Description = "Noise-cancelling headphones",
            Price = 199.99,
            Stock = 75,
            Category = "Electronics"
        }
    };

    public ProductGrpcService(ILogger<ProductGrpcService> logger)
    {
        _logger = logger;
    }
     public override Task<ProductResponse> GetProduct(ProductRequest request, ServerCallContext context)
    {
        _logger.LogInformation("GetProduct called for ProductId: {ProductId}", request.ProductId);

        var product = Products.FirstOrDefault(p => p.ProductId == request.ProductId);
        
        if (product == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Product with ID {request.ProductId} not found"));
        }

        return Task.FromResult(product);
    }

     public override Task<ProductListResponse> ListProducts(EmptyRequest request, ServerCallContext context)
    {
        _logger.LogInformation("ListProducts called");

        var response = new ProductListResponse();
        response.Products.AddRange(Products);

        return Task.FromResult(response);
    }

     public override Task<StockResponse> CheckStock(StockRequest request, ServerCallContext context)
    {
        _logger.LogInformation("CheckStock called for ProductId: {ProductId}, Quantity: {Quantity}", 
            request.ProductId, request.Quantity);

        var product = Products.FirstOrDefault(p => p.ProductId == request.ProductId);
        
        if (product == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Product with ID {request.ProductId} not found"));
        }

        var available = product.Stock >= request.Quantity;
        var message = available 
            ? $"Stock available. Current stock: {product.Stock}" 
            : $"Insufficient stock. Required: {request.Quantity}, Available: {product.Stock}";

        return Task.FromResult(new StockResponse
        {
            Available = available,
            CurrentStock = product.Stock,
            Message = message
        });
    }

     public override async Task StreamProducts(EmptyRequest request, IServerStreamWriter<ProductResponse> responseStream, ServerCallContext context)
    {
        _logger.LogInformation("StreamProducts called");

        foreach (var product in Products)
        {
            if (context.CancellationToken.IsCancellationRequested)
            {
                break;
            }

            await responseStream.WriteAsync(product);
            await Task.Delay(500);
        }
    }
}