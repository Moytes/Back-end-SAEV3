namespace Models.DB;

public enum evaluationStatus
{
    EN_PROCESO = 0,
    COMPLETADA = 1,
    REVISADA = 2
}

public class CIEEvaluation
{
    public Guid Id { get; set; }
    public DateOnly EvaluationDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public string? Observations { get; set; }
    public evaluationStatus Status { get; set; } = evaluationStatus.EN_PROCESO;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Student
    public Student Student { get; set; } = null!;
    public Guid StudentId { get; set; }

    // Evaluator
    public User Evaluator { get; set; } = null!;
    public Guid EvaluatorId { get; set; }

    // School year
    public SchoolYear SchoolYear { get; set; } = null!;
    public Guid SchoolYearId { get; set; }

    // CIE Dimension
    public CIEDimension Dimension { get; set; } = null!;
    public Guid DimensionId { get; set; }
}
