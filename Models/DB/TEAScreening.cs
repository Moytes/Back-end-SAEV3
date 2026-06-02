namespace Models.DB;

public enum alertLevel
{
    SIN_ALERTA = 0,
    LEVE = 1,
    MODERADO = 2,
    SIGNIFICATIVO = 3
}

public class TEAScreening
{
    public Guid Id { get; set; }
    public DateOnly ScreeningDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public string? ObservationContext { get; set; }
    public string? GeneralObservations { get; set; }
    public short? TotalScore { get; set; }
    public alertLevel? AlertLevel { get; set; }
    public BoolStatus RequiresCanalization { get; set; } = BoolStatus.False;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Student
    public Student Student { get; set; } = null!;
    public Guid StudentId { get; set; }

    // Evaluator
    public User Evaluator { get; set; } = null!;
    public Guid EvaluatorId { get; set; }

    // School year
    public SchoolYear SchoolYear { get; set; } = null!;
    public Guid SchoolYearId { get; set; }
}
