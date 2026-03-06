using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductGraphQLService.Models;

namespace ProductGraphQLService.GraphQL
{
    public class Mutation
    {
          public Product? UpdateStock(int productId, int newStock)
    {
        var product = DataStore.Products.FirstOrDefault(p => p.Id == productId);
        if (product == null)
            return null;

        product.Stock = newStock;
        return product;
    }
    }
}