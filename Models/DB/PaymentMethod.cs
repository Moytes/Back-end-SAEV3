namespace Models.DB;

public class PaymentMethod
{
    public int Id { get; set; }
    public int? AcademySubscriptionId { get; set; }
    public int? IndividualSubscriptionId { get; set; }
    public PaymentMethodType Tipo { get; set; }
    public PaymentProvider Proveedor { get; set; } = PaymentProvider.STRIPE;
    public string? ReferenciaExt { get; set; }
    public string? UltimosDigitos { get; set; }
    public string? Marca { get; set; }
    public string? Titular { get; set; }
    public bool EsPredeterminado { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public AcademySubscription? AcademySubscription { get; set; }
    public IndividualSubscription? IndividualSubscription { get; set; }
}
