using Models.DB;

namespace Models.Dto;

public class CIEEvaluationListItemDto
{
    public Guid Id { get; set; }
    public DateOnly EvaluationDate { get; set; }
    public Guid StudentId { get; set; }
    public string StudentFullName { get; set; } = null!;
    public Guid EvaluatorId { get; set; }
    public string EvaluatorFullName { get; set; } = null!;
    public Guid SchoolYearId { get; set; }
    public string SchoolYearName { get; set; } = null!;
    public Guid DimensionId { get; set; }
    public string DimensionName { get; set; } = null!;
    public string DimensionCVE { get; set; } = null!;
    public string? Observations { get; set; }
    public evaluationStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}