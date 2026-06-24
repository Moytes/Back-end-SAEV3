namespace Models.DB;

public class IndividualSubscription
{
    public int Id { get; set; }
    public Guid UsuarioId { get; set; }
    public SubscriptionStatus Estado { get; set; } = SubscriptionStatus.ACTIVA;
    public DateOnly FechaInicio { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public DateOnly? FechaFin { get; set; }
    public SubscriptionPeriod Periodo { get; set; } = SubscriptionPeriod.MENSUAL;
    public string? ReferenciaPago { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public SubscriptionPlan Plan { get; set; } = null!;
    public int PlanId { get; set; }
}
