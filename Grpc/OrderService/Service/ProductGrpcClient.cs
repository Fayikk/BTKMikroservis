using Grpc.Net.Client;
using ProductService.Protos;

public class ProductGrpcClient
{
     private readonly ILogger<ProductGrpcClient> _logger;
    private readonly string _productServiceUrl;
    private readonly GrpcChannel _channel;
    private readonly ProductGrpc.ProductGrpcClient _client;

    public ProductGrpcClient(ILogger<ProductGrpcClient> logger, IConfiguration configuration)
    {
        _logger = logger;
        _productServiceUrl = configuration["ProductService:Url"] ?? "http://localhost:5001";
        
        // gRPC kanalı oluştur
        _channel = GrpcChannel.ForAddress(_productServiceUrl);
        _client = new ProductGrpc.ProductGrpcClient(_channel);
        
        _logger.LogInformation("ProductGrpcClient initialized with URL: {Url}", _productServiceUrl);
    }

     public async Task<ProductResponse?> GetProductAsync(int productId)
    {
        try
        {
            _logger.LogInformation("Calling GetProduct for ProductId: {ProductId}", productId);
            
            var request = new ProductRequest { ProductId = productId };
            var response = await _client.GetProductAsync(request);
            
            _logger.LogInformation("Successfully received product: {ProductName}", response.Name);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling GetProduct for ProductId: {ProductId}", productId);
            return null;
        }
    }

    public async Task<List<ProductResponse>> ListProductsAsync()
    {
        try
        {
            _logger.LogInformation("Calling ListProducts");
            
            var request = new EmptyRequest();
            var response = await _client.ListProductsAsync(request);
            
            _logger.LogInformation("Successfully received {Count} products", response.Products.Count);
            return response.Products.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling ListProducts");
            return new List<ProductResponse>();
        }
    }

     public async Task<StockResponse?> CheckStockAsync(int productId, int quantity)
    {
        try
        {
            _logger.LogInformation("Calling CheckStock for ProductId: {ProductId}, Quantity: {Quantity}", 
                productId, quantity);
            
            var request = new StockRequest 
            { 
                ProductId = productId, 
                Quantity = quantity 
            };
            var response = await _client.CheckStockAsync(request);
            
            _logger.LogInformation("Stock check result: {Available}, {Message}", 
                response.Available, response.Message);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling CheckStock for ProductId: {ProductId}", productId);
            return null;
        }
    }

     public async Task<List<ProductResponse>> StreamProductsAsync()
    {
        var products = new List<ProductResponse>();
        
        try
        {
            _logger.LogInformation("Calling StreamProducts");
            
            var request = new EmptyRequest();
            var call = _client.StreamProducts(request);

            while (await call.ResponseStream.MoveNext(CancellationToken.None))
            {
                var product = call.ResponseStream.Current;
                _logger.LogInformation("Received product from stream: {ProductName}", product.Name);
                products.Add(product);
            }
            
            _logger.LogInformation("Stream completed. Received {Count} products", products.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during StreamProducts");
        }

        return products;
    }

}