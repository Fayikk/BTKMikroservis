using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRSPattern.Database;
using MediatR;

namespace CQRSPattern.Queries
{
    public record GetAllProductsQuery(string? Kategori = null) : IRequest<List<ProductReadModel>>;
  public class GetAllProductsHandler : IRequestHandler<GetAllProductsQuery, List<ProductReadModel>>
{
    private readonly ReadDb _readDb;

    public GetAllProductsHandler(ReadDb readDb) => _readDb = readDb;

    public Task<List<ProductReadModel>> Handle(GetAllProductsQuery query, CancellationToken ct)
    {
        var products = _readDb.Products.Values.AsEnumerable();

        if (!string.IsNullOrEmpty(query.Kategori))
            products = products.Where(p => p.Kategori == query.Kategori);

        Console.WriteLine($"[READ ] Ürün listesi sorgulandı. Filtre: {query.Kategori ?? "Yok"}");

        return Task.FromResult(products.ToList());
    }
}

}