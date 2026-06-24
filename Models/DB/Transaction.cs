namespace Models.DB;

public class Transaction
{
    public int Id { get; set; }
    public int? AcademySubscriptionId { get; set; }
    public int? IndividualSubscriptionId { get; set; }
    public int? PaymentMethodId { get; set; }
    public decimal Monto { get; set; }
    public string Moneda { get; set; } = "MXN";
    public TransactionType Tipo { get; set; }
    public TransactionStatus Estado { get; set; } = TransactionStatus.PENDIENTE;
    public string? ReferenciaExt { get; set; }
    public string? Descripcion { get; set; }
    public DateTime? FechaPago { get; set; }
    public string? ErrorMensaje { get; set; }
    public string? MetadataJson { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public AcademySubscription? AcademySubscription { get; set; }
    public IndividualSubscription? IndividualSubscription { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
}
