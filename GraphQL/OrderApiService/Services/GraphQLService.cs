using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using OrderApiService.Models;

namespace OrderApiService.Services
{
    public class GraphQLService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GraphQLService> _logger;
        private readonly string _graphqlUrl;

        public GraphQLService(HttpClient httpClient, IConfiguration config, ILogger<GraphQLService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _graphqlUrl = config["GraphQL:Url"] ?? "http://localhost:5003/graphql";
        }

         public async Task<List<Product>> GetAllProductsAsync()
    {
        var query = @"
        {
          products {
            id
            name
            description
            price
            stock
            category
          }
        }";

        var response = await ExecuteQueryAsync<ProductsData>(query);
        return response?.Products ?? new List<Product>();
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        var query = $@"
        {{
          product(id: {id}) {{
            id
            name
            description
            price
            stock
            category
          }}
        }}";

        var response = await ExecuteQueryAsync<ProductData>(query);
        return response?.Product;
    }

    public async Task<List<Product>> GetProductsByCategoryAsync(string category)
    {
        var query = $@"
        {{
          productsByCategory(category: ""{category}"") {{
            id
            name
            description
            price
            stock
            category
          }}
        }}";

        var response = await ExecuteQueryAsync<ProductsData>(query);
        return response?.Products ?? new List<Product>();
    }

    public async Task<List<Product>> GetInStockProductsAsync()
    {
        var query = @"
        {
          inStockProducts {
            id
            name
            description
            price
            stock
            category
          }
        }";

        var response = await ExecuteQueryAsync<ProductsData>(query);
        return response?.Products ?? new List<Product>();
    }

    public async Task<Product?> UpdateStockAsync(int productId, int newStock)
    {
        var mutation = $@"
        mutation {{
          updateStock(productId: {productId}, newStock: {newStock}) {{
            id
            name
            stock
          }}
        }}";

        var response = await ExecuteQueryAsync<ProductData>(mutation);
        return response?.Product;
    }

    private async Task<T?> ExecuteQueryAsync<T>(string query) where T : class
    {
        try
        {
            var request = new { query };
            var content = new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8,
                "application/json");

            _logger.LogInformation("GraphQL isteği gönderiliyor: {Url}", _graphqlUrl);

            var response = await _httpClient.PostAsync(_graphqlUrl, content);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("GraphQL yanıtı alındı");

            var result = JsonSerializer.Deserialize<GraphQLResponse<T>>(
                jsonResponse, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result?.Errors != null && result.Errors.Any())
            {
                _logger.LogError("GraphQL hatası: {Errors}", 
                    string.Join(", ", result.Errors.Select(e => e.Message)));
            }

            return result?.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GraphQL isteği sırasında hata oluştu");
            throw;
        }
    }
    }
}