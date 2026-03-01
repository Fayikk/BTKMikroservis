using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CQRSPattern.Database
{
    public class Product
{
    public Guid   Id        { get; set; }
    public string Ad        { get; set; } = "";
    public string Kategori  { get; set; } = "";
    public decimal Fiyat   { get; set; }
    public int    Stok      { get; set; }
    public DateTime OlusturulmaTarihi { get; set; }
}
public class ProductReadModel
{
    public Guid   Id         { get; set; }
    public string Ad         { get; set; } = "";
    public string Kategori   { get; set; } = "";
    public decimal Fiyat     { get; set; }
    public int    Stok       { get; set; }
    public string StokDurumu { get; set; } = ""; // "Yeterli" / "Az" / "Tükendi" → hesaplanmış
    public string Ozet       { get; set; } = ""; // "Nike Ayakkabı — 250₺" → önceden birleştirilmiş
}

public class WriteDb
{
    public Dictionary<Guid, Product> Products { get; } = [];
}

public class ReadDb
{
    public Dictionary<Guid, ProductReadModel> Products { get; } = [];
}


}