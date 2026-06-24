namespace Models.DB;

public class Invoice
{
    public int Id { get; set; }
    public int TransactionId { get; set; }
    public string? Folio { get; set; }
    public string? RFC { get; set; }
    public string? RazonSocial { get; set; }
    public string? RegimenFiscal { get; set; }
    public string UsoCFDI { get; set; } = "G03";
    public decimal Subtotal { get; set; }
    public decimal IVA { get; set; }
    public decimal Total { get; set; }
    public InvoiceStatus Estado { get; set; } = InvoiceStatus.EMITIDA;
    public string? ArchivoR2Key { get; set; }
    public string? ArchivoUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Transaction Transaction { get; set; } = null!;
}
