using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRSPattern.Database;
using MediatR;

namespace CQRSPattern.Commands
{


    public record CreateProductCommand(
        string Ad,
        string Kategori,
        decimal Fiyat,
        int Stok

    ) : IRequest<Guid>;


    public class CreateProductHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly WriteDb _writeDb;
    private readonly ReadDb  _readDb;

    public CreateProductHandler(WriteDb writeDb, ReadDb readDb)
    {
        _writeDb = writeDb;
        _readDb  = readDb;
    }

    public Task<Guid> Handle(CreateProductCommand cmd, CancellationToken ct)
    {
        var product = new Product
        {
            Id                = Guid.NewGuid(),
            Ad                = cmd.Ad,
            Kategori          = cmd.Kategori,
            Fiyat             = cmd.Fiyat,
            Stok              = cmd.Stok,
            OlusturulmaTarihi = DateTime.UtcNow
        };
        _writeDb.Products[product.Id] = product;

        Console.WriteLine($"[WRITE] Ürün kaydedildi → {product.Ad}");

        _readDb.Products[product.Id] = ToReadModel(product);

        Console.WriteLine($"[READ ] Read model güncellendi → {product.Ad}");

        return Task.FromResult(product.Id);
    }

      private static ProductReadModel ToReadModel(Product p) => new()
    {
        Id         = p.Id,
        Ad         = p.Ad,
        Kategori   = p.Kategori,
        Fiyat      = p.Fiyat,
        Stok       = p.Stok,
        StokDurumu = p.Stok == 0 ? "Tükendi" : p.Stok < 10 ? "Az" : "Yeterli",
        Ozet       = $"{p.Ad} — {p.Fiyat}₺ ({p.Kategori})"
    };
    }
}