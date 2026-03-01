// ────── Ödeme ──────────────────────────────────────────────────────────────
public class PaymentRequest
{
    public Guid    SiparisId { get; set; }
    public decimal Tutar     { get; set; }
    public string  KartNo    { get; set; } = string.Empty;
    public Guid    MusteriId { get; set; }
}

public class PaymentResult
{
    public bool    Basarili   { get; set; }
    public Guid?   OdemeId    { get; set; }
    public string? HataMesaji { get; set; }
    public ErrorType HataTipi  { get; set; }
}

// ────── Sipariş ────────────────────────────────────────────────────────────
public class OrderDTO
{
    public decimal Tutar     { get; set; }
    public string  KartNo    { get; set; } = string.Empty;
    public Guid    MusteriId { get; set; }
}

public class OrderResult
{
    public bool     Basarili   { get; set; }
    public Guid?    SiparisId  { get; set; }
    public Guid?    OdemeId    { get; set; }
    public string?  HataMesaji { get; set; }
    public ErrorType HataTipi   { get; set; }
}

// ────── Enums ──────────────────────────────────────────────────────────────
public enum ErrorType
{
    Yok,
    ServisDevreDisi,
    ZamanAsimi,
    BaglantiHatasi
}
