using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductGraphQLService.Models;

namespace ProductGraphQLService.GraphQL
{
    public class Query
    {
         public IQueryable<Product> GetProducts()
    {
        return DataStore.Products.AsQueryable();
        
    }

      public IQueryable<Product> GetProductsByCategory(string category)
    {
        return DataStore.Products.Where(p => p.Category == category).AsQueryable();
    }

    }
}